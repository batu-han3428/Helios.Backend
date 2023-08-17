import React, { Component, useState, useContext } from 'react';
import Sidebar from '../../Layouts/Sidebar/sidebar'

export default class home extends Component {

    render() {
        return (
            <div style={({ height: "100vh" }, { display: "flex" })} >
                <Sidebar></Sidebar>
                <div id="page-wrap" >
                    <button className="btn btn-primary">
                        <small>Add Tenant</small>
                    </button>
                </div>                        
            </div>
        );
    }
}

