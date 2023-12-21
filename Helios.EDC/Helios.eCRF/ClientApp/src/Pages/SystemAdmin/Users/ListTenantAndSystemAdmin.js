import React, { useState, useEffect, useRef } from "react";
import PropTypes from 'prop-types';
import { withTranslation } from "react-i18next";
import { Row, Col, Button, Card, CardBody, Dropdown, DropdownToggle, DropdownMenu, DropdownItem } from "reactstrap";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { MDBDataTable } from "mdbreact";
import ModalComp from "../../../components/Common/ModalComp/ModalComp";
import AddOrUpdateTenantAndSystemAdmin from "./AddOrUpdateTenantAndSystemAdmin";
import ToastComp from "../../../components/Common/ToastComp/ToastComp";
import { useSelector, useDispatch } from 'react-redux';
import { useSystemAdminListGetQuery, useSystemAdminActivePassiveMutation, useSystemAdminResetPasswordMutation, useSystemAdminDeleteMutation } from '../../../store/services/SystemAdmin/SystemAdmin';
import { useUserListGetQuery } from '../../../store/services/SystemAdmin/Users/SystemUsers';
import { startloading, endloading } from '../../../store/loader/actions';
import Swal from 'sweetalert2';
import { countryNumber } from "../../../helpers/phonenumber_helper";
import DeleteTenantAndSystemAdmin from "./DeleteTenantAndSystemAdmin";
import ReactDOM from 'react-dom/client'
import { createPortal } from 'react-dom'

const ListTenantAndSystemAdmin = props => {

    const toastRef = useRef();

    const modalRef = useRef();

    const modalContentRef = useRef();

    const userInformation = useSelector(state => state.rootReducer.Login);

    const dispatch = useDispatch();

    const [modalTitle, setModalTitle] = useState("");
    const [modalButtonText, setModalButtonText] = useState("");
    const [adminId, setAdminId] = useState(0);
    const [table, setTable] = useState([]);

    const toastHandle = (message, state) => {
        toastRef.current.setToast({
            message: message,
            stateToast: state
        });
    }

    const addSystemAdmin = () => {
        setModalTitle(props.t("Add a admin"));
        setModalButtonText(props.t("Save"));
        modalRef.current.tog_backdrop();
    }

    const getActions = (item) => {
        const actions = (
            <div className="icon-container">
                <div title={props.t("Active or passive")} className="icon icon-lock" onClick={() => { activePassiveUser(item) }}></div>
                <div title={props.t("Delete")} className="icon icon-delete" onClick={() => { deleteUser(item) }}></div>
                <div title={props.t("Send a new password")} className="icon icon-resetpassword" onClick={() => { resetPasswordUser(item) }}></div>
            </div>);
        return actions;
    };

    const data = {
        columns: [
            {
                label: props.t("Name"),
                field: "name",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("Last Name"),
                field: "lastName",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("e-Mail"),
                field: "email",
                sort: "asc",
                width: 150
            },         
            {
                label: props.t("State"),
                field: "isActive",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("Phone number"),
                field: "phoneNumber",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("Roles"),
                field: "roles",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("Tenants"),
                field: "tenants",
                sort: "asc",
                width: 150
            },
            {
                label: props.t('Actions'),
                field: 'actions',
                sort: 'disabled',
                width: 100,
            }
        ],
        rows: table
    }

    const [dropdownOpen, setDropdownOpen] = useState({});

    const toggle = (userId) => {
        setDropdownOpen(prevState => {
            return {
                ...prevState,
                [userId]: !prevState[userId]
            };
        });
    };

    const getRolesDropdown = (roles, userId) => {
        const rolesDropdown = (
            <Dropdown isOpen={dropdownOpen[userId]} toggle={() => toggle(userId)}>
                <DropdownToggle caret>
                    {props.t("Roles")}
                </DropdownToggle>
                <DropdownMenu>
                    {roles.map((item, index) => (
                        <DropdownItem key={index} disabled>{item.roleName}</DropdownItem>
                    ))}
                </DropdownMenu>
            </Dropdown>
        );
        return rolesDropdown;
    }

    const [dropdownTenantOpen, setDropdownTenantOpen] = useState({});

    const toggleTenant = (userId) => {
        setDropdownTenantOpen(prevState => {
            return {
                ...prevState,
                [userId]: !prevState[userId]
            };
        });
    };

    const getTenantsDropdown = (tenants, userId) => {
        const tenantsDropdown = (
            <Dropdown isOpen={dropdownTenantOpen[userId]} toggle={() => toggleTenant(userId)}>
                <DropdownToggle caret>
                    {props.t("Tenants")}
                </DropdownToggle>
                <DropdownMenu>
                    {tenants.map((item, index) => (
                        <DropdownItem key={index} disabled>{item.tenantName}</DropdownItem>
                    ))}
                </DropdownMenu>
            </Dropdown>
        );
        return tenantsDropdown;
    }

    const { data: usersData, error, isLoading } = useUserListGetQuery();

    useEffect(() => {
        dispatch(startloading());
        if (!isLoading && !error && usersData) {
            console.log(usersData);
            const updatedUsersData = usersData.map(item => {
                return {
                    ...item,
                    isActive: item.isActive ? props.t("Active") : props.t("Passive"),
                    phoneNumber: item.phoneNumber ? countryNumber(item.phoneNumber) : "",
                    roles: item.roles !== null && item.roles.length > 0 ? getRolesDropdown(item.roles, item.id) : "",
                    tenants: item.tenants !== null && item.tenants.length > 0 ? getTenantsDropdown(item.tenants, item.id) : "",
                    actions: getActions(item)
                };
            });
            setTable(updatedUsersData);

            dispatch(endloading());
        }
    }, [usersData, error, isLoading, dropdownOpen, dropdownTenantOpen]);

    const [systemAdminActivePassive] = useSystemAdminActivePassiveMutation();

    const activePassiveUser = (item) => {
        Swal.fire({
            title: props.t("User active/passive status will be changed."),
            text: props.t("Do you confirm?"),
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3bbfad",
            confirmButtonText: props.t("Yes"),
            cancelButtonText: props.t("Cancel"),
            closeOnConfirm: false
        }).then(async (result) => {
            if (result.isConfirmed) {
                try {
                    dispatch(startloading());
                    const response = await systemAdminActivePassive({
                        id: item.id,
                        userId: userInformation.userId,
                        email: item.email
                    });
                    if (response.data.isSuccess) {
                        dispatch(endloading());
                        Swal.fire({
                            title: "",
                            text: props.t(response.data.message),
                            icon: "success",
                            confirmButtonText: props.t("Ok"),
                        });
                    } else {
                        dispatch(endloading());
                        Swal.fire({
                            title: "",
                            text: response.data.message,
                            icon: "error",
                            confirmButtonText: props.t("OK"),
                        });
                    }
                } catch (error) {
                    dispatch(endloading());
                    Swal.fire({
                        title: "",
                        text: props.t("An error occurred while processing your request."),
                        icon: "error",
                        confirmButtonText: props.t("OK"),
                    });
                }
            }
        });
    }

    const [systemAdminResetPassword] = useSystemAdminResetPasswordMutation();

    const resetPasswordUser = async (item) => {
        try {
            dispatch(startloading());
            if (!item.isActive) {
                dispatch(endloading());
                toastRef.current.setToast({
                    message: props.t("Please activate the account first and then try this process again."),
                    stateToast: false
                });
                return;
            }
            const response = await systemAdminResetPassword({
                id: item.id,
                userId: userInformation.userId,
                email: item.email,
                language: props.i18n.language
            });
            if (response.data.isSuccess) {
                dispatch(endloading());
                toastRef.current.setToast({
                    message: props.t(response.data.message),
                    stateToast: true
                });
            } else {
                dispatch(endloading());
                toastRef.current.setToast({
                    message: props.t(response.data.message),
                    stateToast: false
                });
            }
        } catch (error) {
            dispatch(endloading());
            toastRef.current.setToast({
                message: props.t("An error occurred while processing your request."),
                stateToast: false
            });
        }
    }

    const [systemAdminDelete] = useSystemAdminDeleteMutation();

    const [swalShown, setSwalShown] = useState({ show: false, value: null })
    const [deleteSubmit, setDeleteSubmit] = useState(false);

    const deleteUser = (item) => {
        Swal.fire({
            title: props.t("You will not be able to recover this user!"),
            html: '<div id="custom-container"></div>',
            showCancelButton: true,
            didOpen: () => {
                const customContainer = document.getElementById('custom-container');
                if (customContainer) {
                    customContainer.style.display = 'block';
                    setSwalShown({ show: true, value: item });
                }
            },
            didClose: () => setSwalShown({ show: false, value: null }),
            preConfirm: () => {
                setDeleteSubmit(true);
                return false;
            },
        })
    }

    return (
        <>
            <div className="page-content">
                <div className="container-fluid">
                    <div className="page-title-box">
                        <Row className="align-items-center" style={{ borderBottom: "1px solid black", paddingBottom: "5px" }}>
                            <Col md={8}>
                                <h6 className="page-title">{props.t("Add system admin")}</h6>
                            </Col>
                            <Col md="4">
                                <div className="float-end d-none d-md-block" style={{ marginLeft: "10px" }}>
                                    <Button
                                        color="success"
                                        className="btn btn-success waves-effect waves-light"
                                        type="button"
                                        onClick={addSystemAdmin}
                                    >
                                        <FontAwesomeIcon icon="fa-solid fa-plus" /> {props.t("Add a system admin")}
                                    </Button>
                                </div>
                            </Col>
                        </Row>
                    </div>
                    <Row>
                        <Col className="col-12">
                            <Card>
                                <CardBody>
                                    <MDBDataTable
                                        paginationLabel={[props.t("Previous"), props.t("Next")]}
                                        entriesLabel={props.t("Show entries")}
                                        searchLabel={props.t("Search")}
                                        noRecordsFoundLabel={props.t("No matching records found")}
                                        hover
                                        responsive
                                        striped
                                        bordered
                                        data={data}
                                    />
                                </CardBody>
                            </Card>
                        </Col>
                    </Row>
                </div>
            </div>
            <ModalComp
                refs={modalRef}
                title={modalTitle}
                body={<AddOrUpdateTenantAndSystemAdmin id={adminId} userId={userInformation.userId} refs={modalContentRef} toast={toastHandle} />}
                buttonText={modalButtonText}
                isButton={true}
            />
            <ToastComp
                ref={toastRef}
            />
            {swalShown.show &&
                createPortal(
                    <DeleteTenantAndSystemAdmin data={swalShown.value} submit={deleteSubmit} setSubmit={setDeleteSubmit} />
                    ,Swal.getHtmlContainer()
                )
            }
        </>
    )
};

ListTenantAndSystemAdmin.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(ListTenantAndSystemAdmin);