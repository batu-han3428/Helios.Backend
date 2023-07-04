import React from 'react';
import { LanguageProvider } from './LanguageContext';
import App from './App';

const RootComponent = () => {
    return (
        <LanguageProvider>
            <App />
        </LanguageProvider>
    );
};

export default RootComponent;
