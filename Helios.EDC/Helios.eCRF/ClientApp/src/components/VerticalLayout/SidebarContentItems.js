const menuItems = {
    common: [
        {
            label: "Home",
            to: "/home",
        },
        {
            label: "About",
            to: "/about",
        },
    ],
    admin: [
        {
            label: "Studies",
            to: "/studylist",
        },
        {
            label: "Tenant",
            to: "/tenant",
/*            icon: "ti-home",*/
        },
        {
            label: "Form Builder",
            to: "/formBuilder",
/*            icon: "ti-home",*/
        },
        {
            label: "Dashboard",
            to: "/dashboard",
        },
        {
            label: "Email",
          /*  to: "/#",*/
            subMenu: [
                {
                    label: "Inbox",
                    to: "/email-inbox",
                },
                {
                    label: "Email Read",
                    to: "/email-read",
                },
                {
                    label: "Email Compose",
                    to: "/email-compose",
                },
            ],
        },
        {
            label: "UI Elements",
         /*   to: "/#",*/
            subMenu: [
                {
                    label: "Alerts",
                    to: "/ui-alerts",
                },
                {
                    label: "Buttons",
                    to: "/ui-buttons",
                },
                {
                    label: "Cards",
                    to: "/ui-cards",
                },
                {
                    label: "Carousel",
                    to: "/ui-carousel",
                },
                {
                    label: "Dropdowns",
                    to: "/ui-dropdowns",
                },
                {
                    label: "Grid",
                    to: "/ui-grid",
                },
                {
                    label: "Lightbox",
                    to: "/ui-lightbox",
                },
                {
                    label: "Modals",
                    to: "/ui-modals",
                },
                {
                    label: "Offcanvas",
                    to: "/ui-offcanvas",
                },
                {
                    label: "Slider",
                    to: "/ui-rangeslider",
                },
                {
                    label: "Session Timeout",
                    to: "/ui-session-timeout",
                },
                {
                    label: "Progress Bars",
                    to: "/ui-progressbars",
                },
                {
                    label: "Tabs & Accordions",
                    to: "/ui-tabs-accordions",
                },
                {
                    label: "Typography",
                    to: "/ui-typography",
                },
                {
                    label: "Video",
                    to: "/ui-video",
                },
                {
                    label: "General",
                    to: "/ui-general",
                },
                {
                    label: "Colors",
                    to: "/ui-colors",
                },
                {
                    label: "Rating",
                    to: "/ui-rating",
                },
                {
                    label: "Utilities",
                    to: "/ui-utilities",
                },
                {
                    label: "Utilities",
                    to: "/ui-utilities",
                },
            ],
        },
        {
            label: "Forms",
        /*    to: "/#",*/
            subMenu: [
                {
                    label: "Form Elements",
                    to: "/form-elements",
                },
                {
                    label: "Form Validation",
                    to: "/form-validation",
                },
                {
                    label: "Form Advanced",
                    to: "/form-advanced",
                },
                {
                    label: "Form Editors",
                    to: "/form-editors",
                },
                {
                    label: "Form File Upload",
                    to: "/form-uploads",
                },
                {
                    label: "Form Xeditable",
                    to: "/form-xeditable",
                },
                {
                    label: "Form Repeater",
                    to: "/form-repeater",
                },
                {
                    label: "Form Wizard",
                    to: "/form-wizard",
                },
                {
                    label: "Form Mask",
                    to: "/form-mask",
                },
            ]
        },
        {
            label: "Tables",
          /*  to: "/#",*/
            subMenu: [
                {
                    label: "Basic Tables",
                    to: "/tables-basic"
                },
                {
                    label: "Data Tables",
                    to: "/tables-datatable"
                },
                {
                    label: "Responsive Table",
                    to: "/tables-responsive"
                },
                {
                    label: "Editable Table",
                    to: "/tables-editable"
                },
            ]
        },
    ],
    study: [
        {
            label: "Admin page",
            to: "/"
        },
        {
            label: "Go to active study",
            to: "/invalid",
            isDemo: false
        },
        {
            label: "Go to demo study",
            to: "/invalid",
            isDemo: true
        },
        {
            label: "Visits",
            to: "/visits",
        },
        {
            label: "Site & Laboratories",
            subMenu: [
                {
                    label: "Sites",
                    to: "/sitelaboratories-sites-site",
                },
                {
                    label: "Laboratories",
                    to: "/sitelaboratories-laboratories",
                },
            ]
        },
        {
            label: "Randomization",
            to: "/randomization",
        },
        {
            label: "Study documents",
            to: "/studydocuments",
        },
        {
            label: "E-mail templates",
            to: "/emailtemplates",
        },
        {
            label: "System audit trail",
            to: "/systemaudittrail",
        },
        {
            label: "E-consent",
            to: "/econsent",
        },
        {
            label: "TMF template list",
            to: "/tmftemplatelist",
        },
        {
            label: "TMF e-mail template",
            to: "/tmfemailtemplate",
        },
        {
            label: "Permissions",
            to: "/permissions",
        },
        {
            label: "Users",
            to: "/users",
        },
    ],
};

export default menuItems;