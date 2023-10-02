import { rootReducer } from "./reducers"


import { configureStore } from '@reduxjs/toolkit'
import { setupListeners } from '@reduxjs/toolkit/query'
import { LoginApi } from './services/Login'
import { StudyApi } from './services/Study'

export const store = configureStore({
    reducer: {
        rootReducer,
        [LoginApi.reducerPath]: LoginApi.reducer,
        [StudyApi.reducerPath]: StudyApi.reducer
    },
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware().concat(LoginApi.middleware).concat(StudyApi.middleware),
})

setupListeners(store.dispatch)