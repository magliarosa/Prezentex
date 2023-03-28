import { observer } from 'mobx-react-lite';
import { Grid } from 'semantic-ui-react';
import { useStore } from '../../../app/stores/store';
import GiftDetails from '../details/GiftDetails';
import GiftForm from '../form/GiftForm';
import GiftList from './GiftList';

export default observer(function GiftDashboard() {

  const { giftStore } = useStore();
  const {selectedGift, editMode} = giftStore;

  return (
    <Grid style={{ minHeight: '100px' }}>
      <Grid.Column width='10'>
        <GiftList />
      </Grid.Column>
      <Grid.Column width='6'>
        {selectedGift &&
          <GiftDetails />}
        {editMode &&
          <GiftForm />}
      </Grid.Column>
    </Grid>
  )
})