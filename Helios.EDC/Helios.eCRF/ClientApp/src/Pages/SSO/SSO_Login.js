import PropTypes from 'prop-types';
import React, { useState, useEffect } from "react";
import { useNavigate, Link } from "react-router-dom";

import { Row, Col, CardBody, Card, Container, Label, Form, Input } from "reactstrap";

// Redux
import { connect, useDispatch } from "react-redux";
import withRouter from '../../components/Common/withRouter';

// actions
import { apiError, loginuser } from "../../store/actions";
import { startloading, endloading } from '../../store/loader/actions';

// import images
import logoSm from "../../assets/images/logo-sm.png";

import { useLoginPostMutation } from '../../store/services/Login';
import { onLogin } from '../../helpers/Auth/useAuth';
import { setLocalStorage, getLocalStorage } from '../../helpers/local-storage/localStorageProcess';
import "./sso.css";

const SSO_Login = props => {

    const [loading, setLoading] = useState(true);

    const navigate = useNavigate();

    useEffect(() => {
        const storedUser = getLocalStorage("accessToken");
        if (storedUser) {
            navigate("/");
        }
        setLoading(false);
    }, [navigate])

    const dispatch = useDispatch();
    const [loginPost, { isLoading }] = useLoginPostMutation();
    const [formData, setFormData] = useState({ Email: '', Password: '' });
    const [login, setLogin] = useState(false);

    const handleSubmit = async (e) => {
        try {
            dispatch(startloading());

            const response = await loginPost(formData);

            if (response.data.isSuccess) {
                setLocalStorage("accessToken", response.data.values.accessToken);
                let result = onLogin();

                dispatch(endloading())
                if (result === false) {
                    setLogin(false);
                } else {
                    dispatch(loginuser(result));
                    setLogin(true);
                    navigate("/SSO-tenants");
                }
            } else {

            }
        } catch (error) {
            dispatch(endloading());
        }
    };

    document.title = "Login | Veltrix - React Admin & Dashboard Template";
    return (
        loading
        ||
        <>
            <div className="account-pages my-5 pt-sm-5">
                <Container>
                    <Row className="justify-content-center">
                        <Col md={8} lg={6} xl={4}>
                            <Card className="overflow-hidden">
                                <div className="bg-primary">
                                    <div className="text-primary text-center p-4">
                                        <h5 className="text-white font-size-20">
                                            SSO - Login
                                        </h5>
                                        <Link to="/" className="logo logo-admin">
                                            <img src={logoSm} height="24" alt="logo" />
                                        </Link>
                                    </div>
                                </div>

                                <CardBody className="p-4">
                                    <div className="p-3">
                          
                                  
                                            <Form className="mt-4"
                                                onSubmit={(e) => {
                                                    e.preventDefault();
                                                    handleSubmit();
                                                    return false;
                                                }}
                                                action="#">

                                                <div className="mb-3">
                                                    <Label className="form-label" htmlFor="username">Username</Label>
                                                    <Input
                                                        name="email"
                                                        className="form-control"
                                                        placeholder="Enter Username"
                                                        type="email"
                                                        id="username"
                                                        onChange={(e) => setFormData({ ...formData, Email: e.target.value })}
                                                    />
                                                </div>

                                                <div className="mb-3">
                                                    <Label className="form-label" htmlFor="userpassword">Password</Label>
                                                    <Input
                                                        name="password"
                                                        type="password"
                                                        className="form-control"
                                                        placeholder="Enter Password"
                                                        onChange={(e) => setFormData({ ...formData, Password: e.target.value })}
                                                    />
                                                </div>

                                                <div className="mb-3 row">
                                                    <div className="col-sm-6">
                                                        <div className="form-check">
                                                            <input type="checkbox" className="form-check-input" id="customControlInline" />
                                                            <label className="form-check-label" htmlFor="customControlInline">Remember me</label>
                                                        </div>
                                                    </div>
                                                    <div className="col-sm-6 text-end">
                                                        <button className="btn btn-primary w-md waves-effect waves-light" type="submit">Log In</button>
                                                    </div>
                                                </div>

                                                <div className="mt-2 mb-0 row">
                                                    <div className="col-12 mt-4">
                                                        <Link to="/forgot-password"><i className="mdi mdi-lock"></i> Forgot your password?</Link>
                                                    </div>
                                                </div>

                                            </Form>

                                    </div>
                                </CardBody>
                            </Card>



                            <div className="mt-5 text-center">
                                <p>
                                    Don&#39;t have an account ?{" "}
                                    <Link
                                        to="/register"
                                        className="fw-medium text-primary"
                                    >
                                        {" "}
                                        Signup now{" "}
                                    </Link>{" "}
                                </p>
                                <p>
                                    © {new Date().getFullYear()} Veltrix. Crafted with{" "}
                                    <i className="mdi mdi-heart text-danger" /> by Themesbrand
                                </p>
                            </div>
                        </Col>
                    </Row>
                </Container>
            </div>
        </>
    );
};

const mapStateToProps = state => {
    const { error } = state.rootReducer.Login;
    return { error };
};

export default withRouter(
    connect(mapStateToProps, { loginuser, apiError })(SSO_Login)
);

SSO_Login.propTypes = {
    error: PropTypes.any,
    history: PropTypes.object,
    loginuser: PropTypes.func,
};