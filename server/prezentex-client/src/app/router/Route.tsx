import { createBrowserRouter, RouteObject } from "react-router-dom";
import GiftDashboard from "../../features/gifts/dashboard/GiftDashboard";
import GiftDetails from "../../features/gifts/details/GiftDetails";
import GiftForm from "../../features/gifts/form/GiftForm";
import HomePage from "../../features/home/HomePage";
import App from "../layout/App";

export const routes: RouteObject[] = [
    {
        path: '/',
        element: <App />,
        children: [
            {path: 'gifts', element: <GiftDashboard />},
            {path: 'gifts/:id', element: <GiftDetails />},
            {path: 'createGift', element: <GiftForm key={'create'}/>},
            {path: 'manage/:id', element: <GiftForm key={'manage'}/>},
        ]
    }
]

export const router = createBrowserRouter(routes);