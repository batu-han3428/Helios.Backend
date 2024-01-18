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

class DateElementProperties extends Component {
    constructor(props) {
        super(props);

        this.state = {
            AddTodayDate: props.AddTodayDate,
        }

        this.handleDefaultValueChange = this.handleDefaultValueChange.bind(this);
        this.handleAddTodayDateChange = this.handleAddTodayDateChange.bind(this);
        this.handleStartDayChange = this.handleStartDayChange.bind(this);
        this.handleEndDayChange = this.handleEndDayChange.bind(this);
        this.handleStartMonthChange = this.handleStartMonthChange.bind(this);
        this.handleEndMonthChange = this.handleEndMonthChange.bind(this);
        this.handleStartYearChange = this.handleStartYearChange.bind(this);
        this.handleEndYearChange = this.handleEndYearChange.bind(this);
    }

    handleDefaultValueChange(e) {
        this.props.changeDefaultValue(e.target.value);
    };

    handleAddTodayDateChange(e) {
        var val = e.target.value ==="on" ? true : false;
        this.props.changeAddTodayDate(val);
        this.state.AddTodayDate = val;
    };

    handleStartDayChange(e) {
        this.props.changeStartDay(e.target.value);
    };

    handleEndDayChange(e) {
        this.props.changeEndDay(e.target.value);
    };

    handleStartMonthChange(e) {
        this.props.changeStartMonth(e.target.value);
    };

    handleEndMonthChange(e) {
        this.props.changeEndMonth(e.target.value);
    };

    handleStartYearChange(e) {
        this.props.changeStartYear(e.target.value);
    };

    handleEndYearChange(e) {
        this.props.changeEndYear(e.target.value);
    };

    render() {
        return (
            <>
                <Row className="mb-3">
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label">
                        Start day
                    </label>
                    <div className="col-md-3" style={{ marginRight: '6px' }}>
                        <input
                            value={this.props.StartDay}
                            onChange={this.handleStartDayChange}
                            className="form-control"
                            type="number"
                            placeholder="Start day" />
                    </div>
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label">
                        End day
                    </label>
                    <div className="col-md-3" style={{ marginRight: '6px' }}>
                        <input
                            value={this.props.EndDay}
                            onChange={this.handleEndDayChange}
                            className="form-control"
                            type="number"
                            placeholder="End day" />
                    </div>
                </Row>
                <Row className="mb-3">
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label">
                        Start month
                    </label>
                    <div className="col-md-3" style={{ marginRight: '6px' }}>
                        <input
                            value={this.props.StartMonth}
                            onChange={this.handleStartMonthChange}
                            className="form-control"
                            type="number"
                            placeholder="Start month" />
                    </div>
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label">
                        End month
                    </label>
                    <div className="col-md-3" style={{ marginRight: '6px' }}>
                        <input
                            value={this.props.EndMonth}
                            onChange={this.handleEndMonthChange}
                            className="form-control"
                            type="number"
                            placeholder="End month" />
                    </div>
                </Row>
                <Row className="mb-3">
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label">
                        Start year
                    </label>
                    <div className="col-md-3" style={{ marginRight: '6px' }}>
                        <input
                            value={this.props.StartYear}
                            onChange={this.handleStartYearChange}
                            className="form-control"
                            type="number"
                            placeholder="Start year" />
                    </div>
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label">
                        End year
                    </label>
                    <div className="col-md-3" style={{ marginRight: '6px' }}>
                        <input
                            value={this.props.EndYear}
                            onChange={this.handleEndYearChange}
                            className="form-control"
                            type="number"
                            placeholder="End year" />
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
                        <span style={{ fontSize:'7pt' }}>Format : DD.MM.YYYY or UNK.UNK.YYYY / Format : DD.MM.YYYY veya UNK.UNK.YYYY</span>
                    </div>
                    <div className="form-check col-md-3" style={{ marginTop: '7px' }}>
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

export default DateElementProperties;
