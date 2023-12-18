import React, { useEffect } from "react";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const ToastComp = ({ message, showToast, stateToast, autohide = true }) => {

    useEffect(() => {
        if (showToast) {
            if (stateToast) {
                toast.success(message, {
                    autoClose: autohide
                });
            } else {
                toast.error(message, {
                    autoClose: autohide
                });
            }
        }
    }, [showToast, stateToast, autohide]);


    return (
        <ToastContainer />
    );
}

export default ToastComp;