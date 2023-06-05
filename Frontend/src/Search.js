import React from 'react';
import { useEffect, useState, useContext } from 'react';
import axios from './api/axios';
import { useNavigate } from 'react-router-dom';
import AuthContext from "./context/AuthProvider";

const SEARCH_URL = '/search';

const Search = (props) => {

    const { auth } = useContext(AuthContext);

    const [activeCriterion, setActiveCriterion] = useState(1);
    const handleClickCriterion = (num) => {
      setActiveCriterion(num);
      setSortingCriterion(num)
    };

    const [activeType, setActiveType] = useState(1);
    const handleClickType = (num) => {
      setActiveType(num);
      setSortingType(num)
    };

    const [videosData, setVideosData] = useState([]);
    const [usersData, setUsersData] = useState([]);
    const [playlistsData, setPlaylistsData] = useState([]);
    const [sortingCriterion, setSortingCriterion] = useState(1);
    const [sortingType, setSortingType] = useState(1);
    const navigate = useNavigate();

    useEffect(() => {
      axios.get(SEARCH_URL, {
        params:{
          query: props.query,
          sortingCriterion: sortingCriterion,
          sortingType: sortingType
        },
        headers: { 
          'Content-Type': 'application/json',
          "Authorization" : `Bearer ${auth?.accessToken}`
        }
      })
      .then(response => {
        setVideosData(response?.data?.videos);
        setUsersData(response?.data?.users);
        setPlaylistsData(response?.data?.playlists);
      })
      .catch(error => {
        console.log("error: ", error);
      });
    }, [sortingCriterion, sortingType, props.query]);

    const handleVideoClick = (id) => {
      navigate(`/videoplayer/${id}`);
    }

    const handelPlaylistClick = (id) => {
      navigate(`/playlist/${id}`);
    }

    const goToProfile = (user_id) => {
      if(user_id){
        navigate(`/creatorprofile/${user_id}`);
      }
    }

  // Component logic
  return (
    <div style={{marginTop:"200px"}} class="container">
          <section class="container" style={{ marginBottom:"50px",
              color:"white", borderRadius:"15px", padding:"20px", backgroundColor:"#333333"}}>
            <h2 class="display-6"> Sorting criterion: </h2>
            <div class="row" style={{marginBottom:"50px"}}>
              <button class="col-sm btn btn-dark mx-2" style={{ backgroundColor: activeCriterion==1 ? "gray" : "black" }} onClick={()=>handleClickCriterion(1)}>Publish date</button>
              <button class="col-sm btn btn-dark mx-2" style={{ backgroundColor: activeCriterion==2 ? "gray" : "black" }} onClick={()=>handleClickCriterion(2)}>Alphabetical</button>
              <button class="col-sm btn btn-dark mx-2" style={{ backgroundColor: activeCriterion==3 ? "gray" : "black" }} onClick={()=>handleClickCriterion(3)}>Popularity</button>
            </div>
            <h2 class="display-6"> Sorting type: </h2>
            <div class="row">
              <button class="col-sm btn btn-dark mx-2" style={{ backgroundColor: activeType==1 ? "gray" : "black" }} onClick={()=>handleClickType(1)}>Ascending</button>
              <button class="col-sm btn btn-dark mx-2" style={{ backgroundColor: activeType==2 ? "gray" : "black" }} onClick={()=>handleClickType(2)}>Descending</button>
            </div>
          </section>
          <div class="row" style={{marginBottom:"100px"}}>
            <div class="col-sm">
              <h2 class="display-5"> Videos: </h2>
              <ul style={{padding:"0px", display:"inline"}}>
                  {videosData.map(video => (
                      (video.processingProgress === 'Ready' || video.authorId === auth.id) && (
                      <li className="search_list_item" class="mx-5" style={{marginTop:"20px", color:"white", borderRadius:"15px",
                              padding:"20px", backgroundColor:"#222222", cursor: "pointer"}} onClick={() => handleVideoClick(video.id)}>
                            <div className="box3" style={{width:"200px", height:"170px", cursor: "pointer"}}>
                              <div className="box4" style={{width:"180px", height:"150px", backgroundImage: `url(${video.thumbnail})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                                  <table style={{backgroundColor: "transparent"}}>
                                      <tr style={{backgroundColor: "transparent"}}>
                                      <div className="movie_thumbnail" style={{width:"180px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" } } onClick={() => handleVideoClick(video.id)}>

                                      </div>
                                      </tr>
                                  </table>
                                </div> 
                            </div>
                            <div>
                                <h1 style={{marginTop:"30px"}}>{video.title}</h1>
                                <h4 style={{marginTop:"30px"}}>{video.authorNickname}</h4>
                            </div>
                            <div>
                                <h4 style={{marginTop:"30px"}}>Views: {video.viewCount}</h4>
                            </div>
                      </li>
                  ))).reverse()}
              </ul>
            </div>
            <div class="col-sm">
              <h2 class="display-5"> Users: </h2>
              <ul style={{padding:"0px", display:"inline"}}>
                  {usersData.map(user => (
                      <li className="search_list_item" class="mx-5" style={{marginTop:"20px", color:"white", borderRadius:"15px",
                              padding:"20px", backgroundColor:"#222222", cursor: "pointer"}} onClick={() => goToProfile(user.id)}>
                            <div className="box3" style={{width:"200px", height:"170px", cursor: "pointer"}}>
                              <div className="box4" style={{width:"180px", height:"150px", backgroundImage: `url(${user.avatarImage})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                                  <table style={{backgroundColor: "transparent"}}>
                                      <tr style={{backgroundColor: "transparent"}}>
                                      <div className="movie_thumbnail" style={{width:"180px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" } }>

                                      </div>
                                      </tr>
                                  </table>
                                </div> 
                            </div>
                            <div>
                                <h1 style={{marginTop:"30px"}}>{user.nickname}</h1>
                            </div>
                            <div>
                                <h4 style={{marginTop:"30px"}}>{user.userType}</h4>
                            </div>
                      </li>
                  )).reverse()}
              </ul>
            </div>
            <div class="col-sm">
              <h2 class="display-5"> Playlists: </h2>
              <ul style={{padding:"0px", display:"inline"}}>
                  {playlistsData.map(playlist => (
                      <li style={{listStyleType: "none"}}>
                      <div className="box" style={{width:"300px", height:"100px", backgroundSize:"cover", cursor: "pointer", backgroundRepeat:"no-repeat", backgroundPosition:"center", backgroundColor: '#FF4500'}}>
                        <div className="box2" style={{width:"280px", height:"60px", backgroundColor: "transparent"}} onClick={() => handelPlaylistClick(playlist.id)}>
                            <table style={{backgroundColor: "transparent"}}>
                                <tr style={{backgroundColor: "transparent"}}>
                                <div className="movie_title" style={{width:"180px", height:"60px", fontSize:"10px", marginTop:"0", whiteSpace: 'nowrap', overflow: 'hidden', position:"center", color:"black", backgroundColor:"transparent" }}>
                                  <h2 class="text-with-stroke" style={{backgroundColor: "transparent"}}>{playlist.name}</h2>
                                </div>
                                </tr>
                            </table>
                            </div> 
                        </div>
                      </li>
                  )).reverse()}
              </ul>
            </div>
          </div>
    </div>
  );
}

export default Search;
