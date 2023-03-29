import { observer } from 'mobx-react-lite';
import { ChangeEvent, useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { Button, Form, Segment } from 'semantic-ui-react';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import { Gift } from '../../../app/models/gift';
import { useStore } from '../../../app/stores/store';


export default observer(function GiftForm() {
    const { giftStore } = useStore();
    const { closeForm, createGift, updateGift,
        loading, loadGift, loadingInitial } = giftStore;
    const { id } = useParams();
    const navigate = useNavigate();

    const [gift, setGift] = useState<Gift>(
        {
            id: '',
            name: '',
            description: '',
            price: 0,
            productUrl: '',
            createdDate: new Date(),
            recipients: []
        }
    )

    useEffect(() => {
        if (id) loadGift(id).then(gift => setGift(gift!))
    }, [id, loadGift]);


    function handleSubmit() {
        if (!gift.id) {
            createGift(gift).then(newGift => navigate(`/gifts/${newGift.id}`));
        } else {
            updateGift(gift).then(() => navigate(`/gifts/${gift.id}`));
        }
        gift.id ? updateGift(gift) : createGift(gift);
    }

    function handleInputChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
        const { name, value } = event.target;
        setGift({ ...gift, [name]: value })
    }

    if (loadingInitial) return <LoadingComponent content='Ładowanie prezentu... ' />

    return (
        <Segment clearing>
            <Form onSubmit={handleSubmit} autoComplete='off' >
                <Form.Input placeholder='Nazwa' value={gift.name} name='name' onChange={handleInputChange} />
                <Form.TextArea placeholder='Opis' value={gift.description} name='description' onChange={handleInputChange} />
                <Form.Input type='number' placeholder='Cena' value={gift.price} name='price' onChange={handleInputChange} />
                <Form.Input placeholder='Link do produktu' value={gift.productUrl} name='productUrl' onChange={handleInputChange} />
                <Button loading={loading} floated='left' positive type='submit' content='Zapisz' />
                <Button as={Link} to='/gifts' floated='right' neutral='true' type='button' content='Odrzuć' />
            </Form>
        </Segment>
    )
})