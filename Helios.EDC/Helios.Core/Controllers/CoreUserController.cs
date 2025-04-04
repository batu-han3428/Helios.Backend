﻿using Helios.Caching.Services.Interfaces;
using Helios.Common.DTO;
using Helios.Common.Enums;
using Helios.Common.Helpers.Api;
using Helios.Common.Model;
using Helios.Core.Contexts;
using Helios.Core.Domains.Entities;
using Helios.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Helios.Core.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CoreUserController : Controller
    {
        private CoreContext _context;
        private IUserService _userService;
        private IRedisCacheService _cacheService;

        public CoreUserController(CoreContext context, IUserService userService, IRedisCacheService cacheService)
        {
            _context = context;
            _userService = userService;
            _cacheService = cacheService;
        }

        #region Tenants     
        [HttpGet]
        public async Task<List<TenantModel>> GetTenantsStudyCount(string tenantIds)
        {
            string[] tenantIdsArray = tenantIds.Split(',');
            List<Int64> tenantIdsInt = new List<Int64>();
            foreach (string id in tenantIdsArray)
            {
                if (Int64.TryParse(id, out Int64 guid))
                {
                    tenantIdsInt.Add(guid);
                }
            }
            return await _context.Studies.Where(x => x.IsActive && !x.IsDeleted && tenantIdsInt.Contains(x.TenantId) && !x.IsDemo).GroupBy(s => s.TenantId).Select(g => new TenantModel
            {
                Id = g.Key,
                ActiveStudies = g.Count().ToString()
            }).ToListAsync();
        }

        [HttpGet]
        public async Task<int> GetStudyCount(Int64? tenantId)
        {
            return await _context.Studies.Where(x => x.IsActive && !x.IsDeleted && x.TenantId == tenantId && !x.IsDemo).CountAsync();
        }
        #endregion

        #region Permissions

        [HttpGet]
        public async Task<List<RoleVisitPermissionsModel>> GetPermissionsVisitList(Int64 roleId)
        {
            BaseDTO baseDTO = Request.Headers.GetBaseInformation();

            var roleVisitPagePermissions = await _context.Permissions.Where(x => x.StudyRoleId == roleId && (x.StudyVisitId != null || x.StudyVisitPageId != null)).ToListAsync();

            var visits = await _context.StudyVisits.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == baseDTO.StudyId).Include(x => x.Permissions).Include(x => x.StudyVisitPages).ThenInclude(x => x.Permissions).ToListAsync();

            var resultVisits = visits.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == baseDTO.StudyId).Select(x => new RoleVisitPermissionsModel
            {
                Id = x.Id,
                Name = x.Name,
                Permissions = x.Permissions.Where(a => a.StudyRoleId == null).Select(a => new RolePermissions { IsActive = a.IsActive, Key = a.PermissionKey, IsDisabled = !a.IsActive }).ToList(),
                Children = x.StudyVisitPages.Where(page => page.IsActive && !page.IsDeleted).Select(page => new RoleVisitPermissionsModel
                {
                    Id = page.Id,
                    Name = page.Name,
                    Permissions = page.Permissions.Where(a => a.StudyRoleId == null).Select(a => new RolePermissions { IsActive = a.IsActive, Key = a.PermissionKey, IsDisabled = !a.IsActive }).ToList(),
                }).ToList()
            }).ToList();


            foreach (var visit in resultVisits)
            {
                var permissions = visit.Permissions;

                foreach (VisitPermission permission in Enum.GetValues(typeof(VisitPermission)))
                {
                    if (!permissions.Any(p => p.Key == (int)permission))
                    {
                        visit.Permissions.Add(new RolePermissions { IsActive = false, IsDisabled = true, Key = (int)permission });
                    }
                    else if (permissions.Any(p => p.Key == (int)permission && p.IsActive))
                    {
                        var rolePermission = roleVisitPagePermissions.FirstOrDefault(p => p.PermissionKey == (int)permission && p.StudyVisitId == visit.Id);

                        if (rolePermission == null || !rolePermission.IsActive)
                        {
                            var visitPermission = permissions.FirstOrDefault(p => p.Key == (int)permission);
                            visitPermission.IsActive = false;
                        }
                    }
                }

                foreach (var child in visit.Children)
                {
                    var childPermissions = child.Permissions;

                    foreach (VisitPermission permission in Enum.GetValues(typeof(VisitPermission)))
                    {
                        if (!childPermissions.Any(p => p.Key == (int)permission))
                        {
                            child.Permissions.Add(new RolePermissions { IsActive = false, IsDisabled = true, Key = (int)permission });
                        }
                        else if (childPermissions.Any(p => p.Key == (int)permission && p.IsActive))
                        {
                            var rolePermission = roleVisitPagePermissions.FirstOrDefault(p => p.PermissionKey == (int)permission && p.StudyVisitPageId == child.Id);

                            if (rolePermission == null || !rolePermission.IsActive)
                            {
                                var childPermission = childPermissions.FirstOrDefault(p => p.Key == (int)permission);
                                childPermission.IsActive = false;
                            }
                        }
                    }
                }
            }

            return resultVisits;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetPermissionsVisitPage(PermissionsRoleVisitPageDTO dto)
        {
            BaseDTO baseDTO = Request.Headers.GetBaseInformation();

            var permission = await _context.Permissions.FirstOrDefaultAsync(x => x.StudyRoleId == dto.StudyRoleId && x.StudyId == baseDTO.StudyId && x.PermissionKey == dto.PermissionKey && ((dto.StudyVisitId != null && x.StudyVisitId == dto.StudyVisitId) || (dto.StudyPageId != null && x.StudyVisitPageId == dto.StudyPageId)));

            if (permission != null)
            {
                permission.IsActive = !permission.IsActive;
                _context.Permissions.Update(permission);
            }
            else
            {
                await _context.Permissions.AddAsync(new Permission
                {
                    StudyRoleId = dto.StudyRoleId,
                    PermissionKey = dto.PermissionKey,
                    StudyVisitId = dto.StudyVisitId,
                    StudyVisitPageId = dto.StudyPageId,
                    StudyId = baseDTO.StudyId,
                    TenantId = baseDTO.TenantId
                });
            }

            var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

            if (result)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = true,
                    Message = "Successful"
                };
            }
            else
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }
        }

        [HttpGet]
        public async Task<List<UserPermissionDTO>> GetPermissionRoleList(Int64 studyId)
        {
            var result = await _context.StudyRoles.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == studyId).Include(x => x.Permissions).AsNoTracking().Select(x => new UserPermissionDTO
            {
                Id = x.Id,
                StudyId = x.StudyId,
                RoleName = x.Name,
                RolePermissions = x.Permissions.Where(a => a.IsActive).Select(a => a.PermissionKey)
            }).ToListAsync();

            return result;
        }
      
        [HttpGet]
        public async Task<bool> GetHasRole(Int64 studyId, Int64 userId)
        {
            var role = await _context.StudyUsers.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == studyId && x.AuthUserId == userId && x.StudyRole != null).Include(x => x.StudyRole).Select(x => new StudyUsersRolesDTO
            {
                RoleId = x.StudyRole.Id,
                RoleName = x.StudyRole.Name
            }).ToListAsync();
            if (role.Count() > 0)
            {
                return true;
            }
            return false;
        }
        [HttpGet]
        public async Task<List<UserPermissionDTO>> GetRoleList(Int64 studyId)
        {
            var result = await _context.StudyRoles.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == studyId).AsNoTracking().Select(x => new UserPermissionDTO
            {
                Id = x.Id,
                RoleName = x.Name,
            }).ToListAsync();

            return result;
        }

        [HttpGet]
        public async Task<UserPermissionRoleDTO> GetUserIdsRole(Int64 roleId)
        {
            return await _context.StudyRoles.Where(x => x.Id == roleId && x.IsActive && !x.IsDeleted).Include(x => x.StudyUsers).Select(x => new UserPermissionRoleDTO
            {
                Role = x.Name,
                UserIds = x.StudyUsers.Select(x => x.AuthUserId).ToList()
            }).FirstOrDefaultAsync();
        }

        [HttpGet]
        public async Task<List<StudyUsersRolesDTO>> GetStudyUserIdsRole(Int64 studyId)
        {
            return await _context.StudyUsers.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == studyId && x.StudyRole != null).Include(x => x.StudyRole).Select(x => new StudyUsersRolesDTO
            {
                Id = x.AuthUserId,
                RoleName = x.StudyRole.Name
            }).ToListAsync();
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetPermission(StudyUserRolePermissionDTO dto)
        {
            BaseDTO baseDTO = Request.Headers.GetBaseInformation();

            var permission = await _context.Permissions.FirstOrDefaultAsync(x => x.StudyRoleId == dto.StudyRoleId && x.StudyId == baseDTO.StudyId && x.PermissionKey == dto.PermissionKey);

            if (permission != null)
            {
                permission.IsActive = !permission.IsActive;
                _context.Permissions.Update(permission);
            }
            else
            {
                await _context.Permissions.AddAsync(new Permission
                {
                    StudyRoleId = dto.StudyRoleId,
                    PermissionKey = dto.PermissionKey,
                    StudyId = baseDTO.StudyId,
                    TenantId = baseDTO.TenantId
                });
            }

            var study = await _context.Studies.FirstOrDefaultAsync(x => x.Id == baseDTO.StudyId);
            study.UpdatedAt = DateTimeOffset.Now;

            var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

            if (result)
            {
                await RemoveUserPermissions(baseDTO.StudyId);

                return new ApiResponse<dynamic>
                {
                    IsSuccess = true,
                    Message = "Successful"
                };
            }
            else
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }
        }

        private async Task RemoveUserPermissions(Int64 studyId)
        {
            string prefix = "Study:Permissions";
            var localCacheKey = prefix + ":" + studyId;

            await _cacheService.RemoveAsync(localCacheKey);
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> AddOrUpdatePermissionRole(UserPermissionRoleModel userPermission)
        {
            BaseDTO baseDTO = Request.Headers.GetBaseInformation();

            var userPermissionCheck = await _context.StudyRoles.FirstOrDefaultAsync(p => p.Name == userPermission.RoleName && p.IsActive && !p.IsDeleted && p.StudyId == baseDTO.StudyId);
            if (userPermissionCheck != null)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "This role already exists."
                };
            }
            else if (userPermission.Id == 0)
            {
                StudyRole studyRole = new StudyRole();
                studyRole.StudyId = baseDTO.StudyId;
                studyRole.Name = userPermission.RoleName;
                studyRole.TenantId = baseDTO.TenantId;
                await _context.StudyRoles.AddAsync(studyRole);
                var study = await _context.Studies.FirstOrDefaultAsync(x => x.Id == baseDTO.StudyId);
                study.UpdatedAt = DateTimeOffset.Now;
                var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

                if (result)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            else
            {
                var oldEntity = await _context.StudyRoles.SingleOrDefaultAsync(p => p.Id == userPermission.Id && p.IsActive && !p.IsDeleted);
                if (oldEntity != null)
                {
                    oldEntity.Name = userPermission.RoleName;

                    _context.StudyRoles.Update(oldEntity);

                    var study = await _context.Studies.FirstOrDefaultAsync(x => x.Id == oldEntity.StudyId);
                    study.UpdatedAt = DateTimeOffset.Now;

                    var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

                    if (result)
                    {
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = true,
                            Message = "Successful"
                        };
                    }
                }

                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> DeleteRole(UserPermissionRoleModel userPermission)
        {
            BaseDTO baseDTO = Request.Headers.GetBaseInformation();

            var oldEntity = await _context.StudyRoles.Include(r => r.Permissions).FirstOrDefaultAsync(p => p.Id == userPermission.Id);

            if (oldEntity == null)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "No record to delete was found."
                };
            }

            _context.Permissions.RemoveRange(oldEntity.Permissions);

            _context.StudyRoles.Remove(oldEntity);

            var study = await _context.Studies.FirstOrDefaultAsync(x => x.Id == baseDTO.StudyId);
            study.UpdatedAt = DateTimeOffset.Now;

            var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

            if (result)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = true,
                    Message = "Successful"
                };
            }
            else
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }
        }

        #endregion

        #region Study User

        [HttpGet]
        public async Task<List<StudyUserDTO>> GetStudyUsers(Int64 studyId)
        {
            return await _context.StudyUsers.Where(x => x.StudyId == studyId && !x.IsDeleted).Include(x => x.StudyRole).Include(x => x.StudyUserSites).AsNoTracking().Select(x => new StudyUserDTO
            {
                StudyUserId = x.Id,
                AuthUserId = x.AuthUserId,
                IsActive = x.IsActive,
                RoleName = x.StudyRole.Name,
                RoleId = x.StudyRoleId,
                Sites = x.StudyUserSites.Where(s => !s.IsDeleted).Select(s => new SiteDTO { Id = s.Site.Id, SiteFullName = s.Site.FullName }).ToList(),
                ResponsiblePerson = x.SuperUserIdList != "" ? JsonConvert.DeserializeObject<List<Int64>>(x.SuperUserIdList) : new List<Int64>(),
                CreatedOn = x.CreatedAt,
                LastUpdatedOn = x.UpdatedAt
            }).ToListAsync();
        }

        [HttpGet]
        public async Task<StudyUserDTO> GetStudyUserSites(Int64 authUserId, Int64 studyId)
        {
            return await _context.StudyUsers.Where(x => x.StudyId == studyId && x.AuthUserId == authUserId && !x.IsDeleted).Include(x => x.StudyUserSites).AsNoTracking().Select(x => new StudyUserDTO
            {
                StudyUserId = x.Id,
                AuthUserId = x.AuthUserId,
                Sites = x.StudyUserSites.Where(s => !s.IsDeleted).Select(s => new SiteDTO { Id = s.Site.Id, Name = s.Site.Name }).ToList(),
            }).FirstOrDefaultAsync();
        }

        [HttpGet]
        public async Task<bool> GetCheckStudyUser(Int64 authUserId, Int64 studyId)
        {
            return await _context.StudyUsers.AnyAsync(x => x.StudyId == studyId && x.AuthUserId == authUserId && !x.IsDeleted);
        }

        [HttpGet]
        public async Task<List<Int64>> GetStudyUserIds(Int64 studyId)
        {
            return await _context.StudyUsers.Where(x => x.StudyId == studyId && !x.IsDeleted).Select(x => x.AuthUserId).ToListAsync();
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetStudyUser(StudyUserModel studyUserModel)
        {
            if (studyUserModel.StudyUserId == 0)
            {
                var user = new StudyUser()
                {
                    StudyId = studyUserModel.StudyId,
                    AuthUserId = studyUserModel.AuthUserId,
                    SuperUserIdList = studyUserModel.ResponsiblePersonIds.Count > 0 ? JsonConvert.SerializeObject(studyUserModel.ResponsiblePersonIds) : "",
                    TenantId = studyUserModel.TenantId,
                    StudyRoleId = studyUserModel.RoleId != 0 && studyUserModel.RoleId != null ? studyUserModel.RoleId : null
                };
                await _context.StudyUsers.AddAsync(user);

                var study = await _context.Studies.FirstOrDefaultAsync(x => x.Id == studyUserModel.StudyId);
                study.UpdatedAt = DateTimeOffset.Now;

                var result = await _context.SaveCoreContextAsync(studyUserModel.UserId, DateTimeOffset.Now) > 0;

                if (!result)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }

                var userSites = studyUserModel.SiteIds.Select(x => new StudyUserSite
                {
                    StudyUserId = user.Id,
                    SiteId = x,
                    TenantId = studyUserModel.TenantId
                });

                if (userSites.Count() > 0)
                {
                    await _context.StudyUserSites.AddRangeAsync(userSites);
                }

                var result1 = await _context.SaveCoreContextAsync(studyUserModel.UserId, DateTimeOffset.Now) > -1;

                if (result1)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            else
            {
                var user = await _context.StudyUsers.Where(x => x.Id == studyUserModel.StudyUserId).Include(x => x.StudyRole).Include(x => x.StudyUserSites).FirstOrDefaultAsync();

                if (user != null)
                {

                    if (JsonConvert.SerializeObject(studyUserModel.ResponsiblePersonIds) != user.SuperUserIdList)
                    {
                        user.SuperUserIdList = studyUserModel.ResponsiblePersonIds.Count > 0 ? JsonConvert.SerializeObject(studyUserModel.ResponsiblePersonIds) : "";
                    }

                    if (user.StudyRoleId != studyUserModel.RoleId)
                    {
                        user.StudyRoleId = studyUserModel.RoleId != 0 && studyUserModel.RoleId != null ? studyUserModel.RoleId : null;
                        _context.StudyUsers.Update(user);
                    }

                    var currentSiteIds = user.StudyUserSites.Where(x => x.IsActive && !x.IsDeleted).Select(s => s.SiteId).ToList();
                    var newSiteIds = studyUserModel.SiteIds.ToList();

                    if (!currentSiteIds.SequenceEqual(newSiteIds))
                    {
                        _context.StudyUserSites.RemoveRange(user.StudyUserSites);

                        foreach (var siteId in newSiteIds)
                        {
                            var newUserSite = new StudyUserSite
                            {
                                StudyUserId = user.Id,
                                SiteId = siteId,
                                TenantId = user.TenantId
                            };
                            await _context.StudyUserSites.AddAsync(newUserSite);
                        }
                    }

                    var study = await _context.Studies.FirstOrDefaultAsync(x => x.Id == studyUserModel.StudyId);
                    study.UpdatedAt = DateTimeOffset.Now;

                    var result = await _context.SaveCoreContextAsync(studyUserModel.UserId, DateTimeOffset.Now);

                    if (result > 0)
                    {
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = true,
                            Message = "Successful"
                        };
                    }
                    else if (result == 0)
                    {
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = false,
                            Message = "No changes were made. Please make changes to save."
                        };
                    }
                    else
                    {
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = false,
                            Message = "Unsuccessful"
                        };
                    }
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "An unexpected error occurred."
                    };
                }
            }
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> ActivePassiveStudyUser(StudyUserModel studyUserModel)
        {
            if (studyUserModel.StudyUserId != 0)
            {
                var user = await _context.StudyUsers.Where(x => x.Id == studyUserModel.StudyUserId).Include(x => x.StudyUserSites).FirstOrDefaultAsync();

                if (user != null)
                {
                    foreach (var site in user.StudyUserSites.Where(x => !studyUserModel.IsActive ? !x.IsActive && !x.IsDeleted : x.IsActive && !x.IsDeleted))
                    {
                        site.IsActive = !studyUserModel.IsActive;
                    }

                    user.IsActive = !studyUserModel.IsActive;
                }

                var study = await _context.Studies.FirstOrDefaultAsync(x => x.Id == studyUserModel.StudyId);
                study.UpdatedAt = DateTimeOffset.Now;

                var result = await _context.SaveCoreContextAsync(studyUserModel.UserId, DateTimeOffset.Now);

                if (result > 0)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            else
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };
            }
        }
        [HttpPost]
        public async Task<ApiResponse<dynamic>> ActivePassiveByAuthUserId(Int64 authUserId, Int64 tenantId)
        {
            BaseDTO baseDTO = Request.Headers.GetBaseInformation();
            if (authUserId != 0 && tenantId != 0)
            {
                var users = await _context.StudyUsers.Where(x => x.AuthUserId == authUserId && x.TenantId == tenantId && !x.IsDeleted).Include(x => x.StudyUserSites).ToListAsync();

                if (users.Count() != 0)
                {
                    foreach (var user in users)
                    {
                        foreach (var site in user.StudyUserSites.Where(x => !user.IsActive ? !x.IsActive && !x.IsDeleted : x.IsActive && !x.IsDeleted))
                        {
                            site.IsActive = !user.IsActive;
                        }

                        user.IsActive = !user.IsActive;
                    }
                }

                var studys = await _context.Studies.Where(x => users.Select(t=>t.StudyId).Contains(x.Id)).ToListAsync();
                foreach (var study in studys)
                {
                    study.UpdatedAt = DateTimeOffset.Now;
                }
               
                var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now);

                if (result > 0)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            else
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };
            }
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> ActivePassiveStudyUsers(StudyUserModel studyUserModel)
        {
            if (studyUserModel.StudyId != 0)
            {
                var users = await _context.StudyUsers.Where(x => x.StudyId == studyUserModel.StudyId && !x.IsDeleted).Include(x => x.StudyUserSites).ToListAsync();

                if (users.Count > 0)
                {
                    users.ForEach(x =>
                    {
                        foreach (var site in x.StudyUserSites.Where(x => !studyUserModel.IsActive ? !x.IsActive && !x.IsDeleted : x.IsActive && !x.IsDeleted))
                        {
                            site.IsActive = !studyUserModel.IsActive;
                        }

                        x.IsActive = !studyUserModel.IsActive;
                    });
                }

                var study = await _context.Studies.FirstOrDefaultAsync(x => x.Id == studyUserModel.StudyId);
                study.UpdatedAt = DateTimeOffset.Now;

                var result = await _context.SaveCoreContextAsync(studyUserModel.UserId, DateTimeOffset.Now);

                if (result > 0)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else if (result == 0)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "All users are passive."
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            else
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };
            }
        }

        [HttpPost]
        public async Task<ApiResponse<DeleteStudyUserDTO>> DeleteStudyUser(StudyUserModel studyUserModel)
        {
            if (studyUserModel.StudyUserId != 0)
            {
                var user = await _context.StudyUsers.Where(x => x.Id == studyUserModel.StudyUserId).Include(x => x.StudyUserSites).FirstOrDefaultAsync();

                if (user != null)
                {
                    _context.StudyUserSites.RemoveRange(user.StudyUserSites);
                    _context.StudyUsers.Remove(user);
                }

                var study = await _context.Studies.FirstOrDefaultAsync(x => x.Id == studyUserModel.StudyId);
                study.UpdatedAt = DateTimeOffset.Now;

                var result = await _context.SaveCoreContextAsync(studyUserModel.UserId, DateTimeOffset.Now);

                if (result > 0)
                {
                    bool count = await _context.StudyUsers.AnyAsync(x => x.IsActive && !x.IsDeleted && x.AuthUserId == studyUserModel.AuthUserId);

                    return new ApiResponse<DeleteStudyUserDTO>
                    {
                        IsSuccess = true,
                        Message = "Successful",
                        Values = new DeleteStudyUserDTO { AuthDelete = !count }
                    };
                }
                else
                {
                    return new ApiResponse<DeleteStudyUserDTO>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            else
            {
                return new ApiResponse<DeleteStudyUserDTO>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };
            }
        }

        [HttpGet]
        public async Task<bool> StudyUserActiveControl(Int64 authUserId)
        {
            return await _context.StudyUsers.AnyAsync(x => x.IsActive && !x.IsDeleted && x.AuthUserId == authUserId);
        }
        #endregion

        #region Tenant User
        [HttpGet]
        public async Task<List<TenantUserDTO>> GetTenantUsers(Int64 tenantId)
        {
            var aa = await _context.StudyUsers.Where(x => x.TenantId == tenantId && !x.IsDeleted).Include(x => x.Study).Include(x => x.Study).Include(x => x.StudyRole).AsNoTracking().ToListAsync();
            return await _context.StudyUsers.Where(x => x.TenantId == tenantId && !x.IsDeleted).Include(x => x.Study).Include(x => x.Study).Include(x => x.StudyRole).AsNoTracking().Select(x => new TenantUserDTO
            {
                StudyUserId = x.Id,
                AuthUserId = x.AuthUserId,
                StudyId = x.StudyId,
                IsActive = x.IsActive,
                StudyName = x.Study.StudyName,
                UserRoleName = x.StudyRole.Name,
                StudyDemoLive = x.Study.IsDemo,
                CreatedOn = x.CreatedAt,
                LastUpdatedOn = x.UpdatedAt
            }).ToListAsync();
        }
        #endregion

        #region SSO
        [HttpGet]
        public async Task<List<Int64>> GetUserStudyIds(Int64 userId)
        {
            return await _context.StudyUsers.Where(x => x.IsActive && !x.IsDeleted && x.AuthUserId == userId).Select(x => x.StudyId).ToListAsync();
        }

        [HttpGet]
        public async Task<int> GetUserStudyCount(Int64 userId)
        {
            return await _context.StudyUsers.CountAsync(x => x.IsActive && !x.IsDeleted && x.AuthUserId == userId);
        }

        [HttpGet]
        public async Task<List<Int64>> GetUserTenantList(Int64 userId)
        {
            return await _context.StudyUsers.Where(x => x.IsActive && !x.IsDeleted && x.AuthUserId == userId).Select(x => x.TenantId).ToListAsync();
        }

        [HttpGet]
        public async Task<List<SSOUserStudyModel>> GetUserStudiesList(Int64 tenantId, Int64 userId)
        {
            return await _context.StudyUsers.Where(x => x.IsActive && !x.IsDeleted && x.AuthUserId == userId && x.TenantId == tenantId).Include(x => x.Study).Include(x => x.StudyRole).Select(x => new SSOUserStudyModel
            {
                UserId = userId,
                TenantId = tenantId,
                StudyId = x.StudyId,
                StudyName = x.Study.StudyName,
                UserRoleName = x.StudyRole.Name,
                Statu = x.Study.IsDemo ? "1" : "0"
            }).ToListAsync();
        }
        #endregion
    }
}
