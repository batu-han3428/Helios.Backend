import React, { Component, useState, useContext } from 'react';
import TopNavBarLayout from '../../Layouts/TopNavBar/TopNavBarLayout';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { TextBox, Label, LinkButton, Form, FormField, ComboBox, CheckBox } from 'rc-easyui';
import expand from '../../Common/images/expand.png';
import './studies.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSearch } from '../../../node_modules/@fortawesome/free-solid-svg-icons/index';
import { PopupMenu } from "react-simple-widgets";

export default class Studies extends Component {

    render() {
        return (
            <section>
                <TopNavBarLayout>
                    <Routes>
                        {/*<Route path='/' exact component={Home} />*/}
                    </Routes>
                </TopNavBarLayout>
                <div className="component">
                    <div className="col-lg-12" style={{ border: '1px solid #e7eaec', margin: '10px' }}>
                        <div className="ibox float-e-margins">
                            <div className="ibox-title" style={{ display: 'flex', justifyContent: 'space-between' }}>
                                <h5 style={{ margin: '10px 7px' }}>Lütfen giriş yapmak istediğiniz çalışmayı seçiniz</h5>
                                <div>
                                    {/*<form novalidate="novalidate" onsubmit="return false;" className="searchbox">*/}
                                    <div role="search" className="searchbox__wrapper">
                                        <input id="search-input" type="search" name="search" placeholder="Arama" className="searchbox__input" />
                                        {/*<button type="submit" title="Submit your search query." className="searchbox__submit">*/}

                                        {/*</button>*/}
                                        {/*<button type="reset" title="Clear the search query." className="searchbox__reset searchbox-hide">*/}
                                        {/*    <FontAwesomeIcon icon={faSearch} />*/}

                                        {/*</button>*/}
                                    </div>
                                    {/*</form>*/}
                                    {/*<PopupMenu>*/}
                                    {/*    <div className="card text-start">*/}
                                    {/*        <div className="card-body px-4 py-4">*/}
                                    {/*           <div className="list-group list-group-flush" style={{ margin: "0 -24px 0" }}>*/}
                                    {/*                <button className="list-group-item list-group-item-action px-4">*/}
                                    {/*                    <small>Canlı çalışmalarım</small>*/}
                                    {/*                </button>*/}
                                    {/*                <button className="list-group-item list-group-item-action px-4">*/}
                                    {/*                    <small>Demo çalışmalarım</small>*/}
                                    {/*                </button>*/}
                                    {/*                <button className="list-group-item list-group-item-action px-4">*/}
                                    {/*                    <small>Kilitli çalışmalarım</small>*/}
                                    {/*                </button>*/}
                                    {/*            </div>*/}
                                    {/*        </div>*/}
                                    {/*    </div>*/}
                                    {/*</PopupMenu>*/}
                                    {/*<div className="dropdown">*/}
                                    {/*    <button className="btn btn-outline-primary">*/}
                                    {/*        <FontAwesomeIcon icon={faSearch} />*/}
                                    {/*    </button>*/}
                                    {/*    <div id="myDropdown" className="dropdown-content">*/}
                                    {/*        <a href="javascript:void(0)" id="live">Canlı çalışmalarım<i className="fa fa-check" data-filter="1" aria-hidden="true" style={{ float: 'right', fontSize: '16px' }}></i></a>*/}
                                    {/*        <a href="javascript:void(0)" id="demo">Demo çalışmalarım</a>*/}
                                    {/*        <a href="javascript:void(0)" id="lock">Kilitli çalışmalarım</a>*/}
                                    {/*    </div>*/}
                                    {/*</div>*/}
                                </div>
                            </div>

                            <div id="ibox-content" className="ibox-content main-content" style={{ display: 'flex', flexWrap: 'wrap', justifyContent: 'center' }} >
                                <div data-lock="False" data-statu="False" className="col-lg-3" style={{ border: '1px solid #e7eaec', margin: '10px', padding: '0' }} >
                                    <div className="ibox float-e-margins" style={{ marginBottom: '0' }} >
                                        <div className="ibox-title" style={{ backgroundColor: '#d7b21733' }} >
                                            <h5>adenoviral-vektor-asisi</h5>
                                        </div>
                                        <div className="ibox-content" style={{ padding: '0' }} >
                                            <div>
                                                <div style={{ width: '90%' }} >
                                                    ProjectManager Default
                                                </div>
                                                <div style={{ width: '10%' }} >
                                                    {/*<form id="frm_91521316-bb9d-4b3d-8e73-2faed9440395" action="/Sso/Select" method="post" style="width:100%;">*/}
                                                    <input name="__RequestVerificationToken" type="hidden" value="CfDJ8HGObynaxXdCtDwsD6AKncoGAagZIfKFj9J2n_BWoQqR2IjxxmXHMn52PHVVPcwVO7xP79Larbpaa-P9qTMcQmXKxi9wKx2xA2JdMOtdzfoZFmEErtQ44t-Ria_qHfGam5RqaDjNnK2AatNfT6qS9WV0k7-_OH2MqCDtrPvaxHi41ismkyew_DZxt5siGg49Mg" />

                                                    <input type="hidden" name="Id" value="91521316-bb9d-4b3d-8e73-2faed9440395" />
                                                    <a href="javascript:void(0)" style={{ float: 'right' }} ><img src={expand} /></a>
                                                    {/*</form>*/}
                                                </div>
                                            </div>
                                            <div>
                                                <div style={{ width: '90%' }} >
                                                    Kör SC
                                                </div>
                                                <div style={{ width: '10%' }} >
                                                    {/*<form id="frm_ac79f687-04b0-4b3e-b0a9-942c1ffcf161" action="/Sso/Select" method="post" style="width:100%;">*/}
                                                    <input name="__RequestVerificationToken" type="hidden" value="CfDJ8HGObynaxXdCtDwsD6AKncoGAagZIfKFj9J2n_BWoQqR2IjxxmXHMn52PHVVPcwVO7xP79Larbpaa-P9qTMcQmXKxi9wKx2xA2JdMOtdzfoZFmEErtQ44t-Ria_qHfGam5RqaDjNnK2AatNfT6qS9WV0k7-_OH2MqCDtrPvaxHi41ismkyew_DZxt5siGg49Mg" />

                                                    <input type="hidden" name="Id" value="ac79f687-04b0-4b3e-b0a9-942c1ffcf161" />
                                                    <a href="javascript:void(0)" style={{ float: 'right' }} ><img src={expand} /></a>
                                                    {/*</form>*/}
                                                </div>
                                            </div>
                                            <div>
                                                <div style={{ width: '90%' }} >
                                                    Kör CRA
                                                </div>
                                                <div style={{ width: '10%' }} >
                                                    {/*<form id="frm_f4647268-26b4-47e0-9a19-7a3487aa5ba7" action="/Sso/Select" method="post" style="width:100%;">*/}
                                                    <input name="__RequestVerificationToken" type="hidden" value="CfDJ8HGObynaxXdCtDwsD6AKncoGAagZIfKFj9J2n_BWoQqR2IjxxmXHMn52PHVVPcwVO7xP79Larbpaa-P9qTMcQmXKxi9wKx2xA2JdMOtdzfoZFmEErtQ44t-Ria_qHfGam5RqaDjNnK2AatNfT6qS9WV0k7-_OH2MqCDtrPvaxHi41ismkyew_DZxt5siGg49Mg" />

                                                    <input type="hidden" name="Id" value="f4647268-26b4-47e0-9a19-7a3487aa5ba7" />
                                                    <a href="javascript:void(0)" style={{ float: 'right' }} ><img src={expand} /></a>
                                                    {/*</form>*/}
                                                </div>
                                            </div>
                                            <div>
                                                <div style={{ width: '90%' }}>
                                                    Eczacı Eczacı
                                                </div>
                                                <div style={{ width: '10%' }}>
                                                    {/*<form id="frm_83932f0e-475e-4fe3-bf13-5bff2899b1a9" action="/Sso/Select" method="post" style="width:100%;">*/}
                                                    <input name="__RequestVerificationToken" type="hidden" value="CfDJ8HGObynaxXdCtDwsD6AKncoGAagZIfKFj9J2n_BWoQqR2IjxxmXHMn52PHVVPcwVO7xP79Larbpaa-P9qTMcQmXKxi9wKx2xA2JdMOtdzfoZFmEErtQ44t-Ria_qHfGam5RqaDjNnK2AatNfT6qS9WV0k7-_OH2MqCDtrPvaxHi41ismkyew_DZxt5siGg49Mg" />

                                                    <input type="hidden" name="Id" value="83932f0e-475e-4fe3-bf13-5bff2899b1a9" />
                                                    <a href="javascript:void(0)" style={{ float: 'right' }} ><img src={expand} /></a>
                                                    {/*</form>*/}
                                                </div>
                                            </div>
                                            <div>
                                                <div style={{ width: '90%' }}>
                                                    DataManager Default
                                                </div>
                                                <div style={{ width: '10%' }}>
                                                    {/*<form id="frm_04fb6876-37b8-405f-9987-1fe6005f1912" action="/Sso/Select" method="post" style="width:100%;">*/}
                                                    <input name="__RequestVerificationToken" type="hidden" value="CfDJ8HGObynaxXdCtDwsD6AKncoGAagZIfKFj9J2n_BWoQqR2IjxxmXHMn52PHVVPcwVO7xP79Larbpaa-P9qTMcQmXKxi9wKx2xA2JdMOtdzfoZFmEErtQ44t-Ria_qHfGam5RqaDjNnK2AatNfT6qS9WV0k7-_OH2MqCDtrPvaxHi41ismkyew_DZxt5siGg49Mg" />

                                                    <input type="hidden" name="Id" value="04fb6876-37b8-405f-9987-1fe6005f1912" />
                                                    <a href="javascript:void(0)" style={{ float: 'right' }} ><img src={expand} /></a>
                                                    {/*</form>*/}
                                                </div>
                                            </div>
                                            <div>
                                                <div style={{ width: '90%' }}>
                                                    DataManager Default
                                                </div>
                                                <div style={{ width: '10%' }}>
                                                    {/*<form id="frm_110e1916-c8d3-4e37-9a10-d1b63ed1ad75" action="/Sso/Select" method="post" style="width:100%;">*/}
                                                    <input name="__RequestVerificationToken" type="hidden" value="CfDJ8HGObynaxXdCtDwsD6AKncoGAagZIfKFj9J2n_BWoQqR2IjxxmXHMn52PHVVPcwVO7xP79Larbpaa-P9qTMcQmXKxi9wKx2xA2JdMOtdzfoZFmEErtQ44t-Ria_qHfGam5RqaDjNnK2AatNfT6qS9WV0k7-_OH2MqCDtrPvaxHi41ismkyew_DZxt5siGg49Mg" />

                                                    <input type="hidden" name="Id" value="110e1916-c8d3-4e37-9a10-d1b63ed1ad75" />
                                                    <a href="javascript:void(0)" style={{ float: 'right' }} ><img src={expand} /></a>
                                                    {/*</form>*/}
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        );
    }
}

