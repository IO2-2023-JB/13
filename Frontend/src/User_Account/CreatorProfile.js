import { useState, useEffect } from "react";
import AuthContext from "../context/AuthProvider";
import { useContext } from "react";
import axios from '../api/axios';
import '@fortawesome/fontawesome-svg-core/styles.css';
import { config } from '@fortawesome/fontawesome-svg-core';
import { useNavigate, useLocation, useParams } from 'react-router-dom';
import TextField from "@material-ui/core/TextField";

config.autoAddCss = false;

const PROFILE_URL = '/user';
const USER_VIDEOS_URL = '/user/videos';
const USER_PLAYLISTS_URL = '/playlist/user';
const SUBSCRIPTIONS_URL = '/subscriptions';
const DONATE_SEND_URL = '/donate/send';
const REPORT_URL = '/ticket';

const CreatorProfile = () => {
  const { auth } = useContext(AuthContext);
  const navigate = useNavigate();
  const params = useParams();
  const [creator_id, setCreator_id] = useState(params.creatorid);

  const [data, setData] = useState(null);
  const [currentData, setCurrnetData] = useState(null);

  const [showModal, setShowModal] = useState(false);
  const [reason, setReason] = useState('');

  const [videosData, setVideosData] = useState([]);
  const [playlistsData, setPlaylistsData] = useState([]);
  const [subscriptionsData, setSubscriptionsData] = useState([]);

  const [errMsg, setErrMsg] = useState('');
  const [isDonating, setIsDonating] = useState(false);
  const [donateAmount, setDonateAmount] = useState(1);

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
      withCredentials: false 
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
      withCredentials: false 
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
      withCredentials: false 
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
      withCredentials: false 
    })
    .then(response => {
      setPlaylistsData(response?.data);
    })
    .catch(error => {
      console.log("error: ", error);
    });

    axios.get(PROFILE_URL, {
      headers: { 
        'Content-Type': 'application/json',
        "Authorization" : `Bearer ${auth?.accessToken}`
      },
      withCredentials: false 
    })
    .then(response => {
      setCurrnetData(response?.data);
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

  const [currnetUserData, setCurrentUserData] = useState({
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
    if (currentData) {
      setCurrentUserData({
        firstName: currentData?.name,
        lastName: currentData?.surname,
        nickname: currentData?.nickname,
        email: currentData?.email,
        accountBalance: currentData?.accountBalance,
        avatarImage: currentData?.avatarImage,
        userType: currentData?.userType,
      });
    }
  }, [currentData]);

  const handleVideoClick = (id) => {
    navigate(`/videoplayer/${id}`);
  }

  const handelPlaylistClick = (id) => {
    navigate(`/playlist/${id}`);
  }

  const handleSubscribeClick = () => {
    axios.post(SUBSCRIPTIONS_URL + "?creatorId=" + creator_id, {}, {
      headers: { 
        'Content-Type': 'application/json',
        "Authorization" : `Bearer ${auth?.accessToken}`
      },
      withCredentials: false 
    })
    .then(() => {
      axios.get(SUBSCRIPTIONS_URL + "?id=" + auth?.id, {
        headers: { 
          'Content-Type': 'application/json',
          "Authorization" : `Bearer ${auth?.accessToken}`
        },
        withCredentials: false
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
  }

  const handleUnSubscribeClick = () => {
    axios.delete(SUBSCRIPTIONS_URL + "?subId=" + creator_id, {
      headers: { 
        'Content-Type': 'application/json',
        "Authorization" : `Bearer ${auth?.accessToken}`
      },
      withCredentials: false 
    })
    .then(() => {
      const updatedSubscriptionsData = subscriptionsData.filter(subscription => subscription.id !== creator_id);
      setSubscriptionsData(updatedSubscriptionsData);
    })
    .catch(error => {
      console.log("error: ", error);
    });
  }

  const handleDonateClick = () => {
    setDonateAmount(1);
    setIsDonating(!isDonating);
  }

  const handleDonateAmountChange = (event) => {
    const newAmount = parseInt(event.target.value);
    if (newAmount >= 1) {
      setDonateAmount(newAmount);
    }
  };

  const handleDonateCancelClick = () => {
    setDonateAmount(1);
    setIsDonating(false);
  }

  const handleSendDonateClick = () => {
    axios.post(DONATE_SEND_URL, {},
        {
          params: {
            id: creator_id,
            amount: donateAmount
          },
          headers: { 
            'Content-Type': 'application/json',
            "Authorization" : `Bearer ${auth?.accessToken}`
          },
          withCredentials: false
        }
      ).then(() => {
        setDonateAmount(1);
        setIsDonating(false);
        axios.get(PROFILE_URL, {
          headers: { 
            'Content-Type': 'application/json',
            "Authorization" : `Bearer ${auth?.accessToken}`
          },
          withCredentials: false 
        })
        .then(response => {
          setCurrnetData(response?.data);
        })
        .catch(error => {
          console.log("error: ", error);
        });
      }).catch(err => {
        if(!err?.response) {
            setErrMsg('No Server Response')
        } else if(err.response?.status === 400) {
            setErrMsg('Bad request');
        } else if(err.response?.status === 404){
            setErrMsg('Not Found');
        } else {
            setErrMsg('Getting metadata failed');
        }
      });
  }

  const reportUser = () => {
    setShowModal(true);
  }

  const handleCloseModal = () => {
    setShowModal(false);
    setReason('');
  };

  const handleReasonChange = (event) => {
    setReason(event.target.value);
  };

  const handleReportSubmit = () => {
    axios.post(REPORT_URL, 
      {
        "targetId": creator_id,
        "targetType": "User",
        "reason": reason
      },
      {
        headers: { 
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${auth?.accessToken}`
        },
        withCredentials: false //cred
      }
      ).catch(err => {
        if (!err?.response) {
          setErrMsg('No Server Response');
        } else if (err.response?.status === 400) {
            setErrMsg('Bad request');
        } else if (err.response?.status === 401) {
          setErrMsg('Unauthorized');
        }else if (err.response?.status === 403) {
          setErrMsg('Forbidden');
        }else if (err.response?.status === 404) {
          setErrMsg('Not found');
        }else {
          setErrMsg('Reporting user with id ' + creator_id + ' Failed');
        }
      });
    setShowModal(false);
    setReason('');
  };

return (
  <div style={{marginTop: "200px", marginBottom: "50px"}} class="container">
      <div class="row mx-5">
        <h1 class="display-3">{userData.nickname}</h1>
        <div class ="mt-2 row">
          <div class="col-sm">
            <h3>User's data</h3>
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
            {(userData.userType !== 'Simple' && userData.userType !== 'Administrator' ) ? (
              <div style={{display: "flex", alignItems: "center", marginLeft: "20px", marginTop: "-15px"}}>
                {!subscriptionsData.some(subscription => subscription.id === creator_id) ? (
                  <div class="container-fluid justify-content-center" style={{marginBottom: "50px"}}>
                    <button onClick={handleSubscribeClick} class="btn btn-dark" style={{marginRight:"20px", marginLeft:"-20px"}}>Subscribe</button>
                  </div>
                ):(
                  <div class="container-fluid justify-content-center" style={{marginBottom: "50px"}}>
                    <button onClick={handleUnSubscribeClick} class="btn btn-danger" style={{marginRight:"20px", marginLeft:"-30px"}}>Subscribed</button>
                  </div>
                )}
                  <button onClick={handleDonateClick} class="btn btn-success" style={{marginRight:"80px", marginLeft: "-40px", marginBottom: "50px"}}>Donate</button>
                  <button class="btn btn-danger" style={{marginRight:"20px", marginBottom:"50px", marginLeft:"-55px"}} onClick={() => reportUser(userData.id)}>
                    <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" class="bi bi-flag" viewBox="0 0 16 16">
                      <path d="M14.778.085A.5.5 0 0 1 15 .5V8a.5.5 0 0 1-.314.464L14.5 8l.186.464-.003.001-.006.003-.023.009a12.435 12.435 0 0 1-.397.15c-.264.095-.631.223-1.047.35-.816.252-1.879.523-2.71.523-.847 0-1.548-.28-2.158-.525l-.028-.01C7.68 8.71 7.14 8.5 6.5 8.5c-.7 0-1.638.23-2.437.477A19.626 19.626 0 0 0 3 9.342V15.5a.5.5 0 0 1-1 0V.5a.5.5 0 0 1 1 0v.282c.226-.079.496-.17.79-.26C4.606.272 5.67 0 6.5 0c.84 0 1.524.277 2.121.519l.043.018C9.286.788 9.828 1 10.5 1c.7 0 1.638-.23 2.437-.477a19.587 19.587 0 0 0 1.349-.476l.019-.007.004-.002h.001M14 1.221c-.22.078-.48.167-.766.255-.81.252-1.872.523-2.734.523-.886 0-1.592-.286-2.203-.534l-.008-.003C7.662 1.21 7.139 1 6.5 1c-.669 0-1.606.229-2.415.478A21.294 21.294 0 0 0 3 1.845v6.433c.22-.078.48-.167.766-.255C4.576 7.77 5.638 7.5 6.5 7.5c.847 0 1.548.28 2.158.525l.028.01C9.32 8.29 9.86 8.5 10.5 8.5c.668 0 1.606-.229 2.415-.478A21.317 21.317 0 0 0 14 7.655V1.222z"/>
                    </svg>
                  </button>
              </div>
            ):(
              (userData.userType !== 'Administrator' &&
            <button class="btn btn-danger" style={{marginRight:"20px", marginTop:"10px", marginLeft:"15px"}} onClick={reportUser}>
              <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" class="bi bi-flag" viewBox="0 0 16 16">
                <path d="M14.778.085A.5.5 0 0 1 15 .5V8a.5.5 0 0 1-.314.464L14.5 8l.186.464-.003.001-.006.003-.023.009a12.435 12.435 0 0 1-.397.15c-.264.095-.631.223-1.047.35-.816.252-1.879.523-2.71.523-.847 0-1.548-.28-2.158-.525l-.028-.01C7.68 8.71 7.14 8.5 6.5 8.5c-.7 0-1.638.23-2.437.477A19.626 19.626 0 0 0 3 9.342V15.5a.5.5 0 0 1-1 0V.5a.5.5 0 0 1 1 0v.282c.226-.079.496-.17.79-.26C4.606.272 5.67 0 6.5 0c.84 0 1.524.277 2.121.519l.043.018C9.286.788 9.828 1 10.5 1c.7 0 1.638-.23 2.437-.477a19.587 19.587 0 0 0 1.349-.476l.019-.007.004-.002h.001M14 1.221c-.22.078-.48.167-.766.255-.81.252-1.872.523-2.734.523-.886 0-1.592-.286-2.203-.534l-.008-.003C7.662 1.21 7.139 1 6.5 1c-.669 0-1.606.229-2.415.478A21.294 21.294 0 0 0 3 1.845v6.433c.22-.078.48-.167.766-.255C4.576 7.77 5.638 7.5 6.5 7.5c.847 0 1.548.28 2.158.525l.028.01C9.32 8.29 9.86 8.5 10.5 8.5c.668 0 1.606-.229 2.415-.478A21.317 21.317 0 0 0 14 7.655V1.222z"/>
              </svg>
            </button>
              )
            )}
            {isDonating && (
              <div class="container-fluid justify-content-center" style={{marginTop:"20px", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#282828"}}>
                <TextField
                  label={<span style={{ color: 'white' }}>Amount</span>}
                  type="number"
                  style={{color: "white", width: "160px"}}
                  value={donateAmount}
                  onChange={(event) => handleDonateAmountChange(event)}
                  InputProps={{
                    inputProps: {
                      style: { textAlign: 'right', color: 'white' },
                    },
                    style: { color: 'white' },
                  }}
                />
                <button onClick={handleSendDonateClick} class="btn btn-success" style={{marginLeft: "10px"}}>Send</button>
                <button onClick={handleDonateCancelClick} class="btn btn-danger" style={{marginLeft: "10px"}}>Cancel</button>
              </div>
            )}
          </div>
          <div class="col-sm">
            <h3>Avatar Image</h3>
            <section class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
              <img key={userData.avatarImage} src = {userData.avatarImage+"?time=" + new Date()} alt="No avatar image"/>
            </section>
          </div>
          <div class="col-sm">
          {(userData.userType !== 'Simple'  && userData.userType !== 'Administrator' ) && (
            <div>
          <h2>Videos</h2>
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
            </div>
          )}
          { userData.userType !== 'Administrator' && (
            <div>
          <h2 style={{marginTop:"50px"}}> Playlists</h2>
            <section class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#333333"}}>
              <ul style={{padding:"0px"}}>
                {playlistsData.map(playlist => (
                  <div>
                  <li style={{listStyleType: "none"}}>
                  <div className="box" style={{width:"300px", height:"100px", backgroundSize:"cover", backgroundRepeat:"no-repeat", cursor: "pointer", backgroundPosition:"center", backgroundColor: '#FF4500'}}>
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
          )}
          </div>
        </div>
      </div>
      {showModal && (
        <div className="modal" tabIndex="-1" role="dialog" style={{ display: "block" }}>
          <div className="modal-dialog modal-dialog-centered" role="document">
            <div className="modal-content" style={{ backgroundColor: "black", color: "white" }}>
              <div className="modal-header">
                <h5 className="modal-title">Report this user</h5>
              </div>
              <div className="modal-body">
                <div className="form-group">
                  <label htmlFor="reasonInput">Reason:</label>
                  <textarea className="form-control" id="reasonInput" rows="3" value={reason} onChange={handleReasonChange}></textarea>
                </div>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-dark" onClick={handleCloseModal}>Close</button>
                <button type="button" className="btn btn-danger" onClick={handleReportSubmit}>Submit</button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
);
};
export default CreatorProfile;
