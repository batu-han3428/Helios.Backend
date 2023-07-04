import React, { useEffect, useRef } from 'react';
import $ from 'jquery';
import 'rc-easyui/dist/themes/default/easyui.css';
import 'rc-easyui/dist/rc.easyui.min';

const EasyUIComponent = () => {
    const ref = useRef();

    useEffect(() => {
        $(ref.current).datagrid();
    }, []);

    return <table ref={ref}></table>;
};

export default EasyUIComponent;