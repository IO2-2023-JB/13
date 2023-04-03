import logo from './logo.svg';
import './App.css';
import {Home} from './Home';
import {Department} from './Department';
import {Employee} from './Employee';
import { FetchData } from './FetchData';
import { Route, Routes, NavLink } from 'react-router-dom';
import Register from './User_Account/Register'
import Login from './User_Account/Login'
import ProfilePage from './User_Account/ProfilePage';
import RequireAuth from './RequireAuth';
import { useNavigate, useLocation } from 'react-router-dom';
import useAuth from './hooks/useAuth';
import Cookies from 'universal-cookie'
import { useContext } from "react";
import AuthContext from "./context/AuthProvider";
import { useRef, useEffect } from 'react';
import jwt_decode  from 'jwt-decode';
import React, { useState } from 'react';
import * as FaIcons from 'react-icons/fa';
import * as AiIcons from 'react-icons/ai';
import { Link } from 'react-router-dom';
import { SidebarData } from './SidebarData';
import './Sidebar.css';
import { IconContext } from 'react-icons';

export const cookies = new Cookies();

const LOGIN_URL = '/login';

function App() {

  const navigate = useNavigate();
  const location = useLocation();
  const { setAuth } = useAuth();
  const { auth } = useContext(AuthContext);

  const from = location.state?.from?.pathname || "/home";

  useEffect(() => {
    const accessToken = cookies.get('accessToken');
    if (accessToken) {
      const payload = jwt_decode(accessToken);
      const roles = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      const email = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
      const id = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
      //console.log(payload.sub)
      //console.log(roles);
      setAuth({user: email, pwd: "", roles, accessToken, id});
      navigate(from, {replace: true});
    }
  }, []);

  const [sidebar, setSidebar] = useState(false);

  const showSidebar = () => setSidebar(!sidebar);

  const logout = async () => {
    // if used in more components, this should be in context
    setAuth({});
    cookies.remove("accessToken");
    navigate('/login');
  }

  const isLoggedIn = () =>{
    console.log(auth?.user ? "Logged In" : "Logged Out");
    return(
      auth?.user
    )
  }

  return (
    <div class="container-fluid">
      <nav className='navbar navbar-expand-sm m-3 bg-light-dark'>
        <ul className='navbar-nav'>
          <li className='nav-item- m-1'>
            <NavLink className="btn btn-outline-light" to='/home'>
              Home
            </NavLink>
          </li>
          {isLoggedIn() &&
          <li className='nav-item m-1'>
            <NavLink className="btn btn-outline-light" to='/profile'>
              Profile
            </NavLink>
          </li>
          }
          {isLoggedIn()?
            <li className='nav-item m-0 mr-auto'>
               <button className="btn btn-outline-light m-1" onClick={logout} style={{ verticalAlign: 'middle' }}>
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
      <button className='btn btn-outline-light navbar-toggle ms-auto' onClick={showSidebar}>
        <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" fill="white" class="bi bi-list" viewBox="0 0 16 16">
          <path fill-rule="evenodd" d="M2.5 12a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5z"/>
        </svg>
      </button>
      </nav>
        <nav className={sidebar ? 'nav-menu active' : 'nav-menu'}>
          <ul className='nav-menu-items' onClick={showSidebar}>
            <li className='navbar-toggle'>
              <Link to='#' className='menu-bars'>
                <AiIcons.AiOutlineClose />
              </Link>
            </li>
            {SidebarData.map((item, index) => {
              return (
                <li key={index} className={item.cName}>
                  <Link to={item.path}>
                    {item.icon}
                    <span>{item.title}</span>
                  </Link>
                </li>
              );
            })}
          </ul>
        </nav>

      <Routes>
        <Route element={<RequireAuth />}>
          <Route path='/home' element={<Home/>}/>
          <Route path='/profile' element={<ProfilePage />} />
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