import PropTypes from 'prop-types';
import React from "react";
import { withTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import {
    Row,
    Col,
} from "reactstrap";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'


const Study = props => {

    const navigate = useNavigate();

    const backPage = () => {
        navigate('/studylist');
    };

    return (
        <React.Fragment>
            <div className="page-content">
                <div className="container-fluid">
                    <div className="page-title-box">
                        <Row className="align-items-center" style={{ borderBottom: "1px solid black" }}>
                            <Col md={8}>
                                <h6 className="page-title"><FontAwesomeIcon style={{ marginRight: "10px", cursor: "pointer", position: "relative", top: "0.5px" }} onClick={backPage} icon="fa-solid fa-left-long" />Study</h6>
                            </Col>
                        </Row>
                    </div>
                    <Row>
                        <Col className="col-12">
                           Study Sayfası
                        </Col>
                    </Row>
                </div>
            </div>
        </React.Fragment>
    );
};

Study.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(Study);