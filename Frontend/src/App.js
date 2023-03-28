import logo from './logo.svg';
import './App.css';
import {Home} from './Home';
import {Department} from './Department';
import {Employee} from './Employee';
import { FetchData } from './FetchData';
import { BrowserRouter, Route, Switch, NavLink } from 'react-router-dom';
import Register from './User_Account/Register'
import Login from './User_Account/Login'

function App() {
  return (
    <BrowserRouter>
    <div className="App container">
      <h3 className='d-flex justify-content-center m-3'></h3>
      
      <nav className='navbar fixed-top navbar-expand-sm bg-light-dark'>
        <ul className='navbar-nav'>
          <li className='nav-item- m-1'>
            <NavLink className="btn btn-outline-dark" to='/home'>
              Home
            </NavLink>
          </li>
          <li className='nav-item m-1'>
            <NavLink className="btn btn-outline-dark" to='/login'>
              Login
            </NavLink>
          </li>
          <li className='nav-item m-1'>
            <NavLink className="btn btn-outline-dark" to='/register'>
              Register
            </NavLink>
          </li>
        </ul>
      </nav>
      <Switch>
        <Route path='/home' component={Home}/>
        <Route path='/department' component={Department}/>
        <Route path='/employee' component={Employee}/>
        <Route path='/login' component={Login}/>
        <Route path='/register' component={Register}/>
      </Switch>
    </div>
    </BrowserRouter>
  );
}

export default App;
