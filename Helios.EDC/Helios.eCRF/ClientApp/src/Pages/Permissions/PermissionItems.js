const permissionItems = {
    Subject: [
        {
            label: "Add",
            name: "add"
        },
        {
            label: "View",
            name: "view"
        },
        {
            label: "Edit",
            name: "edit"
        },
        {
            label: "Archive",
            name: "archivePatient"
        },
        {
            label: "Delete",
            name: "removePatient"
        },
        {
            label: "Change state",
            name: "patientStateChange"
        },
        {
            label: "Randomize",
            name: "randomize"
        },
        {
            label: "View randomization",
            name: "viewRandomization"
        },
        {
            label: "View e-Consent",
            name: "eConsentView"
        },
        {
            label: "Export patient form",
            name: "exportPatientForm"
        },
        {
            label: "Signature",
            name: "sign"
        },
    ],
    Monitoring: [
        {
            label: "On-site SDV",
            name: "sdv"
        },
        {
            label: "Remote SDV",
            name: "remoteSdv"
        },
        {
            label: "Query",
            name: "queryView"
        },
        {
            label: "Close auto query",
            name: "autoQueryClosed"
        },
        {
            label: "Lock",
            name: "lock"
        },
        {
            label: "Unlock",
            name: "hasPageUnLock"
        },
        {
            label: "Freeze",
            name: "hasPageFreeze"
        },
        {
            label: "Unfreeze",
            name: "hasPageUnFreeze"
        },
        {
            label: "Lock-Freeze audit trails",
            name: "seePageActionAudit"
        },
        {
            label: "Input audit trail",
            name: "inputAuditTrail"
        },
        {
            label: "Mark as null",
            name: "markAsNull"
        },
    ],
    Form: [
        {
            label: "Add Multi-Form",
            name: "addMultiVisit"
        },
        {
            label: "Archive multi-form",
            name: "archiveMultiVisit"
        },
        {
            label: "Remove multi-form",
            name: "removeMultiVisit"
        },
        {
            label: "Add Adverse Event",
            name: "addAdverseEvent"
        },
        {
            label: "Archive Adverse Event",
            name: "aeArchive"
        },
        {
            label: "Remove Adverse Event",
            name: "aeRemove"
        },
    ],
    FileUpload: [
        {
            label: "View",
            name: "canFileView"
        },
        {
            label: "Upload",
            name: "canFileUpload"
        },
        {
            label: "Download",
            name: "canFileDownload"
        },
        {
            label: "Delete",
            name: "canFileDeleted"
        },
    ],
    StudyDocument: [
        {
            label: "View",
            name: "studyFoldersView"
        },
    ],
    DashboardOld: [
        {
            label: "Subject state",
            name: "subjectstate"
        },
        {
            label: "Query status",
            name: "querystatus"
        },
        {
            label: "Randomization",
            name: "randomization"
        },
        {
            label: "SDV status",
            name: "sdvstatus"
        },
        {
            label: "Total subject number",
            name: "totalsubjectnumber"
        },
        {
            label: "Randomization Total subject number by country/ site",
            name: "randomizationtotalsubjectnumberbycountrysite"
        },
    ],											
    DataExport:[
        {
            label: "Full study report",
            name: "fullstudyreport"
        },
        {
            label: "Study reports",
            name: "studyreports"
        },
        {
            label: "Query report",
            name: "queryreport"
        },
        {
            label: "Comment report",
            name: "commentreport"
        },
        {
            label: "Form data report",
            name: "formdatareport"
        },
        {
            label: "Input audit trail report",
            name: "inputaudittrailreport"
        },
        {
            label: "Missing SDV data report",
            name: "missingsdvdatareport"
        },
        {
            label: "Missing data report",
            name: "missingdatareport"
        },
        {
            label: "Adverse Event detail report",
            name: "adverseeventdetailreport"
        },
        {
            label: "Serious Adverse Event detail report",
            name: "seriousadverseeventdetailreport"
        },
        {
            label: "Form detail report",
            name: "formdetailreport"
        },
        {
            label: "Subject state with randomization",
            name: "subjectstatewithrandomization"
        },
        {
            label: "MRI file report",
            name: "mrifilereport"
        },
        {
            label: "Missing data summary",
            name: "missingdatasummary"
        },
        {
            label: "Randomization audit trail report",
            name: "randomizationaudittrailreport"
        },
        {
            label: "Randomization treatment group report",
            name: "randomizationtreatmentgroupreport"
        },
        {
            label: "Custom coding report",
            name: "customcodingreport"
        },
        {
            label: "File Attachment Detail Report",
            name: "fileattachmentdetailreport"
        },
        {
            label: "Metadata report",
            name: "metadatareport"
        },
        {
            label: "Local lab report",
            name: "locallabreport"
        },
        {
            label: "Lock/ Freeze status report",
            name: "lockfreezestatusreport"
        },
    ],																																											
    IWRS:[
        {
            label: "Transfer",
            name: "iwrsTransfer"
        },
        {
            label: "Receive",
            name: "iwrsMarkAsRecieved"
        },
    ],											
    MedicalCoding:[
        {
            label: "Code",
            name: "canCode"
        },
    ],
    Dashboard:[
        {
            label: "Dashboard Admin",
            name: "dashboardBuilderAdmin"
        },
        {
            label: "Download Pivot",
            name: "dashboardBuilderPivotExport"
        },
        {
            label: "Download Source Data",
            name: "dashboardBuilderSourceExport"
        },
    ],
    TMF:[
        {
            label: "Admin",
            name: "tmfAdmin"
        },
        {
            label: "View & Download",
            name: "viewdownload"
        },
        {
            label: "Update",
            name: "update"
        },
        {
            label: "Add & Upload",
            name: "addupload"
        },
        {
            label: "Add place holder",
            name: "addplaceholder"
        },
        {
            label: "View Audit Trail",
            name: "viewaudittrail"
        },
        {
            label: "Delete",
            name: "delete"
        },
        {
            label: "Preview",
            name: "preview"
        },
        {
            label: "Request",
            name: "request"
        },
        {
            label: "Approve & Reject file",
            name: "approverejectfile"
        },
        {
            label: "Comment",
            name: "comment"
        },
        {
            label: "Quality approval",
            name: "qualityapproval"
        },
        {
            label: "History",
            name: "history"
        },
        {
            label: "View file status",
            name: "viewfilestatus"
        },
        {
            label: "Unblinded",
            name: "unblinded"
        },
        {
            label: "Share",
            name: "share"
        },
    ],
};

export default permissionItems;