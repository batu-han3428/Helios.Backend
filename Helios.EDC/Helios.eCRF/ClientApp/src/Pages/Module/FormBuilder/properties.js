import React, { useState, useEffect, Component } from "react";
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
    Input,
    Button,
    FormGroup,
    Label
} from "reactstrap";
import Select from "react-select";
import classnames from "classnames";
import TextElementProperties from '../Elements/textElementProperties.js'
import NumericElementProperties from '../Elements/numericElementProperties.js'
import Conditions from "./conditions.js";
import Actions from "./action.js";
import Swal from 'sweetalert2'

const baseUrl = "https://localhost:7196";

class Properties extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            activeTab: "1",
            activeTab1: "5",
            activeTab2: "9",
            activeTab3: "13",
            verticalActiveTab: "1",
            customActiveTab: "1",
            activeTabJustify: "1",
            col1: true,
            col2: false,
            col3: false,
            col5: true,
            col6: true,
            col7: true,
            col8: true,
            col9: true,
            col10: false,
            col11: false,

            // General properties
            Id: props.Id,
            ModuleId: '08dbc973-abe2-4694-8d6c-b4697950e3f7',
            ElementDetailId: '',
            ElementType: props.Type,
            ElementName: '',
            Title: '',
            IsTitleHidden: true,
            Order: 0,
            Description: '',
            Width: 12,
            IsHidden: false,
            IsRequired: false,
            IsDependent: false,
            IsReadonly: false,
            CanMissing: false,

            // Dependency properties
            DependentSourceFieldId: '',
            DependentTargetFieldId: '',
            DependentCondition: 0,
            DependentAction: 0,
            DependentFieldValue: '',

            // Elements properties
            Unit: '',
            LowerLimit: '',
            UpperLimit: '',

            // Validation
            RequiredError: 'This value is required',
            ElementNameInputClass: 'form-control',

            // Other
            optionGroup: [],
            isOpen: false,
            isOpenClass: "mdi mdi-chevron-up"
        };

        this.toggle = this.toggle.bind(this);
        this.toggleAccordion = this.toggleAccordion.bind(this);
        this.saveProperties = this.saveProperties.bind(this);
        this.getElementData = this.getElementData.bind(this);
        this.fillElementProperties = this.fillElementProperties.bind(this);

        this.handleIdChange = this.handleIdChange.bind(this);
        this.handleModuleIdChange = this.handleModuleIdChange.bind(this);
        this.handleElementDetailIdChange = this.handleElementDetailIdChange.bind(this);
        this.handleElementNameChange = this.handleElementNameChange.bind(this);
        this.handleTitleChange = this.handleTitleChange.bind(this);
        this.handleIsTitleHiddenChange = this.handleIsTitleHiddenChange.bind(this);
        this.handleOrderChange = this.handleOrderChange.bind(this);
        this.handleDescriptionChange = this.handleDescriptionChange.bind(this);
        this.handleWidthChange = this.handleWidthChange.bind(this);
        this.handleIsHiddenChange = this.handleIsHiddenChange.bind(this);
        this.handleIsRequiredChange = this.handleIsRequiredChange.bind(this);
        this.handleIsDependentChange = this.handleIsDependentChange.bind(this);
        this.handleIsReadonlyChange = this.handleIsReadonlyChange.bind(this);
        this.handleCanMissingChange = this.handleCanMissingChange.bind(this);
        this.handleDependentSourceFieldIdChange = this.handleDependentSourceFieldIdChange.bind(this);
        this.handleDependentTargetFieldIdChange = this.handleDependentTargetFieldIdChange.bind(this);
        this.handleDependentConditionChange = this.handleDependentConditionChange.bind(this);
        this.handleDependentActionChange = this.handleDependentActionChange.bind(this);

        this.changeUnit.bind(this);
        this.changeLowerLimit.bind(this);
        this.changeUpperLimit.bind(this);
        //this.handleSaveModuleContent = this.handleSaveModuleContent.bind(this);
        //this.setState({ selectedGroup: null });
        //this.setState({
        //    optionGroup: [
        //        {
        //            label: "Picnic",
        //            options: [
        //                { label: "Mustard", value: "Mustard" },
        //                { label: "Ketchup", value: "Ketchup" },
        //                { label: "Relish", value: "Relish" }
        //            ]
        //        }
        //    ]
        //});

        this.getElementData();
    }

    toggleAccordion() {
        debugger;
        this.state.isOpen = !(this.state.isOpen);
        this.state.isOpenClass = this.state.isOpen ? "mdi mdi-chevron-up" : "mdi mdi-chevron-down";
    }

    getToggleClass() {
        if (this.isOpen)
            return "mdi mdi-chevron-up";
        else
            return "mdi mdi-chevron-down";
    }

    toggle(tab) {
        if (this.state.activeTab !== tab) {
            this.setState({
                activeTab: tab,
            });
        }
    }

    //handleSelectGroup(selectedGroup) {
    //    this.state.selectedGroup = selectedGroup;
    //}

    renderElementPropertiesSwitch(param) {
        switch (param) {
            case 2:
                return <TextElementProperties changeUnit={this.changeUnit} />;
            case 4:
                return <NumericElementProperties
                    changeUnit={this.changeUnit}
                    changeLowerLimit={this.changeLowerLimit}
                    changeUpperLimit={this.changeUpperLimit}
                />;
            default:
                return <TextElementProperties changeUnit={this.changeUnit} />;
        }
    }

    handleIdChange(e) {
        this.setState({ Id: e.target.value });
    };

    handleModuleIdChange(e) {
        this.setState({ ModuleId: e.target.value });
    };

    handleElementDetailIdChange(e) {
        this.setState({ ElementDetailId: e.target.value });
    };

    handleElementNameChange(e) {
        this.setState({ ElementName: e.target.value });
    };

    handleTitleChange(e) {
        this.setState({ Title: e.target.value });
    };

    handleIsTitleHiddenChange(e) {
        this.setState({ IsTitleHidden: e.target.value });
    };

    handleOrderChange(e) {
        this.setState({ Order: e.target.value });
    };

    handleDescriptionChange(e) {
        this.setState({ Description: e.target.value });
    };

    handleWidthChange(e) {
        this.setState({ Width: e.target.value });
    };

    handleIsHiddenChange(e) {
        this.setState({ IsHidden: e.target.value });
    };

    handleIsRequiredChange(e) {
        this.setState({ IsRequired: e.target.value });
    };

    handleIsDependentChange(e) {
        this.setState({ IsDependent: e.target.value });
    };

    handleIsReadonlyChange(e) {
        this.setState({ IsReadonly: e.target.value });
    };

    handleCanMissingChange(e) {
        this.setState({ CanMissing: e.target.value });
    };

    handleDependentSourceFieldIdChange(e) {
        this.setState({ DependentSourceFieldId: e.target.value });
    };

    handleDependentTargetFieldIdChange(e) {
        this.setState({ DependentTargetFieldId: e.target.value });
    };

    handleDependentConditionChange(e) {
        this.setState({ DependentCondition: e.target.value });
    };

    handleDependentActionChange(e) {
        this.setState({ DependentAction: e.target.value });
    };

    handleDependentFieldValueChange(e) {
        this.setState({ DependentFieldValue: e.target.value });
    };

    changeUnit = (newValue) => {
        this.setState({ Unit: newValue });
    };

    changeLowerLimit = (newValue) => {
        this.setState({ LowerLimit: newValue });
    };

    changeUpperLimit = (newValue) => {
        this.setState({ UpperLimit: newValue });
    };

    getElementData() {
        if (this.state.Id != "") {
            fetch(baseUrl + '/Module/GetElementData?id=' + this.state.Id, {
                method: 'GET',
            })
                .then(response => response.json())
                .then(data => {

                    this.fillElementProperties(data);
                })
                .catch(error => {
                    //console.error('Error:', error);
                });
        }
    }

    fillElementProperties(data) {
        this.state.Title = data.title;
        this.state.ElementName = data.elementName;
        this.state.Description = data.description;
        this.state.ElementType = data.elementType;
        this.state.Unit = data.unit != null ? data.unit : "";
        this.state.LowerLimit = data.lowerLimit != null ? data.lowerLimit : "";
        this.state.UpperLimit = data.upperLimit != null ? data.upperLimit : "";
        this.state.IsRequired = data.isRequired;
        this.state.IsHidden = data.isHidden;
        this.state.CanMissing = data.canMissing;
    }

    saveProperties(e) {
        if (this.state.ElementName == "") {
            this.setState({ ElementNameInputClass: "is-invalid form-control" });
            e.preventDefault();
        }
        else {
            debugger;
            fetch(baseUrl + '/Module/SaveModuleContent', {
                method: 'POST',
                headers: {
                    'Accept': 'application/json, text/plain, */*',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    Id: this.state.Id,
                    ModuleId: this.state.ModuleId,
                    UserId: '',
                    ElementDetailId: this.state.ElementDetailId,
                    ElementType: this.state.ElementType,
                    ElementName: this.state.ElementName,
                    Title: this.state.Title,
                    IsTitleHidden: this.state.IsTitleHidden,
                    Order: this.state.Order,
                    Description: this.state.Description,
                    Width: this.state.Width,
                    IsHidden: this.state.IsHidden == 'on' ? true : false,
                    IsRequired: this.state.IsRequired == 'on' ? true : false,
                    IsDependent: this.state.IsDependent == 'on' ? true : false,
                    IsReadonly: this.state.IsReadonly,
                    CanMissing: this.state.CanMissing == 'on' ? true : false,

                    // Dependency properties
                    DependentSourceFieldId: this.state.DependentSourceFieldId,
                    DependentTargetFieldId: this.state.DependentTargetFieldId,
                    DependentCondition: this.state.DependentCondition,
                    DependentAction: this.state.DependentAction,
                    DependentFieldValue: this.state.DependentFieldValue,

                    // Elements properties
                    Unit: this.state.Unit,
                    LowerLimit: this.state.LowerLimit,
                    UpperLimit: this.state.UpperLimit,


                })
            }).then(res => res.json())
                .then(data => {
                    if (data.isSuccess) {
                        Swal.fire(data.message, '', 'success');
                    } else {
                        Swal.fire(data.message, '', 'error');
                    };
                })
                .catch(error => {
                    //console.error('Error:', error);
                });
        }
    }

    render() {
        return (
            <>
                {/*<ElementBase>*/}
                <form onSubmit={this.saveProperties}>
                    <Col lg={12}>
                        <Card>
                            <CardBody>
                                <Nav tabs>
                                    <NavItem>
                                        <NavLink
                                            style={{ cursor: "pointer" }}
                                            className={classnames({
                                                active: this.state.activeTab === "1",
                                            })}
                                            onClick={() => {
                                                this.toggle("1");
                                            }}
                                        >
                                            General
                                        </NavLink>
                                    </NavItem>
                                    <NavItem>
                                        <NavLink
                                            style={{ cursor: "pointer" }}
                                            className={classnames({
                                                active: this.state.activeTab === "2",
                                            })}
                                            onClick={() => {
                                                this.toggle("2");
                                            }}
                                        >
                                            Dependency
                                        </NavLink>
                                    </NavItem>
                                    <NavItem>
                                        <NavLink
                                            style={{ cursor: "pointer" }}
                                            className={classnames({
                                                active: this.state.activeTab === "3",
                                            })}
                                            onClick={() => {
                                                this.toggle("3");
                                            }}
                                        >
                                            Validation
                                        </NavLink>
                                    </NavItem>
                                    <NavItem>
                                        <NavLink
                                            style={{ cursor: "pointer" }}
                                            className={classnames({
                                                active: this.state.activeTab === "4",
                                            })}
                                            onClick={() => {
                                                this.toggle("4");
                                            }}
                                        >
                                            Metadata
                                        </NavLink>
                                    </NavItem>
                                </Nav>

                                <TabContent activeTab={this.state.activeTab} className="p-3 text-muted">
                                    <TabPane tabId="1">
                                        <Row>
                                            <Col sm="12">
                                                <CardText className="mb-0">
                                                    <Row className="mb-3">
                                                        <label
                                                            htmlFor="example-text-input"
                                                            className="col-md-2 col-form-label"
                                                        >
                                                            Title
                                                        </label>
                                                        <div className="col-md-10">
                                                            <input
                                                                value={this.state.Title}
                                                                onChange={this.handleTitleChange}
                                                                className="form-control"
                                                                type="text"
                                                                placeholder="Title"

                                                            />
                                                        </div>
                                                    </Row>
                                                    <Row className="mb-3">
                                                        <label
                                                            htmlFor="example-text-input"
                                                            className="col-md-2 col-form-label"
                                                        >
                                                            Input name
                                                        </label>
                                                        <div className="col-md-10">
                                                            <input
                                                                value={this.state.ElementName}
                                                                onChange={this.handleElementNameChange}
                                                                className={this.state.ElementNameInputClass}
                                                                type="text"
                                                                placeholder="Input name"
                                                            />
                                                            <div type="invalid" className="invalid-feedback">{this.state.RequiredError}</div>
                                                        </div>
                                                    </Row>
                                                    <Row className="mb-3">
                                                        <label
                                                            htmlFor="example-text-input"
                                                            className="col-md-2 col-form-label"
                                                        >
                                                            Description
                                                        </label>
                                                        <div className="col-md-10">
                                                            <input
                                                                value={this.state.Description}
                                                                onChange={this.handleDescriptionChange}
                                                                className="form-control"
                                                                type="text"
                                                                placeholder="Description"
                                                            />
                                                        </div>
                                                    </Row>
                                                    {/*<i onClick={this.toggleAccordion} className={this.state.isOpenClass} style={{ fontSize: "12px", marginRight: "5px", cursor: "pointer" }}></i><Label style={{ borderBottom: "1px solid black" }} className="form-label">Advanced options</Label>*/}
                                                    {/*<Collapse isOpen={this.state.isOpen}>*/}
                                                    {this.renderElementPropertiesSwitch(this.state.ElementType)}
                                                    <Row className="mb-3">
                                                        <div className="form-check col-md-6">
                                                            <input type="checkbox" className="form-check-input" checked={this.state.IsRequired} onChange={this.handleIsRequiredChange} id="isRequired" />
                                                            <label className="form-check-label" htmlFor="isRequired">Is required</label>
                                                        </div>
                                                        <div className="form-check col-md-6">
                                                            <input type="checkbox" className="form-check-input" checked={this.state.IsHidden} onChange={this.handleIsHiddenChange} id="isHidden" />
                                                            <label className="form-check-label" htmlFor="isHidden">Is hidden</label>
                                                        </div>
                                                    </Row>
                                                    <Row className="mb-3">
                                                        <div className="form-check col-md-6">
                                                            <input type="checkbox" className="form-check-input" checked={this.state.CanMissing} onChange={this.handleCanMissingChange} id="canMissing" />
                                                            <label className="form-check-label" htmlFor="canMissing">Can missing</label>
                                                        </div>
                                                    </Row>
                                                    {/*</Collapse>*/}
                                                </CardText>
                                            </Col>
                                        </Row>
                                    </TabPane>
                                    <TabPane tabId="2">
                                        <Row>
                                            <Col sm="12">
                                                <div className="mb-3">
                                                    <Label className="form-label mb-3 d-flex">Is dependent</Label>
                                                    <div className="form-check form-check-inline">
                                                        <Input
                                                            type="radio"
                                                            id="dependentRadioInline"
                                                            name="dependentRadioInline"
                                                            className="form-check-input"
                                                        />
                                                        <Label
                                                            className="form-check-label" htmlFor="dependentRadioInline"
                                                        >
                                                            Yes
                                                        </Label>
                                                    </div>
                                                    <div className="form-check form-check-inline">
                                                        <Input
                                                            type="radio"
                                                            id="customRadioInline2"
                                                            name="dependentRadioInline"
                                                            className="form-check-input"
                                                        />
                                                        <Label
                                                            className="form-check-label" htmlFor="customRadioInline2"
                                                        >
                                                            No
                                                        </Label>
                                                    </div>
                                                </div>
                                            </Col>
                                        </Row>
                                        <Row>
                                            <Col sm="12">
                                                <div className="mb-3">
                                                    <Label>Dependent field</Label>
                                                    <Select
                                                        value={this.selectedGroup}
                                                        onChange={() => {
                                                            this.handleSelectGroup();
                                                        }}
                                                        options={this.state.optionGroup}
                                                        classNamePrefix="select2-selection"
                                                    />
                                                </div>
                                            </Col>
                                        </Row>
                                        <Row>
                                            <Col sm="4">
                                                <Conditions></Conditions>
                                            </Col>
                                            <Col sm="4">
                                                <Actions></Actions>
                                            </Col>
                                            <Col sm="4">
                                                <label
                                                    htmlFor="example-text-input"
                                                    className="col-md-12 col-form-label"
                                                >
                                                    Dependent filed value
                                                </label>
                                                <input
                                                    value={this.state.DependentFieldValue}
                                                    onChange={this.handleDependentFieldValueChange}
                                                    className="form-control"
                                                    type="text"
                                                    placeholder="Title"
                                                />
                                            </Col>
                                        </Row>
                                        <Row>
                                            <div className="mb-3">
                                                <Label className="form-label mb-3 d-flex">Is related</Label>
                                                <div className="form-check form-check-inline">
                                                    <Input
                                                        type="radio"
                                                        id="relatedRadioInline"
                                                        name="relatedRadioInline"
                                                        className="form-check-input"
                                                    />
                                                    <Label
                                                        className="form-check-label" htmlFor="relatedRadioInline"
                                                    >
                                                        Yes
                                                    </Label>
                                                </div>
                                                <div className="form-check form-check-inline">
                                                    <Input
                                                        type="radio"
                                                        id="customRadioInline2"
                                                        name="relatedRadioInline"
                                                        className="form-check-input"
                                                    />
                                                    <Label
                                                        className="form-check-label" htmlFor="customRadioInline2"
                                                    >
                                                        No
                                                    </Label>
                                                </div>
                                            </div>
                                        </Row>
                                    </TabPane>
                                    <TabPane tabId="3">
                                        <Row>
                                            <Col sm="12">
                                                <CardText className="mb-0">
                                                    Etsy mixtape wayfarers, ethical wes anderson tofu
                                                    before they sold out mcsweeney's organic lomo
                                                    retro fanny pack lo-fi farm-to-table readymade.
                                                    Messenger bag gentrify pitchfork tattooed craft
                                                    beer, iphone skateboard locavore carles etsy
                                                    salvia banksy hoodie helvetica. DIY synth PBR
                                                    banksy irony. Leggings gentrify squid 8-bit cred
                                                    pitchfork. Williamsburg banh mi whatever
                                                    gluten-free, carles pitchfork biodiesel fixie etsy
                                                    retro mlkshk vice blog. Scenester cred you
                                                    probably haven't heard of them, vinyl craft beer
                                                    blog stumptown. Pitchfork sustainable tofu synth
                                                    chambray yr.
                                                </CardText>
                                            </Col>
                                        </Row>
                                    </TabPane>
                                    <TabPane tabId="4">
                                        <Row>
                                            <Col sm="12">
                                                <CardText className="mb-0">
                                                    Trust fund seitan letterpress, keytar raw denim
                                                    keffiyeh etsy art party before they sold out
                                                    master cleanse gluten-free squid scenester freegan
                                                    cosby sweater. Fanny pack portland seitan DIY, art
                                                    party locavore wolf cliche high life echo park
                                                    Austin. Cred vinyl keffiyeh DIY salvia PBR, banh
                                                    mi before they sold out farm-to-table VHS viral
                                                    locavore cosby sweater. Lomo wolf viral, mustache
                                                    readymade thundercats keffiyeh craft beer marfa
                                                    ethical. Wolf salvia freegan, sartorial keffiyeh
                                                    echo park vegan.
                                                </CardText>
                                            </Col>
                                        </Row>
                                    </TabPane>
                                </TabContent>
                            </CardBody>
                        </Card>
                        <div style={{ float: 'right' }}>
                            <input className="btn btn-primary" type="submit" value="Save" />
                        </div>
                    </Col>
                </form>
                {/*</ElementBase>*/}
            </>

        );
    }
}

export default Properties;