import PropTypes from 'prop-types';
import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { MDBDataTable } from "mdbreact";
import {
    Row, Col, Card, CardBody, Dropdown, DropdownToggle, DropdownItem, DropdownMenu, Tooltip
} from "reactstrap";
import { useStudyListGetQuery, useStudyGetQuery } from '../../store/services/Study';
import './study.css';
import { useDispatch } from "react-redux";
import { startloading, endloading } from '../../store/loader/actions';
import { addStudy } from '../../store/study/actions';
import { formatDate } from "../../helpers/format_date";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'


const StudyList = props => {

    const [studyId, setStudyId] = useState(null); 
    const [skip, setSkip] = useState(true);
    const [menu, setMenu] = useState(false);
    const [tableData, setTableData] = useState([]);

    const navigate = useNavigate();

    const dispatch = useDispatch();

    const toggle = () => {
        setMenu(!menu);
    };

    const studyUpdate = (id) => {
        navigate(`/addstudy`, { state: { studyId: id } });
    };

    const { data: data1, isLoading1, isError1 } = useStudyGetQuery(studyId, {
        skip, refetchOnMountOrArgChange: true
    });

    const goToStudy = (id, equivalentStudyId) => {
        setSkip(false);
        setStudyId(id);
    };

    useEffect(() => {
        if (data1 && !isLoading1 && !isError1) {
            dispatch(addStudy(data1));
            navigate("/visits");
        }
    }, [data1, isLoading1, isError1]);

    const getActions = (id, equivalentStudyId) => {
        const actions = (
            <div className="icon-container">
                <div title={props.t("Update")} className="icon icon-update" onClick={() => { studyUpdate(id) }}></div>
                <div title={props.t("Go to demo study")} className="icon icon-demo" onClick={() => { goToStudy(equivalentStudyId, id) }}></div>
                <div title={props.t("Lock")} className="icon icon-unlock"></div>
                <div title={props.t("Go to active study")} className="icon icon-live" onClick={() => { goToStudy(id, equivalentStudyId) }}></div>
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
                label: props.t("Study name"),
                field: "studyName",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("Study link"),
                field: "studyLink",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("Protocol code"),
                field: "protocolCode",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("Ask subject Initial"),
                field: "askSubjectInitial",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("Last updated on"),
                field: "updatedAt",
                sort: "asc",
                width: 150
            },
            {
                label: props.t('Actions'),
                field: 'actions',
                sort: 'disabled',
                width: 100,
            }
        ],
        rows: tableData
    }

    const { data: studyData, error, isLoading } = useStudyListGetQuery();

    useEffect(() => {
        dispatch(startloading());
        if (!isLoading && !error) {
            const updatedStudyData = studyData.map(item => {
                return {
                    ...item,
                    updatedAt: formatDate(item.updatedAt),
                    actions: getActions(item.id, item.equivalentStudyId)
                };
            });
            setTableData(updatedStudyData);

            const timer = setTimeout(() => {
                generateInfoLabel();
            }, 10)
            
            dispatch(endloading());

            return () => clearTimeout(timer);
        }
    }, [studyData, error, isLoading, props.t]);

    const navigatePage = (root) => {
        navigate(root);
    };

    const generateInfoLabel = () => {
        var infoDiv = document.querySelector('.dataTables_info');
        var infoText = infoDiv.innerHTML;
        let words = infoText.split(" ");
        if (words[0] === "Showing") {
            let from = words[1];
            let to = words[3];
            let total = words[5];
            infoDiv.innerHTML = props.t("Showing entries").replace("{from}", from).replace("{to}", to).replace("{total}", total);
        } else {
            let from = words[2];
            let to = words[4];
            let total = words[0];
            infoDiv.innerHTML = props.t("Showing entries").replace("{from}", from).replace("{to}", to).replace("{total}", total);
        }
    };

    document.title = "Studylist | Veltrix - React Admin & Dashboard Template";
    return (
        <React.Fragment>
            <div className="page-content">
                <div className="container-fluid">
                    <div className="page-title-box">
                        <Row className="align-items-center">
                            <Col md={8}>
                                <h6 className="page-title">{props.t("Study list")}</h6>
                            </Col>

                            <Col md="4">
                                <div className="float-end d-none d-md-block">
                                    <Dropdown isOpen={menu} toggle={toggle}>
                                        <DropdownToggle color="primary" className="btn btn-primary dropdown-toggle waves-effect waves-light">
                                            {props.t("Add a study")}&nbsp;<FontAwesomeIcon icon="fa-solid fa-caret-down" />
                                        </DropdownToggle>
                                        <DropdownMenu end>
                                            <DropdownItem onClick={() => navigatePage("/addstudy")}>
                                                <FontAwesomeIcon style={{ marginRight: "10px" }} icon="fa-solid fa-plus" />
                                                <span>{props.t("Create a new study")}</span>
                                            </DropdownItem>
                                            <DropdownItem divider />
                                            <DropdownItem>{props.t("Add from an existing study")}</DropdownItem>
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
                            <MDBDataTable 
                                paginationLabel={[props.t("Previous"), props.t("Next")]}
                                entriesLabel={props.t("Show entries")}
                                searchLabel={props.t("Search")}
                                hover
                                responsive
                                striped
                                bordered
                                data={data}
                             />
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
