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
import Login from "../Pages/Template/Authentication/Login";
import Logout from "../Pages/Template/Authentication/Logout";
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

//Pages
//import PagesStarter from "../Pages/Template/Utility/Pages-starter";
//import PagesMaintenance from "../Pages/Template/Utility/Pages-maintenance";
//import PagesComingsoon from "../Pages/Template/Utility/Pages-comingsoon";
//import PagesTimeline from "../Pages/Template/Utility/Pages-timeline";
//import PagesInvoice from "../Pages/Template/Utility/PagesInvoice";
//import PagesFaqs from "../Pages/Template/Utility/Pages-faqs";
//import PagesPricing from "../Pages/Template/Utility/Pages-pricing";
//import Pages404 from "../Pages/Utility/Template/Pages-404";
//import Pages500 from "../Pages/Utility/Template/Pages-500";
//import PagesDirectory from "../Pages/Utility/Template/PagesDirectory";
//import PagesProfile from "../Pages/Utility/Template/Pages-profile";

import TenantList from "../Pages/Tenant/tenantList.js";
import TenantAddOrUpdate from "../Pages/Tenant/addOrUpdateTenant.js";

import FormBuilder from "../Pages/Module/FormBuilder/formBuilder.js";

const userRoutes = [
    { path: "/Tenant", component: <TenantList /> },
    { path: "/addTenant", component: <TenantAddOrUpdate /> },

    { path: "/formBuilder", component: <FormBuilder /> },


  { path: "/dashboard", component: <Dashboard /> },

  // //profile
  { path: "/profile", component: <UserProfile /> },

  //Email
  { path: "/email-inbox", component: <EmailInbox /> },
  { path: "/email-read", component: <EmailRead /> },
  { path: "/email-compose", component: <EmailCompose /> },

  // Email Template
  { path: "/email-template-alert", component: <Emailtemplatealert /> },
  { path: "/email-template-basic", component: <Emailtemplatebasic /> },
  { path: "/email-template-billing", component: <Emailtemplatebilling /> },

  // Icons
  { path: "/icons-dripicons", component: <IconDripicons /> },
  { path: "/icons-materialdesign", component: <IconMaterialdesign /> },
  { path: "/icons-fontawesome", component: <IconFontawesome /> },
  { path: "/icons-ion", component: <IconIon /> },
  { path: "/icons-themify", component: <ThemifyIcon /> },
  { path: "/icons-typicons", component: <TypiconsIcon /> },

  // Tables
  { path: "/tables-basic", component: <BasicTables /> },
  { path: "/tables-datatable", component: <DatatableTables /> },
  { path: "/tables-responsive", component: <ResponsiveTables /> },


  // Forms
  { path: "/form-elements", component: <FormElements /> },
  { path: "/form-advanced", component: <FormAdvanced /> },
  { path: "/form-editors", component: <FormEditors /> },
  { path: "/form-mask", component: <FormMask /> },
  { path: "/form-repeater", component: <FormRepeater /> },
  { path: "/form-uploads", component: <FormUpload /> },
  { path: "/form-wizard", component: <FormWizard /> },
  { path: "/form-validation", component: <FormValidations /> },
  { path: "/form-xeditable", component: <FormXeditable /> },

  // Ui
  { path: "/ui-alerts", component: <UiAlert /> },
  { path: "/ui-buttons", component: <UiButtons /> },
  { path: "/ui-cards", component: <UiCards /> },
  { path: "/ui-carousel", component: <UiCarousel /> },
  { path: "/ui-colors", component: <UiColors /> },
  { path: "/ui-dropdowns", component: <UiDropdown /> },
  { path: "/ui-general", component: <UiGeneral /> },
  { path: "/ui-grid", component: <UiGrid /> },
  { path: "/ui-images", component: <UiImages /> },
  { path: "/ui-modals", component: <UiModal /> },
  { path: "/ui-progressbars", component: <UiProgressbar /> },
  { path: "/ui-tabs-accordions", component: <UiTabsAccordions /> },
  { path: "/ui-typography", component: <UiTypography /> },
  { path: "/ui-video", component: <UiVideo /> },
  { path: "/ui-session-timeout", component: <UiSessionTimeout /> },
  { path: "/ui-offcanvas", component: <UiOffcanvas /> },

  //Utility
  //{ path: "/Pages-starter", component: <PagesStarter /> },
  //{ path: "/Pages-timeline", component: <PagesTimeline /> },
  //{ path: "/Pages-invoice", component: <PagesInvoice /> },
  //{ path: "/Pages-directory", component: <PagesDirectory /> },
  //{ path: "/Pages-faqs", component: <PagesFaqs /> },
  //{ path: "/Pages-pricing", component: <PagesPricing /> },
  //{ path: "/Pages-profile", component: <PagesProfile /> },

  // this route should be at the end of all other routes
  { path: "/", component: <TenantList /> },
];

const authRoutes = [
  { path: "/logout", component: <Logout /> },
  { path: "/login", component: <Login /> },
  { path: "/forgot-password", component: <ForgetPwd /> },
  { path: "/register", component: <Register /> },

  //{ path: "/Pages-maintenance", component: <PagesMaintenance /> },
  //{ path: "/Pages-comingsoon", component: <PagesComingsoon /> },
  //{ path: "/Pages-404", component: <Pages404 /> },
  //{ path: "/Pages-500", component: <Pages500 /> },

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
