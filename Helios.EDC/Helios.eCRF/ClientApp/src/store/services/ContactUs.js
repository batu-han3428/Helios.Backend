import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'

export const ContactUsApi = createApi({
    reducerPath: 'contactUsApi',
    baseQuery: fetchBaseQuery({ baseUrl: 'https://localhost:5201/' }),
    endpoints: (builder) => ({
        contactUsPost: builder.mutation({
            query: (data) => ({
                url: '/Account/ContactUs',
                method: 'POST',
                body: data,
            }),
        }),
    }),
});


export const { useContactUsPostMutation } = ContactUsApi;
