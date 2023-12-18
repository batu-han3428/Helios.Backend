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
import { useDispatch } from "react-redux";
import { startloading, endloading } from '../../../store/loader/actions';
import Swal from 'sweetalert2'
import ToastComp from '../../../components/Common/ToastComp/ToastComp';

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
    const baseUrl = "https://localhost:7196";
    const [moduleId, setModuleId] = useState(1);
    const [elementId, setElementId] = useState("");
    const [moduleElementList, setModuleElementList] = useState([]);
    const [elementType, setElementType] = useState(0);
    const [modal_large, setmodal_large] = useState(false);
    const [showToast, setShowToast] = useState(false);
    const [message, setMessage] = useState("");
    const [stateToast, setStateToast] = useState(true);
    const dispatch = useDispatch();

    const removeBodyCss = () => {
        document.body.classList.add("no_padding");
    };

    const tog_large = (e, type, id) => {
        if (id != "") {
            setElementId(id);
        }
        else {
            setElementType(type);
        }

        setmodal_large(!modal_large);
        removeBodyCss();
    };

    const copyElement = (e, id) => {
        fetch(baseUrl + '/Module/CopyElement?id=' + id, {
            method: 'POST',
        })
            .then(response => response.json())
            .then(data => {
                if (data.isSuccess) {
                    setMessage(data.message)
                    setStateToast(true);
                    setShowToast(true);
                    dispatch(endloading());
                } else {
                    setMessage(data.message)
                    setStateToast(false);
                    setShowToast(true);
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

                    fetch(baseUrl + '/Module/DeleteElement?id=' + id, {
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
        <Button className="elmlst" id={l.key} key={l.key} onClick={e => tog_large(e, l.key, '')}><i className={l.icon} style={{ color: '#00a8f3' }}></i> &nbsp;{l.name} </Button>
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

    const content = moduleElementList.map((item) =>
        <Row className="mb-3" key={item.id}>
            <div>
                <label style={{ marginRight: '5px' }}>
                    {item.title}
                </label>
                <Button className="actionBtn" id={item.id} onClick={e => tog_large(e, 0, item.id)}><i className="far fa-edit"></i></Button>
                <Button className="actionBtn"><i className="far fa-copy" onClick={e => copyElement(e, item.id)}></i></Button>
                <Button className="actionBtn"><i className="fas fa-trash-alt" onClick={e => deleteElement(e, item.id)}></i></Button>
            </div>
            <div className="col-md-10">
                <input
                    className="form-control"
                    type="text"
                    disabled />
            </div>
        </Row>
    );

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
                        <ModalHeader className="mt-0" toggle={tog_large}>Properties</ModalHeader>
                        <ModalBody>
                            <Properties Type={elementType} Id={elementId}></Properties>
                        </ModalBody>
                    </Modal>
                </Col>
            </div>
            <div style={{ margin: '10px 0 10px 215px' }}>
                {content}
            </div>
            <ToastComp
                message={message}
                showToast={showToast}
                stateToast={stateToast}
            />
        </div>

    );
}

export default ElementList;
