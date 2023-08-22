import React, { Component, useState, useContext, Form, FormField, TextBox, ComboBox, CheckBox, LinkButton } from 'react';
import TopNavBarLayout from '../../Layouts/TopNavBar/TopNavBarLayout';
//import Sidebar from '../../Layouts/Sidebar/sidebar';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowRight } from '../../../node_modules/@fortawesome/free-solid-svg-icons/index';

function MultiRoleLogin() {    

    return (
        <section>
            <TopNavBarLayout>
            </TopNavBarLayout>
        <div style={({ height: "100vh" }, { display: "flex" })} >
            {/*<Sidebar></Sidebar>*/}
            <div id="page-wrap" style={{ padding: "15px", width: '100%' }}>
                <div className='div-box'>
                    <div className="floatl">
                        <h2>Go to admin panel</h2>
                    </div>
                    <div className="floatr">
                        <button className="btn">
                            <FontAwesomeIcon icon={faArrowRight} />
                        </button>
                    </div>
                    </div>
                    <div className='div-box'>
                    <div className="floatl">
                        <h2>Go to study user panel</h2>
                    </div>
                    <div className="floatr">
                        <button className="btn">
                            <FontAwesomeIcon icon={faArrowRight} />
                        </button>
                    </div>
                </div>
            </div>
            </div>
        </section>
    );
};

export default MultiRoleLogin;