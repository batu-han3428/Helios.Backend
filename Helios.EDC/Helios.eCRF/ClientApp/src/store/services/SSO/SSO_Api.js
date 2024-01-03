import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'
import { getLocalStorage } from '../../../helpers/local-storage/localStorageProcess';

export const SSOApi = createApi({
    reducerPath: 'SSOApi',
    baseQuery: fetchBaseQuery({
        baseUrl: 'https://localhost:5201/',
        prepareHeaders: (headers, { getState }) => {
            let token = getLocalStorage("accessToken");

            if (token) {
                headers.set('Authorization', `Bearer ${token}`);
            }

            return headers;
        },
    }),
    endpoints: (builder) => ({
        tenantListGet: builder.query({
            query: (userId) => `/User/GetUserTenantList/${userId}`,
        }),
        studiesListGet: builder.query({
            query: (data) => `/User/GetUserStudiesList/${data.tenantId}/${data.userId}`,
        }),
    }),
});


export const { useLazyTenantListGetQuery } = SSOApi;

export const { useLazyStudiesListGetQuery } = SSOApi;
