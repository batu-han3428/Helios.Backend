import React, { useState, useEffect } from 'react';
import './module.css';
import { Routes, Route, useNavigate } from "react-router-dom";
import AddOrUpdateModule from './addOrUpdateModule';

function ModuleList() {
    const navigate = useNavigate();

    const navigateAddModule = () => {
        navigate('/addOrUpdateModule');
    };

    //useEffect(() => {
    fetch('/Module/GetModuleList', {
        method: 'GET',
    })
        .then(response => response.json())
        .then(data => {
            setModuleList(data);
        })
        .catch(error => {
            //console.error('Error:', error);
        });
    //});
    const [moduleList, setModuleList] = useState([]);

    const addModule = () => {
        navigate(`/addOrUpdateModule`);
    };

    return (
        <div style={({ height: "100vh" }, { display: "flex" })} >
            <div id="page-wrap" style={{ padding: "15px", width: '100%' }}>
                <div><h1>Modules</h1></div>
                <hr />
                <div className="floatr">
                    <button className="btn btn-primary" onClick={() => navigate("/addModule")}>
                        <small>Add Module</small>
                    </button>
                    {/*<Link to="./addModule" className="btn btn-primary">Sign up</Link>*/}
                    <button onClick={navigateAddModule}>Home</button>
                    {/*<Routes>*/}
                    {/*    <Route path="/addModule" element={<AddModule />} />*/}
                    {/*</Routes>*/}
                </div>
                <br />
                <br />
                <div>
                    {
                        moduleList.map((moduleList) => {
                            return (
                                <div className='module-box'>
                                    <div className="floatl">
                                        <h2>{moduleList.name}</h2>
                                    </div>
                                    <div className="floatr">
                                        <button className="btn" data-id={moduleList.id}>
                                            {/*<FontAwesomeIcon icon={faCog} />*/}
                                        </button>
                                        <br></br>
                                        <button className="btn" data-id={moduleList.id}>
                                            {/*<FontAwesomeIcon icon={faArrowRight} />*/}
                                        </button>
                                        <br></br>
                                        <button className="btn" data-id={moduleList.id}>
                                            {/*<FontAwesomeIcon icon={faEdit} />*/}
                                        </button>
                                    </div>
                                </div>
                            );
                        })
                    }

                </div>
            </div>
        </div>
    );
}

export default ModuleList;

