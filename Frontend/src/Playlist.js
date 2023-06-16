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
const REPORT_URL = '/ticket';

const Playlist = () => {

    const params = useParams();
    const { auth } = useContext(AuthContext);
    const [errMsg, setErrMsg] = useState('');
    const errRef = useRef();
    const navigate = useNavigate();

    const [showModal, setShowModal] = useState(false);
    const [reason, setReason] = useState('');

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
          withCredentials: false 
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
          withCredentials: false 
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
          withCredentials: false 
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
            withCredentials: false
          }
        ).then(() => {
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
              withCredentials: false //cred
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
            withCredentials: false 
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

    const reportPlaylist = () => {
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
          "targetId": playlist_id,
          "targetType": "Playlist",
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
            setErrMsg('Reporting playlist with id ' + playlist_id + ' Failed');
          }
          errRef.current.focus();
        });
      setShowModal(false);
      setReason('');
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
                            <div className="box2" style={{width:"280px", height:"150px", backgroundImage: `url(${video.thumbnail})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center", cursor: "pointer"}}>
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
            {!isPlaylistOwner && (
            <button class="btn btn-danger" style={{marginLeft:"750px", width:"32", height:"32"}} onClick={reportPlaylist}>
              <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" class="bi bi-flag" viewBox="0 0 16 16">
                <path d="M14.778.085A.5.5 0 0 1 15 .5V8a.5.5 0 0 1-.314.464L14.5 8l.186.464-.003.001-.006.003-.023.009a12.435 12.435 0 0 1-.397.15c-.264.095-.631.223-1.047.35-.816.252-1.879.523-2.71.523-.847 0-1.548-.28-2.158-.525l-.028-.01C7.68 8.71 7.14 8.5 6.5 8.5c-.7 0-1.638.23-2.437.477A19.626 19.626 0 0 0 3 9.342V15.5a.5.5 0 0 1-1 0V.5a.5.5 0 0 1 1 0v.282c.226-.079.496-.17.79-.26C4.606.272 5.67 0 6.5 0c.84 0 1.524.277 2.121.519l.043.018C9.286.788 9.828 1 10.5 1c.7 0 1.638-.23 2.437-.477a19.587 19.587 0 0 0 1.349-.476l.019-.007.004-.002h.001M14 1.221c-.22.078-.48.167-.766.255-.81.252-1.872.523-2.734.523-.886 0-1.592-.286-2.203-.534l-.008-.003C7.662 1.21 7.139 1 6.5 1c-.669 0-1.606.229-2.415.478A21.294 21.294 0 0 0 3 1.845v6.433c.22-.078.48-.167.766-.255C4.576 7.77 5.638 7.5 6.5 7.5c.847 0 1.548.28 2.158.525l.028.01C9.32 8.29 9.86 8.5 10.5 8.5c.668 0 1.606-.229 2.415-.478A21.317 21.317 0 0 0 14 7.655V1.222z"/>
              </svg>
            </button>
            )}
            {showModal && (
              <div className="modal" tabIndex="-1" role="dialog" style={{ display: "block" }}>
                <div className="modal-dialog modal-dialog-centered" role="document">
                  <div className="modal-content" style={{ backgroundColor: "black", color: "white" }}>
                    <div className="modal-header">
                      <h5 className="modal-title">Report this playlist</h5>
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
}
export default Playlist;