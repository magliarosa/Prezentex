import { observer } from 'mobx-react-lite';
import { SyntheticEvent, useState } from 'react';
import { Link } from 'react-router-dom';
import { Button, Item, Label, Segment } from 'semantic-ui-react';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import { useStore } from '../../../app/stores/store';

export default observer(function GiftList() {
    const [target, setTarget] = useState('');
    const {giftStore} = useStore();
    const {deleteGift, loading, giftsLoading, giftsByPrice} = giftStore;

    function handleGiftDelete(e: SyntheticEvent<HTMLButtonElement>, id: string){
        setTarget(e.currentTarget.name);
        console.log("ID " + id);
        
        deleteGift(id);
    }
    
    if(giftsLoading) return <LoadingComponent content='Pakowanie prezentów...'/>

    return (
        <Segment>
            <Item.Group divided>
                {giftsByPrice.map(gift => (
                    <Item key={gift.id}>
                        <Item.Content>
                            <Item.Header as='a'>{gift.name}</Item.Header>
                            <Item.Meta>{new Date(gift.createdDate).toLocaleDateString()}</Item.Meta>
                            <Item.Description>
                                <div>{gift.description}</div>
                                <div>{gift.productUrl}</div>
                                <div>{gift.price}</div>
                            </Item.Description>
                            <Item.Extra>
                                <Button 
                                    as={Link}
                                    to={`/gifts/${gift.id}`} 
                                    floated='right' 
                                    content='Otwórz' 
                                    color='blue' />
                                <Button 
                                    name={gift.id}
                                    loading={loading && target === gift.id} 
                                    onClick={(e) => handleGiftDelete(e, gift.id)} 
                                    floated='right' 
                                    content='Usuń' 
                                    color='red' />
                                <Label basic content={gift.price + ' PLN'} />
                            </Item.Extra>
                        </Item.Content>
                    </Item>
                ))}
            </Item.Group>
        </Segment>
    )
})