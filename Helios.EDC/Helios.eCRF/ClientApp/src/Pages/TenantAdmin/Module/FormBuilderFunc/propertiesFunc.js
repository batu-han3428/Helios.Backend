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
    Label
} from "reactstrap";
import Select from "react-select";
import classnames from "classnames";
import ElementBase from '../Elements/Base/elementBase.js';
import { TextElement } from '../Elements/textElement.js'
import TextElementProperties from '../Elements/textElementProperties.js'
import { useFormik } from "formik";
import NumericElementProperties from '../Elements/numericElementProperties.js'

const Properties = props => {
    const state = {
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
        elementType: props.Type,
        optionGroup: [],
        isOpen: false,
        isOpenClass: "mdi mdi-chevron-up"
    };

    const [title, setTitle] = useState('');

    const toggle = (tab)=> {
        if (state.activeTab !== tab) {
            this.setState({
                activeTab: tab,
            });
        }
    }

    return (
        <>
            {/*<ElementBase>*/}
            <Col lg={12}>
                <Card>
                    <CardBody>
                        <Nav tabs>
                            <NavItem>
                                <NavLink
                                    style={{ cursor: "pointer" }}
                                    className={classnames({
                                        active: state.activeTab === "1",
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
                                        active: state.activeTab === "2",
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
                                        active: state.activeTab === "3",
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
                                        active: state.activeTab === "4",
                                    })}
                                    onClick={() => {
                                        this.toggle("4");
                                    }}
                                >
                                    Metadata
                                </NavLink>
                            </NavItem>
                        </Nav>

                        <TabContent activeTab={state.activeTab} className="p-3 text-muted">
                            <TabPane tabId="1">
                                <Row>
                                    <Col sm="12">
                                        <CardText className="mb-0">
                                            {/*<ElementBase title={title}>*/}
                                            <Row className="mb-3">
                                                <label
                                                    htmlFor="example-text-input"
                                                    className="col-md-2 col-form-label"
                                                >
                                                    Title
                                                </label>
                                                <div className="col-md-10">
                                                    <input
                                                        value={title}
                                                        className="form-control"
                                                        type="text"
                                                        placeholder="Title"
                                                    />
                                                </div>
                                            </Row>
                                            {/*<Row className="mb-3">*/}
                                            {/*    <label*/}
                                            {/*        htmlFor="example-text-input"*/}
                                            {/*        className="col-md-2 col-form-label"*/}
                                            {/*    >*/}
                                            {/*        Input name*/}
                                            {/*    </label>*/}
                                            {/*    <div className="col-md-10">*/}
                                            {/*        <input*/}
                                            {/*            value={this.ElementName}*/}
                                            {/*            className="form-control"*/}
                                            {/*            type="text"*/}
                                            {/*            placeholder="Input name"*/}
                                            {/*        />*/}
                                            {/*    </div>*/}
                                            {/*</Row>*/}
                                            {/*<Row className="mb-3">*/}
                                            {/*    <label*/}
                                            {/*        htmlFor="example-text-input"*/}
                                            {/*        className="col-md-2 col-form-label"*/}
                                            {/*    >*/}
                                            {/*        Description*/}
                                            {/*    </label>*/}
                                            {/*    <div className="col-md-10">*/}
                                            {/*        <input*/}
                                            {/*            value={this.Description}*/}
                                            {/*            className="form-control"*/}
                                            {/*            type="text"*/}
                                            {/*            placeholder="Description"*/}
                                            {/*        />*/}
                                            {/*    </div>*/}
                                            {/*    </Row>*/}
                                            {/*</ElementBase>*/}
                                            {/*{this.renderElementPropertiesSwitch(state.elementType)}*/}
                                            {/*<Row className="mb-3">*/}
                                            {/*    <div className="form-check col-md-6">*/}
                                            {/*        <input type="checkbox" className="form-check-input" checked={this.IsRequired} id="isRequired" />*/}
                                            {/*        <label className="form-check-label" htmlFor="isRequired">Is required</label>*/}
                                            {/*    </div>*/}
                                            {/*    <div className="form-check col-md-6">*/}
                                            {/*        <input type="checkbox" className="form-check-input" checked={this.IsHidden} id="isHidden" />*/}
                                            {/*        <label className="form-check-label" htmlFor="isHidden">Is hidden</label>*/}
                                            {/*    </div>*/}
                                            {/*</Row>*/}
                                            {/*<Row className="mb-3">*/}
                                            {/*    <div className="form-check col-md-6">*/}
                                            {/*        <input type="checkbox" className="form-check-input" checked={this.CanMissing} id="canMissing" />*/}
                                            {/*        <label className="form-check-label" htmlFor="canMissing">Can missing</label>*/}
                                            {/*    </div>*/}
                                            {/*</Row>*/}
                                            {/*</Collapse>*/}
                                        </CardText>
                                    </Col>
                                </Row>
                            </TabPane>
                            <TabPane tabId="2">
                                {/*<Row>*/}
                                {/*    <Col sm="12">*/}
                                {/*        <div className="mb-3">*/}
                                {/*            <Label className="form-label mb-3 d-flex">Is dependent</Label>*/}
                                {/*            <div className="form-check form-check-inline">*/}
                                {/*                <Input*/}
                                {/*                    type="radio"*/}
                                {/*                    id="customRadioInline1"*/}
                                {/*                    name="customRadioInline1"*/}
                                {/*                    className="form-check-input"*/}
                                {/*                />*/}
                                {/*                <Label*/}
                                {/*                    className="form-check-label" htmlFor="customRadioInline1"*/}
                                {/*                >*/}
                                {/*                    Yes*/}
                                {/*                </Label>*/}
                                {/*            </div>*/}

                                {/*            <div className="form-check form-check-inline">*/}
                                {/*                <Input*/}
                                {/*                    type="radio"*/}
                                {/*                    id="customRadioInline2"*/}
                                {/*                    name="customRadioInline1"*/}
                                {/*                    className="form-check-input"*/}
                                {/*                />*/}
                                {/*                <Label*/}
                                {/*                    className="form-check-label" htmlFor="customRadioInline2"*/}
                                {/*                >*/}
                                {/*                    No*/}
                                {/*                </Label>*/}
                                {/*            </div>*/}
                                {/*        </div>*/}
                                {/*    </Col>*/}
                                {/*</Row>*/}
                                {/*<Row>*/}
                                {/*    <Col sm="12">*/}
                                {/*        <div className="mb-3">*/}
                                {/*            <Label>Dependent field</Label>*/}
                                {/*            <Select*/}
                                {/*                value={this.selectedGroup}*/}
                                {/*                onChange={() => {*/}
                                {/*                    this.handleSelectGroup();*/}
                                {/*                }}*/}
                                {/*                options={state.optionGroup}*/}
                                {/*                classNamePrefix="select2-selection"*/}
                                {/*            />*/}
                                {/*        </div>*/}
                                {/*    </Col>*/}
                                {/*</Row>*/}
                                {/*<Row>*/}
                                {/*    <Col sm="4">*/}
                                {/*        <div className="mb-3">*/}
                                {/*            <Label>Dependency condition</Label>*/}
                                {/*            <Select*/}
                                {/*                value={this.selectedGroup}*/}
                                {/*                onChange={() => {*/}
                                {/*                    this.handleSelectGroup();*/}
                                {/*                }}*/}
                                {/*                options={state.optionGroup}*/}
                                {/*                classNamePrefix="select2-selection"*/}
                                {/*            />*/}
                                {/*        </div>*/}
                                {/*    </Col>*/}
                                {/*</Row>*/}
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
                    {/*<Button color="primary" onClick={this.saveProperties}>*/}
                    {/*    Save*/}
                    {/*</Button>*/}
                </div>
            </Col>
            {/*</ElementBase>*/}
        </>

    );
}

export default Properties;