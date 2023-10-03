import React, { useEffect } from "react";
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
    const navigate = useNavigate();
    const layoutType = useSelector(state => state.rootReducer.Layout.layoutType);
    const user = getLocalStorage("accessToken");
    const { path: Path, element: Element, roles } = props;
    let matchedRoute = null;
    const Layout = layoutType === layoutTypes.VERTICAL ? VerticalLayout : VerticalLayout; // HorizontalLayout kullanacaksanýz burayý güncelleyin.

    useEffect(() => {
        const handleNavigation = (address) => {
            navigate(address);
        };

        if (!user) {
            handleNavigation('/login');
        }
        else {
            const result = onLogin();
            if (result === false) {
                removeLocalStorage("accessToken");
                handleNavigation('/login');
            } else {
                dispatch(loginuser(result));
                if (roles && !roles.some(role => result.roles.includes(role))) {
                    handleNavigation('/unauthorized');
                }
                if (Path === "/") {
                    const matchedRoute = userRoutes.find(route => route.roles && route.roles.some(role => result.roles.includes(role)));
                    if (matchedRoute) {
                        navigate(matchedRoute.path);
                    } else {
                        handleNavigation('/dashboard');
                    }
                }
            }
        }
    }, [user, roles, navigate]);


    return (
        <Layout>
            {Path !== "/" ? Element : <Navigate to={matchedRoute?.path || '/dashboard'} />}
        </Layout>
    );
};

export default AuthMiddleware;