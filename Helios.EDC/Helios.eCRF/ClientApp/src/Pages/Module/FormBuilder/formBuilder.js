import React, { useState, useEffect } from 'react';
import {
    Row,
    Col,
    Card,
    CardBody,
    CardTitle,
    Modal,
    Container,
    ModalBody,
    ModalHeader,
    ModalFooter,
    Button,
} from "reactstrap";

//Import Breadcrumb
import Breadcrumbs from "../../../components/Common/Breadcrumb";

import ElementList from './elementList.js'
import './formBuilder.css'
//import Properties from './properties.js';

function FormBuilder(props) {

    return (
        <div style={({ height: "100vh" }, { display: "flex" })} >
            <div className="page-content">
                <div className="container-fluid">
                    <div className="page-title-box">
                        <Row className="align-items-center" style={{ borderBottom: "1px solid black" }}>
                            <Col md={8}>
                                <h6 className="page-title">FormBuilder</h6>
                            </Col>
                        </Row>
                    </div>
                    <div>
                        <ElementList />
                    </div>
                </div>
            </div>
        </div>
    );
}

export default FormBuilder;

