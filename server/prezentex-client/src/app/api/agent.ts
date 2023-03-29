import axios, { AxiosResponse } from "axios";
import { Gift } from "../models/gift";

const sleep = (delay: number) => {
    return new Promise((resolve) => {
        setTimeout(resolve, delay)
    })
}

axios.defaults.baseURL = 'https://localhost:7273/api';
axios.defaults.headers.common.Authorization = 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwYjEyOWViMS03MTQ0LTQ2NjctYTU0My1iN2M1MTZiMmRkOWQiLCJqdGkiOiI1OWUxMTE1ZC05ZmM3LTRmN2YtOTg4OS05ZGQ1MGExNDk3ZGQiLCJlbWFpbCI6InRvbWVrbTdAZ21haWwuY29tIiwibmFtZSI6InRvbWVrbTdAZ21haWwuY29tIiwibmJmIjoxNjgwMDY5MTI2LCJleHAiOjE2ODAwOTA3MjYsImlhdCI6MTY4MDA2OTEyNn0.bOkZlRz3_cBhkRp6XtIy2RJ5eZnuHuijbr4_ePcUsd8';

axios.interceptors.response.use(async response => {
    try {
        await sleep(1000); 
        return response;
    } catch (error) {
        console.log(error);
        return await Promise.reject(error);
    }
})

const responseBody = <T> (response: AxiosResponse<T>) => response.data;

const requests = {
    get: <T> (url: string) => axios.get<T>(url).then(responseBody),
    post: <T> (url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    put: <T> (url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
    delete: <T> (url: string) => axios.delete<T>(url).then(responseBody),
}

const Gifts = {
    list: () => requests.get<Gift[]>('/gifts'),
    details: (id: string) => requests.get<Gift>(`/gifts/${id}`),
    create: (gift: Gift) => requests.post<Gift>('gifts', gift),
    update: (gift: Gift) => requests.put<void>(`gifts/${gift.id}`, gift),
    delete: (giftId: string) => requests.delete<void>(`gifts/${giftId}`),
}

const agent = {
    Gifts
}

export default agent;