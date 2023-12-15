using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class UserPermissionDTO
    {
        public Int64 Id { get; set; }
        public Int64 StudyId { get; set; }

        [Required]
        public string RoleName { get; set; }
        public bool? Add { get; set; }
        public bool? View { get; set; }
        public bool? Edit { get; set; }
        public bool? ArchivePatient { get; set; }
        public bool? PatientStateChange { get; set; }
        public bool? Randomize { get; set; }
        public bool? ViewRandomization { get; set; }
        public bool? Sdv { get; set; }
        public bool? Sign { get; set; }
        public bool? Lock { get; set; }
        public bool? MarkAsNull { get; set; }
        public bool? QueryView { get; set; }
        public bool? AutoQueryClosed { get; set; }
        public bool? CanFileView { get; set; }
        public bool? CanFileUpload { get; set; }
        public bool? CanFileDeleted { get; set; }
        public bool? CanFileDownload { get; set; }
        public bool? StudyFoldersView { get; set; }
        public bool? ExportData { get; set; }
        public bool? DashboardView { get; set; }
        public bool? InputAuditTrail { get; set; }
        public bool? AERemove { get; set; }
        public bool? IwrsMarkAsRecieved { get; set; }
        public bool? IwrsTransfer { get; set; }
        public bool? ApproveSourceDocuments { get; set; }
        public bool? Monitoring { get; set; }
        public bool? ApproveAudit { get; set; }
        public bool? Audit { get; set; }
        public bool? CanSeeCycleAuditing { get; set; }
        public bool? CanSeeCycleMonitoring { get; set; }
        public bool? CanSeePatientAuditing { get; set; }
        public bool? CanSeePatientMonitoring { get; set; }
        public bool? CanSeeSiteAuditing { get; set; }
        public bool? CanSeeSiteMonitoring { get; set; }
        public bool? UploadAuditing { get; set; }
        public bool? UploadMonitoring { get; set; }
        public bool? RemoteSdv { get; set; }
        public bool? HasPageFreeze { get; set; }
        public bool? HasPageUnFreeze { get; set; }
        public bool? HasPageUnLock { get; set; }
        public bool? SeePageActionAudit { get; set; }
        public bool? CanCode { get; set; }
        public bool? RemovePatient { get; set; }
        public bool? AEArchive { get; set; }
        public bool? ArchiveMultiVisit { get; set; }
        public bool? RemoveMultiVisit { get; set; }
        public bool? DoubleDataCompare { get; set; }
        public bool? DoubleDataEntry { get; set; }
        public bool? DoubleDataReport { get; set; }
        public bool? DoubleDataViewAll { get; set; }
        public bool? DoubleDataDelete { get; set; }
        public bool? DoubleDataQuery { get; set; }
        public bool? DoubleDataAnswerQuery { get; set; }
        public bool? DashboardBuilderAdmin { get; set; }
        public bool? DashboardBuilderPivotExport { get; set; }
        public bool? DashboardBuilderSourceExport { get; set; }
        public bool? TmfAdmin { get; set; }
        public bool? TmfSiteUser { get; set; }
        public bool? TmfUser { get; set; }
        public bool? MriPage { get; set; }
        public bool? EConsentView { get; set; }
        public bool? ExportPatientForm { get; set; }
        public bool? AddAdverseEvent { get; set; }
        public bool? AddMultiVisit { get; set; }
    }
}
