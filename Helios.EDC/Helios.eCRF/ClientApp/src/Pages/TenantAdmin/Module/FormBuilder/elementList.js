import React, { useState, useEffect, useRef } from 'react';
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
import { useDispatch, useSelector } from "react-redux";
import { startloading, endloading } from '../../../../store/loader/actions';
import Swal from 'sweetalert2'
import ToastComp from '../../../../components/Common/ToastComp/ToastComp';
import TextElement from '../Elements/TextElement/textElement.js';
import NumericElement from '../Elements/NumericElement/numericElement.js';
import RadioElement from '../Elements/RadioElement/radioElement.js';
import CheckElement from '../Elements/CheckElement/checkElement.js';
import DropdownElement from '../Elements/DropdownElement/dropdownElement.js';
import DropdownCheckListElement from '../Elements/DropdownCheckListElement/dropdownCheckListElement.js';
import LabelElement from '../Elements/LabelElement/labelElement.js';
import DateElement from '../Elements/DateElement/dateElement.js';

const elements = [
    { key: 1, name: 'Label', icon: 'fas fa-text-height' },
    { key: 2, name: 'Text', icon: 'fas fa-font' },
    { key: 3, name: 'Hidden', icon: 'fas fa-puzzle-piece' },
    { key: 4, name: 'Numeric', icon: 'fas fa-sort-numeric-down' },
    { key: 5, name: 'Textarea', icon: 'fas fa-ad' },
    { key: 6, name: 'Date', icon: 'far fa-calendar-alt ' },
    { key: 7, name: 'Calculation', icon: 'fas fa-calculator' },
    { key: 8, name: 'Radio Button', icon: 'ion ion-md-radio-button-on' },
    { key: 9, name: 'CheckList', icon: 'fas fa-check-square' },
    { key: 10, name: 'Drop Down', icon: 'ti-arrow-circle-down' },
    { key: 11, name: 'Drop Down Checklist', icon: 'ti-arrow-circle-down' },
    { key: 12, name: 'File Attachmen', icon: 'fas fa-file-import' },
    { key: 13, name: 'Range Slider', icon: 'fas fa-sliders-h' },
    { key: 14, name: 'Concomitant medication', icon: 'dripicons-view-list' },
    { key: 15, name: 'Table', icon: 'fas fa-table' },
    { key: 16, name: 'Datagrid', icon: 'fas fa-table' },
    { key: 17, name: 'Adverse Event', icon: 'fas fa-heartbeat' }
];

function ElementList(props) {
    const toastRef = useRef();

    const baseUrl = "https://localhost:7196";
    const [moduleId, setModuleId] = useState(props.ModuleId);
    const [elementId, setElementId] = useState(0);
    const [moduleElementList, setModuleElementList] = useState([]);
    const [elementType, setElementType] = useState(0);
    const [modal_large, setmodal_large] = useState(false);
    const [activeTab, setActiveTab] = useState(false);
    const [elementName, setElementName] = useState('');
    const [isCalcBtn, setIsCalcBtn] = useState('');
    const dispatch = useDispatch();
    const userInformation = useSelector(state => state.rootReducer.Login);

    const removeBodyCss = () => {
        document.body.classList.add("no_padding");
    };

    const tog_large = (e, type, id, tabid, isCalc = false) => {
        setIsCalcBtn(isCalc);
        getElementNameByKey(type);
        
        if (id !== 0) {
            setElementId(id);
            setElementType(0);
        }
        else {
            setElementType(type);
        }

        setActiveTab(tabid);
        setmodal_large(!modal_large);
        removeBodyCss();
    };

    const copyElement = (e, id) => {
        fetch(baseUrl + '/Module/CopyElement?id=' + id + '&userId=' + userInformation.userId, {
            method: 'POST',
        })
            .then(response => response.json())
            .then(data => {
                if (data.isSuccess) {
                    toastRef.current.setToast({
                        message: data.message,
                        stateToast: true
                    });
                    dispatch(endloading());
                } else {
                    toastRef.current.setToast({
                        message: data.message,
                        stateToast: false
                    });
                    dispatch(endloading());
                }
            })
            .catch(error => {
                //console.error('Error:', error);
            });
    };

    const deleteElement = (e, id) => {
        Swal.fire({
            title: "You will not be able to recover this element!",
            text: "Do you confirm?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3bbfad",
            confirmButtonText: "Yes",
            cancelButtonText: "Cancel",
            closeOnConfirm: false
        }).then(async (result) => {
            if (result.isConfirmed) {
                try {
                    dispatch(startloading());
                    fetch(baseUrl + '/Module/DeleteElement?id=' + id + '&userId=' + userInformation.userId, {
                        method: 'POST',
                    })
                        .then(response => response.json())
                        .then(data => {
                            if (data.isSuccess) {
                                dispatch(endloading());
                                Swal.fire(data.message, '', 'success');
                            } else {
                                dispatch(endloading());
                                Swal.fire(data.message, '', 'error');
                            }
                        })
                        .catch(error => {
                            //console.error('Error:', error);
                        });
                } catch (error) {
                    dispatch(endloading());
                    Swal.fire('An error occurred', '', 'error');
                }
            }
        })
    }

    const elmementItems = elements.map((l) =>
        <Button className="elmlst" id={l.key} key={l.key} onClick={e => tog_large(e, l.key, 0, "1")}><i className={l.icon} style={{ color: '#00a8f3' }}></i> &nbsp;{l.name} </Button>
    );

    const fetchData = () => {
        fetch(baseUrl + '/Module/GetModuleElements?id=' + moduleId, {
            method: 'GET',
        })
            .then(response => response.json())
            .then(data => {
                setModuleElementList(data);
            })
            .catch(error => {
                //console.error('Error:', error);
            });
    }

    const renderElementsSwitch = (param) => {
        switch (param.elementType) {
            case 1:
                return <LabelElement Title={param.title} />;
            case 2:
                return <TextElement IsDisable={"disabled"}
                />;
            case 4:
                return <NumericElement
                    IsDisable={"disabled"}
                    Unit={""}
                    Mask={""}
                    LowerLimit={0}
                    UpperLimit={0}
                />;
            case 6:
                return <DateElement
                    IsDisable={true}
                    StartDay={param.startDay}
                    EndDay={param.endDay}
                    StartMonth={param.startMonth}
                    EndMonth={param.endMonth}
                    StartYear={param.startYear}
                    EndYear={param.endYear}
                />
            case 8:
                return <RadioElement
                    IsDisable={"disabled"}
                    ElementOptions={param.elementOptions}
                />
            case 9:
                return <CheckElement
                    IsDisable={"disabled"}
                    ElementOptions={param.elementOptions}
                />
            case 10:
                return <DropdownElement
                    IsDisable={true}
                    ElementOptions={param.elementOptions}
                />
            case 11:
                return <DropdownCheckListElement
                    IsDisable={true}
                    ElementOptions={param.elementOptions}
                />
            default:
                return <TextElement IsDisable={"disabled"}
                />;
        }
    }

    const content = moduleElementList.map((item) => {
        var w = item.width === 0 ? 12 : item.width;
        var cls = "mb-6 col-md-" + w;

        return <Row className={cls} key={item.id}>
            <div style={{ marginBottom: '3px', marginTop: '10px' }}>
                <label style={{ marginRight: '5px' }}>
                    {item.elementType !== 1 && item.title}
                </label>
                {item.isDependent && (
                    <Button className="actionBtn" id={item.id} onClick={e => tog_large(e, 0, item.id, "2")}><i className="fas fa-link"></i></Button>
                )}
                {item.isRelated && (
                    <Button className="actionBtn" id={item.id} onClick={e => tog_large(e, 0, item.id, "2")}><i className="fas fa-diagram-project"></i></Button>
                )}
                {item.elementType === 7 /*calculated*/ && (
                    <Button className="actionBtn" id={item.id} onClick={e => tog_large(e, 0, item.id, "1", true)}><i className="fas fa-calculator"></i></Button>
                )}
                <Button className="actionBtn" id={item.id} onClick={e => tog_large(e, item.elementType, item.id, "1")}><i className="far fa-edit"></i></Button>
                <Button className="actionBtn"><i className="far fa-copy" onClick={e => copyElement(e, item.id)}></i></Button>
                <Button className="actionBtn"><i className="fas fa-trash-alt" onClick={e => deleteElement(e, item.id)}></i></Button>
            </div>
            {renderElementsSwitch(item)}
            <label style={{ fontSize: "8pt", textDecoration: 'none' }}>
                {item.description}
            </label>
        </Row>
    }
    );

    const getElementNameByKey = (key) => {
        const item = elements.find(item => item.key === key);
        var name = item ? item.name : null;
        setElementName(name + " properties");
    };

    useEffect(() => {
        dispatch(startloading());
        fetchData();
        dispatch(endloading());
    });

    return (
        <div>
            <div style={{ width: "200px", float: 'left' }}>
                <div>
                    {elmementItems}
                </div>
                <Col sm={6} md={4} xl={3}>
                    <Modal isOpen={modal_large} toggle={tog_large} size="lg">
                        <ModalHeader className="mt-0" toggle={tog_large}>{elementName}</ModalHeader>
                        <ModalBody>
                            <Properties ModuleId={moduleId} Type={elementType} Id={elementId} TenantId={userInformation.tenantId} UserId={userInformation.userId} ActiveTab={activeTab} isCalcBtn={isCalcBtn}></Properties>
                        </ModalBody>
                    </Modal>
                </Col>
            </div>
            <div style={{ margin: '10px 20px 10px 215px' }} className="row">
                {content}
            </div>
            <ToastComp
                ref={toastRef}
            />
        </div>

    );
}

export default ElementList;
