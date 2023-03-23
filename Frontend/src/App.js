import logo from './logo.svg';
import './App.css';
import {Home} from './Home';
import {Department} from './Department';
import {Employee} from './Employee';
import { FetchData } from './FetchData';
import { BrowserRouter, Route, Switch, NavLink } from 'react-router-dom';
import Register from './User_Account/Register'

function App() {
  return (
    <BrowserRouter>
    <div className="App container">
      <h3 className='d-flex justify-content-center m-3'></h3>
      <nav className='navbar navbar-expand-sm bg-light-dark'>
        <ul className='navbar-nav'>
          <li className='nav-item- m-1'>
            <NavLink className="btn btn-light btn-outline-primary" to='/home'>
              Home
            </NavLink>
          </li>
          <li className='nav-item m-1'>
            <NavLink className="btn btn-light btn-outline-primary" to='/department'>
              Department
            </NavLink>
          </li>
          <li className='nav-item m-1'>
            <NavLink className="btn btn-light btn-outline-primary" to='/employee'>
              Employee
            </NavLink>
          </li>
          <li className='nav-item m-1'>
            <NavLink className="btn btn-light btn-outline-primary" to='/fetchdata'>
              FetchData
            </NavLink>
          </li>
          <li className='nav-item m-1'>
            <NavLink className="btn btn-light btn-outline-primary" to='/register'>
              Register
            </NavLink>
          </li>
        </ul>
      </nav>
      <Switch>
        <Route path='/home' component={Home}/>
        <Route path='/department' component={Department}/>
        <Route path='/employee' component={Employee}/>
        <Route path='/fetchdata' component={FetchData}/>
        <Route path='/register' component={Register}/>
      </Switch>
    </div>
    </BrowserRouter>
  );
}

export default App;
