import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'
import { getLocalStorage } from '../../helpers/local-storage/localStorageProcess';

export const PermissionsApi = createApi({
    reducerPath: 'permissionsApi',
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
        roleListGet: builder.query({
            query: (studyId) => `/User/GetPermissionRoleList/${studyId}`,
            providesTags: ['Role'],
        }),
        roleSave: builder.mutation({
            query: (data) => ({
                url: '/User/AddOrUpdatePermissionRol',
                method: 'POST',
                body: data,
            }),
            invalidatesTags: ['Role'],
        }),
        setPermission: builder.mutation({
            query: (data) => ({
                url: '/User/SetPermission',
                method: 'POST',
                body: data,
            }),
            invalidatesTags: ['Role'],
        }),
        roleDelete: builder.mutation({
            query: (data) => ({
                url: '/User/DeleteRole',
                method: 'POST',
                body: data,
            }),
            invalidatesTags: ['Role'],
        }),
    }),
});


export const { useRoleListGetQuery } = PermissionsApi;

export const { useRoleSaveMutation } = PermissionsApi;

export const { useSetPermissionMutation } = PermissionsApi;

export const { useRoleDeleteMutation } = PermissionsApi;