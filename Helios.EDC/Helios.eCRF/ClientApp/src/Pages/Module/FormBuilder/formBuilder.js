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
import Properties from './properties.js';

function FormBuilder() {
    const [modal_large, setmodal_large] = useState(false);

    const removeBodyCss = () => {
        document.body.classList.add("no_padding");
    };

    const tog_large = () => {
        setmodal_large(!modal_large);
        removeBodyCss();
    };

    return (
        <div style={({ height: "100vh" }, { display: "flex" })} >
            <div id="page-wrap" style={{ padding: "15px", width: '100%', marginTop: '70px' }}>
                <div><h3>FormBuilder</h3></div>
                <hr />
                <div>
                    <div style={{ width: "20%", float: 'left' }}>
                        <ElementList />
                    </div>
                    <div style={{ width: "80%", float: 'right' }}>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default FormBuilder;

