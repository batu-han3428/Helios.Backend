﻿import React, { useState, useEffect, useImperativeHandle, useRef } from 'react';
import { Modal, ModalHeader, ModalBody, ModalFooter, Button } from 'reactstrap';
import { withTranslation } from 'react-i18next';
import PropTypes from 'prop-types';

const ModalComp = ({ title, body, resetValue = null, handle, buttonText, t, refs, size = "xl" }) => {

    const [modal_backdrop, setmodal_backdrop] = useState(false);

    useEffect(() => {
        if (!modal_backdrop)
            if (resetValue !== null) resetValue();
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
        <Modal isOpen={modal_backdrop} toggle={tog_backdrop} backdrop={false} size={size} ref={refs}>
            <ModalHeader className="mt-0" toggle={tog_backdrop}>
                {title}
            </ModalHeader>
            <ModalBody>{React.cloneElement(body, { onToggleModal: tog_backdrop })}</ModalBody>
            <ModalFooter>
                <Button color="light" onClick={tog_backdrop}>
                    {t('Close')}
                </Button>{' '}
                <Button color="primary" onClick={handle ? handle : () => body.props.refs.current.submitForm()}>
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