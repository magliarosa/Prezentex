import { Fragment, useEffect } from 'react';
import { Container } from 'semantic-ui-react';
import NavBar from './NavBar';
import GiftDashboard from '../../features/gifts/dashboard/GiftDashboard';
import { useStore } from '../stores/store';
import { observer } from 'mobx-react-lite';

function App() {
  const {giftStore} = useStore();

  useEffect(() => {
    giftStore.loadGifts();
  }, [giftStore]);

  return (
    <Fragment>
      <NavBar />
      <Container style={{ marginTop: '7em' }}>
      
        <GiftDashboard />
      </Container>
    </Fragment>
  );
}

export default observer(App);
