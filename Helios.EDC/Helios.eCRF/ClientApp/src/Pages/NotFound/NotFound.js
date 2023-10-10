import React from "react";
import { Link, useLocation } from "react-router-dom";
import { Container, Card, CardBody, Row, Col } from "reactstrap";

//Import Images
import errorImg from "../../assets/images/error.png";

const NotFound = () => {
    document.title = "404 Error Page | Veltrix - React Admin & Dashboard Template";

    return (
        <React.Fragment>
            <div className="authentication-bg d-flex align-items-center pb-0 vh-100">
                <div className="content-center w-100">
                    <Container>
                        <Row className="justify-content-center">
                            <Col xl={10}>
                                <Card>
                                    <CardBody>
                                        <Row className="align-items-center">
                                            <Col lg={4} className="ms-auto">
                                                <div className="ex-page-content">
                                                    <h4 className="mb-4">Sorry, page not found</h4>
                                                    <p className="mb-5">We can't seem to find the page you're looking for.<br />
                                                        Try going <Link to="javascript:history.back()"> Back </Link>to the previous page or see our <Link to="/ContactUs"> Contact us</Link> page for more information</p>                                                   
                                                </div>
                                            </Col>
                                            <Col lg={5} className="mx-auto">
                                                <img
                                                    src={errorImg}
                                                    alt=""
                                                    className="img-fluid mx-auto d-block"
                                                />
                                            </Col>
                                        </Row>
                                    </CardBody>
                                </Card>
                            </Col>
                        </Row>
                    </Container>
                </div>
            </div>
        </React.Fragment>
    );
};

export default NotFound;
