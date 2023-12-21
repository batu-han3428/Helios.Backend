import React, { useState, useEffect, ListItem } from 'react';
import {
    Row,
    Col,
    Card,
    CardBody,
    CardTitle,
    Modal,
    Container,
    ModalBody,
    ModalHeader,
    ModalFooter,
    Button,
} from "reactstrap";
import Properties from './properties.js';
import './formBuilder.css';

let elementType = 0;
const elements = [
    { key: 1, name: 'Label', icon: 'fas fa-ad' },
    { key: 2, name: 'Text', icon: 'fas fa-ad' },
    { key: 3, name: 'Hidden', icon: 'fas fa-puzzle-piece' },
    { key: 4, name: 'Numeric', icon: 'fas fa-ad' },
    { key: 5, name: 'Textarea', icon: 'fas fa-ad' },
    { key: 6, name: 'Date', icon: 'fas fa-calendar-alt' },
    { key: 7, name: 'Calculation', icon: 'fas fa-calculator' },
    { key: 8, name: 'Radio Button', icon: 'fas fa-ad' },
    { key: 9, name: 'CheckList', icon: 'fas fa-check-square' },
    { key: 10, name: 'Drop Down', icon: 'fas fa-ad' },
    { key: 11, name: 'Drop Down Checklist', icon: 'fas fa-ad' },
    { key: 12, name: 'File Attachmen', icon: 'fas fa-file' },
    { key: 13, name: 'Range Slider', icon: 'fas fa-ad' },
    { key: 14, name: 'Concomitant medication', icon: 'fas fa-ad' },
    { key: 15, name: 'Table', icon: 'fas fa-ad' },
    { key: 16, name: 'Datagrid', icon: 'fas fa-ad' },
    { key: 17, name: 'Adverse Event', icon: 'fas fa-ad' }
];

function ElementList(props) {
    const [modal_large, setmodal_large] = useState(false);

    const removeBodyCss = () => {
        document.body.classList.add("no_padding");
    };

    const tog_large = (e, type) => {
        setmodal_large(!modal_large);
        removeBodyCss();
        elementType = type;
    };

    const elmementItems = elements.map((l) =>
        <>
            <Button className="elmlst" id={l.key} onClick={e => tog_large(e, l.key)}><i className={l.icon} style={{ color: '#00a8f3' }}></i> &nbsp;{l.name} </Button><br />
        </>

    );

    return (
        <>
            <div>
                {elmementItems}
            </div><Col sm={6} md={4} xl={3}>
                <Modal isOpen={modal_large} toggle={tog_large} size="lg">
                    <ModalBody>
                        <Properties Type={elementType}></Properties>
                    </ModalBody>
                    {/*<ModalFooter>*/}
                    {/*    <Button color="secondary" onClick={tog_large}>*/}
                    {/*        Close*/}
                    {/*    </Button>{' '}*/}
                    {/*    <Button color="primary" onClick={saveProperties}>*/}
                    {/*        Save*/}
                    {/*    </Button>*/}
                    {/*</ModalFooter>*/}
                </Modal>
            </Col>
        </>
    );
}

export default ElementList;
