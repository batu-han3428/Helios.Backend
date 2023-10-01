import React, { Component } from "react";
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

import classnames from "classnames";
import ElementBase from '../Elements/Base/elementBase.js';

class Properties extends Component {
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
        };
        this.toggle = this.toggle.bind(this);
    }

    toggle(tab) {
        if (this.state.activeTab !== tab) {
            this.setState({
                activeTab: tab,
            });
        }
    }

    render() {
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
                                                            className="form-control"
                                                            type="text"
                                                            placeholder="Input name"
                                                        />
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
                                                            className="form-control"
                                                            type="text"
                                                            placeholder="Description"
                                                        />
                                                    </div>
                                                </Row>
                                            </CardText>
                                        </Col>
                                    </Row>
                                </TabPane>
                                <TabPane tabId="2">
                                    <Row>
                                        <Col sm="12">
                                            <CardText className="mb-0">
                                                Food truck fixie locavore, accusamus mcsweeney's
                                                marfa nulla single-origin coffee squid.
                                                Exercitation +1 labore velit, blog sartorial PBR
                                                leggings next level wes anderson artisan four loko
                                                farm-to-table craft beer twee. Qui photo booth
                                                letterpress, commodo enim craft beer mlkshk
                                                aliquip jean shorts ullamco ad vinyl cillum PBR.
                                                Homo nostrud organic, assumenda labore aesthetic
                                                magna delectus mollit. Keytar helvetica VHS salvia
                                                yr, vero magna velit sapiente labore stumptown.
                                                Vegan fanny pack odio cillum wes anderson 8-bit.
                                            </CardText>
                                        </Col>
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
                </Col>
            {/*</ElementBase>*/}
            </>

        );
    }
}

export default Properties;