import {useState, useEffect } from "react";
import AuthContext from "../context/AuthProvider";
import { useContext } from "react";
import axios from '../api/axios';
import '@fortawesome/fontawesome-svg-core/styles.css';
import { useNavigate, useLocation, NavLink } from 'react-router-dom';

const SUBSCRIPTIONSVIDEOS_URL = '/user/videos/subscribed';

const SubscriptionsVideos = () => {
    const { auth } = useContext(AuthContext);
    const navigate = useNavigate();
    const location = useLocation();

    const [videosData, setVideosData] = useState([]);

    useEffect(() => {
      localStorage.setItem("lastVisitedPage", location.pathname);
    });

    useEffect(() => {
        axios.get(SUBSCRIPTIONSVIDEOS_URL, {
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
    }, [auth?.accessToken, auth?.id]);

    const handleVideoClick = (id) => {
        navigate(`/videoplayer/${id}`);
    }

    return(
        <div style={{marginTop: "200px", marginBottom:"200px"}} class="container">
            <nav className='navbar fixed-bottom navbar-expand-sm m-0 bg-dark'>
                <ul className='navbar-nav mx-2' style={{ justifyContent: "center" }}>
                  <li className='nav-item m-1' style={{ justifyContent: "center" }}>
                    <NavLink className="btn btn-outline-light" to='/subscriptions'>
                      Creators
                    </NavLink>
                  </li>
                  <li className='nav-item m-1' style={{ justifyContent: "center" }}>
                    <NavLink className="btn btn-outline-light" to='/subscriptionsvideos'>
                      Videos
                    </NavLink>
                  </li>
                </ul>
            </nav>
            <h2 class="display-5" style={{textAlign: "center"}}> Videos of creators that you subscribe: </h2>
            <ul style={{padding:"0px", display:"inline", marginBottom:"200px"}}>
                {videosData.map(video => (
                    <li className="search_list_item" class="mx-5" style={{marginTop:"20px", color:"white", borderRadius:"15px",
                            padding:"20px", backgroundColor:"#222222", cursor: "pointer"}} onClick={() => handleVideoClick(video.id)}>
                        <div class="row">
                          <div className="box3" style={{width:"300px", height:"170px", cursor: "pointer"}}>
                            <div className="box4" style={{width:"280px", height:"150px", backgroundImage: `url(${video.thumbnail})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                                <table style={{backgroundColor: "transparent"}}>
                                    <tr style={{backgroundColor: "transparent"}}>
                                    <div className="movie_thumbnail" style={{width:"280px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" } } onClick={() => handleVideoClick(video.id)}>
                
                                    </div>
                                    </tr>
                                </table>
                              </div> 
                          </div>
                          <div class="col-sm">
                              <h1 style={{marginTop:"30px"}}>{video.title}</h1>
                              <h4 style={{marginTop:"30px"}}>{video.authorNickname}</h4>
                          </div>
                          <div class="col-sm">
                              <h4 style={{marginTop:"30px"}}>Views: {video.viewCount}</h4>
                          </div>
                        </div>
                    </li>
                )).reverse()}
            </ul>
        </div>
    );
};
export default SubscriptionsVideos;