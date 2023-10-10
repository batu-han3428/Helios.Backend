import React, { useState, useEffect, ListItem } from 'react';
import { Button, Card, CardBody, Col, Container, Form, FormGroup, Input, InputGroup, Label, Row } from "reactstrap";

const conditionList = [
    { key: 1, name: 'Less', icon: 'fas fa-ad' },
    { key: 2, name: 'More', icon: 'fas fa-ad' },
    { key: 3, name: 'Equal', icon: 'fas fa-puzzle-piece' },
    { key: 4, name: 'More and equal', icon: 'fas fa-ad' },
    { key: 5, name: 'Less and equal', icon: 'fas fa-ad' },
    { key: 6, name: 'Not equal', icon: 'fas fa-calendar-alt' },
];


//const elmementItems = conditionList.map((l) =>
//    <>
//        <li className="elmlst" id={l.key} onClick={e => tog_large(e, l.key)}><i className={l.icon} style={{ color: '#00a8f3' }}></i> &nbsp;{l.name} </Button><br />
//    </>

//);

const Conditions = props => {
    const [selectedGroup, setselectedGroup] = useState(null);

    return (
        <>
            <div className="mb-3">
                <Label>Dependency condition</Label>
                <Select
                    value={this.state.selectedGroup}
                    onChange={() => {
                        this.handleSelectGroup();
                    }}
                    options={this.state.optionGroup}
                    classNamePrefix="select2-selection"
                />
            </div>
        </>
    )
}