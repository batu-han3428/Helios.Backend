import React from 'react';
import {
    CDBSidebar,
    CDBSidebarHeader,
    CDBSidebarMenuItem,
    CDBSidebarContent,
    CDBSidebarMenu,
    CDBSidebarSubMenu,
    CDBSidebarFooter,
} from 'cdbreact';
import { NavLink } from 'react-router-dom';
import { PopupMenu } from "react-simple-widgets";
import "./sidebar.css";
import ProfileMenu from '../profileMenu';

const Sidebar = () => {
    return (
        <div style={{ display: 'flex', height: '100vh', overflow: 'scroll initial' }}>
            <CDBSidebar textColor="#333" backgroundColor="#eaebec">
                <CDBSidebarHeader prefix={<i className="fa fa-bars fa-large" style={{ border: 'none' }}></i>}>
                    {/*<a href="/" className="text-decoration-none" style={{ color: 'inherit' }}>*/}
                    {/*    Sidebar*/}
                    {/*</a>*/}

                    <div style={{ margin:"50px 0px 0px 70px" }}>
                        <ProfileMenu></ProfileMenu>
                    </div>
                </CDBSidebarHeader>

                <CDBSidebarContent className="sidebar-content" style={{ marginTop: "20px" }}>
                    <CDBSidebarMenu>
                        <NavLink exact to="/" activeClassName="activeClicked" className="nav-link">
                            <CDBSidebarMenuItem icon="columns" className="nav-link-a">Dashboard</CDBSidebarMenuItem>
                        </NavLink>
                        <NavLink exact to="/tables" activeClassName="activeClicked" className="nav-link">
                            <CDBSidebarMenuItem icon="table" className="nav-link-a">Tables</CDBSidebarMenuItem>
                        </NavLink>
                        <NavLink exact to="/profile" activeClassName="activeClicked" className="nav-link">
                            <CDBSidebarMenuItem icon="user" className="nav-link-a">Profile page</CDBSidebarMenuItem>
                        </NavLink>
                        {/*<NavLink exact to="/analytics1" activeClassName="activeClicked" className="nav-link">*/}
                        {/*        <CDBSidebarSubMenu title="Sidemenu" icon="th">*/}
                        {/*            <CDBSidebarMenuItem> submenu 1</CDBSidebarMenuItem>*/}
                        {/*            <CDBSidebarMenuItem> submenu 2</CDBSidebarMenuItem>*/}
                        {/*            <CDBSidebarMenuItem> submenu 3</CDBSidebarMenuItem>*/}
                        {/*        </CDBSidebarSubMenu>*/}
                        {/*</NavLink>*/}
                        <NavLink exact to="/analytics" activeClassName="activeClicked" className="nav-link">
                            <CDBSidebarMenuItem icon="chart-line" className="nav-link-a">Analytics</CDBSidebarMenuItem>
                        </NavLink>

                        <NavLink exact to="/hero404" target="_blank" activeClassName="activeClicked" className="nav-link">
                            <CDBSidebarMenuItem icon="exclamation-circle" className="nav-link-a">404 page</CDBSidebarMenuItem>
                        </NavLink>
                    </CDBSidebarMenu>
                </CDBSidebarContent>

                <CDBSidebarFooter style={{ textAlign: 'center' }}>
                    <div style={{ padding: '20px 5px', }}>
                        Sidebar Footer
                    </div>
                </CDBSidebarFooter>
            </CDBSidebar>
        </div>
    );
};

export default Sidebar;