import PropTypes from 'prop-types';
import React from "react";
import {
    Row, Col
} from "reactstrap";
import { withTranslation } from "react-i18next";

const Permission = props => {
    return (
        <React.Fragment>
            <div className="page-content">
                <div className="container-fluid">
                    <div className="page-title-box">
                        <Row className="align-items-center" style={{ borderBottom: "1px solid black" }}>
                            <Col md={8}>
                                <h6 className="page-title">Permission</h6>
                            </Col>
                        </Row>
                    </div>
                    <Row>
                        <Col className="col-12">
                            Permission sayfası
                        </Col>
                    </Row>
                </div>
            </div>
        </React.Fragment>
    );
};


Permission.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(Permission);