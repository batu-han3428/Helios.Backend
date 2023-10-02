import PropTypes from 'prop-types';
import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import { Link, Routes, Route, useNavigate } from "react-router-dom";
import { MDBDataTable } from "mdbreact";
import {
    Row, Col, Card, CardBody, CardTitle, CardSubtitle, Dropdown,
    DropdownToggle,
    DropdownItem,
    DropdownMenu
} from "reactstrap";
import { useStudyListGetQuery } from '../../store/services/Study';
import './study.css';


const StudyList = props => {

    const [menu, setMenu] = useState(false);
    const [tableData, setTableData] = useState([]);

    const navigate = useNavigate();

    const toggle = () => {
        setMenu(!menu);
    };

    const studyUpdate = (id) => {
        console.log(`Edit button clicked for study with ID: ${id}`);
        navigate(`/addstudy`, { state: { studyId: id } });
    };

    const getActions = (id) => {
        const actions = (
            <div className="icon-container">
                <div className="icon icon-update" onClick={() => { studyUpdate(id) }}></div>
                <div className="icon icon-demo"></div>
                <div className="icon icon-unlock"></div>
                <div className="icon icon-live"></div>
            </div>);
        return actions;
    };
    const data = {
        columns: [
            {
                label: "",
                field: "id",
                sort: "asc",
                width: 150
            },
            {
                label: "Study Name",
                field: "studyName",
                sort: "asc",
                width: 150
            },
            {
                label: "Protocol Code",
                field: "protocolCode",
                sort: "asc",
                width: 150
            },
            {
                label: "Ask Subject Initial",
                field: "askSubjectInitial",
                sort: "asc",
                width: 150
            },
            {
                label: "Study Link",
                field: "studyLink",
                sort: "asc",
                width: 150
            },
            {
                label: "Last Updated On",
                field: "updatedAt",
                sort: "asc",
                width: 150
            },
            {
                label: 'Actions',
                field: 'actions',
                sort: 'disabled',
                width: 100,
            }
        ],
        rows: tableData
    }

    const { data: studyData, error, isLoading } = useStudyListGetQuery();

    useEffect(() => {
        if (!isLoading && !error) {
            const updatedStudyData = studyData.map(item => {
                return {
                    ...item,
                    actions: getActions(item.id)
                };
            });
            setTableData(updatedStudyData);
        }
    }, [studyData, error, isLoading]);

    const navigatePage = (root) => {
        navigate(root);
    };

    document.title = "Study | Veltrix - React Admin & Dashboard Template";
    return (
        <React.Fragment>
            <div className="page-content">
                <div className="container-fluid">
                    <div className="page-title-box">
                        <Row className="align-items-center">
                            <Col md={8}>
                                <h6 className="page-title">Study list</h6>
                                {/*<ol className="breadcrumb m-0">*/}
                                {/*    <li className="breadcrumb-item active">Study list</li>*/}
                                {/*</ol>*/}
                            </Col>

                            <Col md="4">
                                <div className="float-end d-none d-md-block">
                                    <Dropdown isOpen={menu} toggle={toggle}>
                                        <DropdownToggle color="primary" className="btn btn-primary dropdown-toggle waves-effect waves-light">
                                            Add Study
                                        </DropdownToggle>
                                        <DropdownMenu end>
                                            <DropdownItem onClick={() => navigatePage("/addstudy")}>
                                                <i className="ti-plus" style={{ marginRight:"10px" }}></i>
                                                <span>Create a new study</span>
                                            </DropdownItem>
                                            <DropdownItem divider />
                                            <DropdownItem>Add from an existing study</DropdownItem>
                                        </DropdownMenu>
                                    </Dropdown>
                                </div>
                            </Col>
                        </Row>
                    </div>
                    <Row>
                <Col className="col-12">
                    <Card>
                        <CardBody>
                            <MDBDataTable hover responsive striped bordered data={data} />
                        </CardBody>
                    </Card>
                </Col>
                    </Row>
                </div>
            </div>
        </React.Fragment>
    );
};

StudyList.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(StudyList);
