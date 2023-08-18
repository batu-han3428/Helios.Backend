import React, { useState, useContext } from 'react';
import { TextBox, Label, LinkButton, Form, FormField, ComboBox, CheckBox } from 'rc-easyui';
import { LanguageContext } from '../../Common/LanguageResources/LanguageContext';
import './login.css';
import logo from '../../Common/images/helios_222_70.png';
import footerLogo from '../../Common/images/med-case.png';
import axios from 'axios';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faUserCircle, faWarning } from '../../../node_modules/@fortawesome/free-solid-svg-icons/index';
import { useTranslation } from 'react-i18next';

//class Login extends React.Component {
const Login = () => {

    const { t, i18n } = useTranslation();

    const [user, setUser] = useState({
        Email: '',
        Password: ''
    });

    const GreetingComponent = () => {
        const { language } = useContext(LanguageContext);
        const translations = require(`./${language}.json`);

        return <h1>{translations.greeting}</h1>;
    };
    //const cntxt = useContext(LanguageContext);
    const Language = () => {
        const { language } = useContext(LanguageContext);
        const greetingText = language.Login;
        const translations = require(`./${language}.json`);
    };

    const handleSubmit = (event) => {
        debugger;

        const formData = new FormData();
        formData.append('Email', user.Email);
        formData.append('Password', user.Password);

        //axios.get('/api/Account').then((response) => {
        //    debugger;
        //    alert(response);
        //});
        // Send form data to the controller
        fetch('/Account/Login', {
            method: 'POST',
            body: formData
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


    const handleEmailChange = (e) => {
        setUser({ ...user, Email: e.target.value });
    };

    const handlePasswordChange = (e) => {
        setUser({ ...user, Password: e.target.value });
    };

    return (
        <>
            {t('Welcome') }
            <button onClick={() => { i18n.changeLanguage(i18n.language === 'tr-TR' ? 'en-US' : 'tr-TR') }}>
                {i18n.language === 'tr-TR' ? 'EN' : 'TR'}
            </button>
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
                                    {/*<Form*/}
                                    {/*    style={{ maxWidth: 500 }}*/}
                                    {/*    model={user}*/}
                                    {/*    labelPosition="top"*/}
                                    {/*    onSubmit={handleSubmit}*/}
                                    {/*    floatingLabel>*/}
                                    {/*onChange={this.handleChange.bind(this)}>*/}
                                    <div className='form-group'>
                                        <label> Email</label>
                                        <input className='form-control' value={user.Email} onChange={handleEmailChange} type='text' id='Email' />
                                    </div>
                                    <div className='form-group'>
                                        <label> Password</label>
                                        <input className='form-control' value={user.Password} onChange={handlePasswordChange} type='password' id='Password' />
                                    </div>
                                    <div name="accept" label="Remember me!">
                                        <CheckBox checked={user.RememberMe} style={{ marginBottom: '5px' }}></CheckBox>
                                        <Label style={{ margin: '0 5px', fontSize: '12px', width: '120px', color: '#6d6e70', marginBottom: '10px' }}>Remember me!</Label>
                                    </div>
                                    <div className='form-group'>
                                        <div style={{ marginBottom: '7px' }}>
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
                                    <div>
                                        <button type='submit' className='btn btn-success' onClick={handleSubmit} style={{ width: '100%' }}> Login</button>
                                    </div>
                                    {/*</Form>*/}
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