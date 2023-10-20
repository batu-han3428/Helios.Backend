using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.Core.Contexts;
using Helios.Core.Domains.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Xml.Linq;

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

        #region Permissions
        [HttpGet]
        public async Task<List<UserPermissionDTO>> GetPermissionRoleList(Guid studyId)
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

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetPermission(SetPermissionModel setPermissionModel)
        {
            var studyRole = await _context.StudyRoles.FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted && x.Id == setPermissionModel.Id);

            if (studyRole != null)
            {
                var propertyName = setPermissionModel.PermissionName;
                var property = typeof(StudyRole).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null && property.PropertyType == typeof(bool))
                {
                    property.SetValue(studyRole, setPermissionModel.Value);
                    var result = await _context.SaveAuthenticationContextAsync(setPermissionModel.UserId, DateTimeOffset.Now) > 0;

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
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "An unexpected error occurred."
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
            else if (userPermission.Id == Guid.Empty)
            {
                StudyRole studyRole = new StudyRole();
                studyRole.StudyId = userPermission.StudyId;
                studyRole.Name = userPermission.RoleName;
                studyRole.NewRole();
                await _context.StudyRoles.AddAsync(studyRole);
                var result = await _context.SaveAuthenticationContextAsync(userPermission.UserId, DateTimeOffset.Now) > 0;

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

                    var result = await _context.SaveAuthenticationContextAsync(userPermission.UserId, DateTimeOffset.Now) > 0;

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

            var result = await _context.SaveAuthenticationContextAsync(userPermission.UserId, DateTimeOffset.Now) > 0;

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
    }
}
