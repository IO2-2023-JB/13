import React, { Component } from 'react';
import axios from './api/axios';
import {faInfoCircle  } from "@fortawesome/free-solid-svg-icons"
import { FontAwesomeIcon} from "@fortawesome/react-fontawesome"
import {useRef, useState, useEffect } from "react"
import AuthContext from "./context/AuthProvider"
import { useContext } from "react";
import {useLocation, useNavigate} from 'react-router-dom';
import { useParams } from "react-router-dom";
const VIDEO_URL = '/video';

const METADATA_URL = '/video-metadata';

const AddVideo = () => {
    const [selectedFile, setSelectedFile] = useState(null);
    
    
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

    const tagsRef = useRef();
    const titleRef = useRef();
    const descriptionRef = useRef();

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
      tags:  [
        "tag1"
      ],
      visibility: "Private",
      processingProgress: "",
      uploadDate: "",
      editDate: "",
      duration: "",

    });
    

    const [tags, setTags] = useState(["tag1"]);
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

    const onChangeHandler = (event) => {
        setSelectedFile(event.target.files[0]);
        console.log(event.target.files[0]);
    };
    const handleCancelClick = () => {
        setTitle(videoData.title);
        setDescription(videoData.description);
        setTags(videoData.tags);
        navigate('/profile');
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
            response = await axios.post(METADATA_URL,
                JSON.stringify({
                  title: title, 
                  description: description,
                  thumbnail: base64String,
                  tags: tags,
                  visibility: visibility?0:1
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
            const formData = new FormData();
            formData.append('videoFile', selectedFile);
            const response2 = await axios.post(VIDEO_URL + "/" + response?.data.id, 
                formData,
                {
                    headers: { 
                      'Content-Type': 'application/json',
                      'Authorization': `Bearer ${auth?.accessToken}`
                    },
                    withCredentials: true //cred
                }
              );
            handleCancelClick();
            //window.location.reload()
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
              response = await axios.post(METADATA_URL,
                JSON.stringify({
                  title: title, 
                  description: description,
                  thumbnail: base64data,
                  tags: tags,
                  visibility: visibility?0:1
                }),
                {
                    headers: { 
                      'Content-Type': 'application/json',
                      'Authorization': `Bearer ${auth?.accessToken}`
                    },
                    withCredentials: true //cred
                }
              );
              const formData = new FormData();
            formData.append('videoFile', selectedFile);
            const response2 = await axios.post(VIDEO_URL + "/" + response?.data.id,
                formData,
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

    // const handleSubmit = (event) => {
    //     event.preventDefault();
    //     const formData = new FormData();
    //     const { selectedFile } = this.state;
    //     formData.append('title', 'test');
    //     formData.append('description', 'test');
    //     formData.append('thumbnail', {  position: 0,
    //                                     readTimeout: 0,
    //                                     writeTimeout: 0 });
    //     formData.append('tags', ["xD"]);
    //     formData.append('inputFile', selectedFile);
    //     fetch(VIDEO_URL, {
    //         method: 'POST',
    //         body: formData,
    //     });
    // }




    return (
        <div style={{marginTop: "200px"}} class="col-xs-1" align="center"> 
    <h1>Edit video metadata</h1>
    <section>
        <p ref={errRef} className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">{errMsg}</p>
        <form onSubmit={handleSubmit}>
                    <label htmlFor="title">
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
                    <label htmlFor="description">
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
                    <label htmlFor="tags">
                        Tags:
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
                    <label htmlFor="terms">
                        <input
                            type="checkbox"
                            id="terms"
                            onChange={() => setVisibility(!visibility)}
                            checked={visibility}
                        />
                        <text> I want my video to be public</text>
                    </label>

                    <h1 style={{color: "white"}}>Upload your video</h1>
                    <input
                        style ={{marginTop:"100px"}}
                        type="file"
                        accept="video/*"
                        id="video"
                        onChange={onChangeHandler}
                    />


                    <button disabled={!tags || !title || !description ? true : false}>Submit</button>
                </form>
                <button onClick={handleCancelClick}>Cancel</button>
    
  
        {/* <section class="container-fluid justify-content-center" style={{marginTop:"200px"}}>
            <h1 style={{color: "white"}}>Upload your video</h1>
            
            <input 
                style ={{marginTop:"100px"}}
                type="file"
                accept="video/*"
                id="video"
                onChange={onChangeHandler}
            />
            <button onClick={handleSubmit}>Upload</button>
        </section> */}
        </section>
    </div>
    )
}
export default AddVideo