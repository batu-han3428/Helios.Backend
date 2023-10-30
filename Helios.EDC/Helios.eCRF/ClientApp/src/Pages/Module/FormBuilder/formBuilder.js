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
            <div id="page-wrap" style={{ padding: "15px", width: '100%', marginTop: '70px' }}>
                <div><h3>FormBuilder</h3>
                </div>
                <hr />
                <div>
                    <ElementList />
                </div>
            </div>
        </div>
    );
}

export default FormBuilder;

