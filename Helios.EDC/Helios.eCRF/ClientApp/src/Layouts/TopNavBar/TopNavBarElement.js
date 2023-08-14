import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './TopNavBarElement.css';
import logo from '../../Common/images/helios_222_70.png';
import profile from '../../Common/images/profile.png';
import { PopupMenu } from "react-simple-widgets";
import ProfileMenu from '../profileMenu';

export default class TopNavBarElement extends Component {
    static displayName = TopNavBarElement.name;

    constructor(props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true
        };
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    render() {
        return (
            <header>
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
                    {/*<NavbarBrand tag={Link} to="/">Helios.eCRF</NavbarBrand>*/}
                    <div className="navLogoContainer">
                        <img className="navLogoImg" src={logo} />
                        <em id="navResearchName"> Studies </em>
                    </div>
                    <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                    <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                        <ul className="navbar-nav flex-grow">
                            <NavItem style={{ marginRight: '50px' }}>
                                {/*<NavLink tag={Link} className="text-dark" to="/">Home</NavLink>*/}
                                {/*</NavItem>*/}
                                {/*<NavItem>*/}
                                {/*  <NavLink tag={Link} className="text-dark" to="/counter">Counter</NavLink>*/}
                                {/*</NavItem>*/}
                                {/*<NavItem>*/}
                                {/*  <NavLink tag={Link} className="text-dark" to="/fetch-data">Fetch data</NavLink>*/}
                            </NavItem>
                        </ul>
                    </Collapse>
                    <div id="rightSideNav">
                        <ProfileMenu></ProfileMenu>
                    </div>
                </Navbar>
            </header>
        );
    }
}

