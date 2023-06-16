import './Home.css';
import { useState, useEffect } from "react";
import axios from './api/axios';
import AuthContext from "./context/AuthProvider";
import { useContext } from "react";
import { useLocation, useNavigate } from 'react-router-dom';

const TICKET_URL = '/ticket';
const TICKET_LIST_URL = "/ticket/list";
const METADATA_URL = '/video-metadata';
const PROFILE_URL = '/user';
const PLAYLIST_DETAILS_URL = '/playlist/video'
const PLAYLIST_URL = '/playlist/details'
const COMMENT_URL = '/comment/commentById';
const RESPONSE_URL = '/comment/responseById';
const VIDEO_URL = '/video';
const BAN_URL = '/user';

const Administrator = () => {

    const { auth } = useContext(AuthContext);
    const location = useLocation();
    const navigate = useNavigate();

    const [ticketsData, setTicketsData] = useState([]);
    const [videosData, setVideosData] = useState({});
    const [usersData, setUsersData] = useState({});
    const [playlistsData, setPlaylistsData] = useState({});
    const [commentsData, setCommentsData] = useState({});
    const [commentsResponseData, setCommentsResponseData] = useState({});

    const [ticketResponseVideosTexts, setTicketResponseVideosTexts] = useState([]);
    const [ticketResponseUsersTexts, setTicketResponseUsersTexts] = useState([]);
    const [ticketResponsePlaylistsTexts, setTicketResponsePlaylistsTexts] = useState([]);
    const [ticketResponseCommentsTexts, setTicketResponseCommentsTexts] = useState([]);
    const [ticketResponseCommentsResponsesTexts, setTicketResponseCommentsResponsesTexts] = useState([]);

    const [errMsg, setErrMsg] = useState('');

    const Actions = {
      REJECT: 'reject',
      DELETE: 'delete',
      BAN: 'ban',
    };

    const TargetType = {
      VIDEO: 0,
      USER: 1,
      PLAYLIST: 2,
      COMMENT: 3,
      COMMENT_RESPONSE: 4,
    };

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
        if(auth.roles === "Administrator"){
          localStorage.setItem("lastVisitedPage", location.pathname);
        }else{
          navigate('/home');
        }
    });

    const handleTicketResponseVideosChange = (event, index) => {
      const newTexts = [...ticketResponseVideosTexts];
      newTexts[index] = event.target.value;
      setTicketResponseVideosTexts(newTexts);
    };

    const handleTicketResponseUsersChange = (event, index) => {
      const newTexts = [...ticketResponseUsersTexts];
      newTexts[index] = event.target.value;
      setTicketResponseUsersTexts(newTexts);
    };

    const handleTicketResponsePlaylistsChange = (event, index) => {
      const newTexts = [...ticketResponsePlaylistsTexts];
      newTexts[index] = event.target.value;
      setTicketResponsePlaylistsTexts(newTexts);
    };

    const handleTicketResponseCommentsChange = (event, index) => {
      const newTexts = [...ticketResponseCommentsTexts];
      newTexts[index] = event.target.value;
      setTicketResponseCommentsTexts(newTexts);
    };

    const handleTicketResponseCommentsResponsesChange = (event, index) => {
      const newTexts = [...ticketResponseCommentsResponsesTexts];
      newTexts[index] = event.target.value;
      setTicketResponseCommentsResponsesTexts(newTexts);
    };

    const clearTicketResponseVideosText = (index) => {
      const newTexts = [...ticketResponseVideosTexts];
      newTexts[index] = '';
      setTicketResponseVideosTexts(newTexts);
    };
    
    const clearTicketResponseUsersText = (index) => {
      const newTexts = [...ticketResponseUsersTexts];
      newTexts[index] = '';
      setTicketResponseUsersTexts(newTexts);
    };
    
    const clearTicketResponsePlaylistsText = (index) => {
      const newTexts = [...ticketResponsePlaylistsTexts];
      newTexts[index] = '';
      setTicketResponsePlaylistsTexts(newTexts);
    };
    
    const clearTicketResponseCommentsText = (index) => {
      const newTexts = [...ticketResponseCommentsTexts];
      newTexts[index] = '';
      setTicketResponseCommentsTexts(newTexts);
    };
    
    const clearTicketResponseCommentsResponsesText = (index) => {
      const newTexts = [...ticketResponseCommentsResponsesTexts];
      newTexts[index] = '';
      setTicketResponseCommentsResponsesTexts(newTexts);
    };

    useEffect(() => {
        axios.get(TICKET_LIST_URL, {
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

    const handleTicketReject = (ticket, targetType, index, responseText) => {
      axios.put(TICKET_URL + "?id=" + ticket.ticketId, { "response": responseText},
        {
          headers: { 
            'Content-Type': 'application/json',
            "Authorization" : `Bearer ${auth?.accessToken}`
          },
          withCredentials: false
        }
      ).then(() => {
        ticket.status = "Resolved";
        if(targetType === TargetType.VIDEO){
          clearTicketResponseVideosText(index);
        }else if(targetType === TargetType.USER){
          clearTicketResponseUsersText(index);
        }else if(targetType === TargetType.PLAYLIST){
          clearTicketResponsePlaylistsText(index);
        }else if(targetType === TargetType.COMMENT){
          clearTicketResponseCommentsText(index);
        }else if(targetType === TargetType.COMMENT_RESPONSE){
          clearTicketResponseCommentsResponsesText(index);
        }
      }).catch(err => {
        if(!err?.response) {
            setErrMsg('No Server Response')
        } else if(err.response?.status === 400) {
            setErrMsg('Bad request');
        } else if(err.response?.status === 401){
            setErrMsg('Unauthorised');
        } else {
            setErrMsg('Getting metadata failed');
        }
      });
    }

    const handleTicketDelete = (ticket, targetType, index, responseText) => {
      if(targetType === TargetType.VIDEO){
        axios.delete(VIDEO_URL + "?id=" + ticket.targetId,
          {
            headers: { 
              'Content-Type': 'application/json',
              "Authorization" : `Bearer ${auth?.accessToken}`
            },
            withCredentials: false
          }
        ).then(() => {
          handleTicketReject(ticket, targetType, index, responseText);
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
              setErrMsg('Deleting video failed');
          }
        });
      }else if(targetType === TargetType.PLAYLIST){
        axios.delete(PLAYLIST_URL + "?id=" + ticket.targetId,
          {
            headers: { 
              'Content-Type': 'application/json',
              "Authorization" : `Bearer ${auth?.accessToken}`
            },
            withCredentials: false
          }
        ).then(() => {
          handleTicketReject(ticket, targetType, index, responseText);
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
      }else if(targetType === TargetType.COMMENT || targetType === TargetType.COMMENT_RESPONSE){
        axios.delete(COMMENT_URL + "?id=" + ticket.targetId,
          {
            headers: { 
              'Content-Type': 'application/json',
              "Authorization" : `Bearer ${auth?.accessToken}`
            },
            withCredentials: false
          }
        ).then(() => {
          handleTicketReject(ticket, targetType, index, responseText);
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
            setErrMsg('Deleting comment failed');
          }
        });
      }
    }

    const handleTicketBan = (ticket, targetType, index, responseText) => {
      let idToBan = '';
      if(targetType === TargetType.VIDEO){
        idToBan = videosData[ticket.ticketId].authorId;
      }else if(targetType === TargetType.USER){
        idToBan = usersData[ticket.ticketId].id;
      }else if(targetType === TargetType.PLAYLIST){
        idToBan = playlistsData[ticket.ticketId].authorId;
      }else if(targetType === TargetType.COMMENT){
        idToBan = commentsData[ticket.ticketId].authorId;
      }else if(targetType === TargetType.COMMENT_RESPONSE){
        idToBan = commentsResponseData[ticket.ticketId].authorId;
      }
      handleTicketReject(ticket, targetType, index, responseText);
      axios.delete(BAN_URL + "?id=" + idToBan,
        {
          headers: { 
            'Content-Type': 'application/json',
            "Authorization" : `Bearer ${auth?.accessToken}`
          },
          withCredentials: false
        }
      ).catch(err => {
        if(!err?.response) {
            setErrMsg('No Server Response')
        } else if(err.response?.status === 400) {
            setErrMsg('Bad request');
        } else if(err.response?.status === 401){
            setErrMsg('Unauthorised');
        } else {
            setErrMsg('Getting metadata failed');
        }
      });
    }

    const handleTicketResponseAdd = (event, ticket, index, targetType, action) => {
      event.preventDefault();
      let responseText = '';
      if(targetType === TargetType.VIDEO){
        responseText = ticketResponseVideosTexts[index];
      }else if(targetType === TargetType.USER){
        responseText = ticketResponseUsersTexts[index];
      }else if(targetType === TargetType.PLAYLIST){
        responseText = ticketResponsePlaylistsTexts[index];
      }else if(targetType === TargetType.COMMENT){
        responseText = ticketResponseCommentsTexts[index];
      }else if(targetType === TargetType.COMMENT_RESPONSE){
        responseText = ticketResponseCommentsResponsesTexts[index];
      }
      if (action === Actions.REJECT) {
        handleTicketReject(ticket, targetType, index, responseText);
      } else if (action === Actions.DELETE) {
        handleTicketDelete(ticket, targetType, index, responseText);
      } else if (action === Actions.BAN) {
        handleTicketBan(ticket, targetType, index, responseText);
      }
    };

    return(
      <div class="container" style={{marginTop: "200px", marginBottom:"50px"}}>
          <h2 class="display-5" style={{textAlign: "center", marginBottom:"50px"}}> Your Reports: </h2>
          <div class="display-5">Videos:</div>
          {ticketsData.map((ticket, index) => (
                <div>
                  {ticket.status === "Submitted" && videosData[ticket.ticketId] && (
                    <div class="justify-content-center" style={{marginTop:"20px", 
                    color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#222222"}}>
                       <div class="justify-content-center" style={{marginTop:"20px", 
            color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                    <h3>Reason:</h3> {ticket.reason}
                    </div>
                    <div class="justify-content-center" style={{marginTop:"20px", color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
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
                        <h3>Response:</h3>
                        <form onSubmit={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.VIDEO ,Actions.REJECT)} style={{marginLeft:"20px", marginRight:"20px", marginBottom:"15px", display: 'flex', flexDirection: 'row', 
                            alignItems: 'center', marginTop:"20px", color:"white", borderRadius:"15px", paddingBottom:"10px", paddingTop:"10px", 
                            backgroundColor:"#111111"}}>
                          <input type="text" placeholder="Add response ..." value={ticketResponseVideosTexts[index] || ''} onChange={(event) => handleTicketResponseVideosChange(event, index)}
                            style={{color:"white", backgroundColor:"black", marginRight: '10px', marginLeft: '10px', width: "500px"}} />
                          <button type="submit" class="btn btn-dark" style={{marginRight: "10px", marginBottom:"20px"}} disabled={!ticketResponseVideosTexts[index]}>Reject ticket</button>
                          <button type="button" className="btn btn-dark" style={{ marginRight: "10px", marginTop: "-0px" }} onClick={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.VIDEO, Actions.DELETE)} disabled={!ticketResponseVideosTexts[index]}>Delete reported content</button>
                          <button type="button" className="btn btn-dark" style={{ marginRight: "10px", marginTop: "-0px", width: "300px"}} onClick={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.VIDEO, Actions.BAN)} disabled={!ticketResponseVideosTexts[index]}>
                            <span style={{ display: "inline-block", maxWidth: "100%", overflowWrap: "break-word" }}>Delete user and</span>
                            <span style={{ display: "inline-block", maxWidth: "100%", overflowWrap: "break-word" }}>reported content</span>
                          </button>
                        </form>
                      </div>
                    </div>
                  )}
                </div>
          ))}
          <div style={{marginTop:"50px"}} class="display-5">Users:</div>
          {ticketsData.map((ticket, index) => (
                <div>
                  {ticket.status === "Submitted" && usersData[ticket.ticketId] && (
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
                        <h3>Response:</h3>
                        <form onSubmit={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.USER ,Actions.REJECT)} style={{marginLeft:"20px", marginRight:"20px", marginBottom:"15px", display: 'flex', flexDirection: 'row', 
                            alignItems: 'center', marginTop:"20px", color:"white", borderRadius:"15px", paddingBottom:"10px", paddingTop:"10px", 
                            backgroundColor:"#111111"}}>
                          <input type="text" placeholder="Add response ..." value={ticketResponseUsersTexts[index] || ''} onChange={(event) => handleTicketResponseUsersChange(event, index)}
                            style={{color:"white", backgroundColor:"black", marginRight: '10px', marginLeft: '10px', width: "500px"}} />
                          <button type="submit" class="btn btn-dark" style={{marginRight: "10px", marginBottom:"20px"}} disabled={!ticketResponseUsersTexts[index]}>Reject ticket</button>
                          <button type="button" className="btn btn-dark" style={{ marginRight: "10px", marginTop: "-0px"}} 
                              onClick={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.USER, Actions.BAN)} disabled={!ticketResponseUsersTexts[index]}>Delete user </button>
                        </form>
                      </div>
                    </div>
                  )}
                </div>
          ))}
          <div style={{marginTop:"50px"}} class="display-5">Playlists:</div>
          {ticketsData.map((ticket, index) => (
                <div>
                  {ticket.status === "Submitted" && playlistsData[ticket.ticketId] && (
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
                        <h3>Response:</h3>
                        <form onSubmit={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.PLAYLIST ,Actions.REJECT)} style={{marginLeft:"20px", marginRight:"20px", marginBottom:"15px", display: 'flex', flexDirection: 'row', 
                            alignItems: 'center', marginTop:"20px", color:"white", borderRadius:"15px", paddingBottom:"10px", paddingTop:"10px", 
                            backgroundColor:"#111111"}}>
                          <input type="text" placeholder="Add response ..." value={ticketResponsePlaylistsTexts[index] || ''} onChange={(event) => handleTicketResponsePlaylistsChange(event, index)}
                            style={{color:"white", backgroundColor:"black", marginRight: '10px', marginLeft: '10px', width: "500px"}} />
                          <button type="submit" class="btn btn-dark" style={{marginRight: "10px", marginBottom:"20px"}} disabled={!ticketResponsePlaylistsTexts[index]}>Reject ticket</button>
                          <button type="button" className="btn btn-dark" style={{ marginRight: "10px", marginTop: "-0px" }} onClick={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.PLAYLIST, Actions.DELETE)} disabled={!ticketResponsePlaylistsTexts[index]}>Delete reported content</button>
                          <button type="button" className="btn btn-dark" style={{ marginRight: "10px", marginTop: "-0px", width: "300px"}} onClick={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.PLAYLIST, Actions.BAN)} disabled={!ticketResponsePlaylistsTexts[index]}>
                            <span style={{ display: "inline-block", maxWidth: "100%", overflowWrap: "break-word" }}>Delete user and</span>
                            <span style={{ display: "inline-block", maxWidth: "100%", overflowWrap: "break-word" }}>reported content</span>
                          </button>
                        </form>
                      </div>
                      </div>
                  )}
                </div>
          ))}
          <div style={{marginTop:"50px"}} class="display-5">Comments:</div>
          {ticketsData.map((ticket, index) => (
                <div>
                  {ticket.status === "Submitted" && commentsData[ticket.ticketId] && (
                    <div class="justify-content-center" style={{marginTop:"20px", 
                    color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#222222"}}>
                       <div class="justify-content-center" style={{marginTop:"20px", 
            color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                    <h3>Reason:</h3> {ticket.reason}
                    </div>
                    <div class="justify-content-center" style={{marginTop:"20px", 
            color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                      {commentsData[ticket.ticketId].nickname}: {commentsData[ticket.ticketId].content}
                    </div>
                    <div class="justify-content-center" style={{marginTop:"20px", color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                        <h3>Response:</h3>
                        <form onSubmit={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.COMMENT ,Actions.REJECT)} style={{marginLeft:"20px", marginRight:"20px", marginBottom:"15px", display: 'flex', flexDirection: 'row', 
                            alignItems: 'center', marginTop:"20px", color:"white", borderRadius:"15px", paddingBottom:"10px", paddingTop:"10px", 
                            backgroundColor:"#111111"}}>
                          <input type="text" placeholder="Add response ..." value={ticketResponseCommentsTexts[index] || ''} onChange={(event) => handleTicketResponseCommentsChange(event, index)}
                            style={{color:"white", backgroundColor:"black", marginRight: '10px', marginLeft: '10px', width: "500px"}} />
                          <button type="submit" class="btn btn-dark" style={{marginRight: "10px", marginBottom:"20px"}} disabled={!ticketResponseCommentsTexts[index]}>Reject ticket</button>
                          <button type="button" className="btn btn-dark" style={{ marginRight: "10px", marginTop: "-0px" }} onClick={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.COMMENT, Actions.DELETE)} disabled={!ticketResponseCommentsTexts[index]}>Delete reported content</button>
                          <button type="button" className="btn btn-dark" style={{ marginRight: "10px", marginTop: "-0px", width: "300px"}} onClick={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.COMMENT, Actions.BAN)} disabled={!ticketResponseCommentsTexts[index]}>
                            <span style={{ display: "inline-block", maxWidth: "100%", overflowWrap: "break-word" }}>Delete user and</span>
                            <span style={{ display: "inline-block", maxWidth: "100%", overflowWrap: "break-word" }}>reported content</span>
                          </button>
                        </form>
                      </div>
                    </div>
                  )}
                </div>
          ))}

          <div style={{marginTop:"50px"}} class="display-5">Comments responses:</div>
          {ticketsData.map((ticket, index) => (
                <div>
                  {ticket.status === "Submitted" && commentsResponseData[ticket.ticketId] && (
                    <div class="justify-content-center" style={{marginTop:"20px", 
                    color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#222222"}}>
                       <div class="justify-content-center" style={{marginTop:"20px", 
            color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                    <h3>Reason:</h3> {ticket.reason}
                    </div>
                    <div class="justify-content-center" style={{marginTop:"20px", 
            color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                      {commentsResponseData[ticket.ticketId].nickname}: {commentsResponseData[ticket.ticketId].content}
                    </div>
                    <div class="justify-content-center" style={{marginTop:"20px", color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
                        <h3>Response:</h3>
                        <form onSubmit={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.COMMENT_RESPONSE ,Actions.REJECT)} style={{marginLeft:"20px", marginRight:"20px", marginBottom:"15px", display: 'flex', flexDirection: 'row', 
                            alignItems: 'center', marginTop:"20px", color:"white", borderRadius:"15px", paddingBottom:"10px", paddingTop:"10px", 
                            backgroundColor:"#111111"}}>
                          <input type="text" placeholder="Add response ..." value={ticketResponseCommentsResponsesTexts[index] || ''} onChange={(event) => handleTicketResponseCommentsResponsesChange(event, index)}
                            style={{color:"white", backgroundColor:"black", marginRight: '10px', marginLeft: '10px', width: "500px"}} />
                          <button type="submit" class="btn btn-dark" style={{marginRight: "10px", marginBottom:"20px"}} disabled={!ticketResponseCommentsResponsesTexts[index]}>Reject ticket</button>
                          <button type="button" className="btn btn-dark" style={{ marginRight: "10px", marginTop: "-0px" }} onClick={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.COMMENT_RESPONSE, Actions.DELETE)} disabled={!ticketResponseCommentsResponsesTexts[index]}>Delete reported content</button>
                          <button type="button" className="btn btn-dark" style={{ marginRight: "10px", marginTop: "-0px", width: "300px"}} onClick={(event) => handleTicketResponseAdd(event, ticket, index, TargetType.COMMENT_RESPONSE, Actions.BAN)} disabled={!ticketResponseCommentsResponsesTexts[index]}>
                            <span style={{ display: "inline-block", maxWidth: "100%", overflowWrap: "break-word" }}>Delete user and</span>
                            <span style={{ display: "inline-block", maxWidth: "100%", overflowWrap: "break-word" }}>reported content</span>
                          </button>
                        </form>
                      </div>
                    </div>
                  )}
                </div>
          ))}

      </div>
  );
}
export default Administrator;