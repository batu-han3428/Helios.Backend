import React from 'react';
import profile from '../Common/images/profile.png';
import '../Common/css/style.css';
import '../Common/css/adminCommon.css';

class _SubAdminLeftMenu extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            activeMenuItem: null
        };
    }

    handleMenuItemClick = (menuItem) => {
        this.setState({ activeMenuItem: menuItem });
        // Perform any additional actions when a menu item is clicked
    }

    render() {
        //const { activeMenuItem } = this.state;

        return (
            //<div classNameName="left-menu">
            //    <ul>
            //        <li classNameName={activeMenuItem === 'Home' ? 'active' : ''} onClick={() => this.handleMenuItemClick('Home')}>
            //            Home
            //        </li>
            //        <li classNameName={activeMenuItem === 'About' ? 'active' : ''} onClick={() => this.handleMenuItemClick('About')}>
            //            About
            //        </li>
            //        <li classNameName={activeMenuItem === 'Contact' ? 'active' : ''} onClick={() => this.handleMenuItemClick('Contact')}>
            //            Contact
            //        </li>
            //    </ul>
            //</div>

            <div id="leftmenuDiv">
                <nav className="navbar-default navbar-static-side scrollStyle" role="navigation">
                    <div className="sidebar-collapse scrollStyle">
                        <ul className="nav metismenu" id="side-menu">

                            <li className="nav-header">
                                <div className="dropdown profile-element">
                                    <span className="centerItem">
                                        <img data-name="Zeynep Mine Holta" className="img-circle avatarRound" style={{ width: '40px', height: '40px' }} src={profile}/>
                                    </span>
                                    <a data-toggle="dropdown" className="dropdown-toggle" href="#">
                                        <span className="clear">
                                            <span className="centerItem m-t-xs">
                                                <strong className="font-bold"> zholta</strong>
                                            </span>
                                            <span className="text-xs centerItem">
                                                Admin
                                            </span>
                                        </span>
                                    </a>
                                    <ul className="dropdown-menu animated fadeInRight m-t-xs">
                                        <li><a href="#">Profile</a></li>
                                        <li className="divider"></li>

                                        <li><a href="#">Logout</a></li>
                                    </ul>
                                </div>
                                <div id="collapsedSideNavLogo">
                                    {/*<img src="/favicon.ico" style={{ width: '45px', paddingBottom: '10px' }}/>*/}
                                </div>
                            </li>


                            <li className="">
                                <a href="#"><i className="icon-profile1"></i> <span className="nav-label">Adminstrators</span></a>
                            </li>

                            <li className="">
                                <a href="#"><i className="fa fa-envelope"></i><span className="nav-label">Email template</span></a>
                            </li>

                            <li className="">
                                <a href="#"><i className="icon-puzzle-piece-plugin"></i> <span className="nav-label">Modules</span></a>
                            </li>

                            <li className="">
                                <a href="#"><i className="fa fa-th-large"></i> <span className="nav-label">TMF template list</span></a>
                            </li>
                            <li className="active noActiveBG">
                                <a href="#">
                                    <i className="icon-open-folder"></i>
                                    <span className="nav-label">Studies </span>
                                    <span className="fa arrow"></span>
                                </a>
                                <ul className="nav nav-third-level active collapse in">
                                    <li className="active">
                                        <a href="#"><i className="icon-Unlock"></i> <span className="nav-label">Active</span></a>
                                    </li>
                                    <li className="">
                                        <a href="#"><i className="icon-locked"></i> <span className="nav-label">Locked</span></a>
                                    </li>
                                </ul>
                            </li>

                            <li className=" noActiveBG">
                                <a href="#"><i className="fa fa-cogs"></i> <span className="nav-label">Settings</span><span className="fa arrow"></span></a>
                                <ul className="nav nav-third-level collapse">

                                    <li className="">
                                        <a href="#">
                                            {/*<img src="/img/icons/contract.png">  <span className="nav-label">Terms of Use</span>*/}
                                        </a>
                                    </li>
                                    <li className="">
                                        <a href="#">
                                            {/*<img src="/img/icons/contract.png">  <span className="nav-label">Privacy policy</span>*/}
                                        </a>
                                    </li>
                                    <li className="">
                                        <a href="#">
                                            <i className="fa fa-envelope"></i>  <span className="nav-label">Setting mail template</span>
                                        </a>
                                    </li>

                                </ul>
                            </li>
                            <li id="collapseSidebar">
                                <a className="navbar-minimalize minimalize-styl-2" href="#"><i className="fa fa-bars"></i><span className="nav-label">Collapse menu</span> </a>
                            </li>
                        </ul>
                    </div>
                </nav>
            </div>
        );
    }
}

export default _SubAdminLeftMenu;