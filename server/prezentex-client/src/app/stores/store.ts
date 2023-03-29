import { createContext, useContext } from "react";
import GiftStore from "./giftStore";

interface Store {
    giftStore: GiftStore
}

export const store: Store = {
    giftStore: new GiftStore()
}

export const StoreContext = createContext(store);

export function useStore() {
    return useContext(StoreContext);
}