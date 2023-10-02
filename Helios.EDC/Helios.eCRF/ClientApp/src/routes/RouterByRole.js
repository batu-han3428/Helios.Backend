import React, { useEffect } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";

const RouterByRole = () => {
    const navigate = useNavigate();

    const { roles } = useSelector((state) => ({
        roles: state.rootReducer.Login.roles,
    }));

    useEffect(() => {
        if (roles.includes("TenantAdmin")) {
            navigate("/study"); 
        } else {
            navigate("/other");
        }
    }, [roles, navigate]);

    return <></>;
}

export default RouterByRole;