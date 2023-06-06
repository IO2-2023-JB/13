import axios from './api/axios';
import {faInfoCircle  } from "@fortawesome/free-solid-svg-icons"
import { FontAwesomeIcon} from "@fortawesome/react-fontawesome"
import {useRef, useState } from "react"
import AuthContext from "./context/AuthProvider"
import { useContext } from "react";
import { useNavigate} from 'react-router-dom';
import { BounceLoader } from "react-spinners";

const VIDEO_URL = '/video';
const METADATA_URL = '/video-metadata';

const AddVideo = () => {
    const [selectedFile, setSelectedFile] = useState(null);
    
    const { auth } = useContext(AuthContext);
    const navigate = useNavigate();

    const tagsRef = useRef();
    const titleRef = useRef();
    const descriptionRef = useRef();

    const [errMsg, setErrMsg] = useState('');
    const errRef = useRef();
    
    const [tags, setTags] = useState([]);
    const [title, setTitle] = useState("");
    const [description, setDescription] = useState('');
    const [visibility, setVisibility] = useState(false);

    const [thumbnail_picture, setThumbnail_picture] = useState(null);
    const [thumbnail_picture_name, setThumbnail_picture_name] = useState('');
    const [validthumbnail_picture, setValidthumbnail_picture] = useState(false);

    const [isLoading, setIsLoading] = useState(false);

    const onChangeHandler = (event) => {
        setSelectedFile(event.target.files[0]);
    };
    
    const handleCancelClick = () => {
        navigate('/profile');
    };

    const handle_picture = (event) => {

        const file = event.target.files[0];
        const maxSize = 5 * 1024 * 1024; // 5 MB
    
        if (file && file.size <= maxSize) {
            setThumbnail_picture(file);
            setThumbnail_picture_name(file.name);
            setValidthumbnail_picture(true);
        } else {
            setThumbnail_picture(null);
            setThumbnail_picture_name('');
            setValidthumbnail_picture(false);
            alert("Choose a file format .jpg or .png with a maximum size of 5MB.");
        }
    }

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
          setIsLoading(true);
          let response;
          if(validthumbnail_picture)
          {
            const reader = new FileReader();
            await reader.readAsDataURL(thumbnail_picture);
            let base64String;
            reader.onload = () => {
              base64String = reader.result; //.split(",")[1];
            };
            setTimeout(async () => {
            try{
            response = await axios.post(METADATA_URL,
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
                    withCredentials: false //cred
                }
            );
          }catch(err1){
            setIsLoading(false);
            if(!err1?.response) {
              setErrMsg('No Server Response')
            } else if(err1.response?.status === 400) {
                setErrMsg('Bad request');
            } else if(err1.response?.status === 401){
              setErrMsg('Unauthorized');
            } else if(err1.response?.status === 413){
              setErrMsg('Video file is too large');
            } else {
              setErrMsg('Uploading Video failed');
            }
            errRef.current.focus();
          }
            const formData = new FormData();
            formData.append('videoFile', selectedFile);
            try{
            await axios.post(VIDEO_URL + "/" + response?.data.id, 
                formData,
                {
                    headers: { 
                      'Content-Type': 'application/json',
                      'Authorization': `Bearer ${auth?.accessToken}`
                    },
                    withCredentials: false //cred
                }
              );
            handleCancelClick();
            setIsLoading(false);
            }catch(err1){
              setIsLoading(false);
              if(!err1?.response) {
                setErrMsg('No Server Response')
              } else if(err1.response?.status === 400) {
                  setErrMsg('Bad request');
              } else if(err1.response?.status === 401){
                setErrMsg('Unauthorized');
              } else if(err1.response?.status === 413){
                setErrMsg('Video file is too large');
              } else {
                setErrMsg('Uploading Video failed');
              }
              errRef.current.focus();
            }
          }, 100);
          }
          else
          {
            let base64data = null;
            setTimeout(async () => {
            try{
              response = await axios.post(METADATA_URL,
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
                    withCredentials: false //cred
                }
              );
            }catch(err1){
              setIsLoading(false);
              if(!err1?.response) {
                setErrMsg('No Server Response')
              } else if(err1.response?.status === 400) {
                  setErrMsg('Bad request');
              } else if(err1.response?.status === 401){
                setErrMsg('Unauthorized');
              } else if(err1.response?.status === 413){
                setErrMsg('Video file is too large');
              } else {
                setErrMsg('Uploading Video failed');
              }
              errRef.current.focus();
            }
              const formData = new FormData();
              formData.append('videoFile', selectedFile);
              try{
              await axios.post(VIDEO_URL + "/" + response?.data.id,
                formData,
                {
                    headers: { 
                      'Content-Type': 'application/json',
                      'Authorization': `Bearer ${auth?.accessToken}`
                    },
                    withCredentials: false //cred
                }
              );
              handleCancelClick();
              setIsLoading(false);
            }
            catch(err2){
              setIsLoading(false);
              if(!err2?.response) {
                setErrMsg('No Server Response')
              } else if(err2.response?.status === 400) {
                  setErrMsg('Bad request');
              } else if(err2.response?.status === 401){
                setErrMsg('Unauthorized');
              } else if(err2.response?.status === 413){
                setErrMsg('Video file is too large');
              } else {
                setErrMsg('Uploading Video failed');
              }
              errRef.current.focus();
            }
            }, 100);
          }
        } catch (err) {
            setIsLoading(false);
            if(!err?.response) {
              setErrMsg('No Server Response')
            } else if(err.response?.status === 400) {
                setErrMsg('Bad request');
            } else if(err.response?.status === 401){
              setErrMsg('Unauthorized');
            } else if(err.response?.status === 413){
              setErrMsg('Video file is too large');
            } else {
              setErrMsg('Uploading Video failed');
            }
            errRef.current.focus();
        }
      };

    return (
    <div style={{marginTop: "200px"}} class="col-xs-1" align="center"> 
    <h1 class="display-3" style={{marginBottom:"60px"}}>Upload your video</h1>
    <section class="container-fluid justify-content-center mb-5" style={{marginTop:"20px", 
              color:"white", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#333333"}}>
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
          />
          <label htmlFor="tags">
              Tags (separated by commas):
          </label>
          <input
              type="text"
              id="tags"
              ref={tagsRef}
              autoComplete="off"
              onChange={(e) => setTags(e.target.value.split(','))}
              value={tags}
              required
              aria-describedby="uidnote"
          />
          <label htmlFor="thumbnail_picture">
              New Thumbnail (Optional):
          </label>
          <input
              class="btn btn-dark"
              type="file"
              accept="image/*"
              id="thumbnail_picture"
              onChange={handle_picture}
              defaultValue={thumbnail_picture_name}
              aria-describedby="confirmnote"
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

          <label htmlFor="video">
              Video File:
          </label>
          <input
              class="btn btn-dark"
              type="file"
              accept="video/*"
              id="video"
              onChange={onChangeHandler}
          />


          <button class="btn btn-dark" disabled={isLoading || !tags || !title || !description || (selectedFile ===null) ? true : false}>Submit</button>
      </form>
      <button class="btn btn-dark" onClick={handleCancelClick}>Cancel</button>
    </section>
    {isLoading && (
      <div className="loading-container">
        <h4>Your video is beeing uploaded...</h4>
        <BounceLoader color="#ff0000" />
      </div>
    )}
    {/* {errMsg !== '' && (
        <div style={{ color: "red", marginTop: "10px" }}>{errMsg}</div>
    )} */}
  </div>
)}
export default AddVideo