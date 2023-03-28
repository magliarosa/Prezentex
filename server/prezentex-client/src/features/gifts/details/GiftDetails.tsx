import { Button, Card, Image } from 'semantic-ui-react';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import { useStore } from '../../../app/stores/store';

export default function GiftDetails() {
    const {giftStore} = useStore();
    const {selectedGift: gift, openForm, cancelSelectedGift} = giftStore;

    if (!gift) return <LoadingComponent />;

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
                    <Button onClick={() => openForm(gift.id)} basic color='blue' content='Edytuj'/>
                    <Button onClick={cancelSelectedGift} basic color='grey' content='Zamknij'/>
                </Button.Group>
            </Card.Content>
        </Card>
    )
}