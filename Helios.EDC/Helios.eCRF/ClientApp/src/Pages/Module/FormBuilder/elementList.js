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
import './formBuilder.css'

const elements = [
    { key: 1, name: 'Label' },
    { key: 2, name: 'Text' },
    { key: 3, name: 'Hidden' },
    { key: 4, name: 'Numeric' },
    { key: 5, name: 'Textarea' },
    { key: 6, name: 'Date' },
    { key: 7, name: 'Calculation' },
    { key: 8, name: 'Radio Button' },
    { key: 9, name: 'CheckList' },
    { key: 10, name: 'Drop Down' },
    { key: 11, name: 'Drop Down Checklist' },
    { key: 12, name: 'File Attachmen' },
    { key: 13, name: 'Range Slider' },
    { key: 14, name: 'Concomitant medication' },
    { key: 15, name: 'Table' },
    { key: 16, name: 'Datagrid' },
    { key: 17, name: 'Adverse Event' }
];

function ElementList(props) {
    const [modal_large, setmodal_large] = useState(false);

    const removeBodyCss = () => {
        document.body.classList.add("no_padding");
    };

    const tog_large = () => {
        setmodal_large(!modal_large);
        removeBodyCss();
    };

    const elmementItems = elements.map((l) =>
        //<div type={l.key} className="elmlst">
        //    {l.name}
        //</div>
        <>            
            <Button className="elmlst" id={l.key} onClick={tog_large}> + {l.name} </Button><br />
        </>

    );

    return (
        <>
            <div>
                {elmementItems}
            </div><Col sm={6} md={4} xl={3}>
                <Modal isOpen={modal_large} toggle={tog_large} size="lg">
                    <ModalBody>
                        <Properties></Properties>
                    </ModalBody>
                    <ModalFooter>
                        <Button color="secondary" onClick={tog_large}>
                            Close
                        </Button>{' '}
                        <Button color="primary">
                            Save
                        </Button>
                    </ModalFooter>
                </Modal>
            </Col>
        </>
    );
}

export default ElementList;
