import React, { Component } from 'react';
import {
    Input,
    Label
} from "reactstrap";

class RadioElement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isDisable: props.IsDisable,
            FieldWidths: props.FieldWidths,
            ElementOptions: JSON.parse(props.ElementOptions),
        }
    }

    render() {
        return (
            <div className="mb-3">
                {this.state.ElementOptions.map((item, index) =>
                    <div className="form-check form-check-inline" key={index}>
                        <Input
                            type="radio"
                            className="form-check-input"
                            disabled={this.state.isDisable}
                        />
                        <Label
                            className="form-check-label"
                        >
                            {item.tagName}
                        </Label>
                    </div>
                )}
            </div>
        )
    }
};
export default RadioElement;
