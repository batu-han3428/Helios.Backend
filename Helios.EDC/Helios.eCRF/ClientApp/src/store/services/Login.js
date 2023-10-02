import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'

export const LoginApi = createApi({
    reducerPath: 'loginApi',
    baseQuery: fetchBaseQuery({ baseUrl: 'https://localhost:7196/' }),
    endpoints: (builder) => ({
        loginPost: builder.mutation({
            query: (data) => ({
                url: '/Account/Login',
                method: 'POST',
                body: data,
            }),
        }),
    }),
});


export const { useLoginPostMutation } = LoginApi;
