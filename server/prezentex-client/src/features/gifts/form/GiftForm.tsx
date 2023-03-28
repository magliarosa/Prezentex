import { observer } from 'mobx-react-lite';
import { ChangeEvent, useState } from 'react';
import { Button, Form, Segment } from 'semantic-ui-react';
import { useStore } from '../../../app/stores/store';


export default observer(function GiftForm() {
const {giftStore} = useStore();
const {selectedGift, closeForm, createGift, updateGift, loading} = giftStore;

    const initialState = selectedGift ?? {
        id: '',
        name: '',
        description: '',
        price: 0,
        productUrl: '',
        createdDate: new Date(),
        recipients: []
    }

    const [gift, setGift] = useState(initialState)

    function handleSubmit() {
        gift.id ? updateGift(gift) : createGift(gift);
    }

    function handleInputChange(event:ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
        const {name, value} = event.target;
        setGift({...gift, [name]: value})
    }

    return (
        <Segment clearing>
            <Form onSubmit={handleSubmit} autoComplete='off' >
                <Form.Input placeholder='Nazwa' value={gift.name} name='name' onChange={handleInputChange}/>
                <Form.TextArea placeholder='Opis' value={gift.description} name='description' onChange={handleInputChange}/>
                <Form.Input type='number' placeholder='Cena' value={gift.price} name='price' onChange={handleInputChange}/>
                <Form.Input placeholder='Link do produktu' value={gift.productUrl} name='productUrl' onChange={handleInputChange}/>
                <Button loading={loading} floated='left' positive type='submit' content='Zapisz' />
                <Button onClick={closeForm} floated='right' neutral='true' type='button' content='Odrzuć' />
            </Form>
        </Segment>
    )
})