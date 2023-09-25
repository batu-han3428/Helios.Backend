//import React, { Component, useState, useContext, Form, FormField, TextBox, ComboBox, CheckBox, LinkButton } from 'react';

////export default class AddTenant extends Component {
//function TextElement() {

//    const [Title, setTitle] = useState('');
//    const [Placeholder, setPlaceholder] = useState('');

//    const handleNameChange = (e) => {
//        setName(e.target.value);
//    };

//    const handleSubmit = (event) => {
//        debugger;
//        fetch('/User/AddTenant?Name=' + Name, {
//            method: 'POST',
//            //body: Name

//        })
//            .then(response => response.json())
//            .then(data => {
//                debugger;
//                // Handle response from the controller
//            })
//            .catch(error => {
//                //console.error('Error:', error);
//            });
//    };

//    return (
//        <div className='row'>
//            <div className='form-group'>
//                <label> Name</label>
//                <input className='form-control' value={Name} onChange={handleNameChange} type='text' id='Name' />
//            </div>
//            <div>
//                <button type='submit' className='btn btn-primary' onClick={handleSubmit} style={{ width: '100%' }}> Save</button>
//            </div>
//        </div>
//    );
//};

//export default AddOrUpdateTenant;