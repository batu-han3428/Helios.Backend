import React, { useState, useContext } from 'react';
import { TextBox, Label, LinkButton } from 'rc-easyui';
import { LanguageContext } from '../../Common/LanguageResources/LanguageContext';
import './login.css';
import axios from 'axios';

const Login = () => {
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
            <div className='page-container'>
                <div className='middle-box  animated fadeInDown mainLoginDiv' style={{ maxWidth: '800px', paddingBottom: '150px' }}>


                    {/*<div className='row'>*/}
                    {/*    <div className='col-md-12' style={{ marginTop: '20px' }}>*/}
                    {/*        <div className='alert alert-danger loginbrowseralert hide'>*/}
                    {/*            <i className='fa fa-warning text-warning'></i> <label><b>HeliosResource.js_TitleBrowserError </b> :  HeliosResource.js_MsgBrowserError</label>*/}
                    {/*        </div>*/}
                    {/*    </div>*/}
                    {/*</div>*/}

                    <div className='row'>
                        <div className='col-md-12' style={{ marginTop: '20px', textAlign: 'center' }}>
                            <div className='alert alert-danger loginbrowseralert'>
                                <i className='fa fa-warning'></i>   You are on Helios EDC administrator page. Please use the login link that is sent to you via email.
                            </div>
                        </div>
                        <div className='col-md-6' style={{ marginLeft: '25%', marginTop: '10px' }}>
                            <div className='panel panel-default'>
                                <div className='panel-heading'>
                                    <h3><i className='fa fa-user-circle-o' style={{ paddingRight: '5px' }}></i> Login</h3>
                                </div>
                                <div>
                                    {/*{GreetingComponent}*/}
                                </div>

                                <div className='panel-body'>
                                    {/*<input type='hidden' value=' HeliosResource.js_BtnNewPassword' />*/}
                                    {/*<input type='hidden' value=' HeliosResource.js_TitleForgotPassword' />*/}
                                    {/*<form asp-route-returnurl=' ViewData['ReturnUrl']' method='post'>*/}
                                    {/*<input type='hidden' name='captcha' />*/}
                                    <form onSubmit={handleSubmit}>
                                        {/*<div className='text-danger'></div>*/}
                                        {/*<label htmlFor="t1" align="top">First Name:</label>*/}
                                        {/*<TextBox inputId="t1" iconCls="icon-man" placeholder="First name" name="fname" style={{ width: '100%' }}></TextBox>*/}
                                        <div className='form-group'>
                                            <label> Username</label>
                                            <input className='form-control' value={Username} onChange={handleUsernameChange} type='text' id='Username' />
                                        </div>
                                        <div className='form-group'>
                                            <label> Password</label>
                                            <input className='form-control' value={Password} onChange={handlePasswordChange} type='password' id='Password' />
                                        </div>
                                        <div className='form-group'>
                                            <div className='checkbox'>
                                                <label>
                                                    Remember me!
                                                </label>
                                            </div>
                                        </div>
                                        <div className='form-group'>
                                            {/*<p>*/}
                                            {/*    <a href='javascript:void(0)' className='forgotlinkclassName'*/}
                                            {/*       data-url='/Crf.Web.AdminUI/Account/ForgotPassword'*/}
                                            {/*       onclick='Admin.GetForgotPassword(this);'>*/}
                                            {/*         HeliosResource.js_ForgotPassword*/}
                                            {/*    </a>*/}
                                            {/*</p>*/}
                                            {/*<p>*/}
                                            {/*    <em>*/}
                                            {/*        <a href='/Account/ContactUs' className='forgotlinkclassName' data-url='/Account/ContactUs'>*/}
                                            {/*             HeliosResource.js_ContactToAdmin*/}
                                            {/*        </a>*/}
                                            {/*    </em>*/}
                                            {/*</p>*/}
                                        </div>
                                        <div className='form-group'>
                                            <button type='submit' className='btn btn-success' style={{ width: '100%' }}> Login</button>
                                            <LinkButton>Submit</LinkButton>
                                            {/*<button type='submit' className='btn btn-success' style={{ width: '100%' }}> {Language.greetingText}</button>*/}
                                            {/*<button type='submit' className='btn btn-success' style={{ width: '100%' }}> {Language.translations.Login}</button>*/}
                                        </div>
                                    </form>
                                </div>


                            </div>
                        </div>
                    </div>
                </div>
            </div></>



    );
};

export default Login;