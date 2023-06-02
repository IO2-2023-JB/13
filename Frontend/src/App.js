import './App.css';
import {Home} from './Home';
import Administrator from './Administrator'
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
import { useEffect } from 'react';
import jwt_decode  from 'jwt-decode';
import VideoPlayer from './VideoPlayer';
import React, { useState } from 'react';
import * as AiIcons from 'react-icons/ai';
import { Link } from 'react-router-dom';
import { SidebarData } from './SidebarData';
import './Sidebar.css';
import Search from './Search'; 
import AddVideo from './AddVideo';
import AddPlaylist from './AddPlaylist';
import Playlist from './Playlist';
import AddVideoToPlaylist from './AddVideoToPlaylist';
import Subscriptions from './User_Account/Subscriptions';
import CreatorProfile from './User_Account/CreatorProfile';
import SubscriptionsVideos from './User_Account/SubscriptionsVideos';
import Reports from './User_Account/Reports';

export const cookies = new Cookies();

const LOGIN_URL = '/login';
const SEARCH_URL = '/search';

function App() {
  
  const navigate = useNavigate();
  const location = useLocation();
  const { setAuth } = useAuth();
  const { auth } = useContext(AuthContext);
  const [inputValue, setInputValue] = useState('');

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
      if(from !== "/home"){
        navigate(from, {replace: true});
      }
      else if(lastVisitedPage){
        navigate(lastVisitedPage);
      }
      else{
        navigate(from, {replace: true});
      }
    }
  }, [setAuth]);

  const [sidebar, setSidebar] = useState(false);

  const showSidebar = () => setSidebar(!sidebar);

  const showSearchResults = (event) => {
    if(event.key == 'Enter'){
      setInputValue(event.target.value);
      navigate(SEARCH_URL); 
    }
  }

  const logout = async () => {
    setAuth({});
    navigate(LOGIN_URL);
    cookies.remove("accessToken");
  }

  const isLoggedIn = () =>{
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
            <NavLink className="btn btn-outline-light" to='/subscriptions'>
                Subscriptions
              </NavLink>
          </li>

          {isLoggedIn()?
            <>
            <li className='nav-item m-1'>
              <NavLink className="btn btn-outline-light" to='/reports'>
                Reports
              </NavLink>
            </li>
            <li className='nav-item m-0 mr-auto'>
               <button className="btn btn-outline-light m-1" onClick={logout} style={{ verticalAlign: 'middle' }}>
                Logout
              </button> 
            </li>
            </>
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
          {/* <button class="btn btn-dark" style={{width:"50", height:"50"}}>
            <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" fill="white" class="bi bi-search" viewBox="0 0 16 16" className="buttons">
              <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
            </svg>
          </button> */}

          <input type="text" placeholder="what are you looking for? ..." onKeyDown={showSearchResults} width="500px" className="search-bars"/>
        </div>
        <button className='btn btn-outline-light navbar-toggle ms-auto my-0 mx-3' onClick={showSidebar}>
          <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" fill="white" class="bi bi-list" viewBox="0 0 16 16">
            <path fill-rule="evenodd" d="M2.5 12a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5z"/>
          </svg>
        </button>
      </nav>
      <nav style={{marginTop: "80px"}} className={sidebar ? 'nav-menu active' : 'nav-menu'}>
        <ul style={{paddingLeft:"0px"}} className='nav-menu-items' onClick={showSidebar}>
          <li className='navbar-toggle'>
            <Link to='#' className='menu-bars'>
              <AiIcons.AiFillCloseCircle />
            </Link>
          </li>
          {SidebarData.map((item, index) => {
            return (
              <li key={index} className={item.cName}>
                <Link to={item.path}>
                  {item.icon}
                  <span style={{marginRight:"10px"}}>{item.title}</span>
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
          <Route path='/addvideo' element={<AddVideo />} />
          <Route path="/administrator" element={<Administrator />}/>
          <Route path="/addplaylist" element={<AddPlaylist />} />
          <Route path="/subscriptions" element={<Subscriptions />} />
          <Route path="/subscriptionsvideos" element={<SubscriptionsVideos />} />
          <Route path='/reports' element={<Reports />} />
        </Route>
        <Route path='/videoplayer/:videoid?' element={<VideoPlayer/>} />
        <Route path='/playlist/:playlistid?' element={<Playlist/>} />
        <Route path='/addvideotoplaylist/:videoid?' element={<AddVideoToPlaylist/>} />
        <Route path='/creatorprofile/:creatorid?' element={<CreatorProfile/>} />
        <Route path='/login' element={<Login/>} />
        <Route path='/search' element={<Search query={inputValue} />} />
        <Route path='/register' element={<Register/>} />
      </Routes>
    </div>
  );
}

export default App;