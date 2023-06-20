import React, { useState, useContext } from 'react';
import { TextBox, Label, LinkButton, Form, FormField, ComboBox, CheckBox } from 'rc-easyui';
import { LanguageContext } from '../Common/LanguageResources/LanguageContext';
import './login.css';
import logo from '../Common/images/helios_222_70.png';
import footerLogo from '../Common/images/med-case.png';
import axios from 'axios';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faUserCircle, faWarning } from '../../../../../node_modules/@fortawesome/free-solid-svg-icons/index';

//class Login extends React.Component {
    const Login = () => {
    const user = [];

    const [Username, setUsername] = useState('');
    const [Password, setPassword] = useState('');

    const url = "https://localhost:7299/api/Account";
    //const GreetingComponent = () => {
    //    const { language } = useContext(LanguageContext);
    //    const translations = require(`./${language}.json`);

    //    return <h1>{translations.greeting}</h1>;
    //};
    //const cntxt = useContext(LanguageContext);
    const Language = () => {
        const { language } = useContext(LanguageContext);
        const greetingText = language.Login;
        const translations = require(`./${language}.json`);
    };
    //const { language } = useContext(LanguageContext);

    const handleSubmit = (e) => {
            debugger;
        //if (Username == '')
        //    alert('Username cannot be empty!');

        //if (Password == '')
        //    alert('Password cannot be empty!')

        axios.get(url).then((response) => {
            debugger;
            alert(response);

            setInterval(function () {
                console.log("Delayed for 1 second.");
            }, 100000);

            //setTimeout(() => {
            //    console.log("Delayed for 1 second.");
            //}, 100000);
        });

        //setTimeout(() => {
        //    console.log("Delayed for 1 second.");
        //}, 100000);
    };



    function handleUsernameChange(e) {
        setUsername(e.target.value);
    }

    function handlePasswordChange(e) {
        setPassword(e.target.value);
    }

    return (
        <>
            <div className='header-login'>
                <img src={logo} />
            </div>
            <div className='page-container'>
                <div className='middle-box  animated fadeInDown mainLoginDiv' style={{ maxWidth: '800px', paddingBottom: '150px' }}>

                    <div className='row'>
                        <div className='col-md-12' style={{ marginTop: '40px', textAlign: 'center' }}>
                            <div className='alert alert-danger loginbrowseralert' style={{ color: '#a94442' }}>
                                <FontAwesomeIcon icon={faWarning} />  You are on Helios EDC administrator page. Please use the login link that is sent to you via email.
                            </div>
                        </div>
                        <div className='col-md-6' style={{ marginLeft: '25%', marginTop: '10px' }}>
                            <div className='panel panel-default'>
                                <div className='panel-heading'>
                                    <h3><FontAwesomeIcon icon={faUserCircle} style={{ paddingRight: '5px' }} />Login</h3>
                                </div>
                                <div className='panel-body' style={{ border: 'none' }}>
                                    <Form
                                        style={{ maxWidth: 500 }}
                                        model={user}
                                        labelPosition="top"
                                        floatingLabel>
                                        {/*onChange={this.handleChange.bind(this)}>*/}
                                        <div className='form-group'>
                                            <label> Username</label>
                                            <input className='form-control' value={user.Username} onChange={handleUsernameChange} type='text' id='Username' />
                                        </div>
                                        <div className='form-group'>
                                            <label> Password</label>
                                            <input className='form-control' value={user.Password} onChange={handlePasswordChange} type='password' id='Username' />
                                        </div>
                                        <FormField name="accept" label="Remember me!" labelPosition="after" labelWidth={120} style={{ fontSize: '12px', marginBottom: '10px' }}>
                                            <CheckBox checked={user.RememberMe}></CheckBox>
                                        </FormField>
                                        <div className='form-group'>
                                            <div style={{ marginBottom:'7px' }}>
                                                <a href="https://example.com" style={{ textDecoration: 'none', color: '#337ab7', fontSize: '12px', fontWeight: '400' }}>
                                                    Forgot password?
                                                </a>
                                            </div>
                                            <div>
                                                <a href="https://example.com" style={{ textDecoration: 'none', color: '#337ab7', fontSize: '12px', fontWeight: '400', marginTop: '5px' }} >
                                                    Contact to admin
                                                </a>
                                            </div>
                                        </div>
                                        <FormField>
                                            <button type='submit' className='btn btn-success' onClick={() => handleSubmit(user)} style={{ width: '100%' }}> Login</button>
                                        </FormField>
                                    </Form>
                                </div>


                            </div>
                        </div>
                    </div>
                </div>
                <footer className="footer" style={{ display: 'flex' }}>
                    <div className="accountFooter container scrollStyle" style={{ overflowY: 'auto', maxHeight: '200px' }} >
                        <p>

                        </p>
                    </div>
                    <a href="https://med-case.io/" className="accountFooterLink">
                        <span style={{ position: 'relative', bottom: '-3px' }} >Prepared by</span><img src={footerLogo} style={{ width: "100px", height: "20px" }} />
                    </a>
                </footer>
            </div>
        </>



    );
};

export default Login;