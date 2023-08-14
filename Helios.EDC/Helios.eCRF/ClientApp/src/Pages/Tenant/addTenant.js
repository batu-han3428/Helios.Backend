import React, { Component, useState, useContext, Form, FormField, TextBox, ComboBox, CheckBox, LinkButton } from 'react';

//export default class AddTenant extends Component {
const AddTenant = () => {

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
                console.log(data);
            })
            .catch(error => {
                //console.error('Error:', error);
            });
    };

    return (
        <div className='page-container'>
            <div className='middle-box  animated fadeInDown mainLoginDiv' style={{ maxWidth: '800px', paddingBottom: '150px' }}>

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

export default AddTenant;