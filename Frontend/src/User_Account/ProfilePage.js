import {useRef, useState, useEffect } from "react";
import AuthContext from "../context/AuthProvider";
import useAuth from '../hooks/useAuth';
import { useContext } from "react";
import axios from '../api/axios';
import {faCheck, faTimes, faInfoCircle  } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import '@fortawesome/fontawesome-svg-core/styles.css';
import { config } from '@fortawesome/fontawesome-svg-core';
import { useNavigate, useLocation } from 'react-router-dom';
import {cookies} from '../App';
import TextField from "@material-ui/core/TextField";

config.autoAddCss = false;

const PROFILE_URL = '/user';
const USER_VIDEOS_URL = '/user/videos';
const USER_PLAYLISTS_URL = '/playlist/user';
const CREATE_PLAYLIST_URL = '/addplaylist';
const ADD_VIDEO_URL = '/addvideo';
const USER_REGEX = /^[A-z][A-z0-9-_]{3,23}$/;
const NAME_REGEX = /^[A-Z][a-z]{2,17}$/;
const EMAIL_REGEX = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
const DONATE_WITHDRAW_URL = '/donate/withdraw';


const ProfilePage = () => {

  const { setAuth } = useAuth();
  const { auth } = useContext(AuthContext);
  const [errMsg, setErrMsg] = useState('');
  const errRef = useRef();
  const navigate = useNavigate();

  const [data, setData] = useState(null);

  const [videosData, setVideosData] = useState([]);
  const [playlistsData, setPlaylistsData] = useState([]);

  const [isWithdrawing, setIsWithdrawing] = useState(false);
  const [withdrawAmount, setWithdrawAmount] = useState(1);

  const location = useLocation();

  useEffect(() => {
    localStorage.setItem("lastVisitedPage", location.pathname);
  })

  useEffect(() => {
    axios.get(PROFILE_URL, {
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
    });
  
    axios.get(USER_VIDEOS_URL + "?id=" + auth?.id, {
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
  
    axios.get(USER_PLAYLISTS_URL + "?id=" + auth?.id, {
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
  }, [auth?.accessToken, auth?.id]);

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

  const userRef = useRef();
  const nameRef = useRef();
  const surnameRef = useRef();
  const [user, setUser] = useState('');
  const [validNickname, setValidNickname] = useState(false);
  const [userFocus, setUserFocus] = useState(false)
  const [name, setName] = useState("");
  const [validName, setValidName] = useState(false);
  const [nameFocus, setNameFocus] = useState(false)
  const [surname, setSurname] = useState('');
  const [validSurname, setValidSurname] = useState(false);
  const [surnameFocus, setSurnameFocus] = useState(false)
  const [email, setEmail] = useState('');
  const [validEmail, setValidEmail] = useState(false);
  const [profile_picture, setProfile_picture] = useState(null);
  const [profile_picture_name, setProfile_picture_name] = useState('');
  const [validprofile_picture, setValidprofile_picture] = useState(false);
  const [wrong_profile_picture, setWrong_profile_picture] = useState(false);

  const [userType, setUserType] = useState('');



  useEffect(() => {
    setName(userData.firstName);
  }, [userData.firstName]);
  useEffect(() => {
    setSurname(userData.lastName);
  }, [userData.lastName]);
  useEffect(() => {
    setUser(userData.nickname);
  }, [userData.nickname]);
  useEffect(() => {
    setEmail(userData.email);
  }, [userData.email]);
  useEffect(() => {
    setUserType(userData.userType);
  }, [userData.userType]);

  useEffect(() => {
    setValidNickname(USER_REGEX.test(user));
  }, [user])

  useEffect(() => {
      setErrMsg('');
  }, [user])

  useEffect(() => {
      setValidName(NAME_REGEX.test(name));
  }, [name])

  useEffect(() => {
      setValidSurname(NAME_REGEX.test(surname));
  }, [surname])

  useEffect(() => {
      setValidEmail(EMAIL_REGEX.test(email));
  }, [email])

  const [editMode, setEditMode] = useState(false);

  const handle_picture = (event) => {

    const file = event.target.files[0];
    const maxSize = 5 * 1024 * 1024; // 5 MB

    if (file && file.size <= maxSize) {
        setProfile_picture(file);
        setProfile_picture_name(file.name);
        setValidprofile_picture(true);
        setWrong_profile_picture(false);
    } else {
        setProfile_picture(null);
        setProfile_picture_name('');
        setValidprofile_picture(false);
        setWrong_profile_picture(true);
        alert("Choose a file format .jpg or .png with a maximum size of 5MB.");
    }
  }

  const handelAddNewVideoClick = () => {
    navigate(ADD_VIDEO_URL);
  }

  const handleEditClick = () => {
    setEditMode(true);
  };

  const handleCancelClick = () => {
    setEditMode(false);
    setName(userData.firstName);
    setSurname(userData.lastName);
    setUser(userData.nickname);
    setEmail(userData.email);
    setUserType(userData.userType);
  };

  const handleDeleteClick = async () => {
    const result = window.confirm("Are you sure you want to delete your account? Approval will delete all your uploaded videos and you will lose all your subscribers. Do you want to continue?");
    if (result) {
      try{
        await axios.delete(PROFILE_URL + "?id=" + auth?.id,
            {
              headers: { 
                'Content-Type': 'application/json',
                "Authorization" : `Bearer ${auth?.accessToken}`
              },
              withCredentials: false
            }
        );
      }catch(err){
        if(!err?.response) {
            setErrMsg('No Server Response')
        } else if(err.response?.status === 400) {
            setErrMsg('Login Failed');
        } else if(err.response?.status === 404){
            setErrMsg('Account does not exist');
        } else if(err.response?.status === 401 ){
            setErrMsg('Incorrect password');
        } else {
            setErrMsg('Login Failed');
        }
        errRef.current.focus();
      }
      setAuth({});
      cookies.remove("accessToken");
      alert("Your account has been deleted.");
      navigate('/register', {replace: true});
    }
  };
  
  const handleCreatorClick = async () => {
    setUserData({
      firstName: data?.name,
      lastName: data?.surname,
      nickname: data?.nickname,
      email: data?.email,
      accountBalance: data?.accountBalance,
      avatarImage: data?.avatarImage,
      userType: data?.userType,
    });
    let changeType = '';
    if(userData.userType === 'Simple')
      changeType = "Creator";
    else if(userData.userType === 'Creator')
    {
      const result = window.confirm(
        "Are you sure you don't want to be a creator any longer? Approval will delete all your uploaded videos and you will lose all your subscribers. Do you want to continue?"
        );
      if (!result) {
        return;
      }
      changeType = "Simple";
    }
    else
      return;
    let base64data = null;
    if(userData.avatarImage){
      const imageUrl = userData.avatarImage+"?time="+new Date();
      const response = await fetch(imageUrl);
      const blob = await response.blob();
      const reader = new FileReader();
      reader.readAsDataURL(blob);
      reader.onloadend = () => {
        base64data = reader.result.split(",")[1];
      }
    }
    else
    {
      base64data = null;
    }
      setTimeout(async () => {
      try{
        await axios.put(PROFILE_URL,
        JSON.stringify({
          nickname: userData.nickname, 
          name: userData.firstName, 
          surname: userData.lastName,
          userType: changeType,
          avatarImage: base64data,
        }),
        {
            headers: { 
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${auth?.accessToken}`
            },
            withCredentials: false //cred
        }
      );
      } catch (err) {
        if (!err?.response) {
          setErrMsg('No Server Response');
        } else if (err.response?.status === 401) {
          setErrMsg('Unauthorized');
        } else {
          setErrMsg('Data Change Failed');
        }
        errRef.current.focus();
      }
      setAuth({});
      cookies.remove("accessToken");
      location.state.form.pathname = location.pathname;
      navigate('/login');
      }, 100);
}

  const handleSubmit = async (e) => {
    e.preventDefault();
    const v1 = USER_REGEX.test(user);
    const v2 = NAME_REGEX.test(name)
    const v3 = NAME_REGEX.test(surname)
    const v4 = EMAIL_REGEX.test(email)
    if (!v1 || !v2 || !v3 || !v4) {
        setErrMsg("Invalid Entry");
        return;
    }
    try {
      let response;
      if(validprofile_picture)
      {
        const reader = new FileReader();
        await reader.readAsDataURL(profile_picture);
        let base64String;
        reader.onload = () => {
          base64String = reader.result.split(",")[1];
        };
        setTimeout(async () => {
        response = await axios.put(PROFILE_URL,
            JSON.stringify({
              nickname: user, 
              name: name, 
              surname: surname,
              userType: auth?.roles === "Simple" ? 1 : (auth?.roles === "Creator" ? 2 : 3),
              avatarImage: base64String
            }),
            {
                headers: { 
                  'Content-Type': 'application/json',
                  'Authorization': `Bearer ${auth?.accessToken}`
                },
                withCredentials: false //cred
            }
        );
        setData(response?.data);
        setUserData({
          firstName: data?.name,
          lastName: data?.surname,
          nickname: data?.nickname,
          email: data?.email,
          accountBalance: data?.accountBalance,
          avatarImage: data?.avatarImage,
          userType: data?.userType,
        });
        handleCancelClick();
        window.location.reload();
      }, 100);
      }
      else
      {
        let base64data = null;
        if(userData.avatarImage){
          const imageUrl = userData.avatarImage+"?time="+new Date();
          const response = await fetch(imageUrl);
          const blob = await response.blob();
          const reader = new FileReader();
          reader.readAsDataURL(blob);
          reader.onloadend = () => {
            base64data = reader.result.split(",")[1];
          }
        }
        else
        {
          base64data = null;
        }
        setTimeout(async () => {
          response = await axios.put(PROFILE_URL,
            JSON.stringify({
              nickname: user, 
              name: name, 
              surname: surname,
              userType: auth?.roles === "Simple" ? 1 : (auth?.roles === "Creator" ? 2 : 3),
              avatarImage: base64data
            }),
            {
                headers: { 
                  'Content-Type': 'application/json',
                  'Authorization': `Bearer ${auth?.accessToken}`
                },
                withCredentials: false //cred
            }
          );
          setData(response?.data);
          setUserData({
            firstName: data?.name,
            lastName: data?.surname,
            nickname: data?.nickname,
            email: data?.email,
            accountBalance: data?.accountBalance,
            avatarImage: data?.avatarImage,
            userType: data?.userType,
          });
          handleCancelClick();
        }, 100);
      }
    } catch (err) {
        if (!err?.response) {
            setErrMsg('No Server Response');
        } else if (err.response?.status === 401) {
            setErrMsg('Unauthorized');
        } else {
            setErrMsg('Data Change Failed');
        }
        errRef.current.focus();
    }
}

const handleVideoClick = (id) => {
  navigate(`/videoplayer/${id}`);
}

const handelCreateNewPlaylistClick = async (e) => {
  navigate(CREATE_PLAYLIST_URL);
}

const handelPlaylistClick = (id) => {
  navigate(`/playlist/${id}`);
}

const handleWithdrawClick = () => {
  setWithdrawAmount(1);
  setIsWithdrawing(!isWithdrawing);
  if(userData.accountBalance === 0){
    setWithdrawAmount(0);
  }
}

const handleWithdrawAmountChange = (event) => {
  const newAmount = parseInt(event.target.value);
  if (newAmount >= 1 && newAmount <= userData.accountBalance) {
    setWithdrawAmount(newAmount);
  }
};

const handleWithdrawCancelClick = () => {
  setWithdrawAmount(1);
  setIsWithdrawing(false);
}

const handleWithdrawConfirmClick = () => {
  axios.post(DONATE_WITHDRAW_URL, {},
      {
        params: {
          amount: withdrawAmount
        },
        headers: { 
          'Content-Type': 'application/json',
          "Authorization" : `Bearer ${auth?.accessToken}`
        },
        withCredentials: false
      }
    ).then(() => {
      setWithdrawAmount(1);
      setIsWithdrawing(false);
      axios.get(PROFILE_URL, {
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
      });
    }).catch(err => {
      if(!err?.response) {
          setErrMsg('No Server Response')
      } else if(err.response?.status === 400) {
          setErrMsg('Bad request');
      } else if(err.response?.status === 403){
          setErrMsg('Forbidden');
      } else {
          setErrMsg('Getting metadata failed');
      }
    });
}

return (
  <div style={{marginTop: "200px", marginBottom: "50px"}} class="container">
    {!editMode ? (
      <div class="row mx-5">
        <h1 class="display-3">{userData.nickname}</h1>
        <div class ="mt-2 row">
          <div class="col-sm">
            <h3>Data</h3>
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
              <div>
                <button onClick={handleEditClick} class="btn btn-dark">Edit</button>
              </div>
            </section>
          </div>
          <div class="col-sm">
            <h3>Avatar Image</h3>
            <section class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
              <img key={userData.avatarImage} src = {userData.avatarImage+"?time="+new Date()} alt="No avatar image"/>
            </section>
          </div>
          <div class="col-sm">
          <h3>Account</h3>
          <div class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", marginBottom:"100px", paddingTop:"0px", backgroundColor:"#333333"}}>
              <div className="row">
                <div className="col">
                  {userType==='Creator' && (
                    <button class="btn btn-danger" style={{ whiteSpace: "nowrap" }} onClick={handleCreatorClick}>Stop being a creator</button>
                  )
                  }
                </div>
              </div>
              <div className="row">
                <div className="col">
                  <button class="btn btn-danger mb-4" onClick={handleDeleteClick}>Delete account</button>
                </div>
              </div>
              { userType !== 'Administrator' && (
              <div className="row">
                <div className="col">
                <button onClick={handleWithdrawClick} class="btn btn-success mb-4">Withdraw money</button>
                </div>
              </div>
              )}
            </div>
            {isWithdrawing && (
              <div class="container-fluid justify-content-center" style={{marginTop:"20px", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#282828"}}>
                <div style={{ display: 'flex', alignItems: 'center' }}>
                  <p style={{ color: 'white' }}>Your current balance: {userData.accountBalance}</p>
                </div>
                <TextField
                  label={<span style={{ color: 'white' }}>Amount</span>}
                  type="number"
                  style={{color: "white", width: "160px"}}
                  value={withdrawAmount}
                  onChange={(event) => handleWithdrawAmountChange(event)}
                  InputProps={{
                    inputProps: {
                      style: { textAlign: 'right', color: 'white' },
                    },
                    style: { color: 'white' },
                  }}
                />
                <button onClick={handleWithdrawConfirmClick} class="btn btn-success" style={{marginLeft: "10px"}}>Withdraw</button>
                <button onClick={handleWithdrawCancelClick} class="btn btn-danger" style={{marginLeft: "10px"}}>Cancel</button>
              </div>
            )}
            </div>
            <div class="row" style={{marginTop:"30px"}}>
            {userType==='Creator' && (
          <div class="col-sm">
            <h3>Your Videos</h3>
            <section class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#333333"}}>
              <ul style={{padding:"0px"}}>
                <li style={{listStyleType: "none"}}>
                  <div className="box" style={{width:"300px", height:"170px", cursor: "pointer"}}>
                    <div className="box2" style={{width:"280px", height:"150px"}} onClick={handelAddNewVideoClick}>
                        <div className="movie_thumbnail" style={{width:"280px", height:"150px"}}>
                          <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" fill="white" class="bi bi-plus" viewBox="0 0 16 16">
                            <path d="M8 4a.5.5 0 0 1 .5.5v3h3a.5.5 0 0 1 0 1h-3v3a.5.5 0 0 1-1 0v-3h-3a.5.5 0 0 1 0-1h3v-3A.5.5 0 0 1 8 4z"/>
                          </svg>
                        </div>
                      </div> 
                  </div>
                </li>
                {videosData.map(video => (
                  video.processingProgress ==='Ready' && (
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
                {(videosData.some(video => video.processingProgress !== 'Ready')) && (
                  <div>
                    <h2 style={{color: 'red', textAlign: 'center'}}>Not ready videos</h2>
                      {videosData.map(video => (
                        video.processingProgress !== 'Ready' && (
                        <div>
                        <li style={{listStyleType: "none"}}>
                        <div className="box" style={{width:"300px", height:"170px", cursor: "pointer" }}>
                          <div className="box2" style={{width:"280px", height:"150px", backgroundImage: `url(${video.thumbnail})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                              <table style={{backgroundColor: "transparent"}}>
                                  <tr style={{backgroundColor: "transparent", position:"center"}}>
                                  <div className="movie_title" style={{width:"280px", height:"60px", fontSize:"10px", marginTop:"0", whiteSpace: 'nowrap', overflow: 'hidden', position:"center", color:"black", backgroundColor:"transparent" }}>
                                    <h2 class="text-with-stroke" style={{backgroundColor: "transparent", position:"center"}}>{video.title}</h2>
                                  </div>
                                  </tr>
                                  <tr style={{backgroundColor: "transparent"}}>
                                  <div className="movie_thumbnail" style={{width:"280px", height:"60px", backgroundColor: "transparent", backgroundSize:"cover", backgroundRepeat:"no-repeat", backgroundPosition:"center" } } onClick={() => handleVideoClick(video.id)}>
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
                  </div>
                )}
              </ul>
            </section>
            </div>
            )}
          { userType !== 'Administrator' && (
          <div class="col-sm">
          <h3>Your Playlists</h3>
            <section class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#333333"}}>
              <ul style={{padding:"0px"}}>
                <li style={{listStyleType: "none"}}>
                  <div className="box" style={{width:"300px", height:"100px", cursor: "pointer"}}>
                    <div className="box2" style={{width:"280px", height:"80px"}} onClick={handelCreateNewPlaylistClick}>
                        <div className="movie_thumbnail" style={{width:"280px", height:"80px"}}>
                          <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" fill="white" class="bi bi-plus" viewBox="0 0 16 16">
                            <path d="M8 4a.5.5 0 0 1 .5.5v3h3a.5.5 0 0 1 0 1h-3v3a.5.5 0 0 1-1 0v-3h-3a.5.5 0 0 1 0-1h3v-3A.5.5 0 0 1 8 4z"/>
                          </svg>
                        </div>
                      </div>
                  </div>
                </li>
                {
                playlistsData.map(playlist => (
                  <div>
                  <li style={{listStyleType: "none"}}>
                  <div className="box" style={{width:"300px", height:"100px", backgroundSize:"cover", cursor: "pointer", backgroundRepeat:"no-repeat", backgroundPosition:"center", backgroundColor: '#FF4500'}}>
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
          { userType!=='Creator' && userType!=='Administrator' && (
            <div class="col-sm">
              <button onClick={handleCreatorClick}>Become Creator</button>
            </div>
          )}
          </div> 
        </div>
      </div>
    ) : (
      <div style={{marginTop: "200px"}} class="col-xs-1" align="center"> 
        <h1 class="display-3 mb-5">Change Data</h1>
        <section class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", marginBottom:"100px", paddingTop:"0px", backgroundColor:"#333333"}}>
            <p ref={errRef} className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">{errMsg}</p>
            <form onSubmit={handleSubmit}>
                        <label htmlFor="name">
                            Name:
                            <FontAwesomeIcon icon={faCheck} className={validName ? "valid" : "hide"} />
                            <FontAwesomeIcon icon={faTimes} className={validName || !name ? "hide" : "invalid"} />
                        </label>
                        <input
                            type="text"
                            id="name"
                            ref={nameRef}
                            autoComplete="off"
                            onChange={(e) => setName(e.target.value)}
                            value={name}
                            required
                            aria-invalid={validName ? "false" : "true"}
                            aria-describedby="uidnote"
                            onFocus={() => setNameFocus(true)}
                            onBlur={() => setNameFocus(false)}
                        />
                        <p id="uidnote" className={nameFocus && name && !validName ? "instructions" : "offscreen"}>
                            <FontAwesomeIcon icon={faInfoCircle} />
                            3 to 18 characters.<br />
                            Must begin with a capital letter.<br />
                            Only letters allowed.
                        </p>

                        <label htmlFor="surname">
                            Surname:
                            <FontAwesomeIcon icon={faCheck} className={validSurname ? "valid" : "hide"} />
                            <FontAwesomeIcon icon={faTimes} className={validSurname || !surname ? "hide" : "invalid"} />
                        </label>
                        <input
                            type="text"
                            id="surname"
                            ref={surnameRef}
                            autoComplete="off"
                            onChange={(e) => setSurname(e.target.value)}
                            value={surname}
                            required
                            aria-invalid={validSurname ? "false" : "true"}
                            aria-describedby="uidnote"
                            onFocus={() => setSurnameFocus(true)}
                            onBlur={() => setSurnameFocus(false)}
                        />
                        <p id="uidnote" className={surnameFocus && surname && !validSurname ? "instructions" : "offscreen"}>
                            <FontAwesomeIcon icon={faInfoCircle} />
                            3 to 18 characters.<br />
                            Must begin with a capital letter.<br />
                            Only letters allowed.
                        </p>

                        <label htmlFor="username">
                            Nickname:
                            <FontAwesomeIcon icon={faCheck} className={validNickname ? "valid" : "hide"} />
                            <FontAwesomeIcon icon={faTimes} className={validNickname || !user ? "hide" : "invalid"} />
                        </label>
                        <input
                            type="text"
                            id="username"
                            ref={userRef}
                            autoComplete="off"
                            onChange={(e) => setUser(e.target.value)}
                            value={user}
                            required
                            aria-invalid={validNickname ? "false" : "true"}
                            aria-describedby="uidnote"
                            onFocus={() => setUserFocus(true)}
                            onBlur={() => setUserFocus(false)}
                        />
                        <p id="uidnote" className={userFocus && user && !validNickname ? "instructions" : "offscreen"}>
                            <FontAwesomeIcon icon={faInfoCircle} />
                            4 to 24 characters.<br />
                            Must begin with a letter.<br />
                            Letters, numbers, underscores, hyphens allowed.
                        </p>

                        <label htmlFor="profile_picture">
                            New Profile Picture (Optional):
                            <FontAwesomeIcon icon={faCheck} className={validprofile_picture && profile_picture ? "valid" : "hide"} />
                            <FontAwesomeIcon icon={faTimes} className={!wrong_profile_picture ? "hide" : "invalid"} /> {/* validprofile_picture || !profile_picture */}
                        </label>
                        <input
                            class="btn btn-dark"
                            type="file"
                            accept="image/*"
                            id="profile_picture"
                            onChange={handle_picture}
                            defaultValue={profile_picture_name}
                            aria-describedby="confirmnote"
                        />
                        <p id="confirmnote" className={!validprofile_picture ? "instructions" : "offscreen"}> {/*profile_pictureFocus && */ }
                            <FontAwesomeIcon icon={faInfoCircle} />
                            Must be image up to 5 MB!
                        </p>

                        <button class="btn btn-dark" disabled={!validNickname || !validName || !validSurname || !validEmail ? true : false}>Submit</button>
                    </form>
                    <button class="btn btn-dark" onClick={handleCancelClick}>Cancel</button>
        </section>
      </div>
    )}
    </div>
);
};
export default ProfilePage;