import {
    ADD_STUDY,
    GET_STUDY
} from "./actionTypes"

export const addStudy = (study) => {
    return {
        type: ADD_STUDY,
        payload: { study },
    }
}

export const getStudy = (study) => {
    return {
        type: GET_STUDY
    }
}