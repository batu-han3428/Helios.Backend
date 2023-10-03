import React, { useState, useEffect } from "react";
import {
    Toast, ToastHeader, ToastBody
} from "reactstrap";
import "./ToastComp.css";


const ToastComp = ({ title, message, showToast, setShowToast, stateToast }) => {
    useEffect(() => {
        if (showToast) {
            const timer = setTimeout(() => {
                setShowToast(false);
            }, 10000);

            return () => clearTimeout(timer);
        }
    }, [showToast, setShowToast]);


    return (
        <Toast isOpen={showToast } className={stateToast? "tost success" : "tost error" }>
            <ToastHeader>
                {title}
            </ToastHeader>
            <ToastBody>
                {message}
            </ToastBody>
        </Toast>
    );
}

export default ToastComp;