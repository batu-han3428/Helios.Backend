import React, { useState, useEffect, useRef } from "react";
import PropTypes from 'prop-types';
import { withTranslation } from "react-i18next";
import { Row, Col, Button, Card, CardBody } from "reactstrap";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { MDBDataTable } from "mdbreact";
import ModalComp from "../../components/Common/ModalComp/ModalComp";
import AddOrUpdateSystemAdmin from "./AddOrUpdateSystemAdmin";
import ToastComp from "../../components/Common/ToastComp/ToastComp";
import { useSelector, useDispatch } from 'react-redux';
import { useSystemAdminListGetQuery, useSystemAdminActivePassiveMutation, useSystemAdminResetPasswordMutation, useSystemAdminDeleteMutation } from '../../store/services/SystemAdmin';
import { startloading, endloading } from '../../store/loader/actions';
import Swal from 'sweetalert2';


const ListSystemAdmin = props => {

    const modalRef = useRef();

    const modalContentRef = useRef();

    const userInformation = useSelector(state => state.rootReducer.Login);

    const dispatch = useDispatch();

    const [showToast, setShowToast] = useState(false);
    const [message, setMessage] = useState("");
    const [stateToast, setStateToast] = useState(true);
    const [modalTitle, setModalTitle] = useState("");
    const [modalButtonText, setModalButtonText] = useState("");
    const [systemAdminId, setSystemAdminId] = useState(0);
    const [table, setTable] = useState([]);

    const toastHandle = (message, state) => {
        setMessage(message);
        setStateToast(state);
        setShowToast(true);
    }

    const addSystemAdmin = () => {
        setModalTitle(props.t("Add a system admin"));
        setModalButtonText(props.t("Save"));
        modalRef.current.tog_backdrop();
    }

    const getActions = (item) => {
        const actions = (
            <div className="icon-container">
                <div title={props.t("Active or passive")} className="icon icon-lock" onClick={() => { activePassiveUser(item) }}></div>
                <div title={props.t("Delete")} className="icon icon-delete" onClick={() => { deleteUser(item) }}></div>
                <div title={props.t("Send a new password")} className="icon icon-resetpassword" onClick={() => { resetPasswordUser(item) }}></div>
            </div>);
        return actions;
    };

    const data = {
        columns: [
            {
                label: props.t("e-Mail"),
                field: "email",
                sort: "asc",
                width: 150
            },
            {
                label: props.t("State"),
                field: "isActive",
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
        rows: table
    }

    const { data: usersData, error, isLoading } = useSystemAdminListGetQuery();

    useEffect(() => {
        dispatch(startloading());
        if (!isLoading && !error && usersData) {
            const updatedUsersData = usersData.map(item => {
                return {
                    ...item,
                    isActive: item.isActive ? props.t("Active") : props.t("Passive"),
                    actions: getActions(item)
                };
            });
            setTable(updatedUsersData);

            dispatch(endloading());
        }
    }, [usersData, error, isLoading]);

    const [systemAdminActivePassive] = useSystemAdminActivePassiveMutation();

    const activePassiveUser = (item) => {
        Swal.fire({
            title: props.t("User active/passive status will be changed."),
            text: props.t("Do you confirm?"),
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3bbfad",
            confirmButtonText: props.t("Yes"),
            cancelButtonText: props.t("Cancel"),
            closeOnConfirm: false
        }).then(async (result) => {
            if (result.isConfirmed) {
                try {
                    dispatch(startloading());
                    const response = await systemAdminActivePassive({
                        id: item.id,
                        userId: userInformation.userId,
                        email: item.email
                    });
                    if (response.data.isSuccess) {
                        dispatch(endloading());
                        Swal.fire({
                            title: "",
                            text: props.t(response.data.message),
                            icon: "success",
                            confirmButtonText: props.t("Ok"),
                        });
                    } else {
                        dispatch(endloading());
                        Swal.fire({
                            title: "",
                            text: response.data.message,
                            icon: "error",
                            confirmButtonText: props.t("OK"),
                        });
                    }
                } catch (error) {
                    dispatch(endloading());
                    Swal.fire({
                        title: "",
                        text: props.t("An error occurred while processing your request."),
                        icon: "error",
                        confirmButtonText: props.t("OK"),
                    });
                }
            }
        });
    }

    const [systemAdminResetPassword] = useSystemAdminResetPasswordMutation(); 

    const resetPasswordUser = async (item) => {
        try {
            dispatch(startloading());
            if (!item.isActive) {
                dispatch(endloading());
                setMessage(props.t("Please activate the account first and then try this process again."));
                setStateToast(false);
                setShowToast(true);
                return;
            }
            const response = await systemAdminResetPassword({
                id: item.id,
                userId: userInformation.userId,
                email: item.email,
                language: props.i18n.language
            });
            if (response.data.isSuccess) {
                dispatch(endloading());
                setMessage(response.data.message)
                setStateToast(true);
                setShowToast(true);
            } else {
                dispatch(endloading());
                setMessage(response.data.message)
                setStateToast(false);
                setShowToast(true);
            }
        } catch (error) {
            dispatch(endloading());
            setMessage(props.t("An error occurred while processing your request."))
            setStateToast(true);
            setShowToast(true);
        }
    }

    const [systemAdminDelete] = useSystemAdminDeleteMutation();

    const deleteUser = (item) => {
        Swal.fire({
            title: props.t("You will not be able to recover this user!"),
            text: props.t("Do you confirm?"),
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3bbfad",
            confirmButtonText: props.t("Yes"),
            cancelButtonText: props.t("Cancel"),
            closeOnConfirm: false
        }).then(async (result) => {
            if (result.isConfirmed) {
                try {
                    dispatch(startloading());
                    const response = await systemAdminDelete({
                        id: item.id,
                        userId: userInformation.userId,
                        email: item.email
                    });
                    if (response.data.isSuccess) {
                        dispatch(endloading());
                        Swal.fire({
                            title: "",
                            text: props.t(response.data.message),
                            icon: "success",
                            confirmButtonText: props.t("Ok"),
                        });
                    } else {
                        dispatch(endloading());
                        Swal.fire({
                            title: "",
                            text: response.data.message,
                            icon: "error",
                            confirmButtonText: props.t("OK"),
                        });
                    }
                } catch (error) {
                    dispatch(endloading());
                    Swal.fire({
                        title: "",
                        text: props.t("An error occurred while processing your request."),
                        icon: "error",
                        confirmButtonText: props.t("OK"),
                    });
                }
            }
        });
    }

    return (
        <>
            <div className="page-content">
                <div className="container-fluid">
                    <div className="page-title-box">
                        <Row className="align-items-center" style={{ borderBottom: "1px solid black", paddingBottom: "5px" }}>
                            <Col md={8}>
                                <h6 className="page-title">{props.t("Add system admin")}</h6>
                            </Col>
                            <Col md="4">
                                <div className="float-end d-none d-md-block" style={{ marginLeft: "10px" }}>
                                    <Button
                                        color="success"
                                        className="btn btn-success waves-effect waves-light"
                                        type="button"
                                        onClick={addSystemAdmin}
                                    >
                                        <FontAwesomeIcon icon="fa-solid fa-plus" /> {props.t("Add a system admin")}
                                    </Button>
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
                                        noRecordsFoundLabel={props.t("No matching records found")}
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
            <ModalComp
                refs={modalRef}
                title={modalTitle}
                body={<AddOrUpdateSystemAdmin id={systemAdminId} userId={userInformation.userId} refs={modalContentRef} toast={toastHandle} />}
                buttonText={modalButtonText}
                isButton={true}
                size="md"
            />
            <ToastComp
                title="İşlem bilgisi"
                message={message}
                showToast={showToast}
                setShowToast={setShowToast}
                stateToast={stateToast}
            />
        </>
    )
};

ListSystemAdmin.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(ListSystemAdmin);