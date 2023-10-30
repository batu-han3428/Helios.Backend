import { rootReducer } from "./reducers"
import { configureStore } from '@reduxjs/toolkit'
import { setupListeners } from '@reduxjs/toolkit/query'
import { LoginApi } from './services/Login'
import { StudyApi } from './services/Study'
import { SiteLaboratoriesApi } from './services/SiteLaboratories'
import { ContactUsApi } from './services/ContactUs'
import { PermissionsApi } from './services/Permissions'
import { UsersApi } from './services/Users'

export const store = configureStore({
    reducer: {
        rootReducer,
        [LoginApi.reducerPath]: LoginApi.reducer,
        [StudyApi.reducerPath]: StudyApi.reducer,
        [SiteLaboratoriesApi.reducerPath]: SiteLaboratoriesApi.reducer,
        [ContactUsApi.reducerPath]: ContactUsApi.reducer,
        [PermissionsApi.reducerPath]: PermissionsApi.reducer,
        [UsersApi.reducerPath]: UsersApi.reducer,
    },
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware().concat(LoginApi.middleware).concat(StudyApi.middleware).concat(SiteLaboratoriesApi.middleware).concat(ContactUsApi.middleware).concat(PermissionsApi.middleware).concat(UsersApi.middleware)
})

setupListeners(store.dispatch);