﻿using Helios.Common.Domains.Core.Entities;
using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.Core.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Helios.Core.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CoreUserController: Controller
    {
        private CoreContext _context;

        public CoreUserController(CoreContext context)
        {
            _context = context;
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
            return await _context.Studies.Where(x => tenantIdsInt.Contains(x.TenantId) && !x.IsDemo).GroupBy(s => s.TenantId).Select(g => new TenantModel
            {
                Id = g.Key,
                ActiveStudies = g.Count().ToString()
            }).ToListAsync();
        }

        [HttpGet]
        public async Task<int> GetStudyCount(Int64? tenantId)
        {
            return await _context.Studies.Where(x => x.TenantId == tenantId && !x.IsDemo).CountAsync();
        }
        #endregion

        #region Permissions
        [HttpGet]
        public async Task<List<UserPermissionDTO>> GetPermissionRoleList(Int64 studyId)
        {
            //auto mapper kullanılmalı. somi hanım kurdum demişti o yüzden kurmuyorum. fakat nereye kurdu bir saat bakmadım. kendisiyle konuşucam. o zaman düzeltiriz burayı.
            var result = await _context.StudyRoles.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == studyId).AsNoTracking().Select(x=> new UserPermissionDTO
            {
                Id = x.Id,
                StudyId = x.StudyId,
                RoleName = x.Name,
                Add = x.Add,
                View = x.View,
                Edit = x.Edit,
                ArchivePatient = x.ArchivePatient,
                PatientStateChange = x.PatientStateChange,
                Randomize = x.Randomize,
                ViewRandomization = x.ViewRandomization,
                Sdv = x.Sdv,
                Sign = x.Sign,
                Lock = x.Lock,
                MarkAsNull = x.MarkAsNull,
                QueryView = x.QueryView,
                AutoQueryClosed = x.AutoQueryClosed,
                CanFileView = x.CanFileView,
                CanFileUpload = x.CanFileUpload,
                CanFileDeleted = x.CanFileDeleted,
                CanFileDownload = x.CanFileDownload,
                StudyFoldersView = x.StudyFoldersView,
                ExportData = x.ExportData,
                DashboardView = x.DashboardView,
                InputAuditTrail = x.InputAuditTrail,
                AERemove = x.AERemove,
                IwrsMarkAsRecieved = x.IwrsMarkAsRecieved,
                IwrsTransfer = x.IwrsTransfer,
                ApproveSourceDocuments = x.ApproveSourceDocuments,
                Monitoring = x.Monitoring,
                ApproveAudit = x.ApproveAudit,
                Audit = x.Audit,
                CanSeeCycleAuditing = x.CanSeeCycleAuditing,
                CanSeeCycleMonitoring = x.CanSeeCycleMonitoring,
                CanSeePatientAuditing = x.CanSeePatientAuditing,
                CanSeePatientMonitoring = x.CanSeePatientMonitoring,
                CanSeeSiteAuditing = x.CanSeeSiteAuditing,
                CanSeeSiteMonitoring = x.CanSeeSiteMonitoring,
                UploadAuditing = x.UploadAuditing,
                UploadMonitoring = x.UploadMonitoring,
                RemoteSdv = x.RemoteSdv,
                HasPageFreeze = x.HasPageFreeze,
                HasPageUnFreeze = x.HasPageUnFreeze,
                HasPageUnLock = x.HasPageUnLock,
                SeePageActionAudit = x.SeePageActionAudit,
                CanCode = x.CanCode,
                RemovePatient = x.RemovePatient,
                AEArchive = x.AEArchive,
                ArchiveMultiVisit = x.ArchiveMultiVisit,
                RemoveMultiVisit = x.RemoveMultiVisit,
                DoubleDataCompare = x.DoubleDataCompare,
                DoubleDataEntry = x.DoubleDataEntry,
                DoubleDataReport = x.DoubleDataReport,
                DoubleDataViewAll = x.DoubleDataViewAll,
                DoubleDataDelete = x.DoubleDataDelete,
                DoubleDataQuery = x.DoubleDataQuery,
                DoubleDataAnswerQuery = x.DoubleDataAnswerQuery,
                DashboardBuilderAdmin = x.DashboardBuilderAdmin,
                DashboardBuilderPivotExport = x.DashboardBuilderPivotExport,
                DashboardBuilderSourceExport = x.DashboardBuilderSourceExport,
                TmfAdmin = x.TmfAdmin,
                TmfSiteUser = x.TmfSiteUser,
                TmfUser = x.TmfUser,
                MriPage = x.MriPage,
                EConsentView = x.EConsentView,
                ExportPatientForm = x.ExportPatientForm,
                AddAdverseEvent = x.AddAdverseEvent,
                AddMultiVisit = x.AddMultiVisit
            }).ToListAsync();

            return result;
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
            var userId = Request.Headers["Authorization"];
            var studyId = Request.Headers["StudyId"];
            var tenantId = Request.Headers["TenantId"];

            var permission = await _context.Permissions.FirstOrDefaultAsync(x => x.StudyRoleId == 1 && x.StudyId == 1 && x.PermissionName == dto.PermissionKey);

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
                    PermissionName = dto.PermissionKey,
                    StudyId = Convert.ToInt64(studyId),
                    TenantId = Convert.ToInt64(tenantId)
                });
            }

            var result = await _context.SaveCoreContextAsync(Convert.ToInt64(userId), DateTimeOffset.Now) > 0;

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

        [HttpPost]
        public async Task<ApiResponse<dynamic>> AddOrUpdatePermissionRol(UserPermissionModel userPermission)
        {
            var userPermissionCheck = await _context.StudyRoles.FirstOrDefaultAsync(p => p.Name == userPermission.RoleName && p.IsActive && !p.IsDeleted && p.StudyId == userPermission.StudyId);
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
                studyRole.StudyId = userPermission.StudyId;
                studyRole.Name = userPermission.RoleName;
                studyRole.NewRole();
                await _context.StudyRoles.AddAsync(studyRole);
                var result = await _context.SaveCoreContextAsync(userPermission.UserId, DateTimeOffset.Now) > 0;

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

                    var result = await _context.SaveCoreContextAsync(userPermission.UserId, DateTimeOffset.Now) > 0;

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
        public async Task<ApiResponse<dynamic>> DeleteRole(UserPermissionModel userPermission)
        {
            var oldEntity = await _context.StudyRoles.FirstOrDefaultAsync(p => p.Id == userPermission.Id);

            if (oldEntity == null)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "No record to delete was found."
                };
            }

            _context.Remove(oldEntity);

            var result = await _context.SaveCoreContextAsync(userPermission.UserId, DateTimeOffset.Now) > 0;

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
            return await _context.StudyUsers.Where(x => x.StudyId == studyId && !x.IsDeleted).Include(x => x.StudyRole).Include(x=>x.StudyUserSites).AsNoTracking().Select(x => new StudyUserDTO
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
        public async Task<bool> GetCheckStudyUser(Int64 authUserId, Int64 studyId)
        {
            return await _context.StudyUsers.AnyAsync(x => x.StudyId == studyId && x.AuthUserId == authUserId && !x.IsDeleted);
        }

        [HttpGet]
        public async Task<List<Int64>> GetStudyUserIds(Int64 studyId)
        {
            return await _context.StudyUsers.Where(x => x.StudyId == studyId && !x.IsDeleted).Select(x=>x.AuthUserId).ToListAsync();
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
                    StudyRoleId = studyUserModel.RoleId != 0 && studyUserModel.RoleId != null? studyUserModel.RoleId : null
                };
                await _context.StudyUsers.AddAsync(user);

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
                    SiteId = x
                });

                if (userSites.Count() > 0)
                {
                    await _context.StudyUserSites.AddRangeAsync(userSites);
                }

                var result1 = await _context.SaveCoreContextAsync(studyUserModel.UserId, DateTimeOffset.Now) > 0;

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
                   
                    var currentSiteIds = user.StudyUserSites.Where(x=>x.IsActive && !x.IsDeleted).Select(s => s.SiteId).ToList();
                    var newSiteIds = studyUserModel.SiteIds.ToList();

                    if (!currentSiteIds.SequenceEqual(newSiteIds))
                    {        
                        _context.StudyUserSites.RemoveRange(user.StudyUserSites);

                        foreach (var siteId in newSiteIds)
                        {
                            var newUserSite = new StudyUserSite
                            {
                                StudyUserId = user.Id,
                                SiteId = siteId
                            };
                            await _context.StudyUserSites.AddAsync(newUserSite);
                        }
                    }

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
                var user = await _context.StudyUsers.Where(x=>x.Id == studyUserModel.StudyUserId).Include(x=>x.StudyUserSites).FirstOrDefaultAsync();

                if (user != null)
                {
                    foreach (var site in user.StudyUserSites.Where(x=> !studyUserModel.IsActive ? !x.IsActive && !x.IsDeleted : x.IsActive && !x.IsDeleted))
                    {
                        site.IsActive = !studyUserModel.IsActive;
                    }

                    user.IsActive = !studyUserModel.IsActive;
                }

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

                var result = await _context.SaveCoreContextAsync(studyUserModel.UserId, DateTimeOffset.Now);

                if (result > 0)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else if(result == 0)
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
            return await _context.StudyUsers.Where(x => x.TenantId == tenantId && !x.IsDeleted).Include(x=>x.Study).AsNoTracking().Select(x => new TenantUserDTO
            {
                StudyUserId = x.Id,
                AuthUserId = x.AuthUserId,
                StudyId = x.StudyId,
                IsActive = x.IsActive,
                StudyName = x.Study.StudyName,
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
            return await _context.StudyUsers.Where(x => x.IsActive && !x.IsDeleted && x.AuthUserId == userId).Select(x=>x.StudyId).ToListAsync();
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
