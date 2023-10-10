import PropTypes from 'prop-types';
import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import { MDBDataTable } from "mdbreact";
import {
    Row, Col, Card, CardBody, Button, Label, Input, Form, FormFeedback, Alert
} from "reactstrap";
import { useSiteListGetQuery, useSiteSaveOrUpdateMutation, useSiteGetQuery, useSiteDeleteMutation } from '../../store/services/SiteLaboratories';
import { useDispatch, useSelector } from "react-redux";
import { startloading, endloading } from '../../store/loader/actions';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import ModalComp from '../../components/Common/ModalComp/ModalComp';
import { useFormik } from "formik";
import * as Yup from "yup";
import ToastComp from '../../components/Common/ToastComp/ToastComp';
import { formatDate } from "../../helpers/format_date";
import Swal from 'sweetalert2'

const Sites = props => {
    
    const userInformation = useSelector(state => state.rootReducer.Login);

    const studyInformation = useSelector(state => state.rootReducer.Study);

    const [skip, setSkip] = useState(true);
    const [tableData, setTableData] = useState([]);
    const [modal_backdrop, setmodal_backdrop] = useState(false);
    const [showToast, setShowToast] = useState(false);
    const [message, setMessage] = useState("");
    const [stateToast, setStateToast] = useState(true);
    const [siteId, setSiteId] = useState('00000000-0000-0000-0000-000000000000');

    const dispatch = useDispatch();

    const { data: apiData, apiError, apiIsLoading } = useSiteGetQuery(siteId, {
        skip, refetchOnMountOrArgChange: true
    });

    const siteUpdate = (id) => {
        dispatch(startloading());
        setSiteId(id);
        setSkip(false);
    };

    const [siteDelete] = useSiteDeleteMutation();

    const siteDeleteHandle = (id) => {
        Swal.fire({
            title: "You will not be able to recover this site!",
            text: "Do you confirm?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3bbfad",
            confirmButtonText: "Yes",
            cancelButtonText: "Cancel",
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
                        code: '',
                        name: '',
                        countrycode: '',
                        countryname: '',
                        maxenrolmentcount: 0,
                    };
                    const response = await siteDelete(deleteData);
                    if (response.data.isSuccess) {
                        dispatch(endloading());
                        Swal.fire(response.data.message, '', 'success');
                    } else {
                        dispatch(endloading());
                        Swal.fire(response.data.message, '', 'error');
                    }
                } catch (error) {
                    dispatch(endloading());
                    Swal.fire('An error occurred', '', 'error');
                }
            }
        });
    };

    useEffect(() => {
        if (!apiIsLoading && !apiError && apiData) {
            validationType.setValues({
                id: apiData.id,
                userid: userInformation.userId,
                tenantid: userInformation.tenantId,
                studyId: studyInformation.studyId,
                code: apiData.code,
                name: apiData.name,
                countrycode: apiData.countryCode,
                countryname: apiData.countryName,
                maxenrolmentcount: apiData.maxEnrolmentCount,
            });
            setSkip(true);
            tog_backdrop();
            dispatch(endloading());
        } 
    }, [apiData, apiError, apiIsLoading]);

    const getActions = (id) => {
        const actions = (
            <div className="icon-container">
                <div className="icon icon-update" onClick={() => { siteUpdate(id) }}></div>
                <div className="icon icon-delete" onClick={() => { siteDeleteHandle(id) } }></div>
            </div>);
        return actions;
    };

    const data = {
        columns: [
            {
                label: "",
                field: "id",
                sort: "asc",
                width: 150
            },
            {
                label: "Site Name",
                field: "siteFullName",
                sort: "asc",
                width: 150
            },
            {
                label: "Site No",
                field: "code",
                sort: "asc",
                width: 150
            },
            {
                label: "Country Code",
                field: "countryCode",
                sort: "asc",
                width: 150
            },
            {
                label: "Country",
                field: "countryName",
                sort: "asc",
                width: 150
            },
            {
                label: "Number of subjects that can be added to the center",
                field: "maxEnrolmentCount",
                sort: "asc",
                width: 150
            },
            {
                label: "Last Updated On",
                field: "updatedAt",
                sort: "asc",
                width: 150
            },
            {
                label: 'Actions',
                field: 'actions',
                sort: 'disabled',
                width: 100,
            }
        ],
        rows: tableData
    }

    const { data: siteData, error, isLoading } = useSiteListGetQuery(studyInformation.studyId);

    useEffect(() => {
        dispatch(startloading());
        if (!isLoading && !error && siteData) {
            const updatedSiteData = siteData.map(item => {
                return {
                    ...item,
                    updatedAt: formatDate(item.updatedAt),
                    actions: getActions(item.id)
                };
            });
            setTableData(updatedSiteData);
            dispatch(endloading());
        } else {
            dispatch(endloading());
        }
    }, [siteData, error, isLoading]);

    useEffect(() => {
        if (!modal_backdrop)
            resetValue();
    }, [modal_backdrop]);

    const removeBodyCss = () => {
        document.body.classList.add("no_padding");
    };

    const tog_backdrop = () => {
        setmodal_backdrop(!modal_backdrop);
        removeBodyCss();
    };

    const [siteSaveOrUpdate] = useSiteSaveOrUpdateMutation();

    const resetValue = () => {
        validationType.validateForm().then(errors => {
            validationType.setErrors({});
            validationType.resetForm();
        });
        setSiteId('00000000-0000-0000-0000-000000000000');
    };

    const validationType = useFormik({
        enableReinitialize: true,
        initialValues: {
            id: siteId,
            userid: userInformation.userId,
            tenantid: userInformation.tenantId,
            studyId: studyInformation.studyId,
            code: '',
            name: '',
            countrycode: '',
            countryname: '',
            maxenrolmentcount: 0,
        },
        validationSchema: Yup.object().shape({
            code: Yup.string().required(
                "This value is required"
            ),
            name: Yup.string().required(
                "This value is required"
            ),
        }),
        onSubmit: async (values) => {
            dispatch(startloading());
            const response = await siteSaveOrUpdate(values);
            if (response.data.isSuccess) {
                setMessage(response.data.message)
                setStateToast(true);
                setShowToast(true);
                tog_backdrop();
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
                        <Row className="align-items-center">
                            <Col md={8}>
                                <h6 className="page-title">Site list</h6>
                            </Col>

                            <Col md="4">
                                <div className="float-end d-none d-md-block">
                                    <Button
                                        color="success"
                                        className="btn btn-success waves-effect waves-light"
                                        type="button"
                                        onClick={tog_backdrop}
                                    >
                                        <FontAwesomeIcon icon="fa-solid fa-plus" /> Add a site
                                    </Button>
                                </div>
                            </Col>
                        </Row>
                    </div>
                    <Row>
                        <Col className="col-12">
                            <Card>
                                <CardBody>
                                    <MDBDataTable hover responsive striped bordered data={data} />
                                </CardBody>
                            </Card>
                        </Col>
                    </Row>
                </div>
            </div>
            <ModalComp
                title={siteId === '00000000-0000-0000-0000-000000000000' ? "Add a site" : "Update site"}
                body={
                    <>
                        <Alert color="warning">
                            If there is no specific subject limit for the site, please enter zero for the subject number. If you have a certain patient limit for the center, please enter numerically.
                        </Alert>
                        <Form
                            onSubmit={(e) => {
                                e.preventDefault();
                                validationType.handleSubmit();
                                return false;
                            }}>
                            <div className="row">
                                <div className="mb-3 col-md-3">
                                    <Label className="form-label">Site No</Label>
                                    <Input
                                        name="code"
                                        placeholder="Site no"
                                        type="text"
                                        onChange={validationType.handleChange}
                                        onBlur={(e) => {
                                            validationType.handleBlur(e);
                                        }}
                                        value={validationType.values.code || ""}
                                        invalid={
                                            validationType.touched.code && validationType.errors.code ? true : false
                                        }
                                    />
                                    {validationType.touched.code && validationType.errors.code ? (
                                        <FormFeedback type="invalid">{validationType.errors.code}</FormFeedback>
                                    ) : null}
                                </div>
                                <div className="mb-3 col-md-9">
                                    <Label className="form-label">Site name</Label>
                                    <Input
                                        name="name"
                                        placeholder="Site name"
                                        type="text"
                                        onChange={validationType.handleChange}
                                        onBlur={(e) => {
                                            validationType.handleBlur(e);
                                        }}
                                        value={validationType.values.name || ""}
                                        invalid={
                                            validationType.touched.name && validationType.errors.name ? true : false
                                        }
                                    />
                                    {validationType.touched.name && validationType.errors.name ? (
                                        <FormFeedback type="invalid">{validationType.errors.name}</FormFeedback>
                                    ) : null}
                                </div>
                            </div>
                            <div className="row">
                                <div className="mb-3 col-md-3">
                                    <Label>Country code</Label>
                                    <Input
                                        name="countrycode"
                                        type="text"
                                        placeholder="Country code"
                                        onChange={validationType.handleChange}
                                        onBlur={(e) => {
                                            validationType.handleBlur(e);
                                        }}
                                        value={validationType.values.countrycode || ""}
                                    />
                                </div>
                                <div className="mb-3 col-md-9">
                                    <Label>Country name</Label>
                                    <Input
                                        name="countryname"
                                        type="text"
                                        placeholder="Country name"
                                        onChange={validationType.handleChange}
                                        onBlur={(e) => {
                                            validationType.handleBlur(e);
                                        }}
                                        value={validationType.values.countryname || ""}
                                    />
                                </div>
                            </div>
                            <div className="mb-3">
                                <Label>Number of subjects that can be added to the center</Label>
                                <Input
                                    name="maxenrolmentcount"
                                    label="Digits"
                                    placeholder="Number of subjects that can be added to the center"
                                    type="number"
                                    onChange={validationType.handleChange}
                                    onBlur={validationType.handleBlur}
                                    value={validationType.values.maxenrolmentcount || 0}
                                    invalid={
                                        validationType.touched.maxenrolmentcount && validationType.errors.maxenrolmentcount ? true : false
                                    }
                                />
                                {validationType.touched.maxenrolmentcount && validationType.errors.maxenrolmentcount ? (
                                    <FormFeedback type="invalid">{validationType.errors.maxenrolmentcount}</FormFeedback>
                                ) : null}
                            </div>
                        </Form>
                    </>

                }
                modal_backdrop={modal_backdrop}
                tog_backdrop={tog_backdrop}
                handle={() => validationType.handleSubmit()}
                buttonText={siteId === '00000000-0000-0000-0000-000000000000' ? "Save" : "Update" }
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


Sites.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(Sites);