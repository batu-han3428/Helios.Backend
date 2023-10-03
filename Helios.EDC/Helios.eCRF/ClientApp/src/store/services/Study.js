import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'
import { getLocalStorage } from '../../helpers/local-storage/localStorageProcess';


export const StudyApi = createApi({
    reducerPath: 'studyApi',
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
        studyListGet: builder.query({
            query: () => '/Study/GetStudyList',
            providesTags: ['Study'],
        }),
        studyGet: builder.query({
            query: (studyId) => `/Study/GetStudy/${studyId}`,
            refetchOnMountOrArgChange: true, 
            keepUnusedDataFor: 0,
        }),
        studySave: builder.mutation({
            query: (data) => ({
                url: '/Study/StudySave',
                method: 'POST',
                body: data,
            }),
            invalidatesTags: ['Study'],
        }),
    }),
});


export const { useStudyListGetQuery } = StudyApi;

export const { useStudyGetQuery, usePrefetch } = StudyApi;

export const { useStudySaveMutation } = StudyApi;