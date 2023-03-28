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
            .sort((a,b) => a.price - b.price);
    }

    loadGifts = async () => {
        this.setGiftsLoadning(true);
        try {
            const gifts = await agent.Gifts.list();
            gifts.forEach(gift => {
                this.giftRegistry.set(gift.id, gift);
            })
            this.setGiftsLoadning(false);
        } catch (error) {
            console.log(error);
            this.setGiftsLoadning(false);
        }
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
        id ? this.selectGift(id) : this.cancelSelectedGift();
        this.editMode = true;
    }

    closeForm = () => {
        this.editMode = false;
    }

    createGift = async (gift:Gift) => {
        this.loading = true;
        try {
            await agent.Gifts.create(gift)
                .then((response) => {
                    this.giftRegistry.set(response.id, response);
                });
            runInAction(() => {
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

    updateGift = async (gift: Gift) => {
        this.loading = true;
        try {
            await agent.Gifts.update(gift);
            runInAction(() =>{
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
        if (id === this.selectedGift?.id){
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