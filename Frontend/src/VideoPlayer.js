import React from "react";
import axios from './api/axios';
import {useRef, useState, useEffect } from "react"
import AuthContext from "./context/AuthProvider"
import { useContext } from "react";
import {useLocation, useNavigate} from 'react-router-dom';
import { useParams } from "react-router-dom";
import 'video-react/dist/video-react.css';
import {faCheck, faTimes, faInfoCircle  } from "@fortawesome/free-solid-svg-icons"
import { FontAwesomeIcon} from "@fortawesome/react-fontawesome"
import '@fortawesome/fontawesome-svg-core/styles.css';

const VIDEO_URL = '/video';
const METADATA_URL = '/video-metadata';

const VideoPlayer = () => {
  const params = useParams();
  const location = useLocation();
  const { auth } = useContext(AuthContext);
  const navigate = useNavigate();
  let video_id = params.videoid; //JSON.stringify(params.id)
  const baseURL = 'https://io2test.azurewebsites.net';
  console.log(auth.accessToken);
  //let video_id;// = "7373CB25-946C-460A-F7C8-08DB425CABE3";//"1637AEEF-B0AC-4E41-7FCB-08DB4119B61C";//"1AC6B4F2-9E85-457D-EC26-08DB4106DCA2"; 
  let videoUrl = baseURL + VIDEO_URL + "/" + video_id + "?access_token=" + auth.accessToken;
  const videoRef = useRef(null);
  const [errMsg, setErrMsg] = useState('');
  const errRef = useRef();
  const [videoData, setVideoData] = useState({
    id: "",
    title: "Loading...",
    description: "Loading...",
    thumbnail: "",
    authorId: "",
    authorNickname: "Loading...",
    viewCount: 0,
    tags: ["loading"],
    visibility: "Private",
    processingProgress: "",
    uploadDate: "",
    editDate: "",
    duration: "",
  });
  const [editMode, setEditMode] = useState(false);
  const tagsRef = useRef();
  const titleRef = useRef();
  const descriptionRef = useRef();

  const [tags, setTags] = useState([]);
  const [tagsFocus, setTagsFocus] = useState(false)
  const [title, setTitle] = useState("");
  const [titleFocus, setTitleFocus] = useState(false)
  const [description, setDescription] = useState('');
  const [descriptionFocus, setDescriptionFocus] = useState(false)
  const [visibility, setVisibility] = useState(false);
  const [visibilityFocus, setVisibilityFocus] = useState(false)

  const [thumbnail_picture, setThumbnail_picture] = useState(null);
  const [thumbnail_picture_name, setThumbnail_picture_name] = useState('');
  const [validthumbnail_picture, setValidthumbnail_picture] = useState(false);
  const [thumbnail_pictureFocus, setThumbnail_pictureFocus] = useState(false);
  const [wrong_thumbnail_picture, setWrong_thumbnail_picture] = useState(false);

  useEffect(() => {
    localStorage.setItem("lastVisitedPage", location.pathname);
  })

  useEffect(() => {
    if(params.videoid)
    {
      video_id = params.videoid;
      videoUrl = baseURL + VIDEO_URL + "/" + video_id + "?access_token=" + auth.accessToken;
    }
    else
    {
      video_id = "7373CB25-946C-460A-F7C8-08DB425CABE3";
      videoUrl = baseURL + VIDEO_URL + "/" + video_id + "?access_token=" + auth.accessToken;
      const videoElement = videoRef.current;
      videoElement.src = videoUrl;
      videoElement.load();
    }//to delete
    if(video_id){
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
  }, [params.videoid])

  const handleEditClick = () => {
    setEditMode(true);
  };

  const handleDeleteClick = () => {
    axios.delete(VIDEO_URL + "?id=" + video_id,
          {
            headers: { 
              'Content-Type': 'application/json',
              "Authorization" : `Bearer ${auth?.accessToken}`
            },
            withCredentials: true
          }
      ).then(response => {
        navigate('/profile');
      }).catch(err => {
        if(!err?.response) {
            setErrMsg('No Server Response')
        } else if(err.response?.status === 400) {
            setErrMsg('Bad request');
        } else if(err.response?.status === 401){
            setErrMsg('Unauthorised');
        } else if(err.response?.status === 403){
            setErrMsg('Forbidden');
        } else if(err.response?.status === 404){
          setErrMsg('Not found');
        } else {
            setErrMsg('Deleting video failed');
        }
      });
  };

  const handleCancelClick = () => {
    setEditMode(false);
    setTitle(videoData.title);
    setDescription(videoData.description);
    setTags(videoData.tags);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      let response;
      if(validthumbnail_picture)
      {
        const reader = new FileReader();
        await reader.readAsDataURL(thumbnail_picture);
        let base64String;
        reader.onload = () => {
          base64String = reader.result.split(",")[1]; //to może być nie ucięte w innych grupach
        };
        setTimeout(async () => {
        response = await axios.put(METADATA_URL + "?id=" + video_id,
            JSON.stringify({
              title: title, 
              description: description,
              thumbnail: base64String,
              tags: tags,
              visibility: visibility?"Public":"Private"
            }),
            {
                headers: { 
                  'Content-Type': 'application/json',
                  'Authorization': `Bearer ${auth?.accessToken}`
                },
                withCredentials: true //cred
            }
        );
        //setVideoData(response?.data);
        handleCancelClick();
        window.location.reload()
      }, 100);
      }
      else
      {
        let base64data = null;
        if(videoData.thumbnail){
          const imageUrl = videoData.thumbnail+"?time="+new Date();
          const response = await fetch(imageUrl);
          const blob = await response.blob();
          const reader = new FileReader();
          reader.readAsDataURL(blob);
          reader.onloadend = () => {
            base64data = reader.result.split(",")[1];
          }
        }
        else
        {
          base64data = "";
        }
        setTimeout(async () => {
          response = await axios.put(METADATA_URL + "?id=" + video_id,
            JSON.stringify({
              title: title, 
              description: description,
              thumbnail: base64data,
              tags: tags,
              visibility: visibility?"Public":"Private"
            }),
            {
                headers: { 
                  'Content-Type': 'application/json',
                  'Authorization': `Bearer ${auth?.accessToken}`
                },
                withCredentials: true //cred
            }
          );
          //setVideoData(response?.data);
          handleCancelClick();
        }, 100);
      }
    } catch (err) {
        if (!err?.response) {
            setErrMsg('No Server Response');
        } else if (err.response?.status === 401) {
            setErrMsg('Unauthorized');
        } else {
            setErrMsg('Data Change Failed');
        }
        errRef.current.focus();
    }
  };

  

  const handle_picture = (event) => {

    const file = event.target.files[0];
    const maxSize = 5 * 1024 * 1024; // 5 MB

    if (file && file.size <= maxSize) {
        setThumbnail_picture(file);
        setThumbnail_picture_name(file.name);
        setValidthumbnail_picture(true);
        setWrong_thumbnail_picture(false);
    } else {
        setThumbnail_picture(null);
        setThumbnail_picture_name('');
        setValidthumbnail_picture(false);
        setWrong_thumbnail_picture(true);
        alert("Choose a file format .jpg or .png with a maximum size of 5MB.");
    }
  }

  return (
    <div class="container-fluid justify-content-center" style={{display: "flex", flexDirection: "column", alignItems: "flex-start", 
      justifyContent: "flex-start", marginTop: "150px", width: "900px", backgroundColor:"#333333", borderTopRightRadius: "25px", borderTopLeftRadius: "25px"}}>
      <div class="container-fluid justify-content-center" style={{marginTop: "50px", width: "900px",}}>

        <video id="videoPlayer" width="830" controls ref={videoRef}>
          <source src={videoUrl} type="video/mp4" />
        </video>
        
      </div>
      {!editMode?(
      <div class="container-fluid" style={{position:"relative", backgroundColor: "black", marginTop:"60px", color: "white", borderTopRightRadius: "25px", borderTopLeftRadius: "25px"}}>
          <div style={{borderRadius:"15px", backgroundColor:"#282828"}}>
            <div class="container-fluid justify-content-center" style={{fontSize:"50px", marginTop:"20px", padding: "20px"}}>  {/*className="movie_title"*/}
              {videoData.title}
            </div>
            <div class="container-fluid justify-content-center" style={{fontSize:"20px", marginTop:"0px", paddingTop:"12px", height:"60px"}}>
              Author: {videoData.authorNickname}
            </div>
          </div>
          <div class="container-fluid justify-content-center" style={{marginTop:"20px", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#282828"}}>
            <button class="btn btn-dark" style={{marginRight:"20px"}}>
              <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" class="bi bi-hand-thumbs-up" viewBox="0 0 16 16">
                <path d="M8.864.046C7.908-.193 7.02.53 6.956 1.466c-.072 1.051-.23 2.016-.428 2.59-.125.36-.479 1.013-1.04 1.639-.557.623-1.282 1.178-2.131 1.41C2.685 7.288 2 7.87 2 8.72v4.001c0 .845.682 1.464 1.448 1.545 1.07.114 1.564.415 2.068.723l.048.03c.272.165.578.348.97.484.397.136.861.217 1.466.217h3.5c.937 0 1.599-.477 1.934-1.064a1.86 1.86 0 0 0 .254-.912c0-.152-.023-.312-.077-.464.201-.263.38-.578.488-.901.11-.33.172-.762.004-1.149.069-.13.12-.269.159-.403.077-.27.113-.568.113-.857 0-.288-.036-.585-.113-.856a2.144 2.144 0 0 0-.138-.362 1.9 1.9 0 0 0 .234-1.734c-.206-.592-.682-1.1-1.2-1.272-.847-.282-1.803-.276-2.516-.211a9.84 9.84 0 0 0-.443.05 9.365 9.365 0 0 0-.062-4.509A1.38 1.38 0 0 0 9.125.111L8.864.046zM11.5 14.721H8c-.51 0-.863-.069-1.14-.164-.281-.097-.506-.228-.776-.393l-.04-.024c-.555-.339-1.198-.731-2.49-.868-.333-.036-.554-.29-.554-.55V8.72c0-.254.226-.543.62-.65 1.095-.3 1.977-.996 2.614-1.708.635-.71 1.064-1.475 1.238-1.978.243-.7.407-1.768.482-2.85.025-.362.36-.594.667-.518l.262.066c.16.04.258.143.288.255a8.34 8.34 0 0 1-.145 4.725.5.5 0 0 0 .595.644l.003-.001.014-.003.058-.014a8.908 8.908 0 0 1 1.036-.157c.663-.06 1.457-.054 2.11.164.175.058.45.3.57.65.107.308.087.67-.266 1.022l-.353.353.353.354c.043.043.105.141.154.315.048.167.075.37.075.581 0 .212-.027.414-.075.582-.05.174-.111.272-.154.315l-.353.353.353.354c.047.047.109.177.005.488a2.224 2.224 0 0 1-.505.805l-.353.353.353.354c.006.005.041.05.041.17a.866.866 0 0 1-.121.416c-.165.288-.503.56-1.066.56z"/>
              </svg>
            </button>
            <button class="btn btn-dark" style={{marginRight:"20px"}}>
              <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" class="bi bi-hand-thumbs-down" viewBox="0 0 16 16">
                <path d="M8.864 15.674c-.956.24-1.843-.484-1.908-1.42-.072-1.05-.23-2.015-.428-2.59-.125-.36-.479-1.012-1.04-1.638-.557-.624-1.282-1.179-2.131-1.41C2.685 8.432 2 7.85 2 7V3c0-.845.682-1.464 1.448-1.546 1.07-.113 1.564-.415 2.068-.723l.048-.029c.272-.166.578-.349.97-.484C6.931.08 7.395 0 8 0h3.5c.937 0 1.599.478 1.934 1.064.164.287.254.607.254.913 0 .152-.023.312-.077.464.201.262.38.577.488.9.11.33.172.762.004 1.15.069.13.12.268.159.403.077.27.113.567.113.856 0 .289-.036.586-.113.856-.035.12-.08.244-.138.363.394.571.418 1.2.234 1.733-.206.592-.682 1.1-1.2 1.272-.847.283-1.803.276-2.516.211a9.877 9.877 0 0 1-.443-.05 9.364 9.364 0 0 1-.062 4.51c-.138.508-.55.848-1.012.964l-.261.065zM11.5 1H8c-.51 0-.863.068-1.14.163-.281.097-.506.229-.776.393l-.04.025c-.555.338-1.198.73-2.49.868-.333.035-.554.29-.554.55V7c0 .255.226.543.62.65 1.095.3 1.977.997 2.614 1.709.635.71 1.064 1.475 1.238 1.977.243.7.407 1.768.482 2.85.025.362.36.595.667.518l.262-.065c.16-.04.258-.144.288-.255a8.34 8.34 0 0 0-.145-4.726.5.5 0 0 1 .595-.643h.003l.014.004.058.013a8.912 8.912 0 0 0 1.036.157c.663.06 1.457.054 2.11-.163.175-.059.45-.301.57-.651.107-.308.087-.67-.266-1.021L12.793 7l.353-.354c.043-.042.105-.14.154-.315.048-.167.075-.37.075-.581 0-.211-.027-.414-.075-.581-.05-.174-.111-.273-.154-.315l-.353-.354.353-.354c.047-.047.109-.176.005-.488a2.224 2.224 0 0 0-.505-.804l-.353-.354.353-.354c.006-.005.041-.05.041-.17a.866.866 0 0 0-.121-.415C12.4 1.272 12.063 1 11.5 1z"/>
              </svg>
            </button>
            <button class="btn btn-dark" style={{marginRight:"20px"}}>
              <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" class="bi bi-share" viewBox="0 0 16 16">
                <path d="M13.5 1a1.5 1.5 0 1 0 0 3 1.5 1.5 0 0 0 0-3zM11 2.5a2.5 2.5 0 1 1 .603 1.628l-6.718 3.12a2.499 2.499 0 0 1 0 1.504l6.718 3.12a2.5 2.5 0 1 1-.488.876l-6.718-3.12a2.5 2.5 0 1 1 0-3.256l6.718-3.12A2.5 2.5 0 0 1 11 2.5zm-8.5 4a1.5 1.5 0 1 0 0 3 1.5 1.5 0 0 0 0-3zm11 5.5a1.5 1.5 0 1 0 0 3 1.5 1.5 0 0 0 0-3z"/>
              </svg>
            </button>
            <button class="btn btn-dark" style={{marginRight:"20px"}}>
              <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" class="bi bi-download" viewBox="0 0 16 16">
                <path d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5z"/>
                <path d="M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708l3 3z"/>
              </svg>
            </button>
            <button class="btn btn-dark" style={{marginRight:"20px", position: "absolute", right: "10px"}}>
              <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" class="bi bi-three-dots" viewBox="0 0 16 16">
                <path d="M3 9.5a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3z"/>
              </svg>
            </button>
          </div>
          <div style={{marginTop:"20px", borderRadius:"15px", paddingBottom:"50px", paddingTop:"20px", backgroundColor:"#282828"}}>
            <div class="container-fluid justify-content-center" style={{fontSize:"18px", marginTop:"0"}}>
              Views: {videoData.viewCount}
            </div>
            <div class="container-fluid justify-content-center" style={{fontSize:"18px", marginTop:"0"}}>
              Upload date: {videoData.uploadDate}
            </div>
            <div class="container-fluid justify-content-center" style={{fontSize:"18px", marginTop:"0"}}>
              Tags: {videoData.tags.join(", ")}
            </div>
            <div class="container-fluid justify-content-center" style={{fontSize:"18px", marginTop:"20px", marginBottom:"200px"}}>
              {videoData.description}
            </div>
          </div>
          {!(videoData.authorId == auth.id) &&(
            <div class="container-fluid justify-content-center" style={{marginBottom: "50px"}}>
              <button onClick={handleEditClick} class="btn btn-dark" style={{marginRight:"20px"}}>Edit video metadata</button>
              <button onClick={handleDeleteClick} class="btn btn-dark">Delete video</button>
            </div>
          )}
      </div>
      ):(
        <div style={{marginTop: "50px"}} class="container-fluid justify-content-center" align="center"> 
        <h1 class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", marginBottom:"50px", padding:"30px", backgroundColor:"#282828"}}>Edit video metadata</h1>
        <section class="container-fluid justify-content-center" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", marginBottom:"100px", padding:"30px", backgroundColor:"#282828"}}>
            <p ref={errRef} className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">{errMsg}</p>
            <form onSubmit={handleSubmit}>
                        <label style={{color:"white"}} htmlFor="title">
                            Title:
                        </label>
                        <input
                            type="text"
                            id="title"
                            ref={titleRef}
                            autoComplete="off"
                            onChange={(e) => setTitle(e.target.value)}
                            value={title}
                            required
                            aria-describedby="uidnote"
                            onFocus={() => setTitleFocus(true)}
                            onBlur={() => setTitleFocus(false)}
                        />

                        <label style={{color:"white"}} htmlFor="description">
                            Description:
                        </label>
                        <input
                            type="text"
                            id="description"
                            ref={descriptionRef}
                            autoComplete="off"
                            onChange={(e) => setDescription(e.target.value)}
                            value={description}
                            required
                            aria-describedby="uidnote"
                            onFocus={() => setDescriptionFocus(true)}
                            onBlur={() => setDescriptionFocus(false)}
                        />

                        <label style={{color:"white"}} htmlFor="tags">
                            Tags:
                        </label>
                        <input
                            type="text"
                            id="tags"
                            ref={tagsRef}
                            autoComplete="off"
                            onChange={(e) => setTags(e.target.value.split(', '))}
                            value={tags}
                            required
                            aria-describedby="uidnote"
                            onFocus={() => setTagsFocus(true)}
                            onBlur={() => setTagsFocus(false)}
                        />

                        <label style={{color:"white"}} htmlFor="thumbnail_picture">
                            Thumbnail (Optional):
                        </label>
                        <input
                          class="btn btn-dark"  
                            type="file"
                            accept="image/*"
                            id="thumbnail_picture"
                            //key={profile_picture}
                            onChange={handle_picture}
                            defaultValue={thumbnail_picture_name}
                            //value={profile_picture_name}
                            //required
                            //aria-invalid={validMatch ? "false" : "true"}//
                            aria-describedby="confirmnote"
                            onFocus={() => setThumbnail_pictureFocus(true)}
                            onBlur={() => setThumbnail_pictureFocus(false)}
                        />
                        <p id="confirmnote" className={!validthumbnail_picture ? "instructions" : "offscreen"}>
                            <FontAwesomeIcon icon={faInfoCircle} />
                            Must be image up to 5 MB!
                        </p>
                        <label style={{color:"white"}} htmlFor="terms">
                            <input
                                type="checkbox"
                                id="terms"
                                onChange={() => setVisibility(!visibility)}
                                checked={visibility}
                            />
                            <text> I want my video to be public</text>
                        </label>

                        <button class="btn btn-dark" disabled={!tags || !title || !description ? true : false}>Submit</button>
                    </form>
                    <button class="btn btn-dark" onClick={handleCancelClick}>Cancel</button>
        </section>
      </div>
      )}
    </div>
  );
};

export default VideoPlayer;