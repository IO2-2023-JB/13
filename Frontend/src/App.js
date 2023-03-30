import logo from './logo.svg';
import './App.css';
import {Home} from './Home';
import {Department} from './Department';
import {Employee} from './Employee';
import { FetchData } from './FetchData';
import { Route, Routes, NavLink } from 'react-router-dom';
import Register from './User_Account/Register'
import Login from './User_Account/Login'
import RequireAuth from './RequireAuth';
import { useNavigate } from 'react-router-dom';
import useAuth from './hooks/useAuth';

import { useContext } from "react";
import AuthContext from "./context/AuthProvider";


function App() {
  const navigate = useNavigate();
  const { setAuth } = useAuth();
  const { auth } = useContext(AuthContext);
  
  const logout = async () => {
    // if used in more components, this should be in context
    setAuth({});
    navigate('/login');
  }

  const isLoggedIn = () =>{
    console.log(auth?.user ? "Logged In" : "Logged Out");
    return(
      auth?.user
    )
  }

  return (
    <div className="App container">
      <h3 className='d-flex justify-content-center m-3'></h3>
      
      <nav className='navbar fixed-top navbar-expand-sm bg-light-dark'>
        <ul className='navbar-nav'>
          <li className='nav-item- m-1'>
            <NavLink className="btn btn-outline-light" to='/home'>
              Home
            </NavLink>
          </li>
          {/* <li className='nav-item m-1'>
            <NavLink className="btn btn-outline-light" to='/department'>
              Department
            </NavLink>
          </li>
          <li className='nav-item m-1'>
            <NavLink className="btn btn-outline-light" to='/employee'>
              Employee
            </NavLink>
          </li> */}
          {isLoggedIn()?
            <li className='nav-item m-1 mr-auto'>
               <button className="btn btn-outline-light" onClick={logout} style={{ verticalAlign: 'middle' }}>
                Logout
              </button> 
            </li>
            :
            <li className='nav-item m-1'>
              <NavLink className="btn btn-outline-light" to='/login'>
                Login
              </NavLink>
            </li>
          }
          {!isLoggedIn() &&
          <li className='nav-item m-1'>
            <NavLink className="btn btn-outline-light" to='/register'>
                Register
              </NavLink>
          </li>
          }
        </ul>
      </nav>
      <Routes>
        <Route element={<RequireAuth />}>
          <Route path='/home' element={<Home/>}/>
        </Route>
        <Route path='/department' element={<Department />} />
        <Route path='/employee' element={<Employee />} />
        <Route path='/login' element={<Login />} />
        <Route path='/register' element={<Register />} />
        <Route path='/employee' element={<Employee />} />
      </Routes>
    </div>
  );
}

export default App;
