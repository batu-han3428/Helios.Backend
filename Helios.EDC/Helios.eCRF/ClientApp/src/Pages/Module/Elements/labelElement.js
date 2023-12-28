import React, { Component } from 'react';

class LabelElement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            Title: props.Title,
        }
    }

    render() {
        return (
            <div style={{ marginRight: "20px", backgroundColor: '#ffffff', textAlign: 'Left', border: '1px solid #ccc', borderRadius:'5px' }} >
                <div dangerouslySetInnerHTML={{ __html: this.state.Title }} />
            </div>
        )
    }
};
export default LabelElement;
