import './App.css';
import Home from './Home';
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
import axios from './api/axios';
import { ConstrainsContext } from './api/ApiConstrains';

export const cookies = new Cookies();

const LOGIN_URL = '/login';
const SEARCH_URL = '/search';

function App() {
  
  const navigate = useNavigate();
  const location = useLocation();
  const { setAuth } = useAuth();
  const { auth } = useContext(AuthContext);
  const [inputValue, setInputValue] = useState('');

  const { baseURL, setApiUrl } = useContext(ConstrainsContext);

  const from = location.state?.from?.pathname || "/home";

  const PROFILE_URL = '/user';

  useEffect(() => {
    const accessToken = cookies.get('accessToken');
    if (accessToken) {
      const payload = jwt_decode(accessToken);
      const roles = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      const email = "";
      let id = payload["sub"];
      setAuth({user: email, roles, accessToken, id});
      if(id === undefined){
        const savedApiUrl = localStorage.getItem('apiUrl');
        if (savedApiUrl) {
          setApiUrl(savedApiUrl);
          axios.defaults.baseURL = savedApiUrl;
        }
        axios.get(PROFILE_URL, {
            headers: { 
              'Content-Type': 'application/json',
              "Authorization" : `Bearer ${accessToken}`
            },
            withCredentials: false 
        }).then((response)=> {
          id = response?.data?.id;
          setAuth({user: email, roles, accessToken, id});
        }).catch((err) => {
          console.log('error' + err);
        });
      }
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
  }, []);

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

  const handleApiChange = (apiUrl) => {
    axios.defaults.baseURL = apiUrl;
    setApiUrl(apiUrl);
    logout();
  };

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
          <input type="text" placeholder="what are you looking for? ..." onKeyDown={showSearchResults} width="500px" className="search-bars"/>
        </div>
        <button className='btn btn-outline-light navbar-toggle ms-auto my-0 mx-3' onClick={showSidebar}>
          <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" fill="white" class="bi bi-list" viewBox="0 0 16 16">
            <path fill-rule="evenodd" d="M2.5 12a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5z"/>
          </svg>
        </button>
      </nav>
      <nav style={{ marginTop: "80px", width: "350px" }} className={sidebar ? 'nav-menu active' : 'nav-menu'}>
  <ul style={{ paddingLeft: "0px" }} className='nav-menu-items' onClick={showSidebar}>
    <li className='navbar-toggle'>
      <Link to='#' className='menu-bars'>
        <AiIcons.AiFillCloseCircle />
      </Link>
    </li>
    {SidebarData.map((item, index) => (
      <li key={index} className="sidebar-menu-item" onClick={() => handleApiChange(item.apiUrl)}>
        <div className="sidebar-menu-content">
          <div className="sidebar-menu-icon">{item.icon}</div>
          <div>
            <span className="sidebar-menu-title">{item.title}</span>
            <span className="sidebar-menu-subtitle">{item.text}</span>
          </div>
        </div>
      </li>
    ))}
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