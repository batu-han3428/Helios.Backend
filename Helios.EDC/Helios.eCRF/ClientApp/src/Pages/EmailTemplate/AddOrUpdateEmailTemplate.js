import PropTypes from 'prop-types';
import React, { useState, useEffect, useRef } from "react";
import {
    Row, Col, Card, CardBody, CardHeader, FormFeedback, Label, Input, Form, Button, CardTitle, CardText, ListGroup, ListGroupItem
} from "reactstrap";
import { withTranslation } from "react-i18next";
import { useSelector, useDispatch } from 'react-redux';
import { startloading, endloading } from '../../store/loader/actions';
import ModalComp from '../../components/Common/ModalComp/ModalComp';
import ToastComp from '../../components/Common/ToastComp/ToastComp';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useNavigate, useParams } from "react-router-dom";
import TemplateTagList from './Comp/TemplateTagList';
import UserList from './Comp/UserList';
import TemplateForm from './Comp/TemplateForm';
import { useLazyEmailTemplateGetQuery } from '../../store/services/EmailTemplate';


const AddOrUpdateEmailTemplate = props => {

    const { studyId, templateId } = useParams();

    //const [table, setTable] = useState(null);

    //const [trigger, { data: mailTemplateData, isLoading, isError }] = useLazyEmailTemplateGetQuery();

    //useEffect(() => {
    //    if (templateId) {
    //        trigger(templateId);
    //    }
    //}, [templateId])

    //useEffect(() => {
    //    dispatch(startloading());
      
    //    if (mailTemplateData && !isLoading && !isError) {
    //        console.log('mailTemplateData', mailTemplateData)
    //        setTable(mailTemplateData);

    //        dispatch(endloading());

    //    } else if (isError && !isLoading) {
    //        dispatch(endloading());
    //    } else {
    //        dispatch(endloading());
    //    }
    //}, [mailTemplateData, isError, isLoading]);

    const modalRef = useRef();

    const userInformation = useSelector(state => state.rootReducer.Login);

    const dispatch = useDispatch();

    const navigate = useNavigate();

    const [templateType, setTemplateType] = useState();

    return (
        <React.Fragment>
            <div className="page-content">
                <div className="container-fluid">
                    <div className="page-title-box">
                        <Row className="align-items-center" style={{ borderBottom: "1px solid black" }}>
                            <Col md={8}>
                                <h6 className="page-title"><FontAwesomeIcon style={{ marginRight: "10px", cursor: "pointer", position: "relative", top: "0.5px" }} onClick={() => navigate(`/email-templates/${studyId}`)} icon="fa-solid fa-left-long" />{props.t("Study information")}</h6>
                            </Col>
                        </Row>
                    </div>
                    <Row>
                        <Col md={4} className="mb-4">
                            <Row>
                                <Col md={12}>
                                    <TemplateTagList userId={userInformation.userId} tenantId={userInformation.tenantId} templateType={templateType} />
                                </Col>
                            </Row>
                            <Row>
                                <Col md={12}>
                                    <UserList studyId={studyId} />
                                </Col>
                            </Row>
                        </Col>
                        <Col md={8}>
                            <TemplateForm templateId={templateId} /*data={table} */ studyId={studyId} userId={userInformation.userId} tenantId={userInformation.tenantId} setTemplateType={setTemplateType} />
                        </Col>
                    </Row>
                </div>
            </div>

            {/*<ToastComp*/}
            {/*    title="İşlem bilgisi"*/}
            {/*    message={message}*/}
            {/*    showToast={showToast}*/}
            {/*    setShowToast={setShowToast}*/}
            {/*    stateToast={stateToast}*/}
            {/*/>*/}
        </React.Fragment>
    );
};


AddOrUpdateEmailTemplate.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(AddOrUpdateEmailTemplate);