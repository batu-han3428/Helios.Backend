import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'
import { getLocalStorage } from '../../../../helpers/local-storage/localStorageProcess';

export const SystemUsersApi = createApi({
    reducerPath: 'systemUsersApi',
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
        userListGet: builder.query({
            query: () => `/User/GetTenantAndSystemAdminUserList`,
            providesTags: ['User'],
        }),
        //userGet: builder.query({
        //    query: (data) => `/User/GetStudyUser/${data.email}/${data.studyId}`,
        //    refetchOnMountOrArgChange: true,
        //    keepUnusedDataFor: 0,
        //}),
        userSet: builder.mutation({
            query: (data) => ({
                url: '/User/SetSystemAdminAndTenantAdminUser',
                method: 'POST',
                body: data,
            }),
            invalidatesTags: ['User'],
        }),
        //userActivePassive: builder.mutation({
        //    query: (data) => ({
        //        url: '/User/ActivePassiveStudyUser',
        //        method: 'POST',
        //        body: data,
        //    }),
        //    invalidatesTags: ['User'],
        //}),
        //usersActivePassive: builder.mutation({
        //    query: (data) => ({
        //        url: '/User/ActivePassiveStudyUsers',
        //        method: 'POST',
        //        body: data,
        //    }),
        //    invalidatesTags: ['User'],
        //}),
        //userDelete: builder.mutation({
        //    query: (data) => ({
        //        url: '/User/DeleteStudyUser',
        //        method: 'POST',
        //        body: data,
        //    }),
        //    invalidatesTags: ['User'],
        //}),
        //userResetPassword: builder.mutation({
        //    query: (data) => ({
        //        url: '/User/UserResetPassword',
        //        method: 'POST',
        //        body: data,
        //    }),
        //    invalidatesTags: ['User'],
        //}),
    }),
});


export const { useUserListGetQuery } = SystemUsersApi;

//export const { useUserGetQuery } = UsersApi;

export const { useUserSetMutation } = SystemUsersApi;

//export const { useUserActivePassiveMutation } = UsersApi;

//export const { useUsersActivePassiveMutation } = UsersApi;

//export const { useUserDeleteMutation } = UsersApi;

//export const { useUserResetPasswordMutation } = UsersApi;