import React, { Component } from 'react';
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
      <li key={v.id}>{v.name}</li>
    ));

    return (
      <div className="App">
        <header className="App-header">
          <h3>Values:</h3>
          <ul>{valuesLi}</ul>
        </header>
      </div>
    );
  }
}

export default App;
