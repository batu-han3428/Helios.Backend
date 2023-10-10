import { rootReducer } from "./reducers"
import { configureStore } from '@reduxjs/toolkit'
import { setupListeners } from '@reduxjs/toolkit/query'
import { LoginApi } from './services/Login'
import { StudyApi } from './services/Study'
import { SiteLaboratoriesApi } from './services/SiteLaboratories'
import { ContactUsApi } from './services/ContactUs'

export const store = configureStore({
    reducer: {
        rootReducer,
        [LoginApi.reducerPath]: LoginApi.reducer,
        [StudyApi.reducerPath]: StudyApi.reducer,
        [SiteLaboratoriesApi.reducerPath]: SiteLaboratoriesApi.reducer,
        [ContactUsApi.reducerPath]: ContactUsApi.reducer
    },
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware().concat(LoginApi.middleware).concat(StudyApi.middleware).concat(SiteLaboratoriesApi.middleware).concat(ContactUsApi.middleware)
})

setupListeners(store.dispatch);