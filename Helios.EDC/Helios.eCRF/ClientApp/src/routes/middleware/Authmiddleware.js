import React, { useEffect } from "react";
import { Navigate, useNavigate } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import VerticalLayout from "../../components/VerticalLayout";
import { getLocalStorage, removeLocalStorage } from '../../helpers/local-storage/localStorageProcess';
import { loginuser } from "../../store/actions";
import { layoutTypes } from "../../constants/layout";
import { onLogin } from "../../helpers/Auth/useAuth";

const AuthMiddleware = (props) => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const layoutType = useSelector(state => state.rootReducer.Layout.layoutType);
    const user = getLocalStorage("accessToken");
    const { element: Element, roles } = props;
    //const performRedirect = () => {
    //    debugger;
    //    if (!user) {
    //        return <Navigate to={{ pathname: "/login", state: { from: props.location } }} />;
    //    } else {
    //        const result = onLogin(); // Burada onLogin fonksiyonunuzu �a��rarak kullan�c� giri�ini kontrol edin.
    //        if (result === false) {
    //            // Kullan�c� giri�i ba�ar�s�zsa localStorage'dan accessToken'� kald�r�n ve login sayfas�na y�nlendirin.
    //            removeLocalStorage("accessToken");
    //            return <Navigate to={{ pathname: "/login", state: { from: props.location } }} />;
    //        } else {
    //            // Kullan�c� giri�i ba�ar�l�ysa redux store'a kullan�c� bilgilerini ekleyin.
    //            dispatch(loginuser(result));
    //            return null; // Y�nlendirme yap�lacaksa, null d�nd�r�n.
    //        }
    //    }
    //};

    //useEffect(() => {
    //    const redirect = performRedirect();
    //    if (redirect) {
    //        navigate({ pathname: "/login", state: { from: props.location } });
    //    }
    //}, [dispatch, navigate, props.location, user]);

    if (!user) {
        return <Navigate to={{ pathname: "/login", state: { from: props.location } }} />;
    } else {
        const result = onLogin(); // Burada onLogin fonksiyonunuzu �a��rarak kullan�c� giri�ini kontrol edin.
        if (result === false) {
            // Kullan�c� giri�i ba�ar�s�zsa localStorage'dan accessToken'� kald�r�n ve login sayfas�na y�nlendirin.
            removeLocalStorage("accessToken");
            return <Navigate to={{ pathname: "/login", state: { from: props.location } }} />;
        } else {
            // Kullan�c� giri�i ba�ar�l�ysa redux store'a kullan�c� bilgilerini ekleyin.
            dispatch(loginuser(result));
    /*        return null; // Y�nlendirme yap�lacaksa, null d�nd�r�n.*/
            
            if (roles && !roles.some(role => result.roles.includes(role))) {
                // Kullan�c�n�n rol� yetki gerektiren sayfay� a�mak i�in yeterli de�ilse unauthorized sayfas�na y�nlendir
                return <Navigate to="/unauthorized" />;
            }

        }
    }

    const Layout = layoutType === layoutTypes.VERTICAL ? VerticalLayout : VerticalLayout; // HorizontalLayout kullanacaksan�z buray� g�ncelleyin.

    return <Layout>{Element}</Layout>;
};

export default AuthMiddleware;