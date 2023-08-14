import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
//import "./index.css";
//import { ProSidebarProvider } from "react-pro-sidebar";

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');
const root = createRoot(rootElement);
//const root = ReactDOM.createRoot(document.getElementById("root"));

root.render(
    <BrowserRouter basename={baseUrl}>
      <App />
    </BrowserRouter>);
    //<React.StrictMode>
    //    <ProSidebarProvider>
    //        <App />
    //    </ProSidebarProvider>
    //</React.StrictMode>
//);