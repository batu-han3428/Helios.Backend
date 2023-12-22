import React, { useState, useEffect, useRef } from "react";
import PropTypes from 'prop-types';
import { withTranslation } from "react-i18next";
import * as Yup from "yup";
import { useFormik } from "formik";
import { Label, Input, Form, FormFeedback } from "reactstrap";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import AddOrUpdateTenantAndSystemAdmin from "./AddOrUpdateTenantAndSystemAdmin";
import { useSelector, useDispatch } from 'react-redux';
import { useSystemAdminListGetQuery, useSystemAdminActivePassiveMutation, useSystemAdminResetPasswordMutation, useSystemAdminDeleteMutation } from '../../../store/services/SystemAdmin/SystemAdmin';
import { useUserListGetQuery } from '../../../store/services/SystemAdmin/Users/SystemUsers';
import { startloading, endloading } from '../../../store/loader/actions';
import makeAnimated from "react-select/animated";
import Select from "react-select";
import { arraysHaveSameItems } from "../../../helpers/General/index";

const DeleteTenantAndSystemAdmin = props => {

    useEffect(() => {
        console.log(props)
    }, [props])

    const animatedComponents = makeAnimated();

    const dispatch = useDispatch();

    const [optionGroupRoles, setOptionGroupRoles] = useState([]);
    const [optionGroupTenants, setOptionGroupTenants] = useState([]);

    useEffect(() => {
        console.log('useeffect role')
        if (props.data.roles && props.data.roles.length > 0) {
            if (!props.isDropdown) {
                validationType.setFieldValue("roleIds", props.data.roles.map(x => x.roleId));
            } else {
                let optionRoles = [
                    {
                        label: props.t("Roles"),
                        options: [
                            {
                                label: props.t("Select all"),
                                value: [
                                    "All",
                                    props.data.roles.map(x => x.roleId)
                                ]
                            }
                        ]
                    }
                ]

                const rolesData = props.data.roles.map(item => {
                    return {
                        label: item.roleName,
                        value: item.roleId
                    };
                });

                optionRoles[0].options.push(...rolesData);
                setOptionGroupRoles(optionRoles);
            }
        }
    }, [props.data.roles]);

    useEffect(() => {
        console.log('useeffect tenants')
        if (props.data.tenants && props.data.tenants.length > 0) {
            if (!props.isDropdown) {
                validationType.setFieldValue("tenantIds", props.data.tenants.map(x => x.tenantId));
            } else {
                let optionTenants = [
                    {
                        label: props.t("Tenants"),
                        options: [
                            {
                                label: props.t("Select all"),
                                value: [
                                    "All",
                                    props.data.tenants.map(x => x.tenantId)
                                ]
                            }
                        ]
                    }
                ]

                const tenantsData = props.data.tenants.map(item => {
                    return {
                        label: item.tenantName,
                        value: item.tenantId
                    };
                });

                optionTenants[0].options.push(...tenantsData);
                setOptionGroupTenants(optionTenants);
            }
          
        }
    }, [props.data.tenants]);

    const validationType = useFormik({
        enableReinitialize: true,
        initialValues: {
            id: props.data.id,
            userid: props.data.userId,
            roleIds: [],
            tenantIds: []
        },
        validationSchema: Yup.object().shape({
            roleIds: Yup.array().min(1, "At least one role must be selected"),
            tenantIds: Yup.array()
                .test('isTenantIdsValid', props.t('At least one tenant must be selected'), function (value) {
                    const roleIds = this.parent.roleIds;
                    return !(roleIds && (roleIds.includes(3) || (roleIds.length > 0 && roleIds[0] === 'All'))) || (value && value.length >= 1);
                }),
        }),
        onSubmit: async (values) => {
            console.log(values)
            //try {
            //    dispatch(startloading());

            //    const response = await userSet({
            //        ...values,
            //        roleIds: values.roleIds[0] === "All" ? values.roleIds[1][1] : values.roleIds,
            //        tenantIds: values.tenantIds[0] === "All" ? values.tenantIds[1][1] : values.tenantIds,
            //    });

            //    if (response.data.isSuccess) {
            //        props.onToggleModal();
            //        props.toast(props.t(response.data.message), true);
            //        dispatch(endloading());
            //    } else {
            //        props.toast(props.t(response.data.message), false);
            //        dispatch(endloading());
            //    }
            //} catch (e) {
            //    props.toast(props.t("An unexpected error occurred."), false);
            //    dispatch(endloading());
            //}
        }
    });

    useEffect(() => {
        console.log(props)
        if (props.submit) {
            validationType.handleSubmit();
            props.setSubmit(false);
        }
    }, [props.submit])


    return (
        <>
            {
                props.isDropdown ?
                <div style={{ padding: "15px 10px", height: "200px" }}>
                    <Form
                        onSubmit={(e) => {
                            e.preventDefault();
                            validationType.handleSubmit();
                            return false;
                        }}>
                        <div className="row">
                            <div className="mb-3 col-md-12">
                                <Label className="form-label">{props.t("Roles")}</Label>
                                <Select
                                    value={optionGroupRoles.length > 0 ? validationType.values.roleIds[0] === "All" ? { label: props.t("Select all"), value: validationType.values.roleIds[1] } : optionGroupRoles[0].options.filter(option => validationType.values.roleIds.includes(option.value)) : []}
                                    name="roleIds"
                                    onChange={(selectedOptions) => {
                                        const selectedValues = selectedOptions.map(option => option.value);
                                        const selectAll = selectedValues.find(value => Array.isArray(value));
                                        if (selectAll !== undefined) {
                                            console.log(["All", selectAll])
                                            validationType.setFieldValue('roleIds', ["All", selectAll]);
                                        } else {
                                            console.log(selectedValues)
                                            validationType.setFieldValue('roleIds', selectedValues);
                                        }
                                    }}
                                    options={(function () {
                                        if (validationType.values.roleIds.includes("All") || optionGroupRoles.length < 1) {
                                            return [];
                                        } else {
                                            const allOptions = optionGroupRoles[0].options;
                                            const selectedOptions = [];
                                            for (const option of allOptions) {
                                                if (option.label !== props.t("Select all")) {
                                                    selectedOptions.push(option.value);
                                                }
                                            }
                                            let result = arraysHaveSameItems(selectedOptions, validationType.values.roleIds);
                                            if (result) {
                                                return [];
                                            } else {
                                                return optionGroupRoles[0].options;
                                            }
                                        }
                                    })()}
                                    classNamePrefix="select2-selection"
                                    placeholder={props.t("Select")}
                                    isMulti={true}
                                    closeMenuOnSelect={false}
                                    components={animatedComponents}
                                    maxMenuHeight={100}
                                />
                                {validationType.touched.roleIds && validationType.errors.roleIds ? (
                                    <div type="invalid" className="invalid-feedback" style={{ display: "block" }}>{validationType.errors.roleIds}</div>
                                ) : null}
                            </div>
                            {
                                validationType.values.roleIds.length > 0 && (validationType.values.roleIds.includes(3) || validationType.values.roleIds[0] === 'All') &&

                                <div className="mb-3 col-md-12">
                                    <Label className="form-label">{props.t("Tenants")}</Label>
                                    <Select
                                        value={validationType.values.tenantIds.length > 0 ? validationType.values.tenantIds[0] === "All" ? { label: props.t("Select all"), value: validationType.values.tenantIds[1] } : optionGroupTenants[0].options.filter(option => validationType.values.tenantIds.includes(option.value)) : []}
                                        name="tenantIds"
                                        onChange={(selectedOptions) => {
                                            const selectedValues = selectedOptions.map(option => option.value);
                                            const selectAll = selectedValues.find(value => Array.isArray(value));
                                            if (selectAll !== undefined) {
                                                validationType.setFieldValue('tenantIds', ["All", selectAll]);
                                            } else {
                                                validationType.setFieldValue('tenantIds', selectedValues);
                                            }
                                        }}
                                        options={(function () {
                                            if (validationType.values.tenantIds.includes("All") || optionGroupTenants.length < 1) {
                                                return [];
                                            } else {
                                                const allOptions = optionGroupTenants[0].options;
                                                const selectedOptions = [];
                                                for (const option of allOptions) {
                                                    if (option.label !== props.t("Select all")) {
                                                        selectedOptions.push(option.value);
                                                    }
                                                }
                                                let result = arraysHaveSameItems(selectedOptions, validationType.values.tenantIds);
                                                if (result) {
                                                    return [];
                                                } else {
                                                    return optionGroupTenants[0].options;
                                                }
                                            }
                                        })()}
                                        classNamePrefix="select2-selection"
                                        placeholder={props.t("Select")}
                                        isMulti={true}
                                        closeMenuOnSelect={false}
                                        components={animatedComponents}
                                    />
                                    {validationType.touched.tenantIds && validationType.errors.tenantIds ? (
                                        <div type="invalid" className="invalid-feedback" style={{ display: "block" }}>{validationType.errors.tenantIds}</div>
                                    ) : null}
                                </div>
                            }
                        </div>
                    </Form>
                    </div>
                    :
                    props.t("Do you confirm?")
            }
        </>
        
       
    );
};

DeleteTenantAndSystemAdmin.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(DeleteTenantAndSystemAdmin);