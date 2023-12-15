import React, { useEffect } from "react";
import { Toast, ToastHeader, ToastBody } from "reactstrap";
import "./ToastComp.css";

const ToastComp = ({ title, message, showToast, setShowToast, stateToast, autohide = true }) => {

    const handleClose = () => {
        setShowToast(false);
    };

    useEffect(() => {
        if (showToast && autohide) {
            const timer = setTimeout(() => {
                setShowToast(false);
            }, 5000);

            return () => clearTimeout(timer);
        }
    }, [showToast, setShowToast, autohide]);


    return (
        <Toast isOpen={showToast} className={stateToast ? "tost success" : "tost error"} onClose={handleClose}>
            {!autohide &&
                <ToastHeader toggle={handleClose}>
                {/*  {title}*/}
                </ToastHeader>
            }   
            <ToastBody>
                {message}
            </ToastBody>
        </Toast>
    );
}

export default ToastComp;