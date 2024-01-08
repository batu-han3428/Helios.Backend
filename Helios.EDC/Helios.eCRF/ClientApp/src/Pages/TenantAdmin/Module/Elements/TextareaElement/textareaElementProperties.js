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

class TextareaElementProperties extends Component {
    constructor(props) {
        super(props);

        this.state = {
            AddTodayDate: props.AddTodayDate,
        }

        this.handleDefaultValueChange = this.handleDefaultValueChange.bind(this);
        this.handleAddTodayDateChange = this.handleAddTodayDateChange.bind(this);
    }

    handleDefaultValueChange(e) {
        this.props.changeDefaultValue(e.target.value);
    };

    handleAddTodayDateChange(e) {
        debugger;
        this.props.changeAddTodayDate(e.target.value);
        this.props.AddTodayDate = e.target.value;
    };

    render() {
        return (
            <>
                <Row className="mb-3">
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label"
                    >
                        Default value
                    </label>
                    <div className="col-md-4" style={{ marginRight:'6px' }}>
                        <input
                            value={this.props.DefaultValue}
                            onChange={this.handleDefaultValueChange}
                            className="form-control"
                            type="text"
                            placeholder="Default value" />
                    </div>
                    <div className="form-check col-md-4" style={{ marginTop:'7px' }}>
                        <input type="checkbox"
                            className="form-check-input"
                            checked={this.state.AddTodayDate}
                            onChange={this.handleAddTodayDateChange} id="addTodayDate" />
                        <label className="form-check-label" htmlFor="addTodayDate">Show "Today" button</label>
                    </div>
                </Row>
            </>
        );
    }
};

export default TextareaElementProperties;
