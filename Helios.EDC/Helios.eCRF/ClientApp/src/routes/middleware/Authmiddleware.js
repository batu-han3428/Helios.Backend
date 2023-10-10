import React, { useEffect, useCallback } from "react";
import { Navigate, useNavigate } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import VerticalLayout from "../../components/VerticalLayout";
import { getLocalStorage, removeLocalStorage } from '../../helpers/local-storage/localStorageProcess';
import { loginuser } from "../../store/actions";
import { layoutTypes } from "../../constants/layout";
import { onLogin } from "../../helpers/Auth/useAuth";
import { userRoutes } from "../allRoutes";

const AuthMiddleware = (props) => {
    const dispatch = useDispatch();
    const layoutType = useSelector(state => state.rootReducer.Layout.layoutType);
    const user = getLocalStorage("accessToken");
    const { path: Path, element: Element, roles } = props;
    let matchedRoute = null;
    const Layout = layoutType === layoutTypes.VERTICAL ? VerticalLayout : VerticalLayout;

    const pageType = userRoutes.find(route => route.path === Path)?.menuType ?? 'common';

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
            if (Path !== "/" && roles && !roles.some(role => result.roles.includes(role))) {
                return (
                    <Navigate to={{ pathname: "/AccessDenied", state: { from: props.location } }} />
                );
            }
            if (Path === "/") {
                const matchedRoute1 = userRoutes.find(route => route.roles && route.roles.some(role => result.roles.includes(role)));
                if (matchedRoute1) {
                    matchedRoute = matchedRoute1;
                }
            }
        }
    }


    return (
        <Layout pageType={pageType}>
            {Path !== "/" ? Element : <Navigate to={matchedRoute?.path || '/dashboard'} />}
        </Layout>
    );
};

export default AuthMiddleware;