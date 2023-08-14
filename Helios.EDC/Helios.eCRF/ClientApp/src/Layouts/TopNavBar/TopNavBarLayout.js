import React, { Component } from 'react';
import { Container } from 'reactstrap';
import TopNavBarElement from './TopNavBarElement';

export default class TopNavBarLayout extends Component {
    static displayName = TopNavBarLayout.name;

    render() {
        return (
            <div>
                <TopNavBarElement />
                <Container tag="main">
                    {this.props.children}
                </Container>
            </div>
        );
    }
}
