import React, { Component } from 'react';
import Select from "react-select";

class DropdownElement extends Component {
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
                <div className="col-md-10">
                    <Select
                        classNamePrefix="select2-selection"
                        options={this.state.ElementOptions}
                        isDisabled={this.state.isDisable}
                    />
                </div>
            </div>
        )
    }
};
export default DropdownElement;
