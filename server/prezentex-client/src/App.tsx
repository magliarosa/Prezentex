import React, { useEffect, useState } from 'react';
import logo from './logo.svg';
import './App.css';
import { Button, Header, List } from 'semantic-ui-react';
import axios from 'axios';

function App() {
  const [gifts, setGifts] = useState([]);

  useEffect(() => {
    axios.get('https://localhost:7273/api/Gifts')
    .then((response) => {      
      setGifts(response.data);
    })
  }, []);
  return (
    <div>
      <Header as='h2' icon='gift' content='Prezentex'/>
      <List>
        {gifts.map((gift : any) => (
          <List.Item key={gift.id}>
            {gift.name}
          </List.Item>
        ))}
      </List>
    </div>
  );
}

export default App;
