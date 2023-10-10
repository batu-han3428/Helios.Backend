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

class NumericElementProperties extends Component {
    constructor(props) {
        super(props);

        this.handleUnitChange = this.handleUnitChange.bind(this);
        this.handleLowerLimitChange = this.handleLowerLimitChange.bind(this);
        this.handleUpperLimitChange = this.handleUpperLimitChange.bind(this);
    }

    handleUnitChange(e) {
        this.props.changeUnit(e.target.value);
    };

    handleLowerLimitChange(e) {
        this.props.changeLowerLimit(e.target.value);
    };

    handleUpperLimitChange(e) {
        this.props.changeUpperLimit(e.target.value);
    };

    render() {
        return (
            <>
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
                            placeholder="Unit" />
                    </div>
                </Row>
                <Row className="mb-3">
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label"
                    >
                        Lower limit
                    </label>
                    <div className="col-md-4">
                        <input
                            value={this.LowerLimit}
                            onChange={this.handleLowerLimitChange}
                            className="form-control"
                            type="text"
                            placeholder="Lower limit" />
                    </div>
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label"
                    >
                        Upper limit
                    </label>
                    <div className="col-md-4">
                        <input
                            value={this.UpperLimit}
                            onChange={this.handleUpperLimitChange}
                            className="form-control"
                            type="text"
                            placeholder="Upper limit" />
                    </div>
                </Row>
            </>
        );
    }
}

export default NumericElementProperties;
