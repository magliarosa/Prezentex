import { makeAutoObservable, runInAction } from "mobx";
import agent from "../api/agent";
import { Gift } from "../models/gift";

export default class GiftStore {
    giftRegistry = new Map<string, Gift>();
    selectedGift: Gift | undefined = undefined;
    editMode: boolean = false;
    loading: boolean = false;
    giftsLoading: boolean = false;
    loadingInitial: boolean = false;
    submitting: boolean = false;

    constructor() {
        makeAutoObservable(this)
    }

    get giftsByPrice() {
        return Array.from(this.giftRegistry.values())
            .sort((a, b) => a.price - b.price);
    }

    loadGifts = async () => {
        runInAction(() => {
            this.setEditMode(false);
            this.setGiftsLoadning(true);
        })
        try {
            const gifts = await agent.Gifts.list();
            gifts.forEach(gift => {
                this.setGift(gift);
            })
            this.setGiftsLoadning(false);
        } catch (error) {
            console.log(error);
            this.setGiftsLoadning(false);
        }
    }

    loadGift = async (id: string) => {
        let gift = this.getGift(id);
        if (gift) {
            this.selectedGift = gift;
            return gift;
        }
        else {
            this.setLoadingInitial(true);
            try {
                gift = await agent.Gifts.details(id);
                this.setGift(gift);
                this.selectedGift = gift;
                this.setLoadingInitial(false);
                return gift;
            } catch (error) {
                console.log(error);
                this.setLoadingInitial(false);
            }
        }
    }

    private setGift = (gift: Gift) => {
        this.giftRegistry.set(gift.id, gift);
    }

    private getGift = (id: string) => {
        return this.giftRegistry.get(id);
    }

    setEditMode = (state: boolean) => {
        this.editMode = state;
    }

    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    setGiftsLoadning = (state: boolean) => {
        this.giftsLoading = state;
    }

    selectGift = (id: string) => {
        this.selectedGift = this.giftRegistry.get(id);
    }

    cancelSelectedGift = () => {
        this.selectedGift = undefined;
    }

    openForm = (id?: string) => {
        console.log('Open form');

        this.closeForm();
        id ? this.selectGift(id) : this.cancelSelectedGift();
        this.setEditMode(true);
    }

    closeForm = () => {
        this.editMode = false;
    }

    createGift = async (gift: Gift) => {
        this.loading = true;
        try {
            let newGift = await agent.Gifts.create(gift);
            this.setGift(newGift);
            runInAction(() => {
                this.loading = false;
                this.editMode = false;
            });
            return newGift;
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            });
            throw error;
        }
    }

    updateGift = async (gift: Gift) => {
        this.loading = true;
        try {
            await agent.Gifts.update(gift);
            runInAction(() => {
                this.giftRegistry.set(gift.id, gift);
                this.loading = false;
                this.editMode = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }

    deleteGift = async (id: string) => {
        this.editMode = false;
        this.loading = true;
        if (id === this.selectedGift?.id) {
            this.cancelSelectedGift();
        }
        try {
            await agent.Gifts.delete(id);
            runInAction(() => {
                this.giftRegistry.delete(id);
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }

}