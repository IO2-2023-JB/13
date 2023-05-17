import { useState, useEffect } from "react";
import AuthContext from "../context/AuthProvider";
import { useContext } from "react";
import axios from '../api/axios';
import '@fortawesome/fontawesome-svg-core/styles.css';
import { config } from '@fortawesome/fontawesome-svg-core';
import { useNavigate, useLocation, useParams } from 'react-router-dom';

config.autoAddCss = false;

const PROFILE_URL = '/user';
const USER_VIDEOS_URL = '/user/videos';
const USER_PLAYLISTS_URL = '/playlist/user';
const SUBSCRIPTIONS_URL = '/subscriptions';

const CreatorProfile = () => {
  const { auth } = useContext(AuthContext);
  const navigate = useNavigate();
  const params = useParams();
  const [creator_id, setCreator_id] = useState(params.creatorid);

  const [data, setData] = useState(null);

  const [videosData, setVideosData] = useState([]);
  const [playlistsData, setPlaylistsData] = useState([]);
  const [subscriptionsData, setSubscriptionsData] = useState([]);

  const location = useLocation();

  useEffect(() => {
    localStorage.setItem("lastVisitedPage", location.pathname);
  })

  useEffect(() => {
    if (!auth?.accessToken) {
      navigate('/login', { state: { from: location } });
    }
  }, [auth]);

  useEffect(() => {
    if(params.creatorid){
      setCreator_id(params.creatorid);
    }
    else{
      navigate('/home');
    }
  }, [params.creatorid, auth.id])

  useEffect(() => {
    if(auth?.id && creator_id && (auth?.id.toUpperCase() === creator_id.toUpperCase())){
      navigate('/profile');
    }
  }, [auth?.id, creator_id])

  useEffect(() => {
    axios.get(SUBSCRIPTIONS_URL + "?id=" + auth?.id, {
      headers: { 
        'Content-Type': 'application/json',
        "Authorization" : `Bearer ${auth?.accessToken}`
      },
      withCredentials: true 
    })
    .then(response => {
        setSubscriptionsData(response?.data?.subscriptions);
    })
    .catch(error => {
      console.log("error: ", error);
    });
  }, [auth?.accessToken, auth?.id]);

  useEffect(() => {
    axios.get(PROFILE_URL + "?id=" + creator_id, {
      headers: { 
        'Content-Type': 'application/json',
        "Authorization" : `Bearer ${auth?.accessToken}`
      },
      withCredentials: true 
    })
    .then(response => {
      setData(response?.data);
    })
    .catch(error => {
      console.log("error: ", error);
      navigate('/home');
    });
  
    axios.get(USER_VIDEOS_URL + "?id=" + creator_id, {
      headers: { 
        'Content-Type': 'application/json',
        "Authorization" : `Bearer ${auth?.accessToken}`
      },
      withCredentials: true 
    })
    .then(response => {
      setVideosData(response?.data?.videos);
    })
    .catch(error => {
      console.log("error: ", error);
    });
  
    axios.get(USER_PLAYLISTS_URL + "?id=" + creator_id, {
      headers: { 
        'Content-Type': 'application/json',
        "Authorization" : `Bearer ${auth?.accessToken}`
      },
      withCredentials: true 
    })
    .then(response => {
      setPlaylistsData(response?.data);
    })
    .catch(error => {
      console.log("error: ", error);
    });
  }, [auth?.accessToken, creator_id]);

  const [userData, setUserData] = useState({
    firstName: "Loading...",
    lastName: "Loading...",
    nickname: "Loading...",
    email: "Loading...",
    accountBalance: 0,
    avatarImage: '',
    userType: '',
  });

  useEffect(() => {
    if (data) {
      setUserData({
        firstName: data?.name,
        lastName: data?.surname,
        nickname: data?.nickname,
        email: data?.email,
        accountBalance: data?.accountBalance,
        avatarImage: data?.avatarImage,
        userType: data?.userType,
      });
    }
  }, [data]);

  useEffect(() => {
    if(userData.userType === 'Simple'){
      navigate('/home');
    }
  }, [userData])

  const handleVideoClick = (id) => {
    navigate(`/videoplayer/${id}`);
  }

  const handelPlaylistClick = (id) => {
    navigate(`/playlist/${id}`);
  }

  const handleSubscribeClick = () => {
    axios.post(SUBSCRIPTIONS_URL + "?id=" + creator_id, {}, {
      headers: { 
        'Content-Type': 'application/json',
        "Authorization" : `Bearer ${auth?.accessToken}`
      },
      withCredentials: true 
    })
    .then(() => {
      axios.get(SUBSCRIPTIONS_URL + "?id=" + auth?.id, {
        headers: { 
          'Content-Type': 'application/json',
          "Authorization" : `Bearer ${auth?.accessToken}`
        },
        withCredentials: true
      })
      .then(response => {
          setSubscriptionsData(response?.data?.subscriptions);
      })
      .catch(error => {
        console.log("error: ", error);
      });
    })
    .catch(error => {
      console.log("error: ", error);
    });
    //refresh?
  }

  const handleUnSubscribeClick = () => {
    axios.delete(SUBSCRIPTIONS_URL + "?id=" + creator_id, {
      headers: { 
        'Content-Type': 'application/json',
        "Authorization" : `Bearer ${auth?.accessToken}`
      },
      withCredentials: true 
    })
    .then(() => {
      const updatedSubscriptionsData = subscriptionsData.filter(subscription => subscription.id !== creator_id);
      setSubscriptionsData(updatedSubscriptionsData);
    })
    .catch(error => {
      console.log("error: ", error);
    });
    //refresh?
  }

return (
  <div style={{marginTop: "200px"}} class="container">
      <div class="row">
        <h1 class="display-1">{userData.nickname}</h1>
        <div class ="mt-2 row">
          <div class="col-sm">
            <h2>User's data</h2>
            <section class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#333333"}}>
              <label>Name:</label>
              <div>{userData.firstName}</div>
              <label>Surname:</label>
              <div>{userData.lastName}</div>
              <label>Nickname:</label>
              <div>{userData.nickname}</div>
              <label>Email:</label>
              <div>{userData.email}</div>
            </section>
            {!subscriptionsData.some(subscription => subscription.id === creator_id) ? (
              <div class="container-fluid justify-content-center" style={{marginBottom: "50px"}}>
                <button onClick={handleSubscribeClick} class="btn btn-dark" style={{marginRight:"20px", marginLeft:"-15px"}}>Subscribe</button>
              </div>
            ):(
              <div class="container-fluid justify-content-center" style={{marginBottom: "50px"}}>
                <button onClick={handleUnSubscribeClick} class="btn btn-danger" style={{marginRight:"20px", marginLeft:"-15px"}}>Subscribed</button>
              </div>
            )}
          </div>
          <div class="col-sm">
            <h2>Avatar Image</h2>
            <section class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
              <img key={userData.avatarImage} src = {userData.avatarImage+"?time="+new Date()} alt="No avatar image"/>
            </section>
          </div>
          <div class="col-sm">
          <h2>{userData.nickname}'s videos</h2>
            <section class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#333333"}}>
              <ul style={{padding:"0px"}}>
                {videosData.map(video => (
                  (video.processingProgress ==='Ready') && (video.visibility === 'Public') && (
                  <div>
                  <li style={{listStyleType: "none"}}>
                  <div className="box" style={{width:"300px", height:"170px", cursor: "pointer"}}>
                    <div className="box2" style={{width:"280px", height:"150px", backgroundImage: `url(${video.thumbnail})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                        <table style={{backgroundColor: "transparent"}}>
                            <tr style={{backgroundColor: "transparent"}}>
                              <div className="movie_title" style={{width:"280px", height:"60px", fontSize:"10px", marginTop:"0", whiteSpace: 'nowrap', overflow: 'hidden', position:"center", color:"black", backgroundColor:"transparent" }}>
                                <h2 class="text-with-stroke" style={{backgroundColor: "transparent"}}>{video.title}</h2>
                              </div>
                            </tr>
                            <tr style={{backgroundColor: "transparent"}}>
                            <div className="movie_thumbnail" style={{width:"280px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" } } onClick={() => handleVideoClick(video.id)}>
                                <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="black" class="bi bi-play-circle-fill" viewBox="0 0 16 16" style={{ fill: "white", borderRadius: "100%", marginBottom: "40px" }}>
                                  <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                                </svg>
                            </div>
                            </tr>
                        </table>
                      </div> 
                    </div>
                    </li>
                  </div>
                  )
                )).reverse()}
              </ul>
            </section>
          <h2>{userData.nickname}'s playlists</h2>
            <section class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#333333"}}>
              <ul style={{padding:"0px"}}>
                {playlistsData.map(playlist => (
                  <div>
                  <li style={{listStyleType: "none"}}>
                  <div className="box" style={{width:"300px", height:"100px", backgroundSize:"cover", backgroundRepeat:"no-repeat", cursor: "pointer", backgroundPosition:"center", backgroundColor: `#${Math.floor(Math.random()*16777215).toString(16)}`}}>
                    <div className="box2" style={{width:"280px", height:"60px", backgroundColor: "transparent"}} onClick={() => handelPlaylistClick(playlist.id)}>
                        <table style={{backgroundColor: "transparent"}}>
                            <tr style={{backgroundColor: "transparent"}}>
                            <div className="movie_title" style={{width:"280px", height:"60px", fontSize:"10px", marginTop:"0", whiteSpace: 'nowrap', overflow: 'hidden', position:"center", color:"black", backgroundColor:"transparent" }}>
                              <h2 class="text-with-stroke" style={{backgroundColor: "transparent"}}>{playlist.name}</h2>
                            </div>
                            </tr>
                        </table>
                        </div> 
                    </div>
                    </li>
                  </div>
                  )
                ).reverse()}
              </ul>
            </section>
          </div>
        </div>
      </div>
    </div>
);
};
export default CreatorProfile;
