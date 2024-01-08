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

const baseUrl = "https://localhost:7196";

class CalculationElementProperties extends Component {
    constructor(props) {
        super(props);

        this.state = {
            elementListOptionGroup: [],
            ModuleId: props.ModuleId,
            Code: "console.log('simple code Editor!');",

            elementRows: [{
                elementFieldSelectedGroup: 0,
                variableName: ''
            }]
        }

        this.handleElementListChange = this.handleElementListChange.bind(this);
        this.handleVariableNameChange = this.handleVariableNameChange.bind(this);
        this.removeRow = this.removeRow.bind(this);
        this.addRow = this.addRow.bind(this);
        this.fillAllElementList = this.fillAllElementList.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
        this.setCode = this.setCode.bind(this);

        this.fillAllElementList();
    }

    handleElementListChange(e) {
        debugger;
        //this.state.dependentFieldsSelectedGroup = e;
    };

    handleVariableNameChange(e) {
        debugger;
        //this.state.dependentFieldsSelectedGroup = e;
    };

    removeRow = (index) => {
        this.setState((prevState) => {
            const newRows = [...prevState.elementRows];
            newRows.splice(index, 1);
            return { elementRows: newRows };
        });
    };

    addRow = () => {
        this.setState((prevState) => ({
            elementRows: [...prevState.elementRows, {
                tagKey: '', tagName: '', tagValue: '',
                tagNameInpCls: 'form-control',
                tagValueInpCls: 'form-control',
            }],
        }));
    };

    fillAllElementList() {
        var allElements = [];

        fetch(baseUrl + '/Module/GetModuleElements?id=' + this.state.ModuleId, {
            method: 'GET',
        })
            .then(response => response.json())
            .then(data => {
                data.map(item => {
                    var itm = { label: item.title, value: item.id };
                    allElements.push(itm);
                });

                this.state.elementListOptionGroup = allElements;
            })
            .catch(error => {
                //console.error('Error:', error);
            });
    }

    handleInputChange = (index, fieldName, value) => {
        this.setState((prevState) => {
            const newRows = [...prevState.elementRows];
            newRows[index][fieldName] = value;
            return { elementRows: newRows };
        });
    };

    setCode = (val) => {
        this.state.Code = val;
    };

    render() {
        return (
            <Row className="mb-3">
                <div className="table-responsive mb-3">
                    <Table className="table table-hover mb-0">
                        <thead>
                            <tr>
                                <th>Source input name</th>
                                <th>Variable name</th>
                                <th>Action</th>
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
                        Add another
                    </Button>
                </div>
                <div style={{ border: "#eee 1px solid", borderRadius: '5px' }}>
                    <div style={{ borderBottom: '#eee 1px solid' }}>
                        <label>
                            Javascript editor
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

export default CalculationElementProperties;
