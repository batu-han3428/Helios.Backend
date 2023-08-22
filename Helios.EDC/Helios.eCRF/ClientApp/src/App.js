import React, { useEffect, useState } from 'react';
import Login from './Pages/Account/login';
import MultiRoleLogin from './Pages/Account/multiRoleLogin';
import 'bootstrap/dist/css/bootstrap.css';
import './index.css';
import './Common/css/adminCommon.css';
import './Common/css/style.css';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import TopNavBarLayout from './Layouts/TopNavBar/TopNavBarLayout';
//import TenantList from './Pages/Tenant/tenantList';
import AddTenant from './Pages/Tenant/addTenant';
import { DataGrid, GridColumn } from 'rc-easyui';
import Studies from './Pages/SSO/studies';
import './Language/i18n';

const App = () => {
    return (
        <div>
            {/*<MultiRoleLogin/>*/}
            {/*<AddTenant></AddTenant>*/}
            {/*{<TenantList />}*/}
            {/*<Studies></Studies>*/}
            <Login></Login>
        </div>
    );
};
export default App;