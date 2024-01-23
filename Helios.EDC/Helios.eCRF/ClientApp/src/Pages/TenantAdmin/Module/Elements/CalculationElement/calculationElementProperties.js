import React, { Component, useState, useContext, Form, FormField, TextBox, ComboBox, CheckBox, LinkButton } from 'react';
import {
    Button,
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
    Table,
    TabPane,
} from "reactstrap";
import Select from "react-select";
import CodeMirror from "@uiw/react-codemirror";
import { withTranslation } from "react-i18next";

const baseUrl = "https://localhost:7196";

class CalculationElementProperties extends Component {
    constructor(props) {
        super(props);

        var inps = props.CalculationSourceInputs !== "" ? JSON.parse(props.CalculationSourceInputs) : [];
        var inpsCount = props.CalculationSourceInputs !== "" ? JSON.parse(props.CalculationSourceInputs).length : 0;

        this.state = {
            elementListOptionGroup: [],
            ModuleId: props.ModuleId,
            Code: props.MainJs,
            inputCounter: inpsCount,
            elementRows: inps,
        }

        this.removeRow = this.removeRow.bind(this);
        this.addRow = this.addRow.bind(this);
        this.fillAllElementList = this.fillAllElementList.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
        this.setCode = this.setCode.bind(this);

        this.fillAllElementList();
    }

    removeRow = (index) => {
        this.setState((prevState) => {
            const newRows = [...prevState.elementRows];
            newRows.splice(index, 1);
            return { elementRows: newRows };
        });
    };

    addRow = () => {
        this.state.inputCounter = this.state.inputCounter + 1;

        this.setState((prevState) => ({
            elementRows: [...prevState.elementRows, {
                elementFieldSelectedGroup: this.state.elementListOptionGroup[0], variableName: 'A' + this.state.inputCounter
            }],
        }));
    };

    fillAllElementList() {
        fetch(baseUrl + '/Module/GetModuleElements?id=' + this.state.ModuleId, {
            method: 'GET',
        })
            .then(response => response.json())
            .then(data => {
                const allElements = data.map(item => ({
                    label: item.type !== 1 ? item.title + ' (' + item.elementName + ')' : item.title,
                    value: item.id,
                }));

                this.state.elementListOptionGroup = allElements;

                if (this.state.inputCounter === 0) {
                    this.addRow();
                }
            })
            .catch(error => {
                // Handle errors
                console.error('Error:', error);
            });

    }

    handleInputChange = (index, fieldName, value) => {
        this.setState((prevState) => {
            const newRows = [...prevState.elementRows];
            newRows[index][fieldName] = value;
            return { elementRows: newRows };
        }, () => {
            this.props.changeCalculationSourceInputs(JSON.stringify(this.state.elementRows));
        });
    };

    setCode = (val) => {
        this.state.Code = val;
        this.props.changeMainJs(val);
    };

    render() {
        return (
            <Row className="mb-3">
                <div className="table-responsive mb-3">
                    <Table className="table table-hover mb-0">
                        <thead>
                            <tr>
                                <th>{this.props.t("Source input name")}</th>
                                <th>{this.props.t("Variable name")}</th>
                                <th>{this.props.t("Action")}</th>
                            </tr>
                        </thead>
                        <tbody>
                            {this.state.elementRows.map((row, index) => (
                                <tr key={index}>
                                    <td>
                                        <Select
                                            value={row.elementFieldSelectedGroup}
                                            onChange={(e) => this.handleInputChange(index, 'elementFieldSelectedGroup', e)}
                                            options={this.state.elementListOptionGroup}
                                            classNamePrefix="select2-selection"
                                            className="form-control"
                                            placeholder={this.props.t("Select")}
                                        />
                                    </td>
                                    <td>
                                        <input
                                            style={{ fontSize: '8pt' }}
                                            value={row.variableName}
                                            className="form-control"
                                            type="text"
                                            placeholder="Variable name"
                                            onChange={(e) => this.handleInputChange(index, 'variableName', e.target.value)}
                                        />
                                    </td>
                                    <td>
                                        <Button className="actionBtn" onClick={() => this.removeRow(index)}>
                                            <i className="far fa-trash-alt"></i>
                                        </Button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </Table>
                    <Button color="success" onClick={this.addRow} className='mt-1'>
                        {this.props.t("Add another")}
                    </Button>
                </div>
                <div style={{ border: "#eee 1px solid", borderRadius: '5px' }}>
                    <div style={{ borderBottom: '#eee 1px solid' }}>
                        <label>
                            {this.props.t("Javascript editor")}
                        </label>
                    </div>
                    <CodeMirror
                        value={this.state.Code}
                        onChange={this.setCode}
                        height="100px"
                    />
                </div>
            </Row>
        );
    }
};

export default withTranslation()(CalculationElementProperties);
