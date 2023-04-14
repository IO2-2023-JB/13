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
import ReactPlayer from 'react-player';

const VIDEO_URL = '/video';
const METADATA_URL = '/video-metadata';

const VideoPlayer = () => {
  const params = useParams();
  const location = useLocation();
  const { auth } = useContext(AuthContext);
  //let video_id = params.videoid; //JSON.stringify(params.id)
  const baseURL = 'https://io2test.azurewebsites.net';
  console.log(auth.accessToken);
  let video_id = "49FB7E85-23E3-47F3-904E-10170839F466"; //"ECF81211-C2B8-4A45-BC2A-61D339C29771";
  let videoUrl = baseURL + VIDEO_URL + "/" + video_id + "?access_token=" + auth.accessToken;
  //const videoUrl = 'https://videioblob.blob.core.windows.net/video/sample-30s.mp4';
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

  // DOBRE
  // useEffect(() => {
  //   // if(!video_id)
  //   // {
  //   //   video_id = "ECF81211-C2B8-4A45-BC2A-61D339C29771";
  //   //   videoUrl = VIDEO_URL + "/" + video_id + "?access_token=" + auth.accessToken;
  //   // }
  //   if(video_id){
  //     console.log('video_id:');
  //     console.log(video_id);
  //     //get video

  //     //get video metadata:
  //     axios.get(METADATA_URL + "?id=" + video_id,
  //         {
  //           headers: { 
  //             'Content-Type': 'application/json',
  //             "Authorization" : `Bearer ${auth?.accessToken}`
  //           },
  //           withCredentials: true
  //         }
  //     ).then(response => {
  //       setVideoData(response?.data);
  //     }).catch(err => {
  //       if(!err?.response) {
  //           setErrMsg('No Server Response')
  //       } else if(err.response?.status === 400) {
  //           setErrMsg('Bad request');
  //       } else if(err.response?.status === 401){
  //           setErrMsg('Unauthorised');
  //       } else if(err.response?.status === 403){
  //           setErrMsg('Forbidden');
  //       } else if(err.response?.status === 403){
  //         setErrMsg('Not found');
  //       } else {
  //           setErrMsg('Getting metadata failed');
  //       }
  //     });
  //   }
  // })

  return (
    <div class="container-fluid justify-content-center" style={{display: "flex", flexDirection: "column", alignItems: "flex-start", justifyContent: "flex-start", marginTop: "200px", width: "900px"}}>
      <div class="container-fluid justify-content-center" style={{marginTop: "200px", width: "900px"}}>

        <video id="videoPlayer" width="650" controls>
          <source src={videoUrl} type="video/mp4" />
        </video>
        {/* <ReactPlayer
          url={videoUrl}
          playing={true}
          controls={true}
          width="650px"
          height="auto"
          config={{
            file: {
              attributes: {
                controlsList: 'nodownload'
              },
              tracks: [],
              forceVideo: true,
              forceAudio: true,
              hlsOptions: {
                maxBufferLength: 2,
                maxBufferSize: 1000,
                liveSyncDurationCount: 2,
                liveMaxLatencyDurationCount: 4,
              }
            },
            youtube: {
              playerVars: {
                controls: 1,
                modestbranding: 1,
                rel: 0
              }
            },
            attributes: {
              autoPlay: true,
              controls: true,
              muted: true
            }
          }}
        /> */}
        
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