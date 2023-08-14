import React, { Component, useState, useContext } from 'react';
import {Container} from "rsuite";
import Sidebar from '../../Layouts/Sidebar/sidebar'

export default class home extends Component {

    render() {
        return (
            <section>
                <Sidebar></Sidebar>
                <Container>
                    <button className="btn btn-primary">
                        <small>Add Tenant</small>
                    </button>
                </Container>
            </section>

        );
    }
}

