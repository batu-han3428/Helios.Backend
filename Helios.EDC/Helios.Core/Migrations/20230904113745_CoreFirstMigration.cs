using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helios.Core.Migrations
{
    /// <inheritdoc />
    public partial class CoreFirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ElementDetails",
                columns: table => new
                {
                    ElementKey = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RowIndex = table.Column<int>(type: "int", nullable: false),
                    ColunmIndex = table.Column<int>(type: "int", nullable: false),
                    StudyVisitModuleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    VisitPageId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ElementQueryId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsQuery = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsRemoteSdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SdvState = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    IsComment = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    QueryState = table.Column<int>(type: "int", nullable: false),
                    IsMarkedNull = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HasDataEntryPermission = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ParentRowIndex = table.Column<int>(type: "int", nullable: false),
                    ParentColumnIndex = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ParentElementType = table.Column<int>(type: "int", nullable: false),
                    ParentElementEProPageNumber = table.Column<int>(type: "int", nullable: false),
                    CanRemoteSdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanQuery = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanInputAuditView = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanMarkAsNull = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanRandomize = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ViewRandomize = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DoubleEntryCanQuery = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DoubleEntryCanAnswerQuery = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PageOrder = table.Column<int>(type: "int", nullable: false),
                    IsShowOnScreen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanUNKSee = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MedraCoding = table.Column<int>(type: "int", nullable: false),
                    LabId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CustomCodingTagId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SelectedLabGuid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MetaDataUID = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MetaDataTags = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EProPageNumber = table.Column<int>(type: "int", nullable: false),
                    ButtonText = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DefaultValue = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserValue = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsHidden = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HasPdfForm = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LabParamName = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MarkedAsNull = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Icon = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementDetails", x => x.ElementKey);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AddedById = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UpdatedById = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "Studies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ReferansKey = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    VersiyonKey = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EquivalentStudyId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    StudyType = table.Column<int>(type: "int", nullable: false),
                    AskInitial = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDemo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Code = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShortName = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubDescription = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StudyLogoPath = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyLogoPath = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Language = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AddedById = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UpdatedById = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Studies_Studies_EquivalentStudyId",
                        column: x => x.EquivalentStudyId,
                        principalTable: "Studies",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "SystemAuditTrails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Changer = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientIp = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SystemAuditChangeType = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Details = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NewValue = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OldValue = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AddedById = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UpdatedById = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAuditTrails", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "Elements",
                columns: table => new
                {
                    ElementKey = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ModuleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ElementType = table.Column<int>(type: "int", nullable: false),
                    ElementName = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsTitleHidden = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomClass = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataAttributes = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Border = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Padding = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Margin = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TextColor = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BackColor = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AlignLayout = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    GridLayout = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    GridOffset = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TextAlign = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    FontSize = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    MainBorder = table.Column<int>(type: "int", nullable: false),
                    MainBorderStyle = table.Column<int>(type: "int", nullable: false),
                    MainBorderColor = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elements", x => x.ElementKey);
                    table.ForeignKey(
                        name: "FK_Elements_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "ModuleElementEvents",
                columns: table => new
                {
                    ModuleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    SourceElementKey = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TargetElementKey = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    JavascriptCode = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MainJsCode = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValueCondition = table.Column<int>(type: "int", nullable: false),
                    ActionResult = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModuleId1 = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleElementEvents", x => x.ModuleId);
                    table.ForeignKey(
                        name: "FK_ModuleElementEvents_Modules_ModuleId1",
                        column: x => x.ModuleId1,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StudyId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Country = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CountryCode = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaxEnrolmentCount = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AddedById = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UpdatedById = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sites_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "StudyRoleModulePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StudyId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Read = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Write = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SDV = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Query = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Freeze = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Lock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AddedById = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UpdatedById = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyRoleModulePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyRoleModulePermissions_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "StudyRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StudyId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Add = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    View = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Edit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchivePatient = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PatientStateChange = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Randomize = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ViewRandomization = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sign = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Lock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MarkAsNull = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    QueryView = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AutoQueryClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanFileView = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanFileUpload = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanFileDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanFileDownload = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    StudyFoldersView = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ExportData = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DashboardView = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InputAuditTrail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AERemove = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IwrsMarkAsRecieved = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IwrsTransfer = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ApproveSourceDocuments = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Monitoring = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ApproveAudit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Audit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSeeCycleAuditing = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSeeCycleMonitoring = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSeePatientAuditing = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSeePatientMonitoring = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSeeSiteAuditing = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSeeSiteMonitoring = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UploadAuditing = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UploadMonitoring = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RemoteSdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HasPageFreeze = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HasPageUnFreeze = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HasPageUnLock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SeePageActionAudit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanCode = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RemovePatient = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AEArchive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveMultiVisit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RemoveMultiVisit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DoubleDataCompare = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DoubleDataEntry = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DoubleDataReport = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DoubleDataViewAll = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DoubleDataDelete = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DoubleDataQuery = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DoubleDataAnswerQuery = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DashboardBuilderAdmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DashboardBuilderPivotExport = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DashboardBuilderSourceExport = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TmfAdmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TmfSiteUser = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TmfUser = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MriPage = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EConsentView = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ExportPatientForm = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AddAdverseEvent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AddMultiVisit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AddedById = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UpdatedById = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyRoles_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "StudyUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StudyId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AuthUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SuperUserIdList = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AddedById = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UpdatedById = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyUsers_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "StudyUserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StudyUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StudyRoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AddedById = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UpdatedById = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyUserRoles_StudyRoles_StudyRoleId",
                        column: x => x.StudyRoleId,
                        principalTable: "StudyRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyUserRoles_StudyUsers_StudyUserId",
                        column: x => x.StudyUserId,
                        principalTable: "StudyUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "StudyUserSites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StudyUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SiteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AddedById = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UpdatedById = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyUserSites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyUserSites_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyUserSites_StudyUsers_StudyUserId",
                        column: x => x.StudyUserId,
                        principalTable: "StudyUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Elements_ModuleId",
                table: "Elements",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleElementEvents_ModuleId1",
                table: "ModuleElementEvents",
                column: "ModuleId1");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_StudyId",
                table: "Sites",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Studies_EquivalentStudyId",
                table: "Studies",
                column: "EquivalentStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyRoleModulePermissions_StudyId",
                table: "StudyRoleModulePermissions",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyRoles_StudyId",
                table: "StudyRoles",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyUserRoles_StudyRoleId",
                table: "StudyUserRoles",
                column: "StudyRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyUserRoles_StudyUserId",
                table: "StudyUserRoles",
                column: "StudyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyUsers_StudyId",
                table: "StudyUsers",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyUserSites_SiteId",
                table: "StudyUserSites",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyUserSites_StudyUserId",
                table: "StudyUserSites",
                column: "StudyUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElementDetails");

            migrationBuilder.DropTable(
                name: "Elements");

            migrationBuilder.DropTable(
                name: "ModuleElementEvents");

            migrationBuilder.DropTable(
                name: "StudyRoleModulePermissions");

            migrationBuilder.DropTable(
                name: "StudyUserRoles");

            migrationBuilder.DropTable(
                name: "StudyUserSites");

            migrationBuilder.DropTable(
                name: "SystemAuditTrails");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "StudyRoles");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "StudyUsers");

            migrationBuilder.DropTable(
                name: "Studies");
        }
    }
}
