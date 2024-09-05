using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helios.Core.Migrations
{
    /// <inheritdoc />
    public partial class Migration141223 : Migration
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ElementId = table.Column<long>(type: "bigint", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: false),
                    RowIndex = table.Column<int>(type: "int", nullable: false),
                    ColunmIndex = table.Column<int>(type: "int", nullable: false),
                    CanQuery = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanRemoteSdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanComment = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanDataEntry = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ParentElementEProPageNumber = table.Column<int>(type: "int", nullable: false),
                    MetaDataTags = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EProPageNumber = table.Column<int>(type: "int", nullable: false),
                    ButtonText = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DefaultValue = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Unit = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LowerLimit = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpperLimit = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Extension = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mask = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Layout = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    StartDay = table.Column<int>(type: "int", nullable: false),
                    EndDay = table.Column<int>(type: "int", nullable: false),
                    StartMonth = table.Column<int>(type: "int", nullable: false),
                    EndMonth = table.Column<int>(type: "int", nullable: false),
                    EndYear = table.Column<int>(type: "int", nullable: false),
                    StartYear = table.Column<int>(type: "int", nullable: false),
                    AddTodayDate = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ElementOptions = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetElementId = table.Column<long>(type: "bigint", nullable: false),
                    LeftText = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RightText = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementDetails", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "MailTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TemplateBody = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TemplateType = table.Column<int>(type: "int", nullable: false),
                    StudyId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalMails = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTemplates", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "MailTemplateTags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Tag = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TemplateType = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTemplateTags", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "MultipleChoiceTag",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Key = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleChoiceTag", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "Studies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReferenceKey = table.Column<long>(type: "bigint", nullable: false),
                    VersionKey = table.Column<long>(type: "bigint", nullable: false),
                    EquivalentStudyId = table.Column<long>(type: "bigint", nullable: true),
                    StudyState = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StudyType = table.Column<int>(type: "int", nullable: false),
                    AskSubjectInitial = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDemo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReasonForChange = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ProtocolCode = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    
                    Description = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubDescription = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StudyLogoPath = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyLogoPath = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StudyLanguage = table.Column<int>(type: "int", nullable: false),
                    StudyName = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
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
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAuditTrails", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "MailTemplatesRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MailTemplateId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTemplatesRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailTemplatesRoles_MailTemplates_MailTemplateId",
                        column: x => x.MailTemplateId,
                        principalTable: "MailTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "Elements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ModuleId = table.Column<long>(type: "bigint", nullable: false),
                    ElementDetailId = table.Column<long>(type: "bigint", nullable: true),
                    ElementType = table.Column<int>(type: "int", nullable: false),
                    ElementName = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsTitleHidden = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Width = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    IsHidden = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsRequired = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDependent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsReadonly = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanMissing = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Elements_ElementDetails_ElementDetailId",
                        column: x => x.ElementDetailId,
                        principalTable: "ElementDetails",
                        principalColumn: "Id");
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ModuleId = table.Column<long>(type: "bigint", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    SourceElementId = table.Column<long>(type: "bigint", nullable: false),
                    TargetElementId = table.Column<long>(type: "bigint", nullable: false),
                    ValueCondition = table.Column<int>(type: "int", nullable: false),
                    ActionValue = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleElementEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleElementEvents_Modules_ModuleId",
                        column: x => x.ModuleId,
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Country = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CountryCode = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaxEnrolmentCount = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
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
                name: "StudyRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyId = table.Column<long>(type: "bigint", nullable: false),
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
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
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
                name: "StudyVisits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyId = table.Column<long>(type: "bigint", nullable: false),
                    ReferenceKey = table.Column<long>(type: "bigint", nullable: false),
                    VersionKey = table.Column<long>(type: "bigint", nullable: false),
                    VisitType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CanFreeze = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanLock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSign = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanVerify = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanQuery = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SAELockHour = table.Column<int>(type: "int", nullable: false),
                    SAELockAction = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyVisits_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SiteId = table.Column<long>(type: "bigint", nullable: false),
                    StudyId = table.Column<long>(type: "bigint", nullable: false),
                    InitialName = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubjectNumber = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataStatus = table.Column<int>(type: "int", nullable: false),
                    ValidationStatus = table.Column<int>(type: "int", nullable: false),
                    SubjectStatus = table.Column<int>(type: "int", nullable: false),
                    Signature = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Lock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Freeze = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RandomData = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RandomDataDate = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UserValueUpdateDate = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subjects_Studies_StudyId",
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyRoleId = table.Column<long>(type: "bigint", nullable: false),
                    StudyVisitPageModuleId = table.Column<long>(type: "bigint", nullable: false),
                    Read = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Write = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SDV = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Query = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Freeze = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Lock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyRoleModulePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyRoleModulePermissions_StudyRoles_StudyRoleId",
                        column: x => x.StudyRoleId,
                        principalTable: "StudyRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "StudyUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyId = table.Column<long>(type: "bigint", nullable: false),
                    AuthUserId = table.Column<long>(type: "bigint", nullable: false),
                    SuperUserIdList = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StudyRoleId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_StudyUsers_StudyRoles_StudyRoleId",
                        column: x => x.StudyRoleId,
                        principalTable: "StudyRoles",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "StudyVisitPages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyVisitId = table.Column<long>(type: "bigint", nullable: false),
                    ReferenceKey = table.Column<long>(type: "bigint", nullable: false),
                    VersionKey = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CanFreeze = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanLock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSign = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanVerify = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanQuery = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyVisitPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyVisitPages_StudyVisits_StudyVisitId",
                        column: x => x.StudyVisitId,
                        principalTable: "StudyVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "SubjectVisits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyVisitId = table.Column<long>(type: "bigint", nullable: false),
                    SubjectId = table.Column<long>(type: "bigint", nullable: false),
                    ParentSubjectVisitId = table.Column<long>(type: "bigint", nullable: false),
                    RelatedSubjectVisitId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Freeze = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Lock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Signature = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Query = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SAELockStatus = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Verification = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FormNo = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FormName = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RowIndex = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectVisits_StudyVisits_StudyVisitId",
                        column: x => x.StudyVisitId,
                        principalTable: "StudyVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectVisits_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "StudyUserSites",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyUserId = table.Column<long>(type: "bigint", nullable: false),
                    SiteId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "StudyVisitPageModules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyVisitPageId = table.Column<long>(type: "bigint", nullable: false),
                    StudyRoleModulePermissionId = table.Column<long>(type: "bigint", nullable: false),
                    ReferenceKey = table.Column<long>(type: "bigint", nullable: false),
                    VersionKey = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CanFreeze = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanLock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSign = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanVerify = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanQuery = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyVisitPageModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyVisitPageModules_StudyRoleModulePermissions_StudyRoleMo~",
                        column: x => x.StudyRoleModulePermissionId,
                        principalTable: "StudyRoleModulePermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyVisitPageModules_StudyVisitPages_StudyVisitPageId",
                        column: x => x.StudyVisitPageId,
                        principalTable: "StudyVisitPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "SubjectVisitPages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SubjectVisitId = table.Column<long>(type: "bigint", nullable: false),
                    StudyVisitPageId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Freeze = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Lock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sign = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Query = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Verification = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectVisitPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectVisitPages_StudyVisitPages_StudyVisitPageId",
                        column: x => x.StudyVisitPageId,
                        principalTable: "StudyVisitPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectVisitPages_SubjectVisits_SubjectVisitId",
                        column: x => x.SubjectVisitId,
                        principalTable: "SubjectVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "StudyVisitPageModuleElements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyVisitPageModuleId = table.Column<long>(type: "bigint", nullable: false),
                    ElementType = table.Column<int>(type: "int", nullable: false),
                    ElementName = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsTitleHidden = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Width = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    IsHidden = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsRequired = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDependent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanMissing = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyVisitPageModuleElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyVisitPageModuleElements_StudyVisitPageModules_StudyVisi~",
                        column: x => x.StudyVisitPageModuleId,
                        principalTable: "StudyVisitPageModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "SubjectVisitPageModules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyVisitPageModuleId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubjectVisitPageId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectVisitPageModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectVisitPageModules_StudyVisitPageModules_StudyVisitPage~",
                        column: x => x.StudyVisitPageModuleId,
                        principalTable: "StudyVisitPageModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectVisitPageModules_SubjectVisitPages_SubjectVisitPageId",
                        column: x => x.SubjectVisitPageId,
                        principalTable: "SubjectVisitPages",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "StudyVisitPageModuleElementDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyVisitPageModuleElementId = table.Column<long>(type: "bigint", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: false),
                    RowIndex = table.Column<int>(type: "int", nullable: false),
                    ColunmIndex = table.Column<int>(type: "int", nullable: false),
                    CanQuery = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanSdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanRemoteSdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanComment = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanDataEntry = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ParentElementEProPageNumber = table.Column<int>(type: "int", nullable: false),
                    MetaDataTags = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EProPageNumber = table.Column<int>(type: "int", nullable: false),
                    ButtonText = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DefaultValue = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Unit = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LowerLimit = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpperLimit = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mask = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Layout = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Options = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyVisitPageModuleElementDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyVisitPageModuleElementDetails_StudyVisitPageModuleEleme~",
                        column: x => x.StudyVisitPageModuleElementId,
                        principalTable: "StudyVisitPageModuleElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "StudyVisitPageModuleElementEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyVisitPageModuleElementId = table.Column<long>(type: "bigint", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    SourceElementId = table.Column<long>(type: "bigint", nullable: false),
                    TargetElementId = table.Column<long>(type: "bigint", nullable: false),
                    ValueCondition = table.Column<int>(type: "int", nullable: false),
                    ActionValue = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyVisitPageModuleElementEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyVisitPageModuleElementEvents_StudyVisitPageModuleElemen~",
                        column: x => x.StudyVisitPageModuleElementId,
                        principalTable: "StudyVisitPageModuleElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateTable(
                name: "SubjectVisitPageModuleElements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudyVisitPageModuleElementId = table.Column<long>(type: "bigint", nullable: false),
                    SubjectVisitModuleId = table.Column<long>(type: "bigint", nullable: false),
                    UserValue = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_turkish_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShowOnScreen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MissingData = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sdv = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Query = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectVisitPageModuleElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectVisitPageModuleElements_StudyVisitPageModuleElements_~",
                        column: x => x.StudyVisitPageModuleElementId,
                        principalTable: "StudyVisitPageModuleElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectVisitPageModuleElements_SubjectVisitPageModules_Subje~",
                        column: x => x.SubjectVisitModuleId,
                        principalTable: "SubjectVisitPageModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_turkish_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Elements_ElementDetailId",
                table: "Elements",
                column: "ElementDetailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Elements_ModuleId",
                table: "Elements",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_MailTemplatesRoles_MailTemplateId",
                table: "MailTemplatesRoles",
                column: "MailTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleElementEvents_ModuleId",
                table: "ModuleElementEvents",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_StudyId",
                table: "Sites",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Studies_EquivalentStudyId",
                table: "Studies",
                column: "EquivalentStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyRoleModulePermissions_StudyRoleId",
                table: "StudyRoleModulePermissions",
                column: "StudyRoleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyRoles_StudyId",
                table: "StudyRoles",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyUsers_StudyId",
                table: "StudyUsers",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyUsers_StudyRoleId",
                table: "StudyUsers",
                column: "StudyRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyUserSites_SiteId",
                table: "StudyUserSites",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyUserSites_StudyUserId",
                table: "StudyUserSites",
                column: "StudyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyVisitPageModuleElementDetails_StudyVisitPageModuleEleme~",
                table: "StudyVisitPageModuleElementDetails",
                column: "StudyVisitPageModuleElementId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyVisitPageModuleElementEvents_StudyVisitPageModuleElemen~",
                table: "StudyVisitPageModuleElementEvents",
                column: "StudyVisitPageModuleElementId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyVisitPageModuleElements_StudyVisitPageModuleId",
                table: "StudyVisitPageModuleElements",
                column: "StudyVisitPageModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyVisitPageModules_StudyRoleModulePermissionId",
                table: "StudyVisitPageModules",
                column: "StudyRoleModulePermissionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyVisitPageModules_StudyVisitPageId",
                table: "StudyVisitPageModules",
                column: "StudyVisitPageId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyVisitPages_StudyVisitId",
                table: "StudyVisitPages",
                column: "StudyVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyVisits_StudyId",
                table: "StudyVisits",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SiteId",
                table: "Subjects",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_StudyId",
                table: "Subjects",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVisitPageModuleElements_StudyVisitPageModuleElementId",
                table: "SubjectVisitPageModuleElements",
                column: "StudyVisitPageModuleElementId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVisitPageModuleElements_SubjectVisitModuleId",
                table: "SubjectVisitPageModuleElements",
                column: "SubjectVisitModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVisitPageModules_StudyVisitPageModuleId",
                table: "SubjectVisitPageModules",
                column: "StudyVisitPageModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVisitPageModules_SubjectVisitPageId",
                table: "SubjectVisitPageModules",
                column: "SubjectVisitPageId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVisitPages_StudyVisitPageId",
                table: "SubjectVisitPages",
                column: "StudyVisitPageId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVisitPages_SubjectVisitId",
                table: "SubjectVisitPages",
                column: "SubjectVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVisits_StudyVisitId",
                table: "SubjectVisits",
                column: "StudyVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVisits_SubjectId",
                table: "SubjectVisits",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Elements");

            migrationBuilder.DropTable(
                name: "MailTemplatesRoles");

            migrationBuilder.DropTable(
                name: "MailTemplateTags");

            migrationBuilder.DropTable(
                name: "ModuleElementEvents");

            migrationBuilder.DropTable(
                name: "MultipleChoiceTag");

            migrationBuilder.DropTable(
                name: "StudyUserSites");

            migrationBuilder.DropTable(
                name: "StudyVisitPageModuleElementDetails");

            migrationBuilder.DropTable(
                name: "StudyVisitPageModuleElementEvents");

            migrationBuilder.DropTable(
                name: "SubjectVisitPageModuleElements");

            migrationBuilder.DropTable(
                name: "SystemAuditTrails");

            migrationBuilder.DropTable(
                name: "ElementDetails");

            migrationBuilder.DropTable(
                name: "MailTemplates");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "StudyUsers");

            migrationBuilder.DropTable(
                name: "StudyVisitPageModuleElements");

            migrationBuilder.DropTable(
                name: "SubjectVisitPageModules");

            migrationBuilder.DropTable(
                name: "StudyVisitPageModules");

            migrationBuilder.DropTable(
                name: "SubjectVisitPages");

            migrationBuilder.DropTable(
                name: "StudyRoleModulePermissions");

            migrationBuilder.DropTable(
                name: "StudyVisitPages");

            migrationBuilder.DropTable(
                name: "SubjectVisits");

            migrationBuilder.DropTable(
                name: "StudyRoles");

            migrationBuilder.DropTable(
                name: "StudyVisits");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "Studies");
        }
    }
}
