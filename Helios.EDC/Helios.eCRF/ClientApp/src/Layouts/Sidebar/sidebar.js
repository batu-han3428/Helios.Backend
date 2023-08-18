import React from 'react';
import { Sidebar, Menu, MenuItem, useProSidebar, SubMenu, useLegacySidebar, sidebarClasses } from "react-pro-sidebar";
import HomeOutlinedIcon from "@mui/icons-material/HomeOutlined";
import PeopleOutlinedIcon from "@mui/icons-material/PeopleOutlined";
import ContactsOutlinedIcon from "@mui/icons-material/ContactsOutlined";
import ReceiptOutlinedIcon from "@mui/icons-material/ReceiptOutlined";
import CalendarTodayOutlinedIcon from "@mui/icons-material/CalendarTodayOutlined";
import HelpOutlineOutlinedIcon from "@mui/icons-material/HelpOutlineOutlined";
import MenuOutlinedIcon from "@mui/icons-material/MenuOutlined";
import ProfileMenu from '../profileMenu';
import "./sidebar.css";

const Side = () => {

    const { collapseSidebar, toggleSidebar, collapsed, toggled, broken, rtl } =
        useProSidebar();

    return (
        <>
            <Sidebar className="sidebar">
                <Menu
                    style={{ height: '95vh', overflowY: 'auto', overflowX: 'hidden' }}
                    menuItemStyles={{
                        button: ({ level, active, disabled }) => {
                            return {
                                '&:hover': {
                                    backgroundColor: '#FFC600',
                                },
                            };
                        },
                    }}
                >
                   {/* <MenuItem disabled={true} style={{ height: '70px', transition: "width, 300ms", width: `${collapsed ? '90px' : ''}` }}>*/}<ProfileMenu></ProfileMenu>{/*</MenuItem>*/}
                    <SubMenu icon={<HomeOutlinedIcon />} label="Home">
                        <MenuItem icon={<HomeOutlinedIcon />}>home 1</MenuItem>
                    </SubMenu>
                    <MenuItem icon={<PeopleOutlinedIcon />}>Team</MenuItem>
                    <MenuItem icon={<ContactsOutlinedIcon />}>Contacts</MenuItem>
                    <MenuItem icon={<ReceiptOutlinedIcon />}>Profile</MenuItem>
                    <MenuItem icon={<HelpOutlineOutlinedIcon />}>FAQ</MenuItem>
                    <MenuItem icon={<CalendarTodayOutlinedIcon />}>Calendar</MenuItem>      
                    <SubMenu icon={<HomeOutlinedIcon />} label="Home">
                        <MenuItem icon={<HomeOutlinedIcon />}>home 1</MenuItem>
                    </SubMenu>
                    <MenuItem icon={<PeopleOutlinedIcon />}>Team</MenuItem>
                    <MenuItem icon={<ContactsOutlinedIcon />}>Contacts</MenuItem>
                    <MenuItem icon={<ReceiptOutlinedIcon />}>Profile</MenuItem>
                    <MenuItem icon={<HelpOutlineOutlinedIcon />}>FAQ</MenuItem>
                    <MenuItem icon={<CalendarTodayOutlinedIcon />}>Calendar</MenuItem>
                    <SubMenu icon={<HomeOutlinedIcon />} label="Home">
                        <MenuItem icon={<HomeOutlinedIcon />}>home 1</MenuItem>
                    </SubMenu>
                    <MenuItem icon={<PeopleOutlinedIcon />}>Team</MenuItem>
                    <MenuItem icon={<ContactsOutlinedIcon />}>Contacts</MenuItem>
                    <MenuItem icon={<ReceiptOutlinedIcon />}>Profile</MenuItem>
                    <MenuItem icon={<HelpOutlineOutlinedIcon />}>FAQ</MenuItem>
                    <MenuItem icon={<CalendarTodayOutlinedIcon />}>Calendar</MenuItem>
                    <SubMenu icon={<HomeOutlinedIcon />} label="Home">
                        <MenuItem icon={<HomeOutlinedIcon />}>home 1</MenuItem>
                    </SubMenu>
                    <MenuItem icon={<PeopleOutlinedIcon />}>Team</MenuItem>
                    <MenuItem icon={<ContactsOutlinedIcon />}>Contacts</MenuItem>
                    <MenuItem icon={<ReceiptOutlinedIcon />}>Profile</MenuItem>
                    <MenuItem icon={<HelpOutlineOutlinedIcon />}>FAQ</MenuItem>
                    <MenuItem icon={<CalendarTodayOutlinedIcon />}>Calendar</MenuItem>
                    <SubMenu icon={<HomeOutlinedIcon />} label="Home">
                        <MenuItem icon={<HomeOutlinedIcon />}>home 1</MenuItem>
                    </SubMenu>
                    <MenuItem icon={<PeopleOutlinedIcon />}>Team</MenuItem>
                    <MenuItem icon={<ContactsOutlinedIcon />}>Contacts</MenuItem>
                    <MenuItem icon={<ReceiptOutlinedIcon />}>Profile</MenuItem>
                    <MenuItem icon={<HelpOutlineOutlinedIcon />}>FAQ</MenuItem>
                    <MenuItem icon={<CalendarTodayOutlinedIcon />}>Calendar</MenuItem>
                    <SubMenu icon={<HomeOutlinedIcon />} label="Home">
                        <MenuItem icon={<HomeOutlinedIcon />}>home 1</MenuItem>
                    </SubMenu>
                    <MenuItem icon={<PeopleOutlinedIcon />}>Team</MenuItem>
                    <MenuItem icon={<ContactsOutlinedIcon />}>Contacts</MenuItem>
                    <MenuItem icon={<ReceiptOutlinedIcon />}>Profile</MenuItem>
                    <MenuItem icon={<HelpOutlineOutlinedIcon />}>FAQ</MenuItem>
                    <MenuItem icon={<CalendarTodayOutlinedIcon />}>Calendar</MenuItem>
                    <SubMenu icon={<HomeOutlinedIcon />} label="Home">
                        <MenuItem icon={<HomeOutlinedIcon />}>home 1</MenuItem>
                    </SubMenu>
                    <MenuItem icon={<PeopleOutlinedIcon />}>Team</MenuItem>
                    <MenuItem icon={<ContactsOutlinedIcon />}>Contacts</MenuItem>
                    <MenuItem icon={<ReceiptOutlinedIcon />}>Profile</MenuItem>
                    <MenuItem icon={<HelpOutlineOutlinedIcon />}>FAQ</MenuItem>
                    <MenuItem icon={<CalendarTodayOutlinedIcon />}>Calendar</MenuItem>
                    {/*<div>*/}
                    {/*    <MenuItem*/}
                    {/*        icon={<MenuOutlinedIcon />}*/}
                    {/*        onClick={() => { collapseSidebar(); }}*/}
                    {/*        style={{ textAlign: "center" }}*/}
                    {/*    >*/}
                    {/*        {" "}*/}
                    {/*        */}{/*<ProfileMenu></ProfileMenu>*/}
                    {/*    </MenuItem>*/}
                    {/*</div>*/}
               
                </Menu>
                <div className="collapse" onClick={() => collapseSidebar()}>
                    <MenuOutlinedIcon /> 
                </div>
            </Sidebar>
            
        </>
    )
}

export default Side;