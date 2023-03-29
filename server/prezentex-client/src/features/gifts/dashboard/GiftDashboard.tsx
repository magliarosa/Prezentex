import { observer } from 'mobx-react-lite';
import { useEffect } from 'react';
import { Grid } from 'semantic-ui-react';
import { useStore } from '../../../app/stores/store';
import GiftDetails from '../details/GiftDetails';
import GiftForm from '../form/GiftForm';
import GiftList from './GiftList';

export default observer(function GiftDashboard() {

  const { giftStore } = useStore();
  const {loadGifts, giftRegistry} = giftStore;

  useEffect(() => {
    if (giftRegistry.size <= 1) loadGifts();
  }, [loadGifts, giftRegistry.size]);


  return (
    <Grid style={{ minHeight: '100px' }}>
      <Grid.Column width='10'>
        <GiftList />
      </Grid.Column>
      <Grid.Column width='6'>
        <h2>Filtry</h2>
      </Grid.Column>
    </Grid>
  )
})