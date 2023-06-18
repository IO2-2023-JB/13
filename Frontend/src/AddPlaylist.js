import axios from './api/axios';
import { FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import { faCheckCircle } from '@fortawesome/free-solid-svg-icons';
import { useRef, useState, useEffect } from "react";
import AuthContext from "./context/AuthProvider";
import { useContext } from "react";
import { useNavigate, useLocation } from 'react-router-dom';

const PLAYLIST_DETAILS_URL = '/playlist/details';
const PLAYLIST_URL ='/playlist';
const USER_VIDEOS_URL = '/user/videos';

const AddPlaylist = () => {

    const { auth } = useContext(AuthContext);
    const navigate = useNavigate();
    const location = useLocation();

    const [name, setName] = useState("");
    const [visibility, setVisibility] = useState(false);
    const [videosData, setVideosData] = useState([]);
    const [videosList, setVideosList] = useState([]);
    const [submiting, setSubmiting] = useState(false);
    function videosListAddItem(element) {
        setVideosList(prevState => [...prevState, element]);
      }
    function videosListRemoveElement(element) {
        const index = videosList.indexOf(element);
        if (index !== -1) {
          const newList = [...videosList];
          newList.splice(index, 1);
          setVideosList(newList);
        }
    }
    
    const nameRef = useRef();
    const [errMsg, setErrMsg] = useState('');
    const errRef = useRef();

    useEffect(() => {
        localStorage.setItem("lastVisitedPage", location.pathname);
    })

    useEffect(() => {
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
    }, [auth])

    const handleCancelClick = () => {
        navigate('/profile');
    };

    const handleVideoAddClick = (id) => {
        videosListAddItem(id);
    };

    const handleVideoRemoveClick = (id) => {
        videosListRemoveElement(id);
    }

    const handleSubmit = async (e) => {
      e.preventDefault();
      try {
        setSubmiting(true);
        const response = await axios.post(PLAYLIST_DETAILS_URL,
          JSON.stringify({
            name: name,
            visibility: visibility?"Public":"Private"
          }),
          {
            headers: { 
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${auth?.accessToken}`
            },
            withCredentials: false //cred
          }
        );
        const requests = videosList.map( (video_id) => {
          axios.post(PLAYLIST_URL + "/" + response?.data.id + "/" + video_id, {},
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
        });
        await Promise.all(requests);
        setSubmiting(false);
        handleCancelClick();
        
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
      setSubmiting(false);
    };

    return (
        <div style={{marginTop: "200px"}} class="col-xs-1" align="center"> 
            <h1 class="display-3" style={{marginBottom:"60px"}}>Create new playlist</h1>
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
                        onChange={(e) => setName(e.target.value)}
                        value={name}
                        required
                        aria-describedby="uidnote"
                    />
                    <label htmlFor="terms">
                        <input
                            type="checkbox"
                            id="terms"
                            onChange={() => setVisibility(!visibility)}
                            checked={visibility}
                        />
                        <text> I want my playlist to be public</text>
                    </label>
                    <label htmlFor="name">
                        Select videos you want on your playlist:
                    </label>

                    {videosData.map(video => (
                      video.processingProgress ==='Ready' && (
                        !videosList.includes(video.id)?(
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
                              <div className="box2" style={{width:"280px", height:"150px", backgroundImage: `url(${video.thumbnail})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center", cursor: "pointer"}}>
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
                    
                    <button class="btn btn-dark" disabled={(!name || submiting) ? true : false}>Submit</button>
                </form>
                <button class="btn btn-dark" onClick={handleCancelClick}>Cancel</button>
             </section>
        </div>
    )
}
export default AddPlaylist