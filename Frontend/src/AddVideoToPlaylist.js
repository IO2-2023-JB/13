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

const AddVideoToPlaylist = () => {

    const params = useParams();
    const { auth } = useContext(AuthContext);
    const [errMsg, setErrMsg] = useState('');
    const errRef = useRef();
    const navigate = useNavigate();
    const from = localStorage.getItem("lastVisitedPage");

    const video_id = params.videoid;

    const [playlistsData, setPlaylistsData] = useState([]);

    const [playlistsList, setPlaylistsList] = useState([]);
    function playlistsListAddItem(element) {
        setPlaylistsList(prevState => [...prevState, element]);
    }
    function playlistsListRemoveElement(element) {
        const index = playlistsList.indexOf(element);
        if (index !== -1) {
          const newList = [...playlistsList];
          newList.splice(index, 1);
          setPlaylistsList(newList);
        }
    }


    useEffect(() => {
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

    }, [auth?.accessToken, auth?.id]);

    const handlePlaylistAddClick = (id) => {
        playlistsListAddItem(id);
    }

    const handlePlaylistRemoveClick = (id) => {
        playlistsListRemoveElement(id);
    }

    const handleConfirmClick = () => {
        playlistsList.forEach(playlist => {
            axios.post(PLAYLIST_URL + "/" + playlist.id + "/" + video_id,
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
                    setErrMsg('Adding video with id ' + video_id + ' to playlist with id' + playlist.id + 'Failed');
                  }
                  errRef.current.focus();
            });
        });
        navigate(from);
    }

    const handleCancelClick = () => {
        navigate(from);
    }

    return(
        <div style={{marginTop: "200px"}} class="container">
            <div class ="mt-2 row">
            <h2>Choose playlists you want to add your video to:</h2>
            <section class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#333333"}}>
              <ul style={{padding:"0px"}}>
                {playlistsData.map(playlist => (
                    !playlistsList.includes(playlist.id)?(
                        <div>
                        <li style={{listStyleType: "none"}}>
                        <div className="box" style={{width:"300px", height:"100px", backgroundSize:"cover", backgroundRepeat:"no-repeat", backgroundPosition:"center", backgroundColor: `#${Math.floor(Math.random()*16777215).toString(16)}`}}>
                          <div className="box2" style={{width:"280px", height:"60px", backgroundColor: "transparent"}} onClick={() => handlePlaylistAddClick(playlist.id)}>
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
                    ):(
                        <div>
                        <li style={{listStyleType: "none"}}>
                        <div className="box" style={{width:"300px", height:"200px", backgroundSize:"cover", backgroundRepeat:"no-repeat", backgroundPosition:"center", backgroundColor: `#${Math.floor(Math.random()*16777215).toString(16)}`}}>
                          <div className="box2" style={{width:"280px", height:"120px", backgroundColor: "transparent"}} onClick={() => handlePlaylistRemoveClick(playlist.id)}>
                              <table style={{backgroundColor: "transparent"}}>
                                    <tr style={{backgroundColor: "transparent"}}>
                                        <div className="movie_title" style={{width:"280px", height:"60px", fontSize:"10px", marginTop:"0", whiteSpace: 'nowrap', overflow: 'hidden', position:"center", color:"black", backgroundColor:"transparent" }}>
                                            <h2 class="text-with-stroke" style={{backgroundColor: "transparent"}}>{playlist.name}</h2>
                                        </div>
                                    </tr>
                                    <tr style={{backgroundColor: "transparent"}}>
                                        <div className="movie_thumbnail" style={{width:"280px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" }}>
                                            <FontAwesomeIcon icon={faCheckCircle} size="5x" width="40" height="40" class="bi bi-play-circle-fill" style={{ color: 'green', borderRadius: "100%", marginBottom: "40px" }} />
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
            <div class="container-fluid justify-content-center" style={{marginBottom: "50px"}}>
                <button class="btn btn-dark" onClick={handleConfirmClick}>Confirm</button>
                <button onClick={handleCancelClick} class="btn btn-danger">Cancel</button>
            </div>
        </div>
    )
}
export default AddVideoToPlaylist;