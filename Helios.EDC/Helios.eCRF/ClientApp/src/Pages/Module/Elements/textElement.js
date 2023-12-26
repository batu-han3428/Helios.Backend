import React, { Component } from 'react';

class TextElement extends Component {
    constructor(props) {
        super(props);
        
        this.state = {
            isDisable: props.IsDisable,
            FieldWidths: "col-md-" + props.FieldWidths,
        }
    }

    render() {
        return (
            <div className={this.state.FieldWidths} >
                <input
                    className="form-control"
                    type="text"
                    disabled={this.state.isDisable} />
            </div>
        )
    }
};
export default TextElement;
