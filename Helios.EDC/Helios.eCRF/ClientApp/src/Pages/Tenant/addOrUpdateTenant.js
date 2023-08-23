import React, { Component, useState, useContext, Form, FormField, TextBox, ComboBox, CheckBox, LinkButton } from 'react';
import Sidebar from '../../Layouts/Sidebar/sidebar';

//export default class AddTenant extends Component {
function AddOrUpdateTenant() {

    const [Name, setName] = useState('');

    const handleNameChange = (e) => {
        setName(e.target.value);
    };

    const handleSubmit = (event) => {
        debugger;
        fetch('/Account/AddTenant?Name=' + Name, {
            method: 'POST',
            //body: Name

        })
            .then(response => response.json())
            .then(data => {
                debugger;
                // Handle response from the controller
            })
            .catch(error => {
                //console.error('Error:', error);
            });
    };

    return (
        <div style={({ height: "100vh" }, { display: "flex" })} >
            <Sidebar></Sidebar>
            <div id="page-wrap" style={{ padding: "15px", width: '100%' }}>
                <div><h1>Add Tenants</h1></div>
                <hr />
                <div className='row'>
                    <div className='form-group'>
                        <label> Name</label>
                        <input className='form-control' value={Name} onChange={handleNameChange} type='text' id='Name' />
                    </div>
                    <div>
                        <button type='submit' className='btn btn-primary' onClick={handleSubmit} style={{ width: '100%' }}> Save</button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AddOrUpdateTenant;