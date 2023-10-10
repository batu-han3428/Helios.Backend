import React, { useState, useEffect, useContext } from 'react';

class ElementBase extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
        }

        //this.setState({ Id: '' });
        ////this.setState({ TenantId: '' });

        //this.setState({ ModuleId: '' });
        //this.setState({ ElementDetailId: '' });
        //this.setState({ ElementType: '' });
        //this.setState({ ElementName: '' });
        //this.setState({ Title: '' });
        //this.setState({ IsTitleHidden: true });
        //this.setState({ Order: 0 });
        //this.setState({ Description: '' });
        //this.setState({ Width: 12 });
        //this.setState({ IsHidden: true });
        //this.setState({ IsRequired: false });
        //this.setState({ IsDependent: false });
        //this.setState({ IsReadonly: false });
        //this.setState({ CanMissing: true });

        ///* Dependency */
        //this.setState({ DependentSourceFieldId: '' });
        //this.setState({ DependentTargetFieldId: '' });
        //this.setState({ DependentCondition: 0 });
        //this.setState({ DependentAction: 0 });
        //this.setState({ DependentFieldValue: '' });

        ///* Validation */
        ////this.setState({ Message: '' });

        //this.setState({ Unit: '' });
        //this.setState({ LowerLimit: '' });
        //this.setState({ UpperLimit: '' });

        this.handleSaveModuleContent = this.handleSaveModuleContent.bind(this);


        //const handleIdChange = (e) => {
        //    this.setState({ Id: e.target.value });
        //};

        //const handleModuleIdChange = (e) => {
        //    this.setState({ ModuleId: e.target.value });
        //};

        //const handleElementDetailIdChange = (e) => {
        //    this.setState({ ElementDetailId: e.target.value });
        //};

        //const handleElementTypeChange = (e) => {
        //    this.setState({ ElementType: e.target.value });
        //};

        //const handleElementNameChange = (e) => {
        //    this.setState({ ElementName: e.target.value });
        //};

        //const handleTitleChange = (e) => {
        //    this.setState({ Title: e.target.value });
        //};

        //const handleIsTitleHiddenChange = (e) => {
        //    this.setState({ IsTitleHidden: e.target.value });
        //};

        //const handleOrderChange = (e) => {
        //    this.setState({ Order: e.target.value });
        //};

        //const handleDescriptionChange = (e) => {
        //    this.setState({ Description: e.target.value });
        //};

        //const handleWidthChange = (e) => {
        //    this.setState({ Width: e.target.value });
        //};

        //const handleIsHiddenChange = (e) => {
        //    this.setState({ IsHidden: e.target.value });
        //};

        //const handleIsRequiredChange = (e) => {
        //    this.setState({ IsRequired: e.target.value });
        //};

        //const handleIsDependentChange = (e) => {
        //    this.setState({ IsDependent: e.target.value });
        //};

        //const handleIsReadonlyChange = (e) => {
        //    this.setState({ IsReadonly: e.target.value });
        //};

        //const handleCanMissingChange = (e) => {
        //    this.setState({ CanMissing: e.target.value });
        //};

    }

    handleSaveModuleContent = (e) => {
        debugger;
        fetch('/Module/SaveModuleContent?Title=' + this.Title, {
            method: 'POST',
            //body: Name

        })
            .then(response => response.json())
            .then(data => {
                debugger;
                // Handle response from the controller
            })
            .catch(error => {
                //console.error('Error:', error);
            });
    };

    //return (
    //    <div></div>
    //) ;
}

export default ElementBase;
