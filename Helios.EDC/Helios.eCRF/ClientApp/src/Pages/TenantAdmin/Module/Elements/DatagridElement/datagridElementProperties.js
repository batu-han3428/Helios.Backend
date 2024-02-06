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
import { withTranslation } from "react-i18next";

class DatagridElementProperties extends Component {
    constructor(props) {
        super(props);

        this.state = {
            elementRows: [],
        }

        this.removeRow = this.removeRow.bind(this);
        this.addRow = this.addRow.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    removeRow = (index) => {
        this.setState((prevState) => {
            const newRows = [...prevState.elementRows];
            newRows.splice(index, 1);
            return { elementRows: newRows };
        }, () => {
            //this.props.changeCalculationSourceInputs(JSON.stringify(this.state.elementRows));
        });
    };

    addRow = () => {
        this.setState((prevState) => ({
            elementRows: [
                ...prevState.elementRows,
                {
                    title: '',
                    width: '',
                },
            ],
        }), () => {
            //this.props.changeCalculationSourceInputs(JSON.stringify(this.state.elementRows));
        });
    };

    handleInputChange = (index, fieldName, value) => {
        this.setState((prevState) => {
            const newRows = [...prevState.elementRows];
            newRows[index][fieldName] = value;
            return { elementRows: newRows };
        }, () => {
            //this.props.changeCalculationSourceInputs(JSON.stringify(this.state.elementRows));
            //this.controlNullValuesInRows();
        });
    };

    render() {
        return (
            <>
                <Row className="mb-3">
                    <div className="table-responsive mb-3">
                        <Table className="table table-hover mb-0">
                            <thead>
                                <tr>
                                    <th>{this.props.t("Title")}</th>
                                    <th>{this.props.t("Width")}</th>
                                    <th>{this.props.t("Action")}</th>
                                </tr>
                            </thead>
                            <tbody>
                                {this.state.elementRows.map((row, index) => (
                                    <tr key={index}>
                                        <td>
                                            <input
                                                style={{ fontSize: '8pt' }}
                                                value={row.title}
                                                className="form-control"
                                                type="text"
                                                placeholder="Title"
                                                onChange={(e) => this.handleInputChange(index, 'title', e.target.value)}
                                            />
                                        </td>
                                        <td>
                                            <input
                                                style={{ fontSize: '8pt' }}
                                                value={row.width}
                                                className="form-control"
                                                type="text"
                                                placeholder="Width"
                                                onChange={(e) => this.handleInputChange(index, 'width', e.target.value)}
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
                            {this.props.t("Add a column")}
                        </Button>
                    </div>
                </Row>
            </>
        );
    }
};

export default withTranslation()(DatagridElementProperties);
