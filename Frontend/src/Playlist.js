import {useRef, useState, useEffect } from "react"
import AuthContext from "./context/AuthProvider"
import { useContext } from "react";
import axios from './api/axios';
import { FontAwesomeIcon} from "@fortawesome/react-fontawesome"
import { faCheckCircle } from '@fortawesome/free-solid-svg-icons';
import '@fortawesome/fontawesome-svg-core/styles.css';
import { useNavigate, useLocation, useParams } from 'react-router-dom';

const PLAYLIST_VIDEOS_URL = '/playlist/video';
const USER_PLAYLISTS_URL = '/playlist/user';
const PLAYLIST_DETAILS_URL = '/playlist/details'
const PLAYLIST_URL = '/playlist'
const USER_VIDEOS_URL = '/user/videos';

const Playlist = () => {

    const params = useParams();
    const { auth } = useContext(AuthContext);
    const [errMsg, setErrMsg] = useState('');
    const errRef = useRef();
    const navigate = useNavigate();

    const playlist_id = params.playlistid;

    const [videosData, setVideosData] = useState([]);
    const [allUserVideosData, setAllUserVideosData] = useState([]);
    const [playlistsData, setPlaylistsData] = useState([]);
    const [name, setName] = useState("");
    const [visibility, setVisibility] = useState(false);
    const [editedName, setEditedName] = useState("");
    const [editedVisibility, setEditedVisibility] = useState(false);
    const [isPlaylistOwner, setIsPlaylistOwner] = useState(false);
    const [editMode, setEditMode] = useState(false);
    const [submiting, setSubmiting] = useState(false);

    const nameRef = useRef();

    const [videosList, setVideosList] = useState([]);
    function videosListAddItem(element) {
        setVideosList(prevState => [...prevState, element]);
    }

    const [editedVideosList, setEditedVideosList] = useState([]);
    function editedVideosListAddItem(element) {
        setEditedVideosList(prevState => [...prevState, element]);
    }
    function editedVideosListRemoveElement(element) {
        const index = editedVideosList.indexOf(element);
        if (index !== -1) {
          const newList = [...editedVideosList];
          newList.splice(index, 1);
          setEditedVideosList(newList);
        }
    }

    const location = useLocation();

    useEffect(() => {
      localStorage.setItem("lastVisitedPage", location.pathname);
    })

    useEffect(() => {
        axios.get(PLAYLIST_VIDEOS_URL + "?id=" + playlist_id, 
        {
          headers: { 
            'Content-Type': 'application/json',
            "Authorization" : `Bearer ${auth?.accessToken}`
          },
          withCredentials: true 
        })
        .then(response => {
          setVideosData(response?.data?.videos);
          response?.data?.videos.forEach(video => {
            videosListAddItem(video.id);
            editedVideosListAddItem(video.id);
          });
          setName(response?.data.name);
          setEditedName(response?.data.name);
          setVisibility(response?.data.visibility);
          setEditedVisibility(response?.data.visibility);
        })
        .catch(error => {
          console.log("error: ", error);
        });

        axios.get(USER_PLAYLISTS_URL + "?id=" + auth?.id, {
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

    }, [auth?.accessToken, auth?.id, playlist_id]);

    useEffect(() => {
      axios.get(USER_VIDEOS_URL + "?id=" + auth?.id, 
        {
          headers: { 
            'Content-Type': 'application/json',
            "Authorization" : `Bearer ${auth?.accessToken}`
          },
          withCredentials: true 
        })
        .then(response => {
          setAllUserVideosData(response?.data?.videos);
        })
        .catch(error => {
          console.log("error: ", error);
        });
    }, [auth?.id, auth?.accessToken])

    useEffect(() => {
        if(playlistsData.some(playlist => playlist.id === playlist_id)){
          setIsPlaylistOwner(true);
        }
        else{
          setIsPlaylistOwner(false);
        }
    }, [playlistsData, playlist_id])

    const handleVideoAddClick = (id) => {
        editedVideosListAddItem(id);
    };

    const handleVideoRemoveClick = (id) => {
        editedVideosListRemoveElement(id);
    }

    const handleVideoClick = (id) => {
        navigate(`/videoplayer/${id}`);
    }

    const handleEditClick = () => {
        setEditMode(true);
    }

    const handleDeleteClick = () => {
        axios.delete(PLAYLIST_DETAILS_URL + "?id=" + playlist_id,
          {
            headers: { 
              'Content-Type': 'application/json',
              "Authorization" : `Bearer ${auth?.accessToken}`
            },
            withCredentials: true
          }
        ).then(response => {
          navigate('/profile');
        }).catch(err => {
          if(!err?.response) {
              setErrMsg('No Server Response')
          } else if(err.response?.status === 400) {
              setErrMsg('Bad request');
          } else if(err.response?.status === 401){
              setErrMsg('Unauthorized');
          } else if(err.response?.status === 403){
              setErrMsg('Forbidden');
          } else if(err.response?.status === 404){
            setErrMsg('Not found');
          } else {
              setErrMsg('Deleting playlist failed');
          }
        });
    }

    const handleCancelClick = () => {
        setEditedName(name);
        setEditedVisibility(visibility);
        setEditMode(false);
    }

    const handleSubmit = async (e) => {
      e.preventDefault();
        try {
          setSubmiting(true);
          const response = await axios.put(PLAYLIST_DETAILS_URL + "?id=" + playlist_id,
            JSON.stringify({
              name: editedName,
              visibility: editedVisibility?"Public":"Private"
            }),
            {
              headers: { 
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${auth?.accessToken}`
              },
              withCredentials: true //cred
            }
          );
          setName(response?.data?.name);
          setEditedName(response?.data?.name);
          setVisibility(response?.data?.visibility);
          setEditedVisibility(response?.data?.visibility);
          const requests = await editedVideosList.map( (video_id) => {
            if(!videosList.some(vid_id => vid_id === video_id)){
                axios.post(PLAYLIST_URL + "/" + playlist_id + "/" + video_id, {},
                {
                  headers: { 
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${auth?.accessToken}`
                  },
                  withCredentials: true //cred
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
                    setErrMsg('Adding video with id ' + video_id + ' to playlist Failed');
                  }
                  errRef.current.focus();
                });
            }
          });
          const requests2 = await videosList.map( (video_id) => {
            if(!editedVideosList.some(vid_id => vid_id === video_id)){
                axios.delete(PLAYLIST_URL + "/" + playlist_id + "/" + video_id,
                {
                  headers: { 
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${auth?.accessToken}`
                  },
                  withCredentials: true //cred
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
                    setErrMsg('Deleting video with id ' + video_id + 'Failed');
                  }
                  errRef.current.focus();
                });
            }
          });
          Promise.all([...requests, ...requests2]).then(() => {
      
          axios.get(PLAYLIST_VIDEOS_URL + "?id=" + playlist_id, 
          {
            headers: { 
              'Content-Type': 'application/json',
              "Authorization" : `Bearer ${auth?.accessToken}`
            },
            withCredentials: true 
          })
          .then(response => {
            setVideosData(response?.data?.videos);
            setVideosList([]);
            setEditedVideosList([]);
            response?.data?.videos.forEach(video => {
              videosListAddItem(video.id);
              editedVideosListAddItem(video.id);
            });
            setName(response?.data.name);
            setEditedName(response?.data.name);
            setVisibility(response?.data.visibility);
            setEditedVisibility(response?.data.visibility);
            setSubmiting(false);
            setEditMode(false);
            window.location.reload();
          })
          .catch(error => {
            console.log("error: ", error);
          });
        })
        } catch (err) {
          if (!err?.response) {
              setErrMsg('No Server Response');
          } else if (err.response?.status === 400) {
              setErrMsg('Bad request');
          } else if (err.response?.status === 401) {
            setErrMsg('Unauthorized');
          }else {
              setErrMsg('Playlist creation Failed');
          }
          errRef.current.focus();
        }
    };


    return (
        <div style={{marginTop: "200px"}} class="container">
            {!editMode ? (
              <div class="row">
                <h1 class="display-1" style={{textAlign:"center" }}>{name}</h1>
                <div class ="mt-2 row">
                  <div class="col-sm">
                    <h2 style={{textAlign:"center" }} >Videos on this playlist:</h2>
                    <section class="container-fluid justify-content-center" style={{marginTop:"20px", 
                      color:"white", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#333333"}}>
                      <ul style={{padding:"0px"}}>
                        {
                        videosData.map(video => (
                          <div>
                          <li style={{listStyleType: "none"}}>
                          <div className="box" style={{width:"300px", height:"170px"}}>
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
                        )).reverse()}
                      </ul>
                    </section>
                   </div>
                </div>
                {isPlaylistOwner &&(
                  <div>
                    <div class="container-fluid justify-content-center" style={{fontSize:"18px", marginTop:"10px", marginBottom:"200px", textAlign:"center"}}>
                      This playlist is {visibility}.
                    </div>
                    <div class="container-fluid justify-content-center" style={{marginBottom: "50px", marginTop:"-200px"}}>
                      <button onClick={handleEditClick} class="btn btn-dark" style={{marginRight:"20px", marginLeft:"400px"}}>Edit playlist</button>
                      <button onClick={handleDeleteClick} class="btn btn-danger" style={{marginRight:"20px"}}>Delete playlist</button>
                    </div>
                  </div>
                )}
              </div>
            ) : (
                <div style={{marginTop: "200px"}} class="col-xs-1" align="center"> 
                <h1 class="display-3" style={{marginBottom:"60px"}}>Upload your video</h1>
                <section class="container-fluid justify-content-center mb-5" style={{marginTop:"20px", 
                  color:"white", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#333333"}}>
                    <p ref={errRef} className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">{errMsg}</p>
                    <form onSubmit={handleSubmit}>
                        <label htmlFor="name">
                            Name:
                        </label>
                        <input
                            type="text"
                            id="name"
                            ref={nameRef}
                            autoComplete="off"
                            onChange={(e) => setEditedName(e.target.value)}
                            value={editedName}
                            required
                            aria-describedby="uidnote"
                        />
                        <label htmlFor="terms">
                            <input
                                type="checkbox"
                                id="terms"
                                onChange={() => setEditedVisibility(!visibility)}
                                checked={editedVisibility}
                            />
                            <text> I want my playlist to be public</text>
                        </label>
                        <label htmlFor="name">
                            Select videos you want on your playlist:
                        </label>
                        {videosData.map(video => (
                            video.processingProgress ==='Ready' && 
                            !allUserVideosData.some(v => v.id === video.id) && (
                              !editedVideosList.includes(video.id)?(
                                  <div>
                                  <li style={{listStyleType: "none"}}>
                                  <div className="box" style={{width:"300px", height:"170px"}}>
                                    <div className="box2" style={{width:"280px", height:"150px", backgroundImage: `url(${video.thumbnail})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                                        <table style={{backgroundColor: "transparent"}}>
                                            <tr style={{backgroundColor: "transparent"}}>
                                              <div className="movie_title" style={{width:"280px", height:"60px", fontSize:"10px", marginTop:"0", whiteSpace: 'nowrap', overflow: 'hidden', position:"center", color:"black", backgroundColor:"transparent" }}>
                                                <h2 class="text-with-stroke" style={{backgroundColor: "transparent"}}>{video.title}</h2>
                                              </div>
                                            </tr>
                                            <tr style={{backgroundColor: "transparent"}}>
                                              <div className="movie_thumbnail" style={{width:"280px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" } } onClick={() => handleVideoAddClick(video.id)} />
                                            </tr>
                                        </table>
                                      </div> 
                                    </div>
                                    </li>
                                  </div>
                              ):(
                                  <div>
                                  <li style={{listStyleType: "none"}}>
                                  <div className="box" style={{width:"300px", height:"170px", backgroundColor: "rgba(0, 255, 0, 0.3)"}}>
                                    <div className="box2" style={{width:"280px", height:"150px", backgroundImage: `url(${video.thumbnail})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                                        <table style={{backgroundColor: "rgba(0, 255, 0, 0.2)"}}>
                                            <tr style={{backgroundColor: "transparent"}}>
                                              <div className="movie_title" style={{width:"280px", height:"60px", fontSize:"10px", marginTop:"0", whiteSpace: 'nowrap', overflow: 'hidden', position:"center", color:"black", backgroundColor:"transparent" }}>
                                                <h2 class="text-with-stroke" style={{backgroundColor: "transparent"}}>{video.title}</h2>
                                              </div>
                                            </tr>
                                            <tr style={{backgroundColor: "transparent"}}>
                                              <div className="movie_thumbnail" style={{width:"280px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" } } onClick={() => handleVideoRemoveClick(video.id)}>
                                                    <FontAwesomeIcon icon={faCheckCircle} size="5x" width="40" height="40" class="bi bi-play-circle-fill" style={{ color: 'green', borderRadius: "100%", marginBottom: "40px" }} />
                                              </div>
                                            </tr>
                                        </table>
                                      </div> 
                                    </div>
                                    </li>
                                  </div>
                              )
                            )
                          )).reverse()
                        }
                        {allUserVideosData.map(video => (
                          video.processingProgress ==='Ready' && (
                            !editedVideosList.includes(video.id)?(
                                <div>
                                <li style={{listStyleType: "none"}}>
                                <div className="box" style={{width:"300px", height:"170px"}}>
                                  <div className="box2" style={{width:"280px", height:"150px", backgroundImage: `url(${video.thumbnail})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                                      <table style={{backgroundColor: "transparent"}}>
                                          <tr style={{backgroundColor: "transparent"}}>
                                            <div className="movie_title" style={{width:"280px", height:"60px", fontSize:"10px", marginTop:"0", whiteSpace: 'nowrap', overflow: 'hidden', position:"center", color:"black", backgroundColor:"transparent" }}>
                                              <h2 class="text-with-stroke" style={{backgroundColor: "transparent"}}>{video.title}</h2>
                                            </div>
                                          </tr>
                                          <tr style={{backgroundColor: "transparent"}}>
                                            <div className="movie_thumbnail" style={{width:"280px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" } } onClick={() => handleVideoAddClick(video.id)} />
                                          </tr>
                                      </table>
                                    </div> 
                                  </div>
                                  </li>
                                </div>
                            ):(
                                <div>
                                <li style={{listStyleType: "none"}}>
                                <div className="box" style={{width:"300px", height:"170px", backgroundColor: "rgba(0, 255, 0, 0.3)"}}>
                                  <div className="box2" style={{width:"280px", height:"150px", backgroundImage: `url(${video.thumbnail})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                                      <table style={{backgroundColor: "rgba(0, 255, 0, 0.2)"}}>
                                          <tr style={{backgroundColor: "transparent"}}>
                                            <div className="movie_title" style={{width:"280px", height:"60px", fontSize:"10px", marginTop:"0", whiteSpace: 'nowrap', overflow: 'hidden', position:"center", color:"black", backgroundColor:"transparent" }}>
                                              <h2 class="text-with-stroke" style={{backgroundColor: "transparent"}}>{video.title}</h2>
                                            </div>
                                          </tr>
                                          <tr style={{backgroundColor: "transparent"}}>
                                            <div className="movie_thumbnail" style={{width:"280px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" } } onClick={() => handleVideoRemoveClick(video.id)}>
                                                  <FontAwesomeIcon icon={faCheckCircle} size="5x" width="40" height="40" class="bi bi-play-circle-fill" style={{ color: 'green', borderRadius: "100%", marginBottom: "40px" }} />
                                            </div>
                                          </tr>
                                      </table>
                                    </div> 
                                  </div>
                                  </li>
                                </div>
                            )
                          )
                        )).reverse()}
                        
                        <button class="btn btn-dark" disabled={(!editedName || submiting) ? true : false}>Submit</button>
                    </form>
                    <button class="btn btn-dark" onClick={handleCancelClick}>Cancel</button>
                 </section>
            </div>
            )}
        </div>
    );
}
export default Playlist;