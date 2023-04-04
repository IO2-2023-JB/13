import React from "react";
import axios from './api/axios';
import {useRef, useState, useEffect } from "react"
import AuthContext from "./context/AuthProvider"
import { useContext } from "react";
import ReactHlsPlayer from 'react-hls-player';
import { Player, ControlBar } from 'video-react';
import 'video-react/dist/video-react.css';


const GETVIDEO_URL = '/video';

//const VideoPlayer = ({ url/id }) => {
const VideoPlayer = () => {

  const video_id = 'aasd';
  const { auth } = useContext(AuthContext);
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
    <div>
      <ReactHlsPlayer
        src={video_url}
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
  );
};

export default VideoPlayer;