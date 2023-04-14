import logo from './logo.svg';
import './App.css';
import {Home} from './Home';
import {Administrator} from './Administrator'
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
import VideoPlayer from './VideoPlayer';
import React, { useState } from 'react';
import * as FaIcons from 'react-icons/fa';
import * as AiIcons from 'react-icons/ai';
import { Link } from 'react-router-dom';
import { SidebarData } from './SidebarData';
import './Sidebar.css';
import whitelogo from './images/logo2.png'
import { IconContext } from 'react-icons';
import AddVideo from './AddVideo';

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
      const email = "";
      const id = payload["sub"];
      setAuth({user: email, pwd: "", roles, accessToken, id});
      const lastVisitedPage = localStorage.getItem("lastVisitedPage");
      if(from!= "/home"){
        navigate(from, {replace: true});
      }
      else if(lastVisitedPage){
        navigate(lastVisitedPage);
      }
      else{
        navigate(from, {replace: true});
      }
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
    //console.log(auth?.accessToken ? "Logged In" : "Logged Out");
    return(
      auth?.accessToken
    )
  }

  return (
    <div class="container-fluid">
      <nav class='navbar fixed-top navbar-expand-sm m-0 bg-dark'>
        <ul className='navbar-nav mx-2'>
          
          <li className='nav-item m-1'>
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
          <li className='nav-item m-1'>
            <NavLink className="btn btn-outline-light" to='/videoplayer'>
                Play video
              </NavLink>
          </li>

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

        <div class = "nav-item m-auto">
          <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" fill="white" class="bi bi-search" viewBox="0 0 16 16" className="buttons">
            <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
          </svg>

          <input type="text" placeholder="what are you looking for? ..." width="300" className="search-bars"/>
        </div>
        <button className='btn btn-outline-light navbar-toggle ms-auto my-0 mx-3' onClick={showSidebar}>
          <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" fill="white" class="bi bi-list" viewBox="0 0 16 16">
            <path fill-rule="evenodd" d="M2.5 12a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5z"/>
          </svg>
        </button>
      </nav>
      <nav style={{marginTop: "80px"}} className={sidebar ? 'nav-menu active' : 'nav-menu'}>
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
        <Route path='/videoplayer/:videoid?' element={<VideoPlayer/>} />
        <Route path='/department' element={<Department />} />
        <Route path='/employee' element={<Employee />} />
        <Route path='/login' element={<Login />} />
        <Route path="/administrator" element={<Administrator />}/>
        <Route path='/register' element={<Register />} />
        <Route path='/employee' element={<Employee />} />
        <Route path='/video' element={<AddVideo />} />
      </Routes>
    </div>
  );
}

export default App;