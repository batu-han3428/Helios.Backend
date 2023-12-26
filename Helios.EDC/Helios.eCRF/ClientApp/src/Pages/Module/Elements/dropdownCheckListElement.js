import React, { Component } from 'react';
import Select from "react-select";

class DropdownCheckListElement extends Component {
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
                <Select
                    options={this.state.ElementOptions}
                    classNamePrefix="select2-selection"
                    isMulti={true}
                    closeMenuOnSelect={false}
                    isDisabled={this.state.isDisable}
                />
            </div>
        )
    }
};
export default DropdownCheckListElement;
