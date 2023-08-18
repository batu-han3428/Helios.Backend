import React, { useState, useEffect } from 'react';
import Sidebar from '../../Layouts/Sidebar/sidebar';
import './tenant.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCog, faArrowRight } from '../../../node_modules/@fortawesome/free-solid-svg-icons/index';

function TenantList() {

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
                    <button className="btn btn-primary">
                        <small>Add Tenant</small>
                    </button>
                </div>
                <div>
                    {
                        tenantList.map((tenantList) => {
                            return (
                                <div className='tenant-box'>
                                    <div className="floatl">
                                        <h2>{tenantList.name}</h2>
                                    </div>
                                    <div className="floatr">
                                        <button className="btn">
                                            <FontAwesomeIcon icon={faCog} />
                                        </button>
                                        <br></br>
                                        <button className="btn">
                                            <FontAwesomeIcon icon={faArrowRight} />
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

