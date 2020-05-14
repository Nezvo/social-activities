import React, { Component } from 'react';
import { Header, Icon, List } from 'semantic-ui-react';
import './App.css';
import Axios from 'axios';

class App extends Component {
  state = {
    values: [],
  };

  componentDidMount() {
    Axios.get('http://localhost:5000/api/values').then((response) => {
      this.setState({
        values: response.data,
      });
    });
  }

  render() {
    let valuesLi = this.state.values.map((v: any) => (
      <List.Item key={v.id}>{v.name}</List.Item>
    ));

    return (
      <div>
        <Header as="h2">
          <Icon name="users" />
          <Header.Content>Social Activities</Header.Content>
        </Header>
        <List>{valuesLi}</List>
      </div>
    );
  }
}

export default App;
