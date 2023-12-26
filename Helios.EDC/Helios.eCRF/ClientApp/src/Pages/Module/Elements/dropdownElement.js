import React, { Component } from 'react';
import Select from "react-select";

class DropdownElement extends Component {
    constructor(props) {
        super(props);
        
        this.state = {
            isDisable: props.IsDisable,
            FieldWidths: "mb-3 col-md-" + props.FieldWidths,
            ElementOptions: JSON.parse(props.ElementOptions),
        }
    }

    render() {
        return (
            <div className={this.state.FieldWidths}>
                <Select
                    classNamePrefix="select2-selection"
                    options={this.state.ElementOptions}
                    isDisabled={this.state.isDisable}
                />
            </div>
        )
    }
};
export default DropdownElement;
