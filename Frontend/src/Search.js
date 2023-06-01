import React from 'react';
import { useEffect, useState } from 'react';
import axios from './api/axios';
import { useNavigate } from 'react-router-dom';

const SEARCH_URL = '/search';

const Search = (props) => {

    const [videosData, setVideosData] = useState([]);
    const [usersData, setUsersData] = useState([]);
    const [playlistsData, setPlaylistsData] = useState([]);
    const navigate = useNavigate();

    useEffect(() => {
      axios.get(SEARCH_URL, {
        params:{
          query: props.query,
          sortingCriterion: 1,
          sortingType: 1
        },
        headers: { 
          'Content-Type': 'application/json',
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
    });

    const handleVideoClick = (id) => {
      navigate(`/videoplayer/${id}`);
    }

  // Component logic
  return (
    <div style={{marginTop:"200px"}} class="container">
          <div class="row" style={{marginBottom:"100px"}}>
            <div class="col-sm">
              <h2 class="display-5"> Videos: </h2>
              <ul style={{padding:"0px", display:"inline"}}>
                  {videosData.map(video => (
                      <li className="search_list_item" class="mx-5" style={{marginTop:"20px", color:"white", borderRadius:"15px",
                              padding:"20px", backgroundColor:"#222222", cursor: "pointer"}} onClick={() => handleVideoClick(video.id)}>
                          <div class="row">
                            <div className="box3" style={{width:"200px", height:"170px", cursor: "pointer"}}>
                              <div className="box4" style={{width:"180px", height:"150px", backgroundImage: `url(${video.thumbnail})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                                  <table style={{backgroundColor: "transparent"}}>
                                      <tr style={{backgroundColor: "transparent"}}>
                                      <div className="movie_thumbnail" style={{width:"280px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" } } onClick={() => handleVideoClick(video.id)}>

                                      </div>
                                      </tr>
                                  </table>
                                </div> 
                            </div>
                            <div class="col-sm">
                                <h1 style={{marginTop:"30px"}}>{video.title}</h1>
                                <h4 style={{marginTop:"30px"}}>{video.authorNickname}</h4>
                            </div>
                            <div class="col-sm">
                                <h4 style={{marginTop:"30px"}}>Views: {video.viewCount}</h4>
                            </div>
                          </div>
                      </li>
                  )).reverse()}
              </ul>
            </div>
            <div class="col-sm">
              <h2 class="display-5"> Users: </h2>
              <ul style={{padding:"0px", display:"inline"}}>
                  {usersData.map(user => (
                      <li className="search_list_item" class="mx-5" style={{marginTop:"20px", color:"white", borderRadius:"15px",
                              padding:"20px", backgroundColor:"#222222", cursor: "pointer"}} >
                          <div class="row">
                            <div className="box3" style={{width:"200px", height:"170px", cursor: "pointer"}}>
                              <div className="box4" style={{width:"180px", height:"150px", backgroundImage: `url(${user.avatarImage})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                                  <table style={{backgroundColor: "transparent"}}>
                                      <tr style={{backgroundColor: "transparent"}}>
                                      <div className="movie_thumbnail" style={{width:"280px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" } }>

                                      </div>
                                      </tr>
                                  </table>
                                </div> 
                            </div>
                            <div class="col-sm">
                                <h1 style={{marginTop:"30px"}}>{user.nickname}</h1>
                            </div>
                            <div class="col-sm">
                                <h4 style={{marginTop:"30px"}}>{user.userType}</h4>
                            </div>
                          </div>
                      </li>
                  )).reverse()}
              </ul>
            </div>
            <div class="col-sm">
              <h2 class="display-5"> Playlists: </h2>
              <ul style={{padding:"0px", display:"inline"}}>
                  {videosData.map(video => (
                      <li className="search_list_item" class="mx-5" style={{marginTop:"20px", color:"white", borderRadius:"15px",
                              padding:"20px", backgroundColor:"#222222", cursor: "pointer"}} onClick={() => handleVideoClick(video.id)}>
                          <div class="row">
                            <div className="box3" style={{width:"200px", height:"170px", cursor: "pointer"}}>
                              <div className="box4" style={{width:"180px", height:"150px", backgroundImage: `url(${video.thumbnail})`, backgroundRepeat:"no-repeat", backgroundSize:"cover", backgroundPosition:"center"}}>
                                  <table style={{backgroundColor: "transparent"}}>
                                      <tr style={{backgroundColor: "transparent"}}>
                                      <div className="movie_thumbnail" style={{width:"280px", height:"60px", backgroundSize:"cover", backgroundColor: "transparent" } } onClick={() => handleVideoClick(video.id)}>

                                      </div>
                                      </tr>
                                  </table>
                                </div> 
                            </div>
                            <div class="col-sm">
                                <h1 style={{marginTop:"30px"}}>{video.title}</h1>
                                <h4 style={{marginTop:"30px"}}>{video.authorNickname}</h4>
                            </div>
                            <div class="col-sm">
                                <h4 style={{marginTop:"30px"}}>Views: {video.viewCount}</h4>
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
