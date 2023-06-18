import {useState, useEffect } from "react";
import AuthContext from "../context/AuthProvider";
import { useContext } from "react";
import axios from '../api/axios';
import '@fortawesome/fontawesome-svg-core/styles.css';
import { useNavigate, useLocation } from 'react-router-dom';

const TICKET_LIST = "/ticket/list";
const METADATA_URL = '/video-metadata';
const PROFILE_URL = '/user';
const PLAYLIST_DETAILS_URL = '/playlist/video'
const COMMENT_URL = '/comment/commentById';
const RESPONSE_URL = '/comment/responseById';

const Reports = () => {
    const { auth } = useContext(AuthContext);
    const location = useLocation();
    const navigate = useNavigate();

    const [ticketsData, setTicketsData] = useState([]);
    const [videosData, setVideosData] = useState({});
    const [usersData, setUsersData] = useState({});
    const [playlistsData, setPlaylistsData] = useState({});
    const [commentsData, setCommentsData] = useState({});
    const [commentsResponseData, setCommentsResponseData] = useState({});

    useEffect(() => {
      if(auth.roles === "Administrator"){
        navigate('/administrator');
      }
    });

    const goToProfile = (user_id) => {
      if(user_id){
        navigate(`/creatorprofile/${user_id}`);
      }
    }

    const handelPlaylistClick = (id) => {
      navigate(`/playlist/${id}`);
    }

    const handleVideoClick = (id) => {
      navigate(`/videoplayer/${id}`);
    }

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
            withCredentials: false 
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
                  withCredentials: false 
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
                  withCredentials: false 
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
                  withCredentials: false 
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
                  withCredentials: false
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
                  withCredentials: false
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
        <div class="container" style={{marginTop: "200px", marginBottom:"50px"}}>
            <div class="mx-5">
            <h1 class="display-4" style={{marginBottom:"50px"}}> Your Reports </h1>
            <div class="display-6 center-div">Videos</div>
            {ticketsData.map(ticket => (
                  <div class="mx-5">
                    {videosData[ticket.ticketId] && (
                      <div class="justify-content-center" style={{marginTop:"20px", 
                      color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#222222"}}>
                         <div class="justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                      <h3>Reason:</h3> {ticket.reason}
                      </div>
                      <div class="justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                      <div className="box" style={{width:"300px", height:"170px", cursor: "pointer"}}>
                        <div className="box2" style={{width:"280px", height:"150px", backgroundImage: `url(${videosData[ticket.ticketId].thumbnail})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                            <table style={{backgroundColor: "transparent"}}>
                                <tr style={{backgroundColor: "transparent"}}>
                                  <div className="movie_title" style={{width:"280px", height:"60px", fontSize:"10px", marginTop:"0", whiteSpace: 'nowrap', overflow: 'hidden', position:"center", color:"black", backgroundColor:"transparent" }}>
                                    <h2 class="text-with-stroke" style={{backgroundColor: "transparent"}}>{videosData[ticket.ticketId].title}</h2>
                                  </div>
                                </tr>
                                <tr style={{backgroundColor: "transparent"}}>
                                <div className="movie_thumbnail" style={{width:"280px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" } } onClick={() => handleVideoClick(videosData[ticket.ticketId].id)}>
                                    <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="black" class="bi bi-play-circle-fill" viewBox="0 0 16 16" style={{ fill: "white", borderRadius: "100%", marginBottom: "40px" }}>
                                      <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                                    </svg>
                                </div>
                                </tr>
                            </table>
                          </div> 
                        </div>
                        </div>
                        <div class="justify-content-center" style={{marginTop:"20px", color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                          <h3>Status: {ticket.status}</h3>
                          { ticket.status === "Resolved" && (
                            <div>
                              <h3>Response: {ticket.response}</h3>
                            </div>
                          )}
                        </div>
                        </div>
                    )}
                  </div>
            ))}
            
            <div style={{marginTop:"50px"}} class="display-6 center-div">Users</div>
            {ticketsData.map(ticket => (
                  <div class="mx-5">
                    {usersData[ticket.ticketId] && (
                      <div class="justify-content-center" style={{marginTop:"20px", 
                      color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#222222"}}>
                         <div class="justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                      <h3>Reason:</h3> {ticket.reason}
                      </div>
                      <div class="justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                      <div className="search_list_item" class="mx-5" style={{marginTop:"20px", color:"white", borderRadius:"15px",
                              padding:"20px", backgroundColor:"#222222", cursor: "pointer"}} onClick={() => goToProfile(usersData[ticket.ticketId].id)}>
                            <div className="box3" style={{width:"200px", height:"170px", cursor: "pointer"}}>
                              <div className="box4" style={{width:"180px", height:"150px", backgroundImage: `url(${usersData[ticket.ticketId].avatarImage})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                                  <table style={{backgroundColor: "transparent"}}>
                                      <tr style={{backgroundColor: "transparent"}}>
                                      <div className="movie_thumbnail" style={{width:"180px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" } }>

                                      </div>
                                      </tr>
                                  </table>
                                </div> 
                            </div>
                            <div>
                                <h1 style={{marginTop:"30px"}}>{usersData[ticket.ticketId].nickname}</h1>
                            </div>
                            <div>
                                <h4 style={{marginTop:"30px"}}>{usersData[ticket.ticketId].userType}</h4>
                            </div>
                      </div>
                      </div>
                      <div class="justify-content-center" style={{marginTop:"20px", color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                          <h3>Status: {ticket.status}</h3>
                          { ticket.status === "Resolved" && (
                            <div>
                              <h3>Response: {ticket.response}</h3>
                            </div>
                          )}
                        </div>
                      </div>
                    )}
                  </div>
            ))}
            <div style={{marginTop:"50px"}} class="display-6 center-div">Playlists</div>
            {ticketsData.map(ticket => (
                  <div class="mx-5">
                    {playlistsData[ticket.ticketId] && (
                      <div class="justify-content-center" style={{marginTop:"20px", 
                      color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#222222"}}>
                         <div class="justify-content-center" style={{marginTop:"20px", color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                            <h3>Reason:</h3> {ticket.reason}
                          </div>
                          <div class="justify-content-center" style={{marginTop:"20px", color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                            <li style={{listStyleType: "none"}}>
                            <div className="box" style={{width:"300px", height:"100px", backgroundSize:"cover", cursor: "pointer", backgroundRepeat:"no-repeat", backgroundPosition:"center", backgroundColor: '#FF4500'}}>
                              <div className="box2" style={{width:"280px", height:"60px", backgroundColor: "transparent"}} onClick={() => handelPlaylistClick(playlistsData[ticket.ticketId].id)}>
                                <table style={{backgroundColor: "transparent"}}>
                                  <tr style={{backgroundColor: "transparent"}}>
                                    <div className="movie_title" style={{width:"280px", height:"60px", fontSize:"10px", marginTop:"0", whiteSpace: 'nowrap', overflow: 'hidden', position:"center", color:"black", backgroundColor:"transparent" }}>
                                      <h2 class="text-with-stroke" style={{backgroundColor: "transparent"}}>{playlistsData[ticket.ticketId].name}</h2>
                                    </div>
                                  </tr>
                                </table>
                              </div> 
                            </div>
                            </li>
                          </div>
                          <div class="justify-content-center" style={{marginTop:"20px", color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                          <h3>Status: {ticket.status}</h3>
                          { ticket.status === "Resolved" && (
                            <div>
                              <h3>Response: {ticket.response}</h3>
                            </div>
                          )}
                        </div>
                        </div>
                    )}
                  </div>
            ))}
            <div style={{marginTop:"50px"}} class="display-6 center-div">Comments</div>
            {ticketsData.map(ticket => (
                  <div class="mx-5">
                    {commentsData[ticket.ticketId] && (
                      <div class="justify-content-center" style={{marginTop:"20px", 
                      color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#222222"}}>
                         <div class="justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                      <h3>Reason:</h3> {ticket.reason}
                      </div>
                      <div style={{marginTop:"20px",  display:"flex",
              color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#c40000"}}>
                        <img src={commentsData[ticket.ticketId].avatarImage} alt="Avatar" style={{width: "60px", height: "60px", borderRadius: "50%", marginRight: "10px", cursor: "pointer"}} 
                          onClick={() => goToProfile(commentsData[ticket.ticketId].authorId)} />
                          <div style={{display: "flex", flexDirection: "column"}}>
                            <div style={{display: "flex", justifyContent: "space-between"}}>
                              <h4 style={{fontSize: "30px", marginTop: "15px", cursor: "pointer"}} onClick={() => goToProfile(commentsData[ticket.ticketId].authorId)}>{commentsData[ticket.ticketId].nickname}</h4>
                              <div style={{marginLeft: "50px", inlineSize: "400px", overflowWrap: "break-word"}}>
                                <h6> {commentsData[ticket.ticketId].content} </h6>
                              </div>
                            </div>
                          </div>
                      </div>
                      <div class="justify-content-center" style={{marginTop:"20px", color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                          <h3>Status: {ticket.status}</h3>
                          { ticket.status === "Resolved" && (
                            <div>
                              <h3>Response: {ticket.response}</h3>
                            </div>
                          )}
                        </div>
                      </div>
                    )}
                  </div>
            ))}

            <div style={{marginTop:"50px"}} class="display-6 center-div">Comments responses</div>
            {ticketsData.map(ticket => (
                  <div class="mx-5">
                    {commentsResponseData[ticket.ticketId] && (
                      <div class="justify-content-center" style={{marginTop:"20px", 
                      color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#222222"}}>
                         <div class="justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                      <h3>Reason:</h3> {ticket.reason}
                      </div>
                      <div style={{marginTop:"20px",  display:"flex",
              color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#c40000"}}>
                        <img src={commentsResponseData[ticket.ticketId].avatarImage} alt="Avatar" style={{width: "60px", height: "60px", borderRadius: "50%", marginRight: "10px", cursor: "pointer"}} 
                          onClick={() => goToProfile(commentsResponseData[ticket.ticketId].authorId)} />
                          <div style={{display: "flex", flexDirection: "column"}}>
                            <div style={{display: "flex", justifyContent: "space-between"}}>
                              <h4 style={{fontSize: "30px", marginTop: "15px", cursor: "pointer"}} onClick={() => goToProfile(commentsResponseData[ticket.ticketId].authorId)}>{commentsResponseData[ticket.ticketId].nickname}</h4>
                              <div style={{marginLeft: "50px", inlineSize: "400px", overflowWrap: "break-word"}}>
                                <h6> {commentsResponseData[ticket.ticketId].content} </h6>
                              </div>
                            </div>
                          </div>
                      </div>
                      <div class="justify-content-center" style={{marginTop:"20px", color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                          <h3>Status: {ticket.status}</h3>
                          { ticket.status === "Resolved" && (
                            <div>
                              <h3>Response: {ticket.response}</h3>
                            </div>
                          )}
                        </div>
                      </div>
                    )}
                  </div>
            ))}
          </div>
        </div>
    );
};
export default Reports;