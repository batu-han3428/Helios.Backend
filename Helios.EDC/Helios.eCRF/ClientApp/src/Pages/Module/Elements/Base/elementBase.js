import React, { useState, useEffect, useContext } from 'react';

function ElementBase(props) {
    const [Id, setId] = useState('');
    //const [TenantId, setTenantId] = useState('');

    const [ModuleId, setModuleId] = useState('');
    const [ElementDetailId, setElementDetailId] = useState('');
    const [ElementType, setElementType] = useState('');
    const [ElementName, setElementName] = useState('');
    const [Title, setTitle] = useState('');
    const [IsTitleHidden, setIsTitleHidden] = useState(true);
    const [Order, setOrder] = useState(0);
    const [Description, setDescription] = useState('');
    const [Width, setWidth] = useState(12);
    const [IsHidden, setIsHidden] = useState(true);
    const [IsRequired, setIsRequired] = useState(false);
    const [IsDependent, setIsDependent] = useState(false);
    const [IsReadonly, setIsReadonly] = useState(false);
    const [CanMissing, setCanMissing] = useState(true);

    const handleIdChange = (e) => {
        setId(e.target.value);
    };

    const handleModuleIdChange = (e) => {
        setModuleId(e.target.value);
    };

    const handleElementDetailIdChange = (e) => {
        setElementDetailId(e.target.value);
    };

    const handleElementTypeChange = (e) => {
        setElementType(e.target.value);
    };

    const handleElementNameChange = (e) => {
        setElementName(e.target.value);
    };

    const handleTitleChange = (e) => {
        setTitle(e.target.value);
    };

    const handleIsTitleHiddenChange = (e) => {
        setIsTitleHidden(e.target.value);
    };

    const handleOrderChange = (e) => {
        setOrder(e.target.value);
    };

    const handleDescriptionChange = (e) => {
        setDescription(e.target.value);
    };

    const handleWidthChange = (e) => {
        setWidth(e.target.value);
    };

    const handleIsHiddenChange = (e) => {
        setIsHidden(e.target.value);
    };

    const handleIsRequiredChange = (e) => {
        setIsRequired(e.target.value);
    };

    const handleIsDependentChange = (e) => {
        setIsDependent(e.target.value);
    };

    const handleIsReadonlyChange = (e) => {
        setIsReadonly(e.target.value);
    };

    const handleCanMissingChange = (e) => {
        setCanMissing(e.target.value);
    };

    return (
        <div></div>
    ) ;
}

export default ElementBase;
