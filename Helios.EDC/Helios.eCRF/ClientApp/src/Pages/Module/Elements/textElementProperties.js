import { extend } from 'jquery';
import React, { Component, useState, useContext, Form, FormField, TextBox, ComboBox, CheckBox, LinkButton } from 'react';
import ElementBase from './Base/elementBase.js';
import {
    Card,
    CardBody,
    CardText,
    CardTitle,
    Col,
    Collapse,
    Container,
    Nav,
    NavItem,
    NavLink,
    Row,
    TabContent,
    TabPane,
} from "reactstrap";

//function TextElementProperties(props) {
//    //const [Unit, setUnit] = useState('');
//    return (
//        <Row className="mb-3">
//            <label
//                htmlFor="example-text-input"
//                className="col-md-2 col-form-label"
//            >
//                Unit
//            </label>
//            <div className="col-md-10">
//                <input
//                    value={props.Unit}
//                    className="form-control"
//                    type="text"
//                    placeholder="Unit"
//                />
//            </div>
//        </Row>
//    );
//}

class TextElementProperties extends Component {
    constructor(props) {
        super(props);

        //this.state = {
        //    Unit: props.Unit
        //}
        //this.setState({ Unit: '' });
        this.handleUnitChange = this.handleUnitChange.bind(this);
    }

    handleUnitChange(e) {
        //this.setState({ Unit: e.target.value });
        this.props.changeUnit(e.target.value);
    };

    render() {
        return (
            <Row className="mb-3">
                <label
                    htmlFor="example-text-input"
                    className="col-md-2 col-form-label"
                >
                    Unit
                </label>
                <div className="col-md-10">
                    <input
                        value={this.props.Unit}
                        onChange={this.handleUnitChange}
                        className="form-control"
                        type="text"
                        placeholder="Unit"
                    />
                </div>
            </Row>
        );
    }
};

export default TextElementProperties;
