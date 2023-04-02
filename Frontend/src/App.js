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
import { useEffect } from 'react';
import jwt_decode  from 'jwt-decode';
import VideoPlayer from './VideoPlayer';

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
    <div className="App container">
      <h3 className='d-flex justify-content-center m-3'></h3>
      
      <nav className='navbar fixed-top navbar-expand-sm bg-light-dark'>
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
          <li className='nav-item m-1'>
            <NavLink className="btn btn-outline-light" to='/videoplayer'>
                Play video
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
          <Route path='/profile' element={<ProfilePage />} />
          <Route path='/videoplayer' element={<VideoPlayer/>} />
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