import { extend } from 'jquery';
import React, { Component, useState, useContext, Form, FormField, TextBox, ComboBox, CheckBox, LinkButton } from 'react';
import ElementBase from './Base/elementBase.js';
import {
    Button,
    Card,
    CardBody,
    CardText,
    CardTitle,
    Col,
    Collapse,
    Container,
    Modal,
    ModalBody,
    ModalHeader,
    ModalFooter,
    Nav,
    NavItem,
    NavLink,
    Row,
    TabContent,
    Table,
    TabPane,
    Label
} from "reactstrap";
import Properties from '../FormBuilder/properties.js';
import AccordionComp from '../../../../components/Common/AccordionComp/AccordionComp';
import Select from "react-select";
import ToastComp from '../../../../components/Common/ToastComp/ToastComp';

const baseUrl = "https://localhost:7196";

class ListElementProperties extends Component {
    constructor(props) {
        super(props);
        debugger;
        this.state = {
            Id: '00000000-0000-0000-0000-000000000000',
            layoutOptionGroup: [
                { label: "Vertical", value: 1 },
                { label: "Horizontal", value: 2 },
            ],
            layoutSelectedGroup: null,
            TagListOptionGroup: [
            ],
            TagListSelectedGroup: null,
            modalState: false,
            allTagList: [],
            savedTagList: props.SavedTagList,

            tagKey: '',
            tagKeyDisableStatus: false,
            tagKeyInpCls: 'form-control',
            rows: [{
                tagName: '',
                tagValue: '',
                tagNameInpCls: 'form-control',
                tagValueInpCls: 'form-control',
            }],
            editIndex: 0,
            operationType: 1,
            isEdit: false,
        }
        this.toastRef = React.createRef();
        var l = this.state.layoutOptionGroup.filter(function (e) {
            if (e.value == props.Layout)
                return e;
        });

        this.setShowToast.bind(this);
        this.state.layoutSelectedGroup = l;
        this.handleLayoutChange = this.handleLayoutChange.bind(this);
        this.updateSavedList = this.updateSavedList.bind(this);
        this.handleTagListChange = this.handleTagListChange.bind(this);
        this.toggleNewTagModal = this.toggleNewTagModal.bind(this);
        this.handleTagKeyChange = this.handleTagKeyChange.bind(this);
        this.addRow = this.addRow.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
        this.removeRow = this.removeRow.bind(this);
        this.handleSaveTag = this.handleSaveTag.bind(this);
        this.handleEditSavedTag = this.handleEditSavedTag.bind(this);
        this.handleDeleteSavedTag = this.handleDeleteSavedTag.bind(this);

        this.getMultipleTagList();
    }

    setShowToast() {
        this.state.showToast = false;
    }

    getMultipleTagList() {
        var tgOptionGroup = [];

        if (this.state.Id != "") {
            fetch(baseUrl + '/Module/GetMultipleTagList?id=' + this.state.Id, {
                method: 'GET',
            })
                .then(response => response.json())
                .then(data => {
                    //fill list
                    this.state.allTagList = data;

                    data.map(item => {
                        var itm = { label: item[0].tagkey, value: item[0].id };
                        tgOptionGroup.push(itm);

                    });

                    this.state.TagListOptionGroup = tgOptionGroup;
                })
                .catch(error => {
                    //console.error('Error:', error);
                });
        }
        //else {
        //    this.fillDependentFieldList();
        //}
    }

    handleLayoutChange = selectedOption => {
        this.setState({ Layout: selectedOption.value });
        this.state.layoutSelectedGroup = selectedOption;
        this.props.changeLayout(selectedOption.value);
        this.props.Layout = selectedOption;
    };

    updateSavedList = () => {
        const { savedTagList, SavedTagList } = this.props;

        //updateSavedTagListProp([...SavedTagList, ...this.state.savedTagList]);
    };

    handleTagListChange = selectedOption => {
        var tgs = [];

        this.state.allTagList.filter(function (e) {
            e.filter(function (ee) {
                if (selectedOption.label == ee.tagkey) {
                    tgs.push(ee);
                }
            });
        });

        this.props.changeSavedTagList(tgs);

        this.setState(prevState => ({
            savedTagList: [...prevState.savedTagList, ...tgs],
        }));
    };

    toggleNewTagModal = (e, isEdit) => {
        this.state.rows = [];
        this.state.tagKey = '';
        this.state.tagKeyInpCls = 'form-control';
        this.state.operationType = e;
        this.state.modalState = !(this.state.modalState);
        this.state.tagKeyDisableStatus = false;
        this.state.isEdit = isEdit;
    };

    handleTagKeyChange(e) {
        this.setState({ tagKey: e.target.value });
    };

    addRow = () => {
        this.setState((prevState) => ({
            rows: [...prevState.rows, {
                tagKey: '', tagName: '', tagValue: '',
                tagNameInpCls: 'form-control',
                tagValueInpCls: 'form-control',
            }], // Add a new row to the state
        }));
    };

    handleInputChange = (index, fieldName, value) => {
        this.setState((prevState) => {
            const newRows = [...prevState.rows];
            newRows[index][fieldName] = value;
            return { rows: newRows };
        });
    };

    removeRow = (index) => {
        this.setState((prevState) => {
            const newRows = [...prevState.rows];
            newRows.splice(index, 1);
            return { rows: newRows };
        });
    };

    handleSaveTag = (e) => {
        var tags = [];
        var tagKey = this.state.tagKey;
        var isValid = true;

        if (this.state.tagKey == '') {
            isValid = false;
        }

        this.state.tagKeyInpCls = this.state.tagKey == '' ? 'is-invalid form-control' : 'form-control';

        this.state.rows.filter(function (e) {
            if (e.tagName == '' || e.tagValue == '')
                isValid = false;

            e.tagNameInpCls = e.tagName == '' ? 'is-invalid form-control' : 'form-control';
            e.tagValueInpCls = e.tagValue == '' ? 'is-invalid form-control' : 'form-control';
        });

        this.state.rows.map(function (e) {
            var t = {
                tagKey: tagKey,
                tagName: e.tagName,
                tagValue: e.tagValue
            };

            tags.push(t);
        });

        if (isValid) {
            if (!this.state.isEdit) {
                if (this.state.operationType == 1) {//add new tag to db
                    fetch(baseUrl + '/Module/AddNewTag', {
                        method: 'POST',
                        headers: {
                            'Accept': 'application/json, text/plain, */*',
                            'Content-Type': 'application/json'
                        },
                        body:
                            JSON.stringify(
                                tags
                            )
                    }).then(res => res.json())
                        .then(data => {
                            //if (data.isSuccess) {
                            //    Swal.fire(data.message, '', 'success');
                            //} else {
                            //    Swal.fire(data.message, '', 'error');
                            //    console.log(data.message);
                            //};
                            if (data.isSuccess) {
                                this.toggleNewTagModal();
                                this.toastRef.current.setToast({
                                    message: data.message,
                                    stateToast: true
                                });
                            } else {
                                this.toastRef.current.setToast({
                                    message: data.message,
                                    stateToast: false
                                });
                            }

                            this.getMultipleTagList();
                        })
                        .catch(error => {
                            //console.error('Error:', error);
                        });
                }
                else {
                    this.setState(prevState => ({
                        savedTagList: [...prevState.savedTagList, ...tags],
                    }));

                    this.toggleNewTagModal();

                    this.toastRef.current.setToast({
                        message: "Successful",
                        stateToast: true
                    });
                }
            }
            else {
                var newtgs = [];
                var index = 0;
                var ei = this.state.editIndex;
                var rws = this.state.rows;

                this.state.savedTagList.filter(function (e) {
                    var t = {
                        id: e.id,
                        tagName: e.tagName,
                        tagValue: e.tagValue,
                        tagNameInpCls: 'form-control',
                        tagValueInpCls: 'form-control',
                    };

                    if (index == ei) {
                        t.tagName = rws[0].tagName;
                        t.tagValue = rws[0].tagValue;
                    }

                    index++;
                    newtgs.push(t);
                });

                this.state.savedTagList = [];

                this.setState(prevState => ({
                    savedTagList: [...prevState.savedTagList, ...newtgs],
                }, () => {
                }));

                this.props.changeSavedTagList(newtgs);
                this.toggleNewTagModal();
                this.toastRef.current.setToast({
                    message: "Successful",
                    stateToast: true
                });
            }
        }
    };

    handleEditSavedTag = (id, index) => {
        var r = this.state.savedTagList.filter(function (e) {
            if (e.id == id)
                return e;
        });

        var n = [];
        this.state.allTagList.filter(function (e) {
            e.filter(function (ee) {
                if (ee.id == id) {
                    //return ee;
                    //break;
                    n.push(ee);
                }
            });
        });

        var t = [{
            tagName: r[0].tagName,
            tagValue: r[0].tagValue,
            tagNameInpCls: 'form-control',
            tagValueInpCls: 'form-control',
        }];

        this.toggleNewTagModal(0, true);
        this.state.tagKey = n[0].tagkey;
        this.state.tagKeyDisableStatus = true;
        this.isEdit = true;
        this.state.editIndex = index;

        this.setState(prevState => ({
            rows: [...prevState.rows, ...t],
        }));
    }

    handleDeleteSavedTag = (index) => {
        this.setState((prevState) => {
            const newRows = [...prevState.savedTagList];
            newRows.splice(index, 1);
            return { savedTagList: newRows };
        }, () => {
            this.props.changeSavedTagList(this.state.savedTagList);
        });
    }

    render() {
        return (
            <div>
                {/*<AccordionComp title="Option properties" body={*/}
                <Row className="mb-3">
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label"
                    >
                        Layout
                    </label>
                    <div className="col-md-10">
                        <Select
                            value={this.state.layoutSelectedGroup}
                            onChange={this.handleLayoutChange}
                            options={this.state.layoutOptionGroup}
                            classNamePrefix="select2-selection" />
                    </div>
                </Row>
                <Row className="mb-3">
                    <label
                        htmlFor="example-text-input"
                        className="col-md-2 col-form-label"
                    >
                        Tag list
                    </label>
                    <div className="col-md-10">
                        <Select
                            value={this.state.TagListSelectedGroup}
                            onChange={this.handleTagListChange}
                            options={this.state.TagListOptionGroup}
                            classNamePrefix="select2-selection" />
                    </div>
                </Row>
                <Row className="mb-3">
                    <div className="table-responsive">
                        <Table className="table table-hover mb-0">
                            <thead>
                                <tr>
                                    <th>Text to be shown to the user</th>
                                    <th>Data to be saved to database</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                {this.props.SavedTagList.map((row, index) => (
                                    <tr key={index}>
                                        <td>
                                            {row.tagName}
                                        </td>
                                        <td>{row.tagValue}</td>
                                        <td>
                                            <Button className="actionBtn" onClick={() => this.handleEditSavedTag(row.id, index)}>
                                                <i className="far fa-edit" ></i>
                                            </Button>
                                            <Button className="actionBtn" onClick={() => this.handleDeleteSavedTag(index)}>
                                                <i className="far fa-trash-alt"></i>
                                            </Button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </Table>
                    </div>
                </Row>
                <Row className="mb-3">
                    <input className="btn btn-primary col-md-3 ml-5" type="button" value="Add another" onClick={() => this.toggleNewTagModal(0, false)} />
                    <input className="btn btn-success col-md-3 ml-5" type="button" value="Add a tag" onClick={() => this.toggleNewTagModal(1, false)} />
                </Row>
                <Col sm={6} md={4} xl={3}>
                    <Modal isOpen={this.state.modalState} toggle={this.toggleNewTagModal} size="lg">
                        <ModalHeader className="mt-0" toggle={this.toggleNewTagModal}>Add new tag</ModalHeader>
                        <ModalBody>
                            <div>
                                <Row className="mb-3">
                                    <label
                                        htmlFor="example-text-input"
                                        className="col-md-2 col-form-label"
                                    >
                                        Tag name
                                    </label>
                                    <div className="col-md-10">
                                        <input
                                            value={this.state.tagKey}
                                            onChange={this.handleTagKeyChange}
                                            className={this.state.tagKeyInpCls}
                                            type="text"
                                            disabled={this.state.tagKeyDisableStatus}
                                        />
                                    </div>
                                </Row>
                                <Row>
                                    <div className="table-responsive">
                                        <Table className="table table-hover mb-0">
                                            <thead>
                                                <tr>
                                                    <th>Text to be shown to the user</th>
                                                    <th>Data to be saved to database</th>
                                                    <th>Action</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {this.state.rows.map((row, index) => (
                                                    <tr key={index}>
                                                        <td>
                                                            <input
                                                                type="text"
                                                                className={row.tagNameInpCls}
                                                                value={row.tagName}
                                                                onChange={(e) => this.handleInputChange(index, 'tagName', e.target.value)}
                                                            />
                                                        </td>
                                                        <td>
                                                            <input
                                                                type="text"
                                                                className={row.tagValueInpCls}
                                                                value={row.tagValue}
                                                                onChange={(e) => this.handleInputChange(index, 'tagValue', e.target.value)}
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
                                        <Button color="success" onClick={this.addRow} className='mt-1' disabled={this.state.tagKeyDisableStatus}>
                                            Add another
                                        </Button>
                                    </div>
                                </Row>
                            </div>
                        </ModalBody>
                        <ModalFooter>
                            {/*<Button color="secondary" onClick={this.toggleNewTagModal}>*/}
                            {/*    Close*/}
                            {/*</Button>*/}
                            <Button color="success" onClick={this.handleSaveTag}>
                                Save
                            </Button>
                        </ModalFooter>
                    </Modal>
                </Col>
                {/* } />*/}
                <ToastComp
                    ref={this.toastRef}
                />
            </div>
        );
    }
};

export default ListElementProperties;
