import React from 'react';
import { useEffect, useState } from 'react';
import axios from './api/axios';

const SEARCH_URL = '/search';

const Search = (props) => {

    const [videosData, setVideosData] = useState([]);

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
      })
      .catch(error => {
        console.log("error: ", error);
      });
    });

  // Component logic
  return (
    <div style={{marginTop:"200px"}} class="container">
      Hello, {props.query}!
      <ul style={{padding:"0px", display:"inline"}}>
          {videosData.map(video => (
              <li class="mx-5" style={{marginTop:"20px", color:"white", borderRadius:"15px",
                      padding:"20px", backgroundColor:"#222222"}}>
                  <div class="row">
                      <div class="col-sm">
                          {video.title}
                      </div>
                      <div class="col-sm">
                          <h4>Nickname:</h4>
                          <h1 style={{marginTop:"30px"}}>{video.authorNickname}</h1>
                      </div>
                  </div>
              </li>
          )).reverse()}
      </ul>
    </div>
  );
}

export default Search;
