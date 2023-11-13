import React, { useState, useEffect } from 'react';
import { onLogout } from '../../../helpers/Auth/useAuth';
import { useNavigate } from 'react-router-dom';

const UiSessionTimeoutComp = () => {
    const [timerAlert, setTimerAlert] = useState(null);
    const [seconds, setSeconds] = useState(0);

    const navigate = useNavigate();

    const tick = () => {
        const intervalId = setInterval(() => {
            setSeconds(prevSeconds => prevSeconds + 1);
        }, 1000);

        return () => clearInterval(intervalId);
    };

    const hideAlert = () => {
        window.location = "/login";
    };

    const confirmAlert = () => {
        setTimerAlert(null);
    };

    const function1 = () => {
        onLogout(navigate, true);
    };

    const function2 = () => {
        tick();
        const nextmsg = () => (
            <></>
        );
        setTimerAlert(nextmsg());
    };

    const resetTimer = () => {
        setSeconds(0);
    };

    useEffect(() => {
        const intervalId = setInterval(() => {
            if (seconds >= 1200) {
                function1();
            }
        }, 1000);

        return () => {
            clearInterval(intervalId);
        };
    }, [seconds]);

    useEffect(() => {
        const activityEvents = ['mousemove', 'keydown', 'touchstart'];

        const handleActivity = () => {
            resetTimer();
        };

        activityEvents.forEach(event => {
            document.addEventListener(event, handleActivity);
        });

        return () => {
            activityEvents.forEach(event => {
                document.removeEventListener(event, handleActivity);
            });
        };
    }, []);

    useEffect(() => {
        function2();

        return () => {
            resetTimer();
            clearInterval(tick());
        };
    }, []);

    return null;
};

export default UiSessionTimeoutComp;