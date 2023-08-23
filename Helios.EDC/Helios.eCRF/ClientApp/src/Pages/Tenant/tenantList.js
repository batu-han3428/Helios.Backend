import React, { useState, useEffect } from 'react';
import Sidebar from '../../Layouts/Sidebar/sidebar';
import './tenant.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faCog, faArrowRight } from '../../../node_modules/@fortawesome/free-solid-svg-icons/index';
import { Routes, Route, useNavigate } from "react-router-dom";
import AddOrUpdateTenant from './addOrUpdateTenant';

function TenantList() {
    const navigate = useNavigate();

    const navigateAddTenant = () => {
        navigate('/addOrUpdateTenant');
    };

    //useEffect(() => {
    fetch('/Account/GetTenantList', {
        method: 'GET',
    })
        .then(response => response.json())
        .then(data => {
            setTenantList(data);
        })
        .catch(error => {
            //console.error('Error:', error);
        });
    //});
    const [tenantList, setTenantList] = useState([]);

    return (
        <div style={({ height: "100vh" }, { display: "flex" })} >
            <Sidebar></Sidebar>
            <div id="page-wrap" style={{ padding: "15px", width: '100%' }}>
                <div><h1>Tenants</h1></div>
                <hr />
                <div className="floatr">
                    <button className="btn btn-primary" onClick={() => navigate("/addTenant")}>
                        <small>Add Tenant</small>
                    </button>
                    {/*<Link to="./addTenant" className="btn btn-primary">Sign up</Link>*/}
                    {/*<button onClick={navigateAddTenant}>Home</button>*/}
                    <Routes>
                        <Route path="/addTenant" element={<AddTenant />} />
                    </Routes>
                </div>
                <br />
                <br />
                <div>
                    {
                        tenantList.map((tenantList) => {
                            return (
                                <div className='tenant-box'>
                                    <div className="floatl">
                                        <h2>{tenantList.name}</h2>
                                    </div>
                                    <div className="floatr">
                                        <button className="btn" data-id={tenantList.id}>
                                            <FontAwesomeIcon icon={faCog} />
                                        </button>
                                        <br></br>
                                        <button className="btn" data-id={tenantList.id}>
                                            <FontAwesomeIcon icon={faArrowRight} />
                                        </button>
                                        <br></br>
                                        <button className="btn" data-id={tenantList.id}>
                                            <FontAwesomeIcon icon={faEdit} />
                                        </button>
                                    </div>
                                </div>
                            );
                        })
                    }

                </div>
            </div>
        </div>
    );
}

export default TenantList;

