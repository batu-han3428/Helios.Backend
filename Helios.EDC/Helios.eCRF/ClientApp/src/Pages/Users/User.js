import PropTypes from 'prop-types';
import React, { useState, useEffect, useRef } from "react";
import {
    Row, Col, Button, Card, CardBody, FormFeedback, Label, Input, Form, Dropdown, DropdownToggle, DropdownMenu, DropdownItem
} from "reactstrap";
import { withTranslation } from "react-i18next";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import ModalComp from '../../components/Common/ModalComp/ModalComp';
import * as Yup from "yup";
import { useFormik } from "formik";
import { useUserListGetQuery, useUserGetQuery, useUserSetMutation, useUserActivePassiveMutation, useUserDeleteMutation, useUserResetPasswordMutation } from '../../store/services/Users';
import { useSelector, useDispatch } from 'react-redux';
import ToastComp from '../../components/Common/ToastComp/ToastComp';
import { startloading, endloading } from '../../store/loader/actions';
import Swal from 'sweetalert2'
import { formatDate } from "../../helpers/format_date";
import { MDBDataTable } from "mdbreact";
import Select from "react-select";
import { useRoleListGetQuery } from '../../store/services/Permissions';
import { useSiteListGetQuery } from '../../store/services/SiteLaboratories';
import makeAnimated from "react-select/animated";
import "./user.css";
import { arraysHaveSameItems } from '../../helpers/General/index';

const User = props => {

    const modalRef = useRef();

    const userInformation = useSelector(state => state.rootReducer.Login);

    const studyInformation = useSelector(state => state.rootReducer.Study);

    const [tableData, setTableData] = useState([]);
    const [studyUserId, setStudyUserId] = useState('00000000-0000-0000-0000-000000000000');
    const [updateItem, setUpdateItem] = useState({});
    const [optionGroupRoles, setOptionGroupRoles] = useState([]);
    const [optionGroupSites, setOptionGroupSites] = useState([]);
    const [showToast, setShowToast] = useState(false);
    const [message, setMessage] = useState("");
    const [stateToast, setStateToast] = useState(true);
    const [userControl, setUserControl] = useState(true);
    const [skip, setSkip] = useState(true);
    const [isRequired, setIsRequired] = useState(false);
    const [dropdownOpen, setDropdownOpen] = useState({});


    const animatedComponents = makeAnimated();

    const toggle = (userId) => {
        setDropdownOpen(prevState => {
            return {
                ...prevState,
                [userId]: !prevState[userId]
            };
        });
    };

    const dispatch = useDispatch();

    const generateInfoLabel = () => {
        var infoDiv = document.querySelector('.dataTables_info');
        var infoText = infoDiv.innerHTML;
        let words = infoText.split(" ");
        if (words[0] === "Showing") {
            let from = words[1];
            let to = words[3];
            let total = words[5];
            if (words[1] === "0") {
                from = "0";
                to = "0";
                total = "0";
            }
            infoDiv.innerHTML = props.t("Showing entries").replace("{from}", from).replace("{to}", to).replace("{total}", total);
        } else {
            let from = words[2];
            let to = words[4];
            let total = words[0];
            if (words[0] === "0") {
                from = "0";
                to = "0";
                total = "0";
            }
            infoDiv.innerHTML = props.t("Showing entries").replace("{from}", from).replace("{to}", to).replace("{total}", total);
        }
    };

    const data = {
        columns: [
            {
                label: "",
                field: "studyUserId",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("First name"),
                field: "name",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("Last name"),
                field: "lastName",
                sort: "asc",
                width: 150
            },
            {
                label: "Email",
                field: "email",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("Study role name"),
                field: "roleName",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("Site name"),
                field: "siteName",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("Created on"),
                field: "createdOn",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("Last updated on"),
                field: "lastUpdatedOn",
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
                label: props.t('Actions'),
                field: 'actions',
                sort: 'disabled',
                width: 100,
            }
        ],
        rows: tableData
    }

    const getActions = (item) => {
        const actions = (
            <div className="icon-container">
                <div title={props.t("Update")} className="icon icon-update" onClick={() => { updateUser(item) }}></div>
                <div title={props.t("Active or passive")} className="icon icon-lock" onClick={() => { activePassiveUser(item) }}></div>
                <div title={props.t("Delete")} className="icon icon-delete" onClick={() => { deleteUser(item) }}></div>
                <div title={props.t("Send a new password")} className="icon icon-resetpassword" onClick={() => { resetPasswordUser(item) }}></div>
            </div>);
        return actions;
    };

    const getSiteDropdown = (sites, userId) => {
        const siteDropdown = (
            <Dropdown isOpen={dropdownOpen[userId]} toggle={() => toggle(userId)}>
                <DropdownToggle caret>
                    {props.t("Sites")}
                </DropdownToggle>
                <DropdownMenu>
                    {sites.map((item, index) => (
                        <DropdownItem key={index} disabled>{item.siteFullName}</DropdownItem>
                    ))}
                </DropdownMenu>
            </Dropdown>
        );
        return siteDropdown;
    }
   
    const { data: usersData, error, isLoading } = useUserListGetQuery(studyInformation.studyId);

    useEffect(() => {
        dispatch(startloading());
        if (usersData && !isLoading && !error) {
            const updatedUsersData = usersData.map(item => {
                return {
                    ...item,
                    siteName: item.sites.length > 0 ? getSiteDropdown(item.sites, item.studyUserId) : "",
                    createdOn: formatDate(item.createdOn),
                    lastUpdatedOn: formatDate(item.lastUpdatedOn),
                    isActive: item.isActive ? props.t("Active") : props.t("Passive"),
                    actions: getActions(item)
                };
            });
            setTableData(updatedUsersData);

            const timer = setTimeout(() => {
                generateInfoLabel();
            }, 10)

            dispatch(endloading());

            return () => clearTimeout(timer);
        }
    }, [usersData, error, isLoading, props.t, dropdownOpen]);

    const { data: rolesData, isLoadingRoles, isErrorRoles } = useRoleListGetQuery(studyInformation.studyId);

    useEffect(() => {
        if (rolesData && !isLoadingRoles && !isErrorRoles) {
            let option = [{
                label: props.t("Roles"),
                options: []
            }]
            const roles = rolesData.map(item => {
                return {
                    label: item.roleName,
                    value: item.id
                };
            });
            option[0].options.push(...roles);
            setOptionGroupRoles(option);
        }
    }, [rolesData, isErrorRoles, isLoadingRoles]);

    const { data: sitesData, isLoadingSites, isErrorSites } = useSiteListGetQuery(studyInformation.studyId);

    useEffect(() => {
        if (sitesData && !isLoadingSites && !isErrorSites) {
            let option = [{
                label: props.t("Sites"),
                options: []
            }]
            const sites = sitesData.map(item => {
                return {
                    label: item.siteFullName,
                    value: item.id
                };
            });
            const allSiteIds = sitesData.map(item => item.id);
            const selectAllOption = {
                label: "Select All",
                value: ["All", allSiteIds]
            };
            sites.unshift(selectAllOption);
            option[0].options.push(...sites);
            setOptionGroupSites(option);
        }
    }, [sitesData, isErrorSites, isLoadingSites]);

    const resetValue = () => {
        validationType.validateForm().then(errors => {
            validationType.setErrors({});
            validationType.resetForm();
        });
        setUserControl(true);
        setIsRequired(false);
        setStudyUserId('00000000-0000-0000-0000-000000000000');
    };

    const [userSet] = useUserSetMutation();

    const validationType = useFormik({
        enableReinitialize: true,
        initialValues: {
            studyUserId: studyUserId,
            authUserId: "00000000-0000-0000-0000-000000000000",
            userid: userInformation.userId,
            tenantid: userInformation.tenantId,
            studyId: studyInformation.studyId,
            name: "",
            lastname: "",
            email: "",
            roleid: "00000000-0000-0000-0000-000000000000",
            siteIds: []
        },
        validationSchema: (values) => {
            return Yup.object().shape({
                name: isRequired ? Yup.string().required(props.t("This field is required")) : Yup.string(),
                lastname: isRequired ? Yup.string().required(props.t("This field is required")) : Yup.string(),
                email: Yup.string().required(props.t("This field is required")).email("Geçerli bir email girin"),
            });
        },
        onSubmit: async (values) => {
            try {
                dispatch(startloading());

                if (values.name !== "") {
                    const response = await userSet({
                        ...values,
                        siteIds: values.siteIds[0] === "All" ? values.siteIds[1][1] : values.siteIds,
                        password: "",
                        researchName: studyInformation.studyName,
                        researchLanguage: studyInformation.studyLanguage,
                        firstAddition: false
                    });
                    if (response.data.isSuccess) {
                        setMessage(response.data.message)
                        setStateToast(true);
                        setShowToast(true);
                        modalRef.current.tog_backdrop();
                        dispatch(endloading());
                    } else {
                        setMessage(response.data.message)
                        setStateToast(false);
                        setShowToast(true);
                        dispatch(endloading());
                    }
                } else {
                    setSkip(false);
                }
            } catch (e) {
                dispatch(endloading());
            }
        }
    });

    const updateUser = (item) => {
        dispatch(startloading());
        setStudyUserId(item.studyUserId);
        setUpdateItem(item);
        setUserControl(false);
    };

    useEffect(() => {
        if (studyUserId !== '00000000-0000-0000-0000-000000000000' && updateItem.authUserId !== "00000000-0000-0000-0000-000000000000") {
            const haveSameItems = arraysHaveSameItems(sitesData.map(site => site.id), updateItem.sites.map(site => site.id));

            let sites = null;

            if (haveSameItems) {
                sites = ["All", ["All", updateItem.sites.map(site => site.id)]];
            } else {
                sites = updateItem.sites.map(site => site.id);
            }

            validationType.setValues({
                studyUserId: studyUserId,
                authUserId: updateItem.authUserId,
                userid: userInformation.userId,
                tenantid: userInformation.tenantId,
                studyId: studyInformation.studyId,
                name: updateItem.name,
                lastname: updateItem.lastName,
                email: updateItem.email,
                roleid: updateItem.roleId,
                siteIds: sites
            });
            modalRef.current.tog_backdrop();
            dispatch(endloading());
        }
    }, [studyUserId, updateItem]);

    const [userActivePassive] = useUserActivePassiveMutation();

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
                    var activePassiveData = {
                        studyUserId: item.studyUserId,
                        authUserId: item.authUserId,
                        studyId: studyInformation.studyId,
                        userId: userInformation.userId,
                        name: item.name,
                        lastName: item.lastName,
                        isActive: item.isActive,
                        email: item.email,
                        roleId: item.roleId,
                        siteIds: [],
                        password: "",
                        researchName: studyInformation.studyName,
                        researchLanguage: studyInformation.studyLanguage,
                        firstAddition: false
                    };
                    const response = await userActivePassive(activePassiveData);
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

    const [userDelete] = useUserDeleteMutation(); 

    const deleteUser = (item) => {
        Swal.fire({
            title: props.t("You will not be able to recover this user!"),
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
                    var deleteData = {
                        studyUserId: item.studyUserId,
                        authUserId: item.authUserId,
                        studyId: studyInformation.studyId,
                        userId: userInformation.userId,
                        name: item.name,
                        lastName: item.lastName,
                        isActive: item.isActive,
                        email: item.email,
                        roleId: item.roleId,
                        siteIds: [],
                        password: "",
                        researchName: studyInformation.studyName,
                        researchLanguage: studyInformation.studyLanguage,
                        firstAddition: false
                    };
                    const response = await userDelete(deleteData);
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

    const [userResetPassword] = useUserResetPasswordMutation();

    const resetPasswordUser = async (item) => {
        try {
            dispatch(startloading());
            if (!item.isActive) {
                dispatch(endloading());
                setMessage(props.t("Please activate the account first and then try this process again."));
                setStateToast(false);
                setShowToast(true);
                return;
            }

            var resetPasswordData = {
                studyUserId: item.studyUserId,
                authUserId: item.authUserId,
                studyId: studyInformation.studyId,
                userId: userInformation.userId,
                name: item.name,
                lastName: item.lastName,
                isActive: item.isActive,
                email: item.email,
                roleId: item.roleId,
                siteIds: [],
                password: "",
                researchName: studyInformation.studyName,
                researchLanguage: studyInformation.studyLanguage,
                firstAddition: false
            };
            const response = await userResetPassword(resetPasswordData);
            if (response.data.isSuccess) {
                dispatch(endloading());
                setMessage(response.data.message)
                setStateToast(true);
                setShowToast(true);
            } else {
                dispatch(endloading());
                setMessage(response.data.message)
                setStateToast(false);
                setShowToast(true);
            }
        } catch (error) {
            dispatch(endloading());
            setMessage(props.t("An error occurred while processing your request."))
            setStateToast(true);
            setShowToast(true);
        }
    }

    const { data: userData, isErrorUser, isLoadingUser } = useUserGetQuery({ email: validationType.values.email, studyId: validationType.values.studyId }, {
        skip
    });

    useEffect(() => {
        if (userData && !isLoadingUser && !isErrorUser) {
            if (userData.isSuccess) {
                if (userData.values.authUserId !== "00000000-0000-0000-0000-000000000000") {
                    validationType.setValues({
                        studyUserId: studyUserId,
                        authUserId: userData.values.authUserId,
                        userid: userInformation.userId,
                        tenantid: userInformation.tenantId,
                        studyId: studyInformation.studyId,
                        name: userData.values.name,
                        lastname: userData.values.lastName,
                        email: userData.values.email,
                        roleid: "00000000-0000-0000-0000-000000000000",
                        siteIds: []
                    });
                } else {
                    setIsRequired(true);
                }
                setUserControl(false);
                setSkip(true);
                dispatch(endloading());
            } else {
                setSkip(true);
                dispatch(endloading());
                setMessage(userData.message)
                setStateToast(false);
                setShowToast(true);
            }
        } 
    }, [userData, isErrorUser, isLoadingUser]);

    return (
        <React.Fragment>
            <ModalComp
                refs={modalRef}
                title={studyUserId === '00000000-0000-0000-0000-000000000000' ? props.t("Add a user") : props.t("Update user")}
                body={
                    <>
                        {userControl ?
                            <Form
                                onSubmit={(e) => {
                                    e.preventDefault();
                                    validationType.handleSubmit();
                                    return false;
                                }}>
                                <div className="row">
                                    <div className="mb-3 col-md-12">
                                        <Label className="form-label">E-mail</Label>
                                        <Input
                                            name="email"
                                            placeholder="abc@hotmail.com"
                                            type="text"
                                            onChange={validationType.handleChange}
                                            onBlur={(e) => {
                                                validationType.handleBlur(e);
                                            }}
                                            value={validationType.values.email || ""}
                                            invalid={
                                                validationType.touched.email && validationType.errors.email ? true : false
                                            }
                                        />
                                    </div>
                                </div>
                            </Form>
                            :
                            <Form
                                onSubmit={(e) => {
                                    e.preventDefault();
                                    validationType.handleSubmit();
                                    return false;
                                }}>
                                <div className="row">
                                    <div className="mb-3 col-md-3">
                                        <Label className="form-label">{props.t("First name")}</Label>
                                        <Input
                                            name="name"
                                            placeholder={props.t("First name")}
                                            type="text"
                                            onChange={validationType.handleChange}
                                            onBlur={(e) => {
                                                validationType.handleBlur(e);
                                            }}
                                            value={validationType.values.name || ""}
                                            invalid={
                                                validationType.touched.name && validationType.errors.name ? true : false
                                            }
                                            disabled={validationType.values.authUserId !== "00000000-0000-0000-0000-000000000000" ? true : false}
                                        />
                                        {validationType.touched.name && validationType.errors.name ? (
                                            <FormFeedback type="invalid">{validationType.errors.name}</FormFeedback>
                                        ) : null}
                                    </div>
                                    <div className="mb-3 col-md-3">
                                        <Label className="form-label">{props.t("Last name")}</Label>
                                        <Input
                                            name="lastname"
                                            placeholder={props.t("Last name")}
                                            type="text"
                                            onChange={validationType.handleChange}
                                            onBlur={(e) => {
                                                validationType.handleBlur(e);
                                            }}
                                            value={validationType.values.lastname || ""}
                                            invalid={
                                                validationType.touched.lastname && validationType.errors.lastname ? true : false
                                            }
                                            disabled={validationType.values.authUserId !== "00000000-0000-0000-0000-000000000000" ? true : false}
                                        />
                                    </div>
                                    <div className="mb-3 col-md-6">
                                        <Label className="form-label">E-mail</Label>
                                        <Input
                                            name="email"
                                            placeholder="abc@hotmail.com"
                                            type="text"
                                            onChange={validationType.handleChange}
                                            onBlur={(e) => {
                                                validationType.handleBlur(e);
                                            }}
                                            value={validationType.values.email || ""}
                                            invalid={
                                                validationType.touched.email && validationType.errors.email ? true : false
                                            }
                                            disabled={true}
                                        />
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="mb-3 col-md-6">
                                        <Label className="form-label">{props.t("Role name")}</Label>
                                        <Select
                                            value={(optionGroupRoles.length > 0 && optionGroupRoles[0].options.find(option => option.value === validationType.values.roleid)) || ""}
                                            name="roleid"
                                            onChange={(selectedOption) => {
                                                const formattedValue = {
                                                    target: {
                                                        name: 'roleid',
                                                        value: selectedOption.value
                                                    }
                                                };
                                                validationType.handleChange(formattedValue);
                                            }}
                                            options={optionGroupRoles}
                                            classNamePrefix="select2-selection"
                                            placeholder={props.t("Select")}
                                        />
                                    </div>
                                    <div className="mb-3 col-md-6">
                                        <Label className="form-label">{props.t("Site name")}</Label>
                                        <Select
                                            value={validationType.values.siteIds[0] === "All" ? { label: "Select All", value: validationType.values.siteIds[1] } : optionGroupSites[0].options.filter(option => validationType.values.siteIds.includes(option.value))}
                                            name="siteIds"
                                            onChange={(selectedOptions) => {
                                                const selectedValues = selectedOptions.map(option => option.value);
                                                const selectAll = selectedValues.find(value => Array.isArray(value));
                                                if (selectAll !== undefined) {
                                                    validationType.setFieldValue('siteIds', ["All", selectAll]);
                                                } else {
                                                    validationType.setFieldValue('siteIds', selectedValues);
                                                }
                                            }}
                                            options={(function () {
                                                if (validationType.values.siteIds.includes("All")) {
                                                    return [];
                                                } else {
                                                    const allOptions = optionGroupSites[0].options;
                                                    const selectedOptions = [];
                                                    for (const option of allOptions) {
                                                        if (option.label !== "Select All") {
                                                            selectedOptions.push(option.value);
                                                        }
                                                    }
                                                    let result = arraysHaveSameItems(selectedOptions, validationType.values.siteIds);
                                                    if (result) {
                                                        return [];
                                                    } else {
                                                        return optionGroupSites[0].options;
                                                    }
                                                }
                                            })()}
                                            classNamePrefix="select2-selection"
                                            placeholder={props.t("Select")}
                                            isMulti={true}
                                            closeMenuOnSelect={false}
                                            components={animatedComponents}
                                        />
                                    </div>
                                </div>
                            </Form>
                        }
                    </>
                }
                resetValue={resetValue}
                handle={() => validationType.handleSubmit()}
                buttonText={studyUserId === '00000000-0000-0000-0000-000000000000' ? props.t("Save") : props.t("Update")}
            />
            <div className="page-content">
                <div className="container-fluid">
                    <div className="page-title-box">
                        <Row className="align-items-center" style={{ borderBottom: "1px solid black", paddingBottom: "5px" }}>
                            <Col md={8}>
                                <h6 className="page-title">{props.t("User list")}</h6>
                            </Col>
                            <Col md="4">
                                <div className="float-end d-none d-md-block">
                                    <Button
                                        color="success"
                                        className="btn btn-success waves-effect waves-light"
                                        type="button"
                                        onClick={() => modalRef.current.tog_backdrop()}
                                    >
                                        <FontAwesomeIcon icon="fa-solid fa-plus" /> {props.t("Add a user")}
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
            <ToastComp
                title="İşlem bilgisi"
                message={message}
                showToast={showToast}
                setShowToast={setShowToast}
                stateToast={stateToast}
            />
        </React.Fragment>
    );
};


User.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(User);