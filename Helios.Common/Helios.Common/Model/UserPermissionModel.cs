using System.ComponentModel.DataAnnotations;

namespace Helios.Common.Model
{
    public class UserPermissionModel
    {
        public bool CanSubjectAdd  { get; set; }
        public bool CanSubjectView { get; set; }
        public bool CanSubjectEdit { get; set; }
        public bool CanSubjectArchive { get; set; }
        public bool CanSubjectDelete { get; set; }
        public bool CanSubjectChangeState { get; set; }
        public bool CanSubjectRandomize { get; set; }
        public bool CanSubjectViewRandomization { get; set; }
        public bool CanSubjectViewEConsent { get; set; }
        public bool CanSubjectExportForm { get; set; }
        public bool CanSubjectSign { get; set; }
        public bool CanMonitoringSdv { get; set; }
        public bool CanMonitoringVerification { get; set; }
        public bool CanMonitoringRemoteSdv { get; set; }
        public bool CanMonitoringQueryView { get; set; }
        public bool CanMonitoringAutoQueryClosed { get; set; }
        public bool CanMonitoringPageLock { get; set; }
        public bool CanMonitoringPageUnLock { get; set; }
        public bool CanMonitoringPageFreeze { get; set; }
        public bool CanMonitoringPageUnFreeze { get; set; }
        public bool CanMonitoringSeePageActionAudit { get; set; }
        public bool CanMonitoringInputAuditTrail { get; set; }
        public bool CanMonitoringMarkAsNull { get; set; }
        public bool CanFormAddMultiVisit { get; set; }
        public bool CanFormArchiveMultiVisit { get; set; }
        public bool CanFormRemoveMultiVisit { get; set; }
        public bool CanFormAddAdverseEvent { get; set; }
        public bool CanFormAEArchive { get; set; }
        public bool CanFormAERemove { get; set; }
        public bool CanFileView { get; set; }
        public bool CanFileUpload { get; set; }
        public bool CanFileDownload { get; set; }
        public bool CanFileDelete { get; set; }
        public bool CanStudyFoldersView { get; set; }
        public bool CanViewFullStudyReport { get; set; }
        public bool CanViewStudyReports { get; set; }
        public bool CanViewQueryReport { get; set; }
        public bool CanViewCommentReport { get; set; }
        public bool CanViewFormDataReport { get; set; }
        public bool CanViewInputAuditTrailReport { get; set; }
        public bool CanViewMissingSdvDataReport { get; set; }
        public bool CanViewMissingDataReport { get; set; }
        public bool CanViewAdverseEventDetailReport { get; set; }
        public bool CanViewSeriousAdverseEventDetailReport { get; set; }
        public bool CanViewFormDetailReport { get; set; }
        public bool CanViewSubjectStateWithRandomization { get; set; }
        public bool CanViewMriFileReport { get; set; }
        public bool CanViewMissingDataSummary { get; set; }
        public bool CanViewRandomizationAuditTrailReport { get; set; }
        public bool CanViewRandomizationTreatmentGroupReport { get; set; }
        public bool CanViewCustomCodingReport { get; set; }
        public bool CanViewFileAttachmentDetailReport { get; set; }
        public bool CanViewMetadataReport { get; set; }
        public bool CanViewLocalLabReport { get; set; }
        public bool CanViewLockFreezeStatusReport { get; set; }
        public bool CanIwrsTransfer { get; set; }
        public bool CanIwrsMarkAsRecieved { get; set; }
        public bool CanMedicalCodingCanCode { get; set; }
        public bool CanDashboardBuilderAdmin { get; set; }
        public bool CanDashboardBuilderPivotExport { get; set; }
        public bool CanDashboardBuilderSourceExport { get; set; }
        public bool CanBeTMFAdmin { get; set; }
        public bool CanTMFViewDownload { get; set; }
        public bool CanTMFUpdate { get; set; }
        public bool CanTMFAddUpload { get; set; }
        public bool CanTMFAddPlaceholder { get; set; }
        public bool CanTMFViewAuditTrail { get; set; }
        public bool CanTMFDelete { get; set; }
        public bool CanTMFPreview { get; set; }
        public bool CanTMFRequest { get; set; }
        public bool CanTMFApproveRejectFile { get; set; }
        public bool CanTMFComment { get; set; }
        public bool CanTMFQualityApproval { get; set; }
        public bool CanTMFHistory { get; set; }
        public bool CanTMFViewFileStatus { get; set; }
        public bool CanTMFUnblinded { get; set; }
        public bool CanTMFShare { get; set; }
        
    }

    public class UserPermissionCacheModel
    {
        public Int64 StudyId { get; set; }
        public UserPermissionModel UserPermissionModel { get; set; }
    }
}