﻿import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'
import { getLocalStorage } from '../../helpers/local-storage/localStorageProcess';

export const TenantsApi = createApi({
    reducerPath: 'tenantsApi',
    baseQuery: fetchBaseQuery({
        baseUrl: 'https://localhost:7196/',
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
            query: () => `/User/GetTenantList`,
            providesTags: ['Tenants'],
        }),
        tenantListAuthGet: builder.query({
            query: () => `/User/GetAuthTenantList`,
        }),
        tenantGet: builder.query({
            query: (data) => `/User/GetTenant/${data.tenantId}`,
            refetchOnMountOrArgChange: true,
            keepUnusedDataFor: 0,
        }),
        tenantSet: builder.mutation({
            query: (data) => ({
                url: '/User/SetTenant',
                method: 'POST',
                body: data,
                formData: true,
            }),
            invalidatesTags: ['Tenants'],
        })
    }),
});


export const { useTenantListGetQuery } = TenantsApi;

export const { useTenantListAuthGetQuery } = TenantsApi;

export const { useLazyTenantGetQuery } = TenantsApi;

export const { useTenantSetMutation } = TenantsApi;
