import PropTypes, { object } from 'prop-types';
import React, { useState, useEffect, useCallback } from "react";
import { withTranslation } from "react-i18next";
import { Link, Routes, Route, useNavigate, useLocation } from "react-router-dom";
import {
    Row,
    Col,
    Card,
    CardBody,
    FormGroup,
    Button,
    CardTitle,
    CardSubtitle,
    Label,
    Input,
    Container,
    Form,
    FormFeedback,
    Collapse
} from "reactstrap";
import { useFormik } from "formik";
import * as Yup from "yup";
import Select from "react-select";
import './study.css';
import { useStudySaveMutation } from '../../store/services/Study';
import { useSelector } from "react-redux";
import { useStudyGetQuery, usePrefetch, StudyApi } from '../../store/services/Study';
import GetStudy from './getStudy';



const AddOrUpdateStudy = props => {

    const [isOpen, setIsOpen] = useState(false);
    const [studyId, setStudyId] = useState('00000000-0000-0000-0000-000000000000');
    const [skip, setSkip] = useState(true);
    const [apiStudyData, setApiStudyData] = useState(null);

    const userInformation = useSelector(state => ({
        userId: state.rootReducer.Login.userId,
        tenantId: state.rootReducer.Login.tenantId
    }));

    const [studySave] = useStudySaveMutation();

    const navigate = useNavigate();

    const location = useLocation();

    const backPage = () => {
        navigate('/study');
    };

    const optionGroup = [
        {
            label: "Language",
            options: [
                { label: "English", value: 1 },
                { label: "Turkish", value: 2 },
            ]
        }
    ];

    const toggleAccordion = () => {
        setIsOpen(!isOpen);
    };

    const validationType = useFormik({
        // enableReinitialize : use this flag when initial values needs to be changed
        enableReinitialize: true,

        initialValues: {
            userid: userInformation.userId,
            tenantid: userInformation.tenantId,
            studyId: studyId,
/*            studyname: '',*/
            studyname: apiStudyData ? apiStudyData.studyName : '',
/*            studylink: '',*/
            studylink: apiStudyData ? apiStudyData.studyLink : '',
/*            protocolcode: '',*/
            protocolcode: apiStudyData ? apiStudyData.protocolCode : '',
   /*         studylanguage: 0,*/
            studylanguage: apiStudyData ? apiStudyData.studyLanguage : 0,
/*            description: '',*/
            description: apiStudyData ? apiStudyData.description : '',
/*            subdescription: '',*/
            subdescription: apiStudyData ? apiStudyData.subDescription : '',
         /*   subjectnumberdigist: '',*/
/*            doubledataentry: false,*/
            doubledataentry: apiStudyData ? apiStudyData.doubleDataEntry : false,
         /*   asksubjectinitial: false,*/
            asksubjectinitial: apiStudyData ? apiStudyData.askSubjectInitial : false,
/*            reasonforchange: true,*/
            reasonforchange: apiStudyData ? apiStudyData.reasonForChange : true,
        },
        validationSchema: Yup.object().shape({
            studyname: Yup.string().required(
                "This value is required"
            ),
            studylink: Yup.string().required(
                "This value is required"
            ),
            //studylanguage: Yup.object().nullable().required(
            //    "This value is required"
            //),
            //description: Yup.string().max(225),
            //subdescription: Yup.string().max(225),
            //subjectnumberdigist: Yup.string().required(
            //    "This value is required"    
            //)
        }),
        onSubmit: async (values) => {
            const response = await studySave(values);
            if (response.data.isSuccess) {
                setStudyId(response.data.values.studyId);
            }
           
        }
    });

    useEffect(() => {
        validationType.setValues({
            ...validationType.values,
            studyId: studyId,
        });
    }, [studyId]);

    useEffect(() => {
        console.log('useffect')
        setApiStudyData(null);
        if (location.state !== null) {
            setStudyId(location.state.studyId);
            setSkip(false);
        }
    }, []);

    const { data: studyData, error, isLoading } = useStudyGetQuery(studyId, {
        skip, refetchOnMountOrArgChange: true 
    });

    useEffect(() => {
        if (!isLoading && !error && studyData) {
            setApiStudyData(studyData);
            setSkip(true);
        }
    }, [studyData, error, isLoading]);


    document.title = "Study | Veltrix - React Admin & Dashboard Template";
    return (
        <React.Fragment>
            <div className="page-content">
                <div className="container-fluid">
                    <div className="page-title-box">
                        <Row className="align-items-center" style={{ borderBottom: "1px solid black" }}>
                            <Col md={8}>
                                <h6 className="page-title"><i onClick={backPage} className="ti-arrow-left" style={{ marginRight: "10px", cursor: "pointer", position: "relative", top: "0.5px" }}></i>Study information</h6>
                            </Col>
                        </Row>
                    </div>
                    <Row>
                        <Col className="col-12">
                            <Card style={{ width: "50%", backgroundColor:"#f8f8fa", boxShadow:"unset", margin: "0 auto" }}>
                                <CardBody>
                                    <Label className="form-label">Study information</Label>
                                    <CardSubtitle className="mb-3">
                                        Your study will be created on the Azure Server because you are logged in the domain: trials.helios-crf.com.
                                    </CardSubtitle>
                                     <Form
                                        onSubmit={(e) => {
                                        e.preventDefault();
                                        validationType.handleSubmit();
                                        return false;
                                    }}>
                                        <div className="mb-3">
                                          <Label className="form-label">Study name</Label>
                                          <Input
                                            name="studyname"
                                            placeholder="Study name"
                                            type="text"
                                            onChange={validationType.handleChange}
                                            onBlur={(e) => {
                                                validationType.handleBlur(e);
                                                validationType.submitForm();
                                            }}
                                            value={validationType.values.studyname || ""}
                                            invalid={
                                              validationType.touched.studyname && validationType.errors.studyname ? true : false
                                            }
                                          />
                                          {validationType.touched.studyname && validationType.errors.studyname ? (
                                            <FormFeedback type="invalid">{validationType.errors.studyname}</FormFeedback>
                                          ) : null}
                                        </div>
                                        <div className="mb-3">
                                            <Label className="form-label">Study link</Label>
                                            <Input
                                                name="studylink"
                                                placeholder="Study link"
                                                type="text"
                                                onChange={validationType.handleChange}
                                                onBlur={(e) => {
                                                    validationType.handleBlur(e);
                                                    validationType.submitForm();
                                                }}
                                                value={validationType.values.studylink || ""}
                                                invalid={
                                                    validationType.touched.studylink && validationType.errors.studylink ? true : false
                                                }
                                            />
                                            {validationType.touched.studylink && validationType.errors.studylink ? (
                                                <FormFeedback type="invalid">{validationType.errors.studylink}</FormFeedback>
                                            ) : null}
                                        </div>
                                        <div className="mb-3">
                                          <Label>Protocol code</Label>
                                          <Input
                                            name="protocolcode"
                                            type="text"
                                            placeholder="Protocol code"
                                            onChange={validationType.handleChange}
                                            onBlur={(e) => {
                                                validationType.handleBlur(e);
                                                validationType.submitForm();
                                            }}
                                            value={validationType.values.protocolcode || ""}
                                          />
                                        </div>
                                        <div className="mb-3">
                                            <Label>Study language</Label>
                                            <Select
                                                value={optionGroup[0].options.find(option => option.value === validationType.values.studylanguage)}
                                                name="studylanguage"
                                                onChange={(selectedOption) => {
                                                    const formattedValue = {
                                                        target: {
                                                            name: 'studylanguage',
                                                            value: selectedOption.value
                                                        }
                                                    };
                                                    validationType.handleChange(formattedValue);
                                                    validationType.submitForm();
                                                }} 
                                                options={optionGroup}
                                                classNamePrefix="select2-selection"
                                              
                                            />
                                        </div>
                                        <div className="mb-3">
                                          <Label className="form-label">Description</Label>
                                            <Input
                                                name="description"
                                                type="textarea"
                                                id="textarea"
                                                onChange={e => {
                                                    validationType.handleChange(e);
                                                }}
                                                onBlur={(e) => {
                                                    validationType.handleBlur(e);
                                                    validationType.submitForm();
                                                }}
                                                value={validationType.values.description || ""}
                                                rows="3"
                                                placeholder="Description"
                                            />
                                        </div>
                                        <div className="mb-3">
                                            <Label className="form-label">Sub description</Label>
                                            <Input
                                                name="subdescription"
                                                type="textarea"
                                                onChange={e => {
                                                    validationType.handleChange(e);
                                                }}
                                                onBlur={(e) => {
                                                    validationType.handleBlur(e);
                                                    validationType.submitForm();
                                                }}
                                                value={validationType.values.subdescription || ""}
                                                rows="3"
                                                placeholder="Sub description"
                                            />
                                        </div>
                                        <div className="mb-3">
                                            <i onClick={toggleAccordion} className={isOpen ? "mdi mdi-chevron-up" : "mdi mdi-chevron-down"} style={{ fontSize: "12px", marginRight: "5px", cursor:"pointer" }}></i><Label style={{ borderBottom: "1px solid black" }} className="form-label">Advanced options</Label>
                                            <Collapse isOpen={isOpen}>
                                                <div style={{ padding:"5px 0" }}>
                                                    <div className="mb-3">
                                                        <Label>Subject number digist</Label>
                                                        <Input
                                                            name="subjectnumberdigist"
                                                            type="text"
                                                            placeholder="Subject number digist"
                                                            onChange={validationType.handleChange}
                                                            onBlur={(e) => {
                                                                validationType.handleBlur(e);
                                                                validationType.submitForm();
                                                            }}
                                                            value={validationType.values.subjectnumberdigist || ""}
                                                        />
                                                        <span className="text-muted" style={{ fontSize:"10px" }}>
                                                            {'{Country code}{Site no}{###1}'}
                                                        </span>
                                                    </div>
                                                    <div className="mb-3">
                                                        <FormGroup check>
                                                            <Label check>
                                                                <Input
                                                                    name="doubledataentry"
                                                                    type="checkbox"
                                                                    checked={validationType.values.doubledataentry || false}
                                                                    onChange={(e) => {
                                                                        validationType.handleChange(e);
                                                                        validationType.submitForm();
                                                                    }}
                                                                />
                                                                Double data entry
                                                            </Label>
                                                        </FormGroup>
                                                    </div>
                                                    <div className="mb-3">
                                                        <FormGroup check>
                                                            <Label check>
                                                                <Input
                                                                    name="asksubjectinitial"
                                                                    type="checkbox"
                                                                    checked={validationType.values.asksubjectinitial || false}
                                                                    onChange={(e) => {
                                                                        validationType.handleChange(e);
                                                                        validationType.submitForm();
                                                                    }}
                                                                />
                                                                Ask subject Initial
                                                            </Label>
                                                        </FormGroup>
                                                    </div>
                                                    <div className="mb-3">
                                                        <FormGroup check>
                                                            <Label check>
                                                                <Input
                                                                    name="reasonforchange"
                                                                    type="checkbox"
                                                                    checked={validationType.values.reasonforchange || false}
                                                                    onChange={(e) => {
                                                                        validationType.handleChange(e);
                                                                        validationType.submitForm();
                                                                    }}
                                                                />
                                                                Reason for change
                                                            </Label>
                                                        </FormGroup>
                                                    </div>
                                                </div>
                                               
                                            </Collapse>
                                        </div>
                                    </Form>
                                </CardBody>
                            </Card>
                        </Col>
                    </Row>
                </div>
            </div>
        </React.Fragment>
    );
};

AddOrUpdateStudy.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(AddOrUpdateStudy);
