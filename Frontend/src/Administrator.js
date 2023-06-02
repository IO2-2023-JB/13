import './Home.css';
import { useState, useEffect } from "react";
import axios from './api/axios';
import AuthContext from "./context/AuthProvider";
import { useContext } from "react";
import { useLocation } from 'react-router-dom';

const TICKET_LIST = "/ticket/list";
const METADATA_URL = '/video-metadata';
const PROFILE_URL = '/user';
const PLAYLIST_DETAILS_URL = '/playlist/video'
const COMMENT_URL = '/comment';
const RESPONSE_URL = '/comment/response'

const Administrator = () => {

    const { auth } = useContext(AuthContext);
    const location = useLocation();

    const [ticketsData, setTicketsData] = useState([]);
    const [videosData, setVideosData] = useState({});
    const [usersData, setUsersData] = useState({});
    const [playlistsData, setPlaylistsData] = useState({});
    const [commentsData, setCommentsData] = useState({});
    const [commentsResponseData, setCommentsResponseData] = useState({});

    const addVideoData = (ticketId, video) => {
        setVideosData(prevData => ({
          ...prevData,
          [ticketId]: video
        }));
    };

    const addUsersData = (ticketId, video) => {
        setUsersData(prevData => ({
          ...prevData,
          [ticketId]: video
        }));
    };

    const addPlaylistsData = (ticketId, video) => {
        setPlaylistsData(prevData => ({
          ...prevData,
          [ticketId]: video
        }));
    };

    const addCommentsData = (ticketId, video) => {
        setCommentsData(prevData => ({
          ...prevData,
          [ticketId]: video
        }));
    };

    const addCommentsResponseData = (ticketId, video) => {
        setCommentsResponseData(prevData => ({
          ...prevData,
          [ticketId]: video
        }));
        console.log(commentsResponseData['519f4277-5f12-4753-c11e-08db62ecf34d']);
    };

    useEffect(() => {
        localStorage.setItem("lastVisitedPage", location.pathname);
    });

    useEffect(() => {
        axios.get(TICKET_LIST, {
            headers: { 
              'Content-Type': 'application/json',
              "Authorization" : `Bearer ${auth?.accessToken}`
            },
            withCredentials: true 
          })
          .then(response => {
            setTicketsData(response?.data);
          })
          .catch(error => {
            console.log("error: ", error);
        });
    }, [auth?.accessToken, auth?.id])

    useEffect(() => {
        ticketsData.forEach(element => {
            if(element.targetType === "Video"){
                axios.get(METADATA_URL + "?id=" + element.targetId, {
                    headers: { 
                      'Content-Type': 'application/json',
                      "Authorization" : `Bearer ${auth?.accessToken}`
                    },
                    withCredentials: true 
                  })
                  .then(response => {
                    addVideoData(element.ticketId, response?.data);
                  })
                  .catch(error => {
                    console.log("error: ", error);
                });
            }else if(element.targetType === "User"){
                axios.get(PROFILE_URL + "?id=" + element.targetId, {
                    headers: { 
                      'Content-Type': 'application/json',
                      "Authorization" : `Bearer ${auth?.accessToken}`
                    },
                    withCredentials: true 
                  })
                  .then(response => {
                    addUsersData(element.ticketId, response?.data);
                  })
                  .catch(error => {
                    console.log("error: ", error);
                });
            }else if(element.targetType === "Playlist"){
                axios.get(PLAYLIST_DETAILS_URL + "?id=" + element.targetId, {
                    headers: { 
                      'Content-Type': 'application/json',
                      "Authorization" : `Bearer ${auth?.accessToken}`
                    },
                    withCredentials: true 
                  })
                  .then(response => {
                    addPlaylistsData(element.ticketId, response?.data);
                  })
                  .catch(error => {
                    console.log("error: ", error);
                });
            }else if(element.targetType === "Comment"){
                axios.get(COMMENT_URL + "?id=" + element.targetId, {
                    headers: { 
                      'Content-Type': 'application/json',
                      "Authorization" : `Bearer ${auth?.accessToken}`
                    },
                    withCredentials: true 
                  })
                  .then(response => {
                    addCommentsData(element.ticketId, response?.data);
                  })
                  .catch(error => {
                    console.log("error: ", error);
                });
            }else if(element.targetType === "CommentResponse"){
                axios.get(RESPONSE_URL + "?id=" + element.targetId, {
                    headers: { 
                      'Content-Type': 'application/json',
                      "Authorization" : `Bearer ${auth?.accessToken}`
                    },
                    withCredentials: true 
                  })
                  .then(response => {
                    addCommentsResponseData(element.ticketId, response?.data);
                  })
                  .catch(error => {
                    console.log("error: ", error);
                });
            }
        });
    }, [ticketsData])

    return(
        <div class="col-xs-1"style={{marginTop:"200px"}}>
            <div class="row mt-5">
                <h1 class="display-3 mx-5">Reported users</h1>
                <section>
                </section>
            </div>
            <div class="row mt-5">
                <h1 class="display-3 mx-5">Reported videos</h1>
                <section>
                    
                </section>
            </div>
            <div class="row mt-5">
                <h1 class="display-3 mx-5">Requests for support ticket</h1>
                <section>
                </section>
            </div>
            
        </div>
    )
}
export default Administrator;