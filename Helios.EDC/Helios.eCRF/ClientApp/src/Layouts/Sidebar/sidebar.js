import React from 'react';
import { Sidebar, Menu, MenuItem, useProSidebar, SubMenu, useLegacySidebar } from "react-pro-sidebar";
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
        <Sidebar style={{ height: "100vh" }}>
            <Menu>
                <MenuItem style={{ height: '70px', transition: "width, 300ms", width: `${collapsed ? '90px' : ''}` }  }><ProfileMenu></ProfileMenu></MenuItem>
                <MenuItem
                    icon={<MenuOutlinedIcon />}
                    onClick={() => { collapseSidebar(); }}
                    style={{ textAlign: "center" }}
                >
                    {" "}
                    {/*<ProfileMenu></ProfileMenu>*/}
                </MenuItem>
                <SubMenu icon={<HomeOutlinedIcon />} label="Home">
                    <MenuItem icon={<HomeOutlinedIcon />}>home 1</MenuItem>
                </SubMenu>
                <MenuItem icon={<PeopleOutlinedIcon />}>Team</MenuItem>
                <MenuItem icon={<ContactsOutlinedIcon />}>Contacts</MenuItem>
                <MenuItem icon={<ReceiptOutlinedIcon />}>Profile</MenuItem>
                <MenuItem icon={<HelpOutlineOutlinedIcon />}>FAQ</MenuItem>
                <MenuItem icon={<CalendarTodayOutlinedIcon />}>Calendar</MenuItem>
            </Menu>
        </Sidebar>
    )
}

export default Side;