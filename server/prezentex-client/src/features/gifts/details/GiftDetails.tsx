import { observer } from 'mobx-react-lite';
import { useEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { Button, Card, Image } from 'semantic-ui-react';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import { useStore } from '../../../app/stores/store';

export default observer(function GiftDetails() {
    const {giftStore} = useStore();
    const {selectedGift: gift, loadGift, loadingInitial} = giftStore;
    const {id} = useParams();

    useEffect(() => {
        if (id) loadGift(id);
    }, [id, loadGift])

    if (loadingInitial || !gift) return <LoadingComponent />;

    return (
        <Card fluid>
            <Image src='/assets/gift.png' />
            <Card.Content>
                <Card.Header>{gift.name}</Card.Header>
                <Card.Meta>
                    <span>{new Date(gift.createdDate).toLocaleString()}</span>
                </Card.Meta>
                <Card.Description>
                    {gift.description}
                </Card.Description>
            </Card.Content>
            <Card.Content extra>
                <Button.Group widths='2'>
                    <Button as={Link} to={`/manage/${gift.id}`} basic color='blue' content='Edytuj'/>
                    <Button as={Link} to='/gifts' basic color='grey' content='Zamknij'/>
                </Button.Group>
            </Card.Content>
        </Card>
    )
})