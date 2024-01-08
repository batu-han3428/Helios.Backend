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
import { useNavigate, useLocation, useParams } from "react-router-dom";

import ElementList from './elementList.js';
import './formBuilder.css';

function FormBuilder() {
    const {moduleId} = useParams();

    return (
        <div style={({ height: "100vh" }, { display: "flex" })} >
            <div className="page-content">
                <div className="container-fluid">
                    <div className="page-title-box">
                        <Row className="align-items-center" style={{ borderBottom: "1px solid black" }}>
                            <Col md={8}>
                                <h6 className="page-title">Form Builder</h6>
                            </Col>
                        </Row>
                    </div>
                    <div>
                        <ElementList ModuleId={moduleId} />
                    </div>
                </div>
            </div>
        </div>
    );
}

export default FormBuilder;

