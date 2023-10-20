import React from "react";
import {
    Modal,
    ModalBody,
    ModalHeader,
    ModalFooter,
    Button,
} from "reactstrap";
import { withTranslation } from "react-i18next";
import PropTypes from 'prop-types';


const ModalComp = ({ title, body, modal_backdrop, tog_backdrop, handle, buttonText, t }) => {
    return (
        <Modal isOpen={modal_backdrop} toggle={tog_backdrop} id="staticBackdrop" backdrop={false} size="xl">
            <ModalHeader className="mt-0" toggle={tog_backdrop}>{ title }</ModalHeader>
            <ModalBody>
                { body }
            </ModalBody>
            <ModalFooter>
                <Button color="light" onClick={tog_backdrop}>
                    {t("Close")}
                </Button>{' '}
                <Button color="primary" onClick={handle}>
                    {buttonText}
                </Button>
            </ModalFooter>
        </Modal>
    );
}

ModalComp.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(ModalComp);