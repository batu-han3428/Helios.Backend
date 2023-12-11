import PropTypes from 'prop-types';
import React, { useImperativeHandle } from "react";
import * as Yup from "yup";
import { useFormik } from "formik";
import { withTranslation } from "react-i18next";
import { Label, Input, Form, FormFeedback } from "reactstrap";
import { startloading, endloading } from '../../store/loader/actions';
import { useDispatch } from 'react-redux';
import { useSystemAdminSetMutation } from '../../store/services/SystemAdmin';


const AddOrUpdateSystemAdmin = props => {

    const dispatch = useDispatch();

    const [systemAdminSet] = useSystemAdminSetMutation();

    const validationType = useFormik({
        enableReinitialize: true,
        initialValues: {
            id: props.id,
            userid: props.userId,
            language: props.i18n.language,
            email: props.email || "",
        },
        validationSchema: Yup.object().shape({
            email: Yup.string().required(props.t("This field is required")).email(props.t("Invalid email format")),
        }),
        onSubmit: async (values) => {
            dispatch(startloading());
            const response = await systemAdminSet(values);
            if (response.data.isSuccess) {
                props.onToggleModal();
                props.toast(props.t(response.data.message), true);
                dispatch(endloading());
            } else {
                props.toast(props.t(response.data.message), false);
                dispatch(endloading());
            }
        }
    });

    useImperativeHandle(props.refs, () => ({
        submitForm: validationType.handleSubmit
    }), [validationType, props]);

    return (
        <Form
            onSubmit={(e) => {
                e.preventDefault();
                validationType.handleSubmit();
                return false;
            }}>
            <div className="row">
                <div className="mb-3 col-md-12">
                    <Label className="form-label">{props.t("e-Mail")}</Label>
                    <Input
                        name="email"
                        placeholder=""
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
                    {validationType.touched.email && validationType.errors.email ? (
                        <FormFeedback type="invalid">{validationType.errors.email}</FormFeedback>
                    ) : null}
                </div>
            </div>
        </Form>
    )
}

AddOrUpdateSystemAdmin.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(AddOrUpdateSystemAdmin);