import React, { useState, useEffect } from "react";
import {
    Modal,
    ModalBody,
    ModalHeader,
    ModalFooter,
    Button,
} from "reactstrap";


const ModalComp = ({ title, body, modal_backdrop, tog_backdrop, handle, buttonText }) => {
    return (
        <Modal isOpen={modal_backdrop} toggle={tog_backdrop} id="staticBackdrop" backdrop={false} size="xl">
            <ModalHeader className="mt-0" toggle={tog_backdrop}>{ title }</ModalHeader>
            <ModalBody>
                { body }
            </ModalBody>
            <ModalFooter>
                <Button color="light" onClick={tog_backdrop}>
                    Close
                </Button>{' '}
                <Button color="primary" onClick={handle}>
                    {buttonText}
                </Button>
            </ModalFooter>
        </Modal>
    );
}

export default ModalComp;