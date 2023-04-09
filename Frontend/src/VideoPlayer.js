import React from "react";
import axios from './api/axios';
import {useRef, useState, useEffect } from "react"
import AuthContext from "./context/AuthProvider"
import { useContext } from "react";
import ReactHlsPlayer from 'react-hls-player';
import { Player, ControlBar } from 'video-react';
import {useLocation} from 'react-router-dom';
import { useParams } from "react-router-dom";
import 'video-react/dist/video-react.css';


const VIDEO_URL = '/video';
const METADATA_URL = '/video-metadata';

const VideoPlayer = () => {
  const params = useParams();
  const location = useLocation();
  const { auth } = useContext(AuthContext);
  const video_id = params.videoid; //JSON.stringify(params.id)

  const [errMsg, setErrMsg] = useState('');
  const [videoData, setVideoData] = useState({
    id: "",
    title: "Loading...",
    description: "Loading...",
    thumbnail: "",
    authorId: "",
    authorNickname: "Loading...",
    viewCount: 0,
    tags: ["tag1", "tag2", "tag3"],
    visibility: "Private",
    processingProgress: "",
    uploadDate: "",
    editDate: "",
    duration: "",
  });

  useEffect(() => {
    localStorage.setItem("lastVisitedPage", location.pathname);
  })

  useEffect(() => {
    if(video_id){
      console.log('video_id:');
      console.log(video_id);
      //get video

      //get video metadata:
      axios.get(METADATA_URL + "?id=" + video_id,
          {
            headers: { 
              'Content-Type': 'application/json',
              "Authorization" : `Bearer ${auth?.accessToken}`
            },
            withCredentials: true
          }
      ).then(response => {
        setVideoData(response?.data);
      }).catch(err => {
        if(!err?.response) {
            setErrMsg('No Server Response')
        } else if(err.response?.status === 400) {
            setErrMsg('Bad request');
        } else if(err.response?.status === 401){
            setErrMsg('Unauthorised');
        } else if(err.response?.status === 403){
            setErrMsg('Forbidden');
        } else if(err.response?.status === 403){
          setErrMsg('Not found');
        } else {
            setErrMsg('Getting metadata failed');
        }
      });
    }
  })

  //const [video_url, setVideo_url] = useState(null);
  
  // useEffect(() => {
  //     setVideo_url(
  //       'https://videiomediaservices-usea.streaming.media.azure.net/bc3544a6-7c3b-4d10-b778-c2936c024b89/testvid.ism/manifest(format=m3u8-cmaf)');

  // })

  // useEffect(() => {
  //   axios.get(GETVIDEO_URL + "?id=" + video_id, {
  //     headers: { 
  //       'Content-Type': 'application/json',
  //       "Authorization" : `Bearer ${auth?.accessToken}`
  //     },
  //     withCredentials: true 
  //   })
  //   .then(response => {
  //     //console.log("success");
  //     console.log(JSON.stringify(response?.data));
  //     setVideo_url(response?.data);
  //   })
  //   .catch(error => {
  //     console.log("error: ", error);
  //   });
  // }, [auth?.accessToken]);

  const long_video_url = "https://videiomediaservices-usea.streaming.media.azure.net/d4a8a09e-b580-462a-a98b-430d109b71ad/videoplayback.ism/manifest(format=m3u8-cmaf)";
  const video_url = 'https://videiomediaservices-usea.streaming.media.azure.net/bc3544a6-7c3b-4d10-b778-c2936c024b89/testvid.ism/manifest(format=m3u8-cmaf)';
  const video_url_dash = "https://videiomediaservices-usea.streaming.media.azure.net/bc3544a6-7c3b-4d10-b778-c2936c024b89/testvid.ism/manifest(format=mpd-time-cmaf)"
  const video_url_smoothstreaming = "https://videiomediaservices-usea.streaming.media.azure.net/bc3544a6-7c3b-4d10-b778-c2936c024b89/testvid.ism/manifest";
  const video_url1 = "https://videiomediaservices-usea.streaming.media.azure.net/992730e7-a54f-4d4c-a167-155dd4d29baa/sample-30s.ism/manifest(format=m3u8-cmaf)"
  return (
    <div class="container-fluid justify-content-center" style={{display: "flex", flexDirection: "column", alignItems: "flex-start", justifyContent: "flex-start", marginTop: "200px", width: "900px"}}>
      <div class="container-fluid justify-content-center" style={{marginTop: "200px", width: "900px"}}>
        <ReactHlsPlayer
          src={video_url1}
          autoPlay={false}
          // loop = {true}
          controls={true}
          width="100%"
          height="auto" />

        {/* <Player ref={player => { this.player = player; }} autoPlay>
            <source src={video_url} />
            <ControlBar autoHide={false} />
          </Player> */}
      </div>
      <div class="container-fluid justify-content-center" style={{fontSize:"50px", marginTop:"0"}}>  {/*className="movie_title"*/}
        {videoData.title}
      </div>
      <div class="container-fluid justify-content-center" style={{fontSize:"30px", marginTop:"0"}}>
        Author: {videoData.authorNickname}
      </div>
      <div class="container-fluid justify-content-center" style={{fontSize:"25px", marginTop:"0"}}>
        Views: {videoData.viewCount}
      </div>
      <div class="container-fluid justify-content-center" style={{fontSize:"25px", marginTop:"0"}}>
        Upload date: {videoData.uploadDate}
      </div>
      <div class="container-fluid justify-content-center" style={{fontSize:"25px", marginTop:"0"}}>
        Tags: {videoData.tags.join(", ")}
      </div>
      <div class="container-fluid justify-content-center" style={{fontSize:"20px", marginTop:"0", marginBottom:"200px"}}>
        {videoData.description}
      </div>

    </div>
  );
};

export default VideoPlayer;