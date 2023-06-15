import React from "react";
import './Home.css';
import logo from './images/logo.png' // relative path to image
import {useLocation, useNavigate} from 'react-router-dom';
import {useRef, useState, useEffect } from "react";
import AuthContext from "./context/AuthProvider";
import { useContext } from "react";
import axios from './api/axios';

const RECOMENDED_URL = '/playlist/recommended';

const Home = () => {

    const { auth } = useContext(AuthContext);
    const location = useLocation();

    const [videosData, setVideosData] = useState([]);

    useEffect(() => {
        localStorage.setItem("lastVisitedPage", location.pathname);
    });

    useEffect(() => {
        axios.get(RECOMENDED_URL,
        {
          headers: { 
            'Content-Type': 'application/json',
            "Authorization" : `Bearer ${auth?.accessToken}`
          },
          withCredentials: false 
        })
        .then(response => {
          setVideosData(response?.data?.videos);
        })
        .catch(error => {
          console.log("error: ", error);
        });
    }, [auth?.accessToken])

    return(
        <div class ="d-flex mt-5 justify-content-center">
            <table class="mt-5">
                <tr class="text-center">
                    <img src={logo} alt="Italian Trulli" width="840" />
                </tr>
                <tr class="text-center">
                    <svg xmlns="http://www.w3.org/2000/svg" width="90" height="90" fill="black" class="bi bi-arrow-down-circle-fill" viewBox="0 0 16 16" className="buttons2">
                        <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM8.5 4.5a.5.5 0 0 0-1 0v5.793L5.354 8.146a.5.5 0 1 0-.708.708l3 3a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V4.5z"/>
                    </svg>
                </tr> 
                <div style={{marginBottom:"20px", marginTop:"20px", 
                            color:"white", borderRadius:"15px", padding:"30px", backgroundColor:"#222222"}}>
                <h1 class="display-3 mb-5 mt-4" style={{marginLeft:"40px"}}>Now popular</h1>
                    <div class="wrapper" style={{marginBottom:"20px", marginTop:"20px", 
                            color:"white", borderRadius:"15px", padding:"30px", backgroundColor:"#111111"}}>
                        <div className="boxT5">
                            <div className="box2">
                                <table>
                                    <tr>
                                    <div className="movieT5">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="72" height="72" fill="white" class="bi bi-play-circle-fill" viewBox="0 0 16 16">
                                            <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                                        </svg>
                                    </div>
                                    </tr>
                                </table>
                            </div>   
                        </div>
                        <div className="boxT5">
                            <div className="box2">
                                <table>
                                    <tr>
                                    <div className="movieT5">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="72" height="72" fill="white" class="bi bi-play-circle-fill" viewBox="0 0 16 16">
                                            <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                                        </svg>
                                    </div>
                                    </tr>
                                </table>
                            </div>   
                        </div>
                        <div className="boxT5">
                            <div className="box2">
                                <table>
                                    <tr>
                                    <div className="movieT5">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="72" height="72" fill="white" class="bi bi-play-circle-fill" viewBox="0 0 16 16">
                                            <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                                        </svg>
                                    </div>
                                    </tr>
                                </table>
                            </div>   
                        </div>
                        <div className="boxT5">
                            <div className="box2">
                                <table>
                                    <tr>
                                    <div className="movieT5">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="72" height="72" fill="white" class="bi bi-play-circle-fill" viewBox="0 0 16 16">
                                            <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                                        </svg>
                                    </div>
                                    </tr>
                                </table>
                            </div>   
                        </div>
                        <div className="boxT5">
                            <div className="box2">
                                <table>
                                    <tr>
                                    <div className="movieT5">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="72" height="72" fill="white" class="bi bi-play-circle-fill" viewBox="0 0 16 16">
                                            <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                                        </svg>
                                    </div>
                                    </tr>
                                </table>
                            </div>   
                        </div>
                        <div className="boxT5">
                            <div className="box2">
                                <table>
                                    <tr>
                                    <div className="movieT5">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="72" height="72" fill="white" class="bi bi-play-circle-fill" viewBox="0 0 16 16">
                                            <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                                        </svg>
                                    </div>
                                    </tr>
                                </table>
                            </div>   
                        </div>
                        <div className="boxT5">
                            <div className="box2">
                                <table>
                                    <tr>
                                    <div className="movieT5">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="72" height="72" fill="white" class="bi bi-play-circle-fill" viewBox="0 0 16 16">
                                            <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                                        </svg>
                                    </div>
                                    </tr>
                                </table>
                            </div>   
                        </div>
                        <div className="boxT5">
                            <div className="box2">
                                <table>
                                    <tr>
                                    <div className="movieT5">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="72" height="72" fill="white" class="bi bi-play-circle-fill" viewBox="0 0 16 16">
                                            <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                                        </svg>
                                    </div>
                                    </tr>
                                </table>
                            </div>   
                        </div>
                        <div className="boxT5">
                            <div className="box2">
                                <table>
                                    <tr>
                                    <div className="movieT5">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="72" height="72" fill="white" class="bi bi-play-circle-fill" viewBox="0 0 16 16">
                                            <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                                        </svg>
                                    </div>
                                    </tr>
                                </table>
                            </div>   
                        </div>
                    </div>
                    </div>
            </table>
        </div>
    )
}
export default Home;