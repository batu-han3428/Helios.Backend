import { extend } from 'jquery';
import React, { Component, useState, useContext, Form, FormField, TextBox, ComboBox, CheckBox, LinkButton } from 'react';
import ElementBase from './Base/elementBase.js';
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

//export default class AddTenant extends Component {
class TextElement extends ElementBase {
    constructor(props) {
        super(props);
    }
    //const [Placeholder, setPlaceholder] = useState('');

    //const handlePlaceholderChange = (e) => {
    //    setPlaceholder(e.target.value);
    //};
    render() {
        return
        <div className='row'>
            <div className='form-group'>
                <label> Name</label>
                <input className='form-control'type='text' id='Name' />
            </div>
            <div>
                <button type='submit' className='btn btn-primary' style={{ width: '100%' }}> Save</button>
            </div>
        </div>

    }
};
export default TextElement;

//function TextElementProperties(props) {
//    //const [Unit, setUnit] = useState('');
//    return (
//        <Row className="mb-3">
//            <label
//                htmlFor="example-text-input"
//                className="col-md-2 col-form-label"
//            >
//                Unit
//            </label>
//            <div className="col-md-10">
//                <input
//                    value={props.Unit}
//                    className="form-control"
//                    type="text"
//                    placeholder="Unit"
//                />
//            </div>
//        </Row>
//    );
//}
//export default TextElementProperties;

//class TextElementProperties extends React.Component {
//    constructor(props) {
//        super(props);
//        this.setState({ Unit: '' });
//    }

//    render() {
//        return
//        <Row className="mb-3">
//            <label
//                htmlFor="example-text-input"
//                className="col-md-2 col-form-label"
//            >
//                Unit
//            </label>
//            <div className="col-md-10">
//                <input
//                    value={this.Unit}
//                    className="form-control"
//                    type="text"
//                    placeholder="Unit"
//                />
//            </div>
//        </Row>
//    }
//};

