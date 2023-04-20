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
  //let video_id = params.videoid; //JSON.stringify(params.id)
  const baseURL = 'https://io2test.azurewebsites.net';
  console.log(auth.accessToken);
  let video_id = "1637AEEF-B0AC-4E41-7FCB-08DB4119B61C";//"1AC6B4F2-9E85-457D-EC26-08DB4106DCA2"; 
  let videoUrl = baseURL + VIDEO_URL + "/" + video_id + "?access_token=" + auth.accessToken;
  //const videoUrl = 'https://videioblob.blob.core.windows.net/video/sample-30s.mp4';
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
    tags: ["tag1", "tag2", "tag3"],
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

  const [tags, setTags] = useState('');
  const [tagsFocus, setTagsFocus] = useState(false)
  const [title, setTitle] = useState("");
  const [titleFocus, setTitleFocus] = useState(false)
  const [description, setDescription] = useState('');
  const [descriptionFocus, setDescriptionFocus] = useState(false)

  const [thumbnail_picture, setThumbnail_picture] = useState(null);
  const [thumbnail_picture_name, setThumbnail_picture_name] = useState('');
  const [validthumbnail_picture, setValidthumbnail_picture] = useState(false);
  const [thumbnail_pictureFocus, setThumbnail_pictureFocus] = useState(false);
  const [wrong_thumbnail_picture, setWrong_thumbnail_picture] = useState(false);

  useEffect(() => {
    localStorage.setItem("lastVisitedPage", location.pathname);
  })

  useEffect(() => {
    // if(!video_id)
    // {
    //   video_id = "ECF81211-C2B8-4A45-BC2A-61D339C29771";
    //   videoUrl = VIDEO_URL + "/" + video_id + "?access_token=" + auth.accessToken;
    // }
    if(video_id){
      //console.log('video_id:');
      //console.log(video_id);
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
    // e.preventDefault();
    // try {
    //   let response;
    //   if(validthumbnail_picture)
    //   {
    //     const reader = new FileReader();
    //     await reader.readAsDataURL(thumbnail_picture);
    //     let base64String;
    //     reader.onload = () => {
    //       base64String = reader.result.split(",")[1]; //to może być nie ucięte w innych grupach
    //     };
    //     setTimeout(async () => {
    //     response = await axios.put(PROFILE_URL,
    //         JSON.stringify({
    //           nickname: user, 
    //           name: name, 
    //           surname: surname,
    //           userType: auth?.roles === "Viewer" ? 1 : (auth?.roles === "Creator" ? 2 : 3),
    //           avatarImage: base64String
    //         }),
    //         {
    //             headers: { 
    //               'Content-Type': 'application/json',
    //               'Authorization': `Bearer ${auth?.accessToken}`
    //             },
    //             withCredentials: true //cred
    //         }
    //     );
    //     setData(response?.data);
    //     setUserData({
    //       firstName: data?.name,
    //       lastName: data?.surname,
    //       nickname: data?.nickname,
    //       email: data?.email,
    //       accountBalance: data?.accountBalance,
    //       avatarImage: data?.avatarImage,
    //       userType: data?.userType,
    //     });
    //     handleCancelClick();
    //     window.location.reload()
    //   }, 100);
    //   }
    //   else
    //   {
    //     let base64data = null;
    //     if(userData.avatarImage){
    //       //console.log(userData.avatarImage)
    //       const imageUrl = userData.avatarImage+"?time="+new Date();
    //       const response = await fetch(imageUrl);
    //       const blob = await response.blob();
    //       const reader = new FileReader();
    //       reader.readAsDataURL(blob);
    //       reader.onloadend = () => {
    //         base64data = reader.result.split(",")[1];
    //       }
    //     }
    //     else
    //     {
    //       base64data = "";
    //     }
    //     setTimeout(async () => {
    //       response = await axios.put(PROFILE_URL,
    //         JSON.stringify({
    //           nickname: user, 
    //           name: name, 
    //           surname: surname,
    //           userType: auth?.roles === "Viewer" ? 1 : (auth?.roles === "Creator" ? 2 : 3),
    //           avatarImage: base64data
    //         }),
    //         {
    //             headers: { 
    //               'Content-Type': 'application/json',
    //               'Authorization': `Bearer ${auth?.accessToken}`
    //             },
    //             withCredentials: true //cred
    //         }
    //       );
    //       setData(response?.data);
    //       setUserData({
    //         firstName: data?.name,
    //         lastName: data?.surname,
    //         nickname: data?.nickname,
    //         email: data?.email,
    //         accountBalance: data?.accountBalance,
    //         avatarImage: data?.avatarImage,
    //         userType: data?.userType,
    //       });
    //       handleCancelClick();
    //     }, 100);
    //   }
    // } catch (err) {
    //     if (!err?.response) {
    //         setErrMsg('No Server Response');
    //     } else if (err.response?.status === 401) {
    //         setErrMsg('Unauthorized');
    //     } else {
    //         setErrMsg('Data Change Failed');
    //     }
    //     errRef.current.focus();
    // }
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

        <video id="videoPlayer" width="830" controls>
          <source src={videoUrl} type="video/mp4" />
        </video>
        
      </div>
      {!editMode?(
      <div class="container-fluid" style={{backgroundColor: "black", marginTop:"60px", color: "white", borderTopRightRadius: "25px", borderTopLeftRadius: "25px"}}>
          <div style={{borderRadius:"15px", backgroundColor:"#282828"}}>
            <div class="container-fluid justify-content-center" style={{fontSize:"50px", marginTop:"20px", padding: "20px"}}>  {/*className="movie_title"*/}
              {videoData.title}
            </div>
            <div class="container-fluid justify-content-center" style={{fontSize:"20px", marginTop:"0px", paddingTop:"12px", height:"60px"}}>
              Author: {videoData.authorNickname}
            </div>
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
            <div class="container-fluid justify-content-center" style={{fontSize:"18px", marginTop:"0", marginBottom:"200px"}}>
              {videoData.description}
            </div>
          </div>
          {!(videoData.authorId == auth.id) &&(
            <div class="container-fluid justify-content-center" style={{marginBottom: "50px"}}>
              <button onClick={handleEditClick} style={{marginRight:"20px"}}>Edit video metadata</button>
              <button onClick={handleDeleteClick}>Delete video</button>
            </div>
          )}
      </div>
      ):(
        <div style={{marginTop: "200px"}} class="col-xs-1" align="center"> 
        <h1>Edit video metadata</h1>
        <section>
            <p ref={errRef} className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">{errMsg}</p>
            <form onSubmit={handleSubmit}>
                        <label htmlFor="title">
                            Title:
                            <FontAwesomeIcon icon={faCheck} className={title ? "valid" : "hide"} />
                            <FontAwesomeIcon icon={faTimes} className={!title ? "hide" : "invalid"} />
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

                        <label htmlFor="description">
                            Description:
                            <FontAwesomeIcon icon={faCheck} className={description ? "valid" : "hide"} />
                            <FontAwesomeIcon icon={faTimes} className={!description ? "hide" : "invalid"} />
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

                        <label htmlFor="tags">
                            Tags:
                            <FontAwesomeIcon icon={faCheck} className={tags ? "valid" : "hide"} />
                            <FontAwesomeIcon icon={faTimes} className={!tags ? "hide" : "invalid"} />
                        </label>
                        <input
                            type="text"
                            id="tags"
                            ref={tagsRef}
                            autoComplete="off"
                            onChange={(e) => setTags(e.target.value)}
                            value={tags}
                            required
                            aria-describedby="uidnote"
                            onFocus={() => setTagsFocus(true)}
                            onBlur={() => setTagsFocus(false)}
                        />

                        <label htmlFor="thumbnail_picture">
                            New Thumbnail (Optional):
                            <FontAwesomeIcon icon={faCheck} className={validthumbnail_picture && thumbnail_picture ? "valid" : "hide"} />
                            <FontAwesomeIcon icon={faTimes} className={!wrong_thumbnail_picture ? "hide" : "invalid"} />
                        </label>
                        <input
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

                        <button disabled={!tags || !title || !description ? true : false}>Submit</button>
                    </form>
                    <button onClick={handleCancelClick}>Cancel</button>
        </section>
      </div>
      )}
    </div>
  );
};

export default VideoPlayer;