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
    //        const result = onLogin(); // Burada onLogin fonksiyonunuzu çaðýrarak kullanýcý giriþini kontrol edin.
    //        if (result === false) {
    //            // Kullanýcý giriþi baþarýsýzsa localStorage'dan accessToken'ý kaldýrýn ve login sayfasýna yönlendirin.
    //            removeLocalStorage("accessToken");
    //            return <Navigate to={{ pathname: "/login", state: { from: props.location } }} />;
    //        } else {
    //            // Kullanýcý giriþi baþarýlýysa redux store'a kullanýcý bilgilerini ekleyin.
    //            dispatch(loginuser(result));
    //            return null; // Yönlendirme yapýlacaksa, null döndürün.
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
        const result = onLogin(); // Burada onLogin fonksiyonunuzu çaðýrarak kullanýcý giriþini kontrol edin.
        if (result === false) {
            // Kullanýcý giriþi baþarýsýzsa localStorage'dan accessToken'ý kaldýrýn ve login sayfasýna yönlendirin.
            removeLocalStorage("accessToken");
            return <Navigate to={{ pathname: "/login", state: { from: props.location } }} />;
        } else {
            // Kullanýcý giriþi baþarýlýysa redux store'a kullanýcý bilgilerini ekleyin.
            dispatch(loginuser(result));
    /*        return null; // Yönlendirme yapýlacaksa, null döndürün.*/
            
            if (roles && !roles.some(role => result.roles.includes(role))) {
                // Kullanýcýnýn rolü yetki gerektiren sayfayý açmak için yeterli deðilse unauthorized sayfasýna yönlendir
                return <Navigate to="/unauthorized" />;
            }

        }
    }

    const Layout = layoutType === layoutTypes.VERTICAL ? VerticalLayout : VerticalLayout; // HorizontalLayout kullanacaksanýz burayý güncelleyin.

    return <Layout>{Element}</Layout>;
};

export default AuthMiddleware;