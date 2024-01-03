import React, { useEffect, useState } from "react";
import { Navigate, useParams } from "react-router-dom";
import { useDispatch } from "react-redux";
import VerticalLayout from "../../components/VerticalLayout";
import SSOLayout from "../../components/SSOLayout";
import { getLocalStorage, removeLocalStorage } from '../../helpers/local-storage/localStorageProcess';
import { addStudy, loginuser, resetStudy } from "../../store/actions";
import { layoutTypes } from "../../constants/layout";
import { onLogin } from "../../helpers/Auth/useAuth";
import { userRoutes } from "../allRoutes";


const AuthMiddleware = (props) => {
    const dispatch = useDispatch();
    const user = getLocalStorage("accessToken");
    const { path: Path, element: Element, roles } = props;
    let matchedRoute = null;
    let pageType = null;
    let Layout = null;
    const { studyId } = useParams();
    const [error, setError] = useState(false);

    const fetchData = async (token) => {
        
        const apiUrl = `https://localhost:5201/Study/GetStudy/${studyId}`;

        fetch(apiUrl, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            dispatch(addStudy(data));
        })
        .catch(error => {
            setError(true);
        });
    };

    useEffect(() => {
        if (pageType !== "study") {
            dispatch(resetStudy());
        }
    }, [pageType])

    var result = null;

    useEffect(() => {
        if (user) {
            dispatch(loginuser(result));
        }
    }, [dispatch, user]);
    if (!user) {
        return (
            <Navigate to={{ pathname: "/login", state: { from: props.location } }} />
        );
    }
    else {
        result = onLogin();

        if (result === false) {
            removeLocalStorage("accessToken");
            return (
                <Navigate to={{ pathname: "/login", state: { from: props.location } }} />
            );
        } else {
            pageType = userRoutes.find(route => route.roles && route.roles.some(role => result.roles.includes(role)) && route.path === Path)?.menuType ?? 'common';
            Layout = pageType === layoutTypes.SSO ? SSOLayout : VerticalLayout;
            if (Path !== "/" && roles && !roles.some(role => result.roles.includes(role))) {
                return (
                    <Navigate to={{ pathname: "/AccessDenied", state: { from: props.location } }} />
                );
            }
            if (Path === "/") {
                const matchedRoute1 = userRoutes.find(route => route.roles && route.roles.some(role => result.roles.includes(role)) && route.path === "/");
                if (matchedRoute1) {
                    matchedRoute = matchedRoute1.redirect;
                }
            }
            if (pageType === "study") {
                fetchData(result.token);
                if (error) {
                    return (
                        <Navigate to={{ pathname: "/AccessDenied", state: { from: props.location } }} />
                    );
                }
            }
        }
    }


    return (
        <Layout pageType={pageType}>
            {Path !== "/" ? Element : <Navigate to={matchedRoute || '/ContactUs'} />}
        </Layout>
    );
};

export default AuthMiddleware;