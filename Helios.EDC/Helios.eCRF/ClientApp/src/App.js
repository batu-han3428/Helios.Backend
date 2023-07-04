import React, { useEffect, useState } from 'react';
import Login from './Pages/Account/login';
import 'bootstrap/dist/css/bootstrap.css';
import './index.css';
import './Common/css/adminCommon.css';
import './Common/css/style.css';

//import { DataGrid, GridColumn } from 'rc-easyui';


const App = () => {
    return (
        <div>
            <Login />
        </div>
    );
};
export default App;