import React, { Component, useState, useContext, Form, FormField, TextBox, ComboBox, CheckBox, LinkButton } from 'react';
import './Base/elementBase';

//export default class AddTenant extends Component {
function TextElement() {

    const [Placeholder, setPlaceholder] = useState('');

    const handlePlaceholderChange = (e) => {
        setPlaceholder(e.target.value);
    };

    return (
        <><elementbase></elementbase>
            <div className='row'>
                <div className='form-group'>
                    <label> Name</label>
                    <input className='form-control' placeholder={Placeholder} type='text' id='Name' />
                </div>
                <div>
                    <button type='submit' className='btn btn-primary' onClick={handleSubmit} style={{ width: '100%' }}> Save</button>
                </div>
            </div></>
    );
};

export default AddOrUpdateTenant;