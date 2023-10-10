import React, { useState, useEffect } from "react";
import {
    Row,
    Col,
    Form,
    Label,
    Input,
    FormFeedback,
    Button
} from "reactstrap";
import { useFormik } from "formik";
import * as Yup from "yup";
import { useContactUsPostMutation } from "../../store/services/ContactUs";
import ToastComp from '../../components/Common/ToastComp/ToastComp';
import { useDispatch } from "react-redux";
import { startloading, endloading } from "../../store/loader/actions";


const ContactUs = () => {
    document.title = "Contact Us Page | Veltrix - React Admin & Dashboard Template";

    const [showToast, setShowToast] = useState(false);
    const [message, setMessage] = useState("");
    const [stateToast, setStateToast] = useState(true);

    const dispatch = useDispatch();

    const [contactUsPost, { isLoading }] = useContactUsPostMutation();

    const validationType = useFormik({
        enableReinitialize: true,
        initialValues: {
            namesurname: '',
            email: '',
            institutionname: '',
            studycode: '',
            yourmessage: '',
        },
        validationSchema: Yup.object().shape({
            namesurname: Yup.string().required(
                "This value is required"
            ),
            email: Yup.string().required(
                "This value is required"
            ),
            institutionname: Yup.string().required(
                "This value is required"
            ),
            studycode: Yup.string().required(
                "This value is required"
            ),
            yourmessage: Yup.string().required(
                "This value is required"
            ),
        }),
        onSubmit: async (values) => {
            dispatch(startloading());
            const response = await contactUsPost(values);
            console.log(response)
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
    });

    return (
        <React.Fragment>
            <div className="page-content" style={{ minHeight: "100vh",
            backgroundColor: "white",
            position: "relative",
          }}>
                <div className="container-fluid">
                    <div className="page-title-box">
                        <Row className="align-items-center justify-content-center">
                            <Col className="col-lg-6 col-md-offset-3" style={{
                                textAlign: "center", marginBottom: "15px",
                            paddingBottom: "15px",
                            borderBottom: "2px solid #ffc600" } }>
                                <h3 style={{
                                    color: "#ffc600",
                                    fontSize: "30pt",
                                    fontWeight: "bold"
                                }}>Contact us</h3>
                                <span style={{ 
                                    fontSize: "12px", color: "#6D6E70" } }>Please complete the form below to contact us.</span>
                            </Col>
                        </Row>
                    </div>
                    <Row className="justify-content-center">
                        <Col lg={4 }>
                            <Form
                                onSubmit={(e) => {
                                    e.preventDefault();
                                    validationType.handleSubmit();
                                    return false;
                                }}>
                                <div className="mb-3">
                                    <Label className="form-label">Name / Surname</Label>
                                    <Input
                                        name="namesurname"
                                        placeholder="Name Surname"
                                        type="text"
                                        onChange={validationType.handleChange}
                                        onBlur={(e) => {
                                            validationType.handleBlur(e);
                                        }}
                                        value={validationType.values.namesurname || ""}
                                        invalid={
                                            validationType.touched.namesurname && validationType.errors.namesurname ? true : false
                                        }
                                    />
                                    {validationType.touched.namesurname && validationType.errors.namesurname ? (
                                        <FormFeedback type="invalid">{validationType.errors.namesurname}</FormFeedback>
                                    ) : null}
                                </div>
                                <div className="mb-3">
                                    <Label className="form-label">Your email address</Label>
                                    <Input
                                        name="email"
                                        placeholder="abc@example.com"
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
                                <div className="mb-3">
                                    <Label>Institution name</Label>
                                    <Input
                                        name="institutionname"
                                        type="text"
                                        placeholder="Institution name"
                                        onChange={validationType.handleChange}
                                        onBlur={(e) => {
                                            validationType.handleBlur(e);
                                        }}
                                        value={validationType.values.institutionname || ""}
                                        invalid={
                                            validationType.touched.institutionname && validationType.errors.institutionname ? true : false
                                        }
                                    />
                                    {validationType.touched.institutionname && validationType.errors.institutionname ? (
                                        <FormFeedback type="invalid">{validationType.errors.institutionname}</FormFeedback>
                                    ) : null}
                                </div>
                                <div className="mb-3">
                                    <Label className="form-label">Study code</Label>
                                    <Input
                                        name="studycode"
                                        type="text"
                                        placeholder="Study code"
                                        onChange={validationType.handleChange}
                                        onBlur={(e) => {
                                            validationType.handleBlur(e);
                                        }}
                                        value={validationType.values.studycode || ""}
                                        invalid={
                                            validationType.touched.studycode && validationType.errors.studycode ? true : false
                                        }
                                    />
                                    {validationType.touched.studycode && validationType.errors.studycode ? (
                                        <FormFeedback type="invalid">{validationType.errors.studycode}</FormFeedback>
                                    ) : null}
                                </div>
                                <div className="mb-3">
                                    <Label className="form-label">Your message</Label>
                                    <Input
                                        name="yourmessage"
                                        type="textarea"
                                        onChange={e => {
                                            validationType.handleChange(e);
                                        }}
                                        onBlur={(e) => {
                                            validationType.handleBlur(e);
                                        }}
                                        value={validationType.values.yourmessage || ""}
                                        rows="3"
                                        placeholder="Your message"
                                        invalid={
                                            validationType.touched.yourmessage && validationType.errors.yourmessage ? true : false
                                        }
                                    />
                                    {validationType.touched.yourmessage && validationType.errors.yourmessage ? (
                                        <FormFeedback type="invalid">{validationType.errors.yourmessage}</FormFeedback>
                                    ) : null}
                                </div>
                                <div>
                                    <Button style={{ float:"right" }} color="success" type="submit">
                                        Send
                                    </Button>
                                </div>
                            </Form>
                        </Col>
                    </Row>
                    <Row className="justify-content-center">
                        <Col className="col-lg-6 col-md-offset-3" style={{
                            textAlign: "center", marginTop: "15px",
                            paddingTop: "15px",
                            borderTop: "2px solid #ffc600"
                        }}>
                            <p style={{ margin: "0", padding: "0" } }><span  style={{
                                color: "black",
                                fontWeight: "bold", fontSize:"12px", fontFamily: "Helvetica Neue, Helvetica, Arial, sans-serif"
                            }}>Send us an email: </span><a href="mailto:contact@helios-crf.com"> contact@helios-crf.com </a></p>
                            <p style={{ margin: "0", padding: "0" }}><span  style={{
                                color: "black",
                                fontWeight: "bold", fontSize: "12px", fontFamily: "Helvetica Neue, Helvetica, Arial, sans-serif"
                            }}> Call us :</span><a href="tel://+902122341260"> +90 212 234 12 60 </a></p>
                            <p style={{ margin: "0", padding: "0" }}> <span  style={{
                                color: "black",
                                fontWeight: "bold", fontSize: "12px", fontFamily: "Helvetica Neue, Helvetica, Arial, sans-serif"
                            }}>Address</span><a target="_blank" href="https://goo.gl/maps/pXPMkwYwHpJUPJjv9"> Cumhuriyet District Haciahmet Silahsor Street Yeniyol Street No: 2/58 Now, 34440 Sisli/Istanbul, Turkey </a></p>
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

export default ContactUs;
