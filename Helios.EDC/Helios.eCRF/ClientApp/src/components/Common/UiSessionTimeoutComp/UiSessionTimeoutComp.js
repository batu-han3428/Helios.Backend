import { onLogout } from '../../../helpers/Auth/useAuth';
import { useNavigate } from 'react-router-dom';
import useIdle from './useIdleTimer';

const UiSessionTimeoutComp = () => {

    const navigate = useNavigate();

    const function1 = () => {
        onLogout(navigate, true);
    };

    const { isIdle } = useIdle({ onIdle: function1, idleTime: 20 });

    return null;
};

export default UiSessionTimeoutComp;