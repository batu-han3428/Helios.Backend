﻿import React, { useState, useEffect, useImperativeHandle } from 'react';
import { Modal, ModalHeader, ModalBody, ModalFooter, Button } from 'reactstrap';
import { withTranslation } from 'react-i18next';
import PropTypes from 'prop-types';

const ModalComp = ({ title, body, resetValue, handle, buttonText, t, refs }) => {

    const [modal_backdrop, setmodal_backdrop] = useState(false);

    useEffect(() => {
        if (!modal_backdrop)
            resetValue();
    }, [modal_backdrop]);

    const removeBodyCss = () => {
        document.body.classList.add('no_padding');
    };

    const tog_backdrop = () => {
        setmodal_backdrop(!modal_backdrop);
        removeBodyCss();
    };
   
    useImperativeHandle(refs, () => ({
        tog_backdrop: tog_backdrop,
    }), [tog_backdrop]);

    return (
        <Modal isOpen={modal_backdrop} toggle={tog_backdrop} backdrop={false} size="xl" ref={refs}>
            <ModalHeader className="mt-0" toggle={tog_backdrop}>
                {title}
            </ModalHeader>
            <ModalBody>{body}</ModalBody>
            <ModalFooter>
                <Button color="light" onClick={tog_backdrop}>
                    {t('Close')}
                </Button>{' '}
                <Button color="primary" onClick={handle}>
                    {buttonText}
                </Button>
            </ModalFooter>
        </Modal>
    );
};

ModalComp.propTypes = {
    t: PropTypes.any,
};

export default withTranslation()(ModalComp);