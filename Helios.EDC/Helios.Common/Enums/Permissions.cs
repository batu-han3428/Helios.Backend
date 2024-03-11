using System.ComponentModel;

namespace Helios.Common.Enums
{
    public enum VisitPermission
    {
        Freeze = 1,
        Lock = 2,
        Signature = 3,
        SDV = 4,
        Query = 5,
        Verification = 6
    }

    public enum StudyRolePermission
    {
        [Description("Add")]
        Subject_Add = 100,
        [Description("View")]
        Subject_View = 101,
        [Description("Edit")]
        Subject_Edit = 102,
        [Description("Archive")]
        Subject_ArchivePatient = 103,
        [Description("Delete")]
        Subject_Delete = 104,
        [Description("Change state")]
        Subject_PatientStateChange = 105,
        [Description("Randomize")]
        Subject_Randomize = 106,
        [Description("View randomization")]
        Subject_ViewRandomization = 107,
        [Description("View e-Consent")]
        Subject_EConsentView = 108,
        [Description("Export subject form")]
        Subject_ExportPatientForm = 109,
        [Description("Signature")]
        Subject_Sign = 110,
        [Description("On-site SDV")]
        Monitoring_Sdv = 111,
        [Description("Verification")]
        Monitoring_Verification = 112,
        [Description("Remote SDV")]
        Monitoring_RemoteSdv = 113,
        [Description("Query")]
        Monitoring_QueryView = 114,
        [Description("Close auto query")]
        Monitoring_AutoQueryClosed = 115,
        [Description("Lock")]
        Monitoring_Lock = 116,
        [Description("Unlock")]
        Monitoring_HasPageUnLock = 117,
        [Description("Freeze")]
        Monitoring_HasPageFreeze = 118,
        [Description("Unfreeze")]
        Monitoring_HasPageUnFreeze = 119,
        [Description("Lock-Freeze audit trails")]
        Monitoring_SeePageActionAudit = 120,
        [Description("Input audit trail")]
        Monitoring_InputAuditTrail = 121,
        [Description("Missing data")]
        Monitoring_MarkAsNull = 122,
        [Description("Add multi-form")]
        Form_AddMultiVisit = 123,
        [Description("Archive multi-form")]
        Form_ArchiveMultiVisit = 124,
        [Description("Remove multi-form")]
        Form_RemoveMultiVisit = 125,
        [Description("Add adverse event")]
        Form_AddAdverseEvent = 126,
        [Description("Archive adverse event")]
        Form_AEArchive = 127,
        [Description("Remove adverse event")]
        Form_AERemove = 128,
        [Description("View")]
        FileUpload_CanFileView = 129,
        [Description("Upload")]
        FileUpload_CanFileUpload = 130,
        [Description("Download")]
        FileUpload_CanFileDownload = 131,
        [Description("Delete")]
        FileUpload_CanFileDeleted = 132,
        [Description("View")]
        StudyDocument_StudyFoldersView = 133,
        [Description("Subject state")]
        DashboardOld_SubjectState = 134,
        [Description("Query status")]
        DashboardOld_QueryStatus = 135,
        [Description("Randomization")]
        DashboardOld_Randomization = 136,
        [Description("SDV status")]
        DashboardOld_SdvStatus = 137,
        [Description("Total subject number")]
        DashboardOld_TotalSubjectNumber = 138,
        [Description("Randomization Total subject number by country/ site")]
        DashboardOld_RandomizationTotalSubjectNumberByCountrySite = 139,
        [Description("Full study report")]
        DataExport_FullStudyReport = 140,
        [Description("Study report")]
        DataExport_StudyReports = 141,
        [Description("Query report")]
        DataExport_QueryReport = 142,
        [Description("Comment report")]
        DataExport_CommentReport = 143,
        [Description("Form data report")]
        DataExport_FormDataReport = 144,
        [Description("Input audit trail report")]
        DataExport_InputAuditTrailReport = 145,
        [Description("Missing SDV data report")]
        DataExport_MissingSdvDataReport = 146,
        [Description("Missing data report")]
        DataExport_MissingDataReport = 147,
        [Description("Adverse event detail report")]
        DataExport_AdverseEventDetailReport = 148,
        [Description("Serious adverse event detail report")]
        DataExport_SeriousAdverseEventDetailReport = 149,
        [Description("Form detail report")]
        DataExport_FormDetailReport = 150,
        [Description("Subject state with randomization")]
        DataExport_SubjectStateWithRandomization = 151,
        [Description("MRI file report")]
        DataExport_MriFileReport = 152,
        [Description("Missing data summary report")]
        DataExport_MissingDataSummary = 153,
        [Description("Randomization audit trail report")]
        DataExport_RandomizationAuditTrailReport = 154,
        [Description("Randomization treatment group report")]
        DataExport_RandomizationTreatmentGroupReport = 155,
        [Description("Custom coding report")]
        DataExport_CustomCodingReport = 156,
        [Description("File attachment detail report")]
        DataExport_FileAttachmentDetailReport = 157,
        [Description("Metadata report")]
        DataExport_MetadataReport = 158,
        [Description("Local lab report")]
        DataExport_LocalLabReport = 159,
        [Description("Lock/Freeze status report")]
        DataExport_LockFreezeStatusReport = 160,
        [Description("Transfer")]
        IWRS_IwrsTransfer = 161,
        [Description("Receive")]
        IWRS_IwrsMarkAsRecieved = 162,
        [Description("Code")]
        MedicalCoding_CanCode = 163,
        [Description("Dashboard Admin")]
        Dashboard_DashboardBuilderAdmin = 164,
        [Description("Download Pivot")]
        Dashboard_DashboardBuilderPivotExport = 165,
        [Description("Download Source Data")]
        Dashboard_DashboardBuilderSourceExport = 166,
        [Description("Admin")]
        TMF_Admin = 167,
        [Description("View & Download")]
        TMF_ViewDownload = 168,
        [Description("Update")]
        TMF_Update = 169,
        [Description("Add & Upload")]
        TMF_AddUpload = 170,
        [Description("Add placeholder")]
        TMF_AddPlaceholder = 171,
        [Description("View audit trail")]
        TMF_ViewAuditTrail = 172,
        [Description("Delete")]
        TMF_Delete = 173,
        [Description("Preview")]
        TMF_Preview = 174,
        [Description("Request")]
        TMF_Request = 174,
        [Description("Approve & Reject file")]
        TMF_ApproveRejectFile = 175,
        [Description("Comment")]
        TMF_Comment = 176,
        [Description("Quality approval")]
        TMF_QualityApproval = 177,
        [Description("History")]
        TMF_History = 178,
        [Description("View file status")]
        TMF_ViewFileStatus = 179,
        [Description("Unblinded")]
        TMF_Unblinded = 180,
        [Description("Share")]
        TMF_Share = 181
    }
}
