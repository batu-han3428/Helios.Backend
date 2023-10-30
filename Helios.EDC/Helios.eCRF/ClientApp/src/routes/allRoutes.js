import React from "react";

// Profile
import UserProfile from "../Pages/Template/Authentication/user-profile";

//Email
import EmailInbox from "../Pages/Template/Email/email-inbox";
import EmailRead from "../Pages/Template/Email/email-read";
import EmailCompose from "../Pages/Template/Email/email-compose";

import Emailtemplatealert from "../Pages/Template/EmailTemplate/email-template-alert";
import Emailtemplatebasic from "../Pages/Template/EmailTemplate/email-template-basic";
import Emailtemplatebilling from "../Pages/Template/EmailTemplate/email-template-billing";

// Authentication related Pages
import Login from "../Pages/Authentication/Login";
import Logout from "../Pages/Authentication/Logout";
import Register from "../Pages/Template/Authentication/Register";
import ForgetPwd from "../Pages/Template/Authentication/ForgetPassword";

//  // Inner Authentication
import Login1 from "../Pages/Template/AuthenticationInner/Login";
import Login2 from "../Pages/Template/AuthenticationInner/Login2";
import Register1 from "../Pages/Template/AuthenticationInner/Register";
import Register2 from "../Pages/Template/AuthenticationInner/Register2";
import Recoverpw from "../Pages/Template/AuthenticationInner/Recoverpw";
import Recoverpw2 from "../Pages/Template/AuthenticationInner/Recoverpw2";
import ForgetPwd1 from "../Pages/Template/AuthenticationInner/ForgetPassword";
import LockScreen from "../Pages/Template/AuthenticationInner/auth-lock-screen";
import LockScreen2 from "../Pages/Template/AuthenticationInner/auth-lock-screen-2";
import ConfirmMail from "../Pages/Template/AuthenticationInner/page-confirm-mail";
import ConfirmMail2 from "../Pages/Template/AuthenticationInner/page-confirm-mail-2";
import EmailVerification from "../Pages/Template/AuthenticationInner/auth-email-verification";
import EmailVerification2 from "../Pages/Template/AuthenticationInner/auth-email-verification-2";
import TwostepVerification from "../Pages/Template/AuthenticationInner/auth-two-step-verification";
import TwostepVerification2 from "../Pages/Template/AuthenticationInner/auth-two-step-verification-2";

// Dashboard
import Dashboard from "../Pages/Template/Dashboard/index";


//Icons
import IconDripicons from "../Pages/Template/Icons/IconDripicons";
import IconMaterialdesign from "../Pages/Template/Icons/IconMaterialdesign";
import TypiconsIcon from "../Pages/Template/Icons/IconTypicons";
import IconIon from "../Pages/Template/Icons/IconIon";
import ThemifyIcon from "../Pages/Template/Icons/IconThemify";
import IconFontawesome from "../Pages/Template/Icons/IconFontawesome";

//Tables
import BasicTables from "../Pages/Template/Tables/BasicTables";
import DatatableTables from "../Pages/Template/Tables/DatatableTables";
import ResponsiveTables from "../Pages/Template/Tables/ResponsiveTables";

// Forms
import FormElements from "../Pages/Template/Forms/FormElements";
import FormAdvanced from "../Pages/Template/Forms/FormAdvanced";
import FormEditors from "../Pages/Template/Forms/FormEditors";
import FormValidations from "../Pages/Template/Forms/FormValidations";
import FormMask from "../Pages/Template/Forms/FormMask";
import FormRepeater from "../Pages/Template/Forms/FormRepeater";
import FormUpload from "../Pages/Template/Forms/FormUpload";
import FormWizard from "../Pages/Template/Forms/FormWizard";
import FormXeditable from "../Pages/Template/Forms/FormXeditable";

//Ui
import UiAlert from "../Pages/Template/Ui/UiAlert";
import UiButtons from "../Pages/Template/Ui/UiButtons";
import UiCards from "../Pages/Template/Ui/UiCards";
import UiCarousel from "../Pages/Template/Ui/UiCarousel";
import UiColors from "../Pages/Template/Ui/UiColors";
import UiDropdown from "../Pages/Template/Ui/UiDropdown";
import UiGeneral from "../Pages/Template/Ui/UiGeneral";
import UiGrid from "../Pages/Template/Ui/UiGrid";
import UiImages from "../Pages/Template/Ui/UiImages";
import UiModal from "../Pages/Template/Ui/UiModal";
import UiProgressbar from "../Pages/Template/Ui/UiProgressbar";
import UiTabsAccordions from "../Pages/Template/Ui/UiTabsAccordions";
import UiTypography from "../Pages/Template/Ui/UiTypography";
import UiVideo from "../Pages/Template/Ui/UiVideo";
import UiSessionTimeout from "../Pages/Template/Ui/UiSessionTimeout";
import UiOffcanvas from "../Pages/Template/Ui/UiOffcanvas";


//tenant
import TenantList from "../Pages/Tenant/tenantList.js";
import TenantAddOrUpdate from "../Pages/Tenant/addOrUpdateTenant.js";

import FormBuilder from "../Pages/Module/FormBuilder/formBuilder.js";
import Module from "../Pages/Module/moduleList.js";
import AddOrUpdateModule from "../Pages/Module/addOrUpdateModule.js";

//Study
import StudyList from "../Pages/Study/StudyList";
import AddOrUpdateStudy from "../Pages/Study/AddOrUpdateStudy";
import Study from "../Pages/Study/Study";

//Site & Laboratories
import Sites from "../Pages/SiteLaboratories/Sites";

//Permissions
import Permission from "../Pages/Permissions/Permission";

//Users
import User from "../Pages/Users/User";

const userRoutes = [
    //tenant
    { path: "/Tenant", component: <TenantList />, menuType: "admin" },
    { path: "/addTenant", component: <TenantAddOrUpdate />, menuType: "admin" },

    { path: "/moduleList", component: <Module />, menuType: "admin" },
    { path: "/addModule", component: <AddOrUpdateModule />, menuType: "admin" },
    { path: "/formBuilder", component: <FormBuilder />, menuType: "admin" },

    //study
    { path: "/studylist", component: <StudyList />, menuType: "admin", roles: ['TenantAdmin'] },
    { path: "/addstudy", component: <AddOrUpdateStudy />, menuType: "admin" },
    { path: "/visits", component: <Study />, menuType: "study" },

    //site & laboratories
    { path: "/sitelaboratories-sites-site", component: <Sites />, menuType: "study" },

    //permissions
    { path: "/permissions", component: <Permission />, menuType: "study" },

    //users
    { path: "/users", component: <User />, menuType: "study" },

    { path: "/dashboard", component: <Dashboard />, menuType: "admin" },

  // //profile
    { path: "/profile", component: <UserProfile />, menuType: "admin" },

  //Email
    { path: "/email-inbox", component: <EmailInbox />, menuType: "admin" },
    { path: "/email-read", component: <EmailRead />, menuType: "admin" },
    { path: "/email-compose", component: <EmailCompose />, menuType: "admin" },

  // Email Template
    { path: "/email-template-alert", component: <Emailtemplatealert />, menuType: "admin" },
    { path: "/email-template-basic", component: <Emailtemplatebasic />, menuType: "admin" },
    { path: "/email-template-billing", component: <Emailtemplatebilling />, menuType: "admin" },

  // Icons
    { path: "/icons-dripicons", component: <IconDripicons />, menuType: "admin" },
    { path: "/icons-materialdesign", component: <IconMaterialdesign />, menuType: "study" },
    { path: "/icons-fontawesome", component: <IconFontawesome />, menuType: "study" },
    { path: "/icons-ion", component: <IconIon />, menuType: "study" },
    { path: "/icons-themify", component: <ThemifyIcon />, menuType: "study" },
    { path: "/icons-typicons", component: <TypiconsIcon />, menuType: "study" },

  // Tables
    { path: "/tables-basic", component: <BasicTables />, menuType: "admin" },
    { path: "/tables-datatable", component: <DatatableTables />, menuType: "admin" },
    { path: "/tables-responsive", component: <ResponsiveTables />, menuType: "admin" },


  // Forms
    { path: "/form-elements", component: <FormElements />, menuType: "admin" },
    { path: "/form-advanced", component: <FormAdvanced />, menuType: "admin" },
    { path: "/form-editors", component: <FormEditors />, menuType: "admin" },
    { path: "/form-mask", component: <FormMask />, menuType: "admin" },
    { path: "/form-repeater", component: <FormRepeater />, menuType: "admin" },
    { path: "/form-uploads", component: <FormUpload />, menuType: "admin" },
    { path: "/form-wizard", component: <FormWizard />, menuType: "admin" },
    { path: "/form-validation", component: <FormValidations />, menuType: "admin" },
    { path: "/form-xeditable", component: <FormXeditable />, menuType: "admin" },

  // Ui
    { path: "/ui-alerts", component: <UiAlert />, menuType: "admin" },
    { path: "/ui-buttons", component: <UiButtons />, menuType: "admin" },
    { path: "/ui-cards", component: <UiCards />, menuType: "admin" },
    { path: "/ui-carousel", component: <UiCarousel />, menuType: "admin" },
    { path: "/ui-colors", component: <UiColors />, menuType: "admin" },
    { path: "/ui-dropdowns", component: <UiDropdown />, menuType: "admin" },
    { path: "/ui-general", component: <UiGeneral />, menuType: "admin" },
    { path: "/ui-grid", component: <UiGrid />, menuType: "admin" },
    { path: "/ui-images", component: <UiImages />, menuType: "admin" },
    { path: "/ui-modals", component: <UiModal />, menuType: "admin" },
    { path: "/ui-progressbars", component: <UiProgressbar />, menuType: "admin" },
    { path: "/ui-tabs-accordions", component: <UiTabsAccordions />, menuType: "admin" },
    { path: "/ui-typography", component: <UiTypography />, menuType: "admin" },
    { path: "/ui-video", component: <UiVideo />, menuType: "admin" },
    { path: "/ui-session-timeout", component: <UiSessionTimeout />, menuType: "admin" },
    { path: "/ui-offcanvas", component: <UiOffcanvas />, menuType: "admin" },


    // this route should be at the end of all other routes
    { path: "/", roles: ["TenantAdmin"], component: <StudyList />, menuType: "admin" }, 
    { path: "/", component: <Dashboard />, menuType: "admin" },
];

const authRoutes = [
  { path: "/logout", component: <Logout /> },
  { path: "/login", component: <Login /> },
  { path: "/forgot-password", component: <ForgetPwd /> },
  { path: "/register", component: <Register /> },


  // Authentication Inner
  { path: "/Pages-login", component: <Login1 /> },
  { path: "/Pages-login-2", component: <Login2 /> },
  { path: "/Pages-register", component: <Register1 /> },
  { path: "/Pages-register-2", component: <Register2 /> },
  { path: "/page-recoverpw", component: <Recoverpw /> },
  { path: "/page-recoverpw-2", component: <Recoverpw2 /> },
  { path: "/Pages-forgot-pwd", component: <ForgetPwd1 /> },
  { path: "/auth-lock-screen", component: <LockScreen /> },
  { path: "/auth-lock-screen-2", component: <LockScreen2 /> },
  { path: "/page-confirm-mail", component: <ConfirmMail /> },
  { path: "/page-confirm-mail-2", component: <ConfirmMail2 /> },
  { path: "/auth-email-verification", component: <EmailVerification /> },
  { path: "/auth-email-verification-2", component: <EmailVerification2 /> },
  { path: "/auth-two-step-verification", component: <TwostepVerification /> },
  { path: "/auth-two-step-verification-2", component: <TwostepVerification2 /> },
];

export { userRoutes, authRoutes };
