import { PopupMenu } from "react-simple-widgets";

const ProfileMenu = () => {
    return (
        <div id="app">
            <div className="text-end">
                <PopupMenu>
                    <div id="circle-avatar" className="text-center mx-auto mb-4">
                        <span>Z</span>
                    </div>
                    <div className="card text-start">
                        <div className="card-body px-4 py-4">

                            <h5 className="text-center mb-0">John Doe</h5>
                            <p className="text-center mb-2">jd@gmail.com</p>
                            <hr />
                            <p className="mb-0" style={{ color: "#bebebe", fontWeight: "bold", fontSize: 12 }}>
                                ROLES
                            </p>
                            <p style={{ fontSize: 12 }}> {["Submitter"].join(", ")}
                            </p>
                            <hr className="mb-0" style={{ margin: "0 -24px 0" }} />
                            <div className="list-group list-group-flush" style={{ margin: "0 -24px 0" }}>
                                <button className="list-group-item list-group-item-action px-4">
                                    <small>Change Requests</small>
                                </button>
                                <button className="list-group-item list-group-item-action px-4">
                                    <small>Pending Requests</small>
                                </button>
                                <button className="list-group-item list-group-item-action px-4">
                                    <small>Other Requests</small>
                                </button>
                            </div>
                            <hr style={{ margin: "0 -24px 24px" }} />
                            <div className="d-grid">
                                <button className="btn btn-secondary">
                                    <small>Logout</small>
                                </button>
                            </div>
                        </div>
                    </div>
                </PopupMenu>
            </div>
        </div>
    );
};

export default ProfileMenu;