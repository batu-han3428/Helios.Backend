import PropTypes from 'prop-types';
import React, { useState, useEffect, useRef } from "react";
import {
    Row, Col, Button, Label, Input, Form, FormFeedback
} from "reactstrap";
import { withTranslation } from "react-i18next";
import permissionItems from './PermissionItems';
import "./Permission.css";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import ModalComp from '../../components/Common/ModalComp/ModalComp';
import * as Yup from "yup";
import { useFormik } from "formik";
import { useRoleSaveMutation, useRolePermissionListGetQuery, useSetPermissionMutation, useRoleDeleteMutation } from '../../store/services/Permissions';
import { useSelector, useDispatch } from 'react-redux';
import ToastComp from '../../components/Common/ToastComp/ToastComp';
import { startloading, endloading } from '../../store/loader/actions';
import Swal from 'sweetalert2'

const Permission = props => {

    const modalRef = useRef();

    const userInformation = useSelector(state => state.rootReducer.Login);

    const studyInformation = useSelector(state => state.rootReducer.Study);

    const dispatch = useDispatch();

    const [openSections, setOpenSections] = useState({});
    const [roleId, setRoleId] = useState('00000000-0000-0000-0000-000000000000');
    const [showToast, setShowToast] = useState(false);
    const [message, setMessage] = useState("");
    const [stateToast, setStateToast] = useState(true);
    const [roles, setRoles] = useState([]);
    const [selectedRole, setSelectedRole] = useState(null);

    const toggleAccordion = (key) => {
        setOpenSections(prevOpenSections => ({
            ...prevOpenSections,
            [key]: !prevOpenSections[key]
        }));
    };

    const resetValue = () => {
        validationType.validateForm().then(errors => {
            validationType.setErrors({});
            validationType.resetForm();
        });
        setRoleId('00000000-0000-0000-0000-000000000000');
        setSelectedRole(null);
    };

    const { data: roleData, error, isLoading } = useRolePermissionListGetQuery(studyInformation.studyId);

    useEffect(() => {
        if (!isLoading && !error && roleData) {
            setRoles(roleData);
        }
    }, [roleData, error, isLoading]);

    const [setPermission] = useSetPermissionMutation();

    const updatePermission = async (id, name, value) => {
        dispatch(startloading());
        let model = {
            userid: userInformation.userId,
            id: id,
            permissionName: name,
            value: value,
        };
        const response = await setPermission(model);
        if (response.data.isSuccess) {
            setMessage(response.data.message)
            setStateToast(true);
            setShowToast(true);
            dispatch(endloading());
        } else {
            setMessage(response.data.message)
            setStateToast(false);
            setShowToast(true);
            dispatch(endloading());
        }
    }

    const updateRole = (id, name) => {
        dispatch(startloading());
        setRoleId(id);
        setSelectedRole(name);
    };

    useEffect(() => {
        if (roleId !== '00000000-0000-0000-0000-000000000000' && selectedRole !== null) {
            validationType.setValues({
                id: roleId,
                userid: userInformation.userId,
                tenantid: userInformation.tenantId,
                studyId: studyInformation.studyId,
                name: selectedRole,
            });
            modalRef.current.tog_backdrop();
            dispatch(endloading());
        }
    }, [roleId, selectedRole]);

    const [roleDelete] = useRoleDeleteMutation();

    const deleteRole = (id, name) => {
        Swal.fire({
            title: props.t("You will not be able to recover this user role!"),
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
                        id: id,
                        userid: userInformation.userId,
                        tenantid: userInformation.tenantId,
                        studyId: studyInformation.studyId,
                        roleName: name,
                    };
                    const response = await roleDelete(deleteData);
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
    };

    const [roleSave] = useRoleSaveMutation();

    const validationType = useFormik({
        enableReinitialize: true,
        initialValues: {
            id: roleId,
            userid: userInformation.userId,
            tenantid: userInformation.tenantId,
            studyId: studyInformation.studyId,
            rolename: selectedRole || "",
        },
        validationSchema: Yup.object().shape({
            rolename: Yup.string().required(
                props.t("This field is required")
            ),
        }),
        onSubmit: async (values) => {
            dispatch(startloading());
            const response = await roleSave(values);
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
        }
    });

    return (      
        <React.Fragment>
            <div className="page-content">
                <div className="container-fluid">
                    <div className="page-title-box">
                        <Row className="align-items-center" style={{ borderBottom: "1px solid black", paddingBottom:"5px" }}>
                            <Col md={8}>
                                <h6 className="page-title">{props.t("Permission list")}</h6>
                            </Col>
                            <Col md="4">
                                <div className="float-end d-none d-md-block">
                                    <Button
                                        color="success"
                                        className="btn btn-success waves-effect waves-light"
                                        type="button"
                                        onClick={() => modalRef.current.tog_backdrop()}
                                    >
                                    <FontAwesomeIcon icon="fa-solid fa-plus" /> {props.t("Add a role")}
                                    </Button>
                                </div>
                            </Col>
                        </Row>
                    </div>
                    <Row>
                        <Col className="col-12">
                            <div className="" id="" datenow="" style={{paddingTop: "28px"}}>
                                <table className="table-header-rotated">
                                    <thead>
                                        <tr>
                                            <th className="rotate">
                                                <div style={{ float: "right" }} >
                                                    <div className="span2">
                                                        <span style={{padding: "3px 0px"}}>&nbsp;</span>
                                                    </div>
                                                </div>
                                            </th>

                                            {roles.map((item, index) => (
                                                <th key={index} className="rotate">
                                                    <div>
                                                        <div className="span2">
                                                            <span>
                                                                <label className={item.roleName.length > 35 ? "lbl-permision tooltip2" : "lbl-permision"} title={item.RoleName}>
                                                                    {item.roleName}
                                                                </label>
                                                                <a title={props.t("Page permissions") } className="tooltip2 btn-permision" >
                                                                    <FontAwesomeIcon icon="fa-solid fa-file-pen" style={{ fontSize: "16px", verticalAlign: "middle" }} />
                                                                </a>
                                                                <a style={{ margin: "5px" }} title={ props.t("Show users") } className="tooltip2 btn-permision">
                                                                    <FontAwesomeIcon icon="fa-solid fa-person" style={{ fontSize: "16px", verticalAlign: "middle" }} />
                                                                </a>
                                                                <a onClick={() => { updateRole(item.id, item.roleName) }} style={{ margin: "5px" }} className="tooltip2 btn-permision" title={ props.t("Update") }>
                                                                    <FontAwesomeIcon icon="fa-solid fa-marker" style={{ fontSize: "16px", verticalAlign: "middle" }} />
                                                                </a>
                                                                <a onClick={() => { deleteRole(item.id, item.roleName) }} style={{ margin: "5px" }} className="tooltip2 btn-permision" title={ props.t("Delete") }>
                                                                    <FontAwesomeIcon icon="fa-solid fa-trash-can" style={{ fontSize: "16px", verticalAlign: "middle" }} />
                                                                </a>
                                                            </span>
                                                        </div>
                                                    </div>
                                                </th>
                                            ))}
                                            <th style={{ width: "140px" }} ></th>
                                        </tr>
                                    </thead>

                                    {Object.keys(permissionItems).map(key => {
                                        return (
                                            <React.Fragment key={key}>
                                                <tbody>
                                                    <tr>
                                                        <td className="rowgroup" style={{ width: "300px" }} >
                                                            <label className="ttlLi floatl hd-tgl" onClick={() => toggleAccordion(key)} data-id="0"><i className={`fa ${openSections[key] ? 'fa-minus sign-hd' : 'fa-plus sign-hd'}`} ></i>{key}</label>
                                                            <input type="checkbox" data-toggle="toggle" /><span className="floatr">
                                                                <a data-name="@HeliosResource.js_Tmf" data-lock="false" /*onclick="lock(this)"*/ style={{ color: "#6D6E70" }}>
                                                                    <i className=" icon-locked "></i>
                                                                </a><a data-name="@HeliosResource.js_Tmf" data-lock="true" /*onclick="lock(this)"*/ style={{ color: "#6D6E70" }} hidden><i className="fa fa-unlock-alt"></i></a>
                                                            </span>
                                                        </td>
                                                        {roles.map((item, index) => (
                                                            <td key={`${key}_${item.id}`} className="rowgroup">{String.fromCharCode(8203)}</td> 
                                                        ))}
                                                    </tr>
                                                </tbody>
                                                {openSections[key] && (
                                                    <tbody className="hide hide-hd">
                                                        {permissionItems[key].map(item => {
                                                            return (
                                                                <tr key={`${key}_${item.name}`}>
                                                                    <td className="tdname">{item.label}</td>
                                                                    {roles.map((role, index) => { 
                                                                        const isPermissionEnabled = role[item.name];
                                                                            return (
                                                                                <td key={`${item.name}_${role.id}`} className="subjectPers">
                                                                                    <input type="checkbox" className="checkbox chck-permision" name="Add" data-userpermissionid="Add_@Model.UserPermissions[i].Id" onChange={(e) => { updatePermission(role.id, item.name, e.target.checked); }} checked={isPermissionEnabled} />
                                                                            </td>
                                                                        )
                                                                     })}
                                                                </tr>
                                                            )
                                                        })}
                                                    </tbody>
                                                )}

                                            </React.Fragment>
                                        );
                                    })}
                                </table>
                            </div>
                        </Col>
                    </Row>
                </div>
            </div>
            <ModalComp
                refs={modalRef}
                resetValue={resetValue}
                title={roleId === '00000000-0000-0000-0000-000000000000' ? props.t("Add a role") : props.t("Update role")}
                body={
                    <>
                        <Form
                            onSubmit={(e) => {
                                e.preventDefault();
                                validationType.handleSubmit();
                                return false;
                            }}>
                            <div className="row">
                                <div className="mb-3 col-md-12">
                                    <Label className="form-label">{props.t("Role name")}</Label>
                                    <Input
                                        name="rolename"
                                        placeholder={props.t("Role name")}
                                        type="text"
                                        onChange={validationType.handleChange}
                                        onBlur={(e) => {
                                            validationType.handleBlur(e);
                                        }}
                                        value={validationType.values.rolename || ""}
                                        invalid={
                                            validationType.touched.rolename && validationType.errors.rolename ? true : false
                                        }
                                    />
                                    {validationType.touched.rolename && validationType.errors.rolename ? (
                                        <FormFeedback type="invalid">{validationType.errors.rolename}</FormFeedback>
                                    ) : null}
                                </div>
                            </div>
                        </Form>
                    </>
                }
                handle={() => validationType.handleSubmit()}
                buttonText={roleId === '00000000-0000-0000-0000-000000000000' ? props.t("Save") : props.t("Update")}
            />
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


Permission.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(Permission);