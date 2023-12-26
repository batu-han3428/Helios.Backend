import React, { Component } from 'react';

class TextElement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isDisable: props.IsDisable,
            FieldWidths: props.FieldWidths,
        }
    }

    render() {
        return(
        <input
            className="form-control"
            type="text"
            disabled={this.state.isDisable} />
        )}
};
export default TextElement;
