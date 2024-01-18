import React, { Component, useState, useContext, Form, FormField, TextBox, ComboBox, CheckBox, LinkButton } from 'react';
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

class RangeSliderElementProperties extends Component {
    constructor(props) {
        super(props);

        this.handleDefaultValueChange = this.handleDefaultValueChange.bind(this);
        this.handleLowerLimitChange = this.handleLowerLimitChange.bind(this);
        this.handleUpperLimitChange = this.handleUpperLimitChange.bind(this);
        this.handleLeftTextChange = this.handleLeftTextChange.bind(this);
        this.handleRightTextChange = this.handleRightTextChange.bind(this);
    }

    handleDefaultValueChange(e) {
        this.props.changeDefaultValue(e.target.value);
    };
    
    handleLowerLimitChange(e) {
        this.props.changeLowerLimit(e.target.value);
    };

    handleUpperLimitChange(e) {
        this.props.changeUpperLimit(e.target.value);
    };

    handleLeftTextChange(e) {
        this.props.changeLeftText(e.target.value);
    };

    handleRightTextChange(e) {
        this.props.changeRightText(e.target.value);
    };

    render() {
        return (
            <>
                <Row className="mb-3">
                    <label
                        htmlFor="example-text-input"
                        style={{ fontSize:'10pt' }}
                        className="col-md-2 col-form-label">
                        Minimum value
                    </label>
                    <div className="col-md-3" style={{ marginRight: '6px' }}>
                        <input
                            value={this.props.LowerLimit}
                            onChange={this.handleLowerLimitChange}
                            className="form-control"
                            type="number"
                            placeholder="Minimum value" />
                    </div>
                    <label
                        htmlFor="example-text-input"
                        style={{ fontSize:'10pt' }}
                        className="col-md-2 col-form-label">
                        Maximum value
                    </label>
                    <div className="col-md-3" style={{ marginRight: '6px' }}>
                        <input
                            value={this.props.UpperLimit}
                            onChange={this.handleUpperLimitChange}
                            className="form-control"
                            type="number"
                            placeholder="Maximum value" />
                    </div>
                </Row>
                <Row className="mb-3">
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label">
                        Left text
                    </label>
                    <div className="col-md-3" style={{ marginRight: '6px' }}>
                        <input
                            value={this.props.LeftText}
                            onChange={this.handleLeftTextChange}
                            className="form-control"
                            type="text"
                            placeholder="Left text" />
                    </div>
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label">
                        Right text
                    </label>
                    <div className="col-md-3" style={{ marginRight: '6px' }}>
                        <input
                            value={this.props.RightText}
                            onChange={this.handleRightTextChange}
                            className="form-control"
                            type="text"
                            placeholder="Right text" />
                    </div>
                </Row>
                <Row className="mb-3">
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label"
                    >
                        Default value
                    </label>
                    <div className="col-md-6" style={{ marginRight: '6px' }}>
                        <input
                            value={this.props.DefaultValue}
                            onChange={this.handleDefaultValueChange}
                            className="form-control"
                            type="text"
                            placeholder="Default value" />
                    </div>
                </Row>
            </>
        );
    }
};

export default RangeSliderElementProperties;
