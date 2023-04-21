import {useRef, useState, useEffect } from "react"
import {faCheck, faTimes, faInfoCircle  } from "@fortawesome/free-solid-svg-icons"
import { FontAwesomeIcon} from "@fortawesome/react-fontawesome"
import '@fortawesome/fontawesome-svg-core/styles.css';
import { config } from '@fortawesome/fontawesome-svg-core';
import {useLocation} from 'react-router-dom';
import axios from './api/axios';
config.autoAddCss = false;

const USER_REGEX = /^[A-z][A-z0-9-_]{3,23}$/;
const NAME_REGEX = /^[A-Z][a-z]{2,17}$/;
const PWD_REGEX = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%]).{8,24}$/;
const EMAIL_REGEX = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
const REGISTER_URL = '/register';
const VIDEO_URL = '/video';

const AddVideo = () => {

    const location = useLocation();
    const userRef = useRef();
    const nameRef = useRef();
    const surnameRef = useRef();
    const emailRef = useRef();
    const errRef = useRef();
    const [user, setUser] = useState('');
    const [validNickname, setValidNickname] = useState(false);
    const [userFocus, setUserFocus] = useState(false);

    const [pwd, setPwd] = useState('');
    const [validPwd, setValidPwd] = useState(false);
    const [pwdFocus, setPwdFocus] = useState(false);

    const [matchPwd, setMatchPwd] = useState('');
    const [validMatch, setValidMatch] = useState(false);
    const [matchFocus, setMatchFocus] = useState(false);

    const [name, setName] = useState('');
    const [validName, setValidName] = useState(false);
    const [nameFocus, setNameFocus] = useState(false);

    const [surname, setSurname] = useState('');
    const [validSurname, setValidSurname] = useState(false);
    const [surnameFocus, setSurnameFocus] = useState(false);

    const [email, setEmail] = useState('');
    const [validEmail, setValidEmail] = useState(false);
    const [emailFocus, setEmailFocus] = useState(false);

    const [profile_picture, setProfile_picture] = useState(null);
    const [profile_picture_name, setProfile_picture_name] = useState('');
    const [validprofile_picture, setValidprofile_picture] = useState(false);
    const [profile_pictureFocus, setProfile_pictureFocus] = useState(false);
    const [wrong_profile_picture, setWrong_profile_picture] = useState(false);

    const [isCreatorChecked, setIsCreatorChecked] = useState(false);

    const [errMsg, setErrMsg] = useState('');
    const [success, setSuccess] = useState(false);

    useEffect(() => {
        localStorage.setItem("lastVisitedPage", location.pathname);
    })

    useEffect(() => {
        userRef.current.focus();
    }, [])

    useEffect(() => {
        setValidNickname(USER_REGEX.test(user));
    }, [user])

    useEffect(() => {
        setValidPwd(PWD_REGEX.test(pwd));
        setValidMatch(pwd === matchPwd);
    }, [pwd, matchPwd])

    useEffect(() => {
        setErrMsg('');
    }, [user, pwd, matchPwd])

    useEffect(() => {
        setValidName(NAME_REGEX.test(name));
    }, [name])

    useEffect(() => {
        setValidSurname(NAME_REGEX.test(surname));
    }, [surname])

    useEffect(() => {
        setValidEmail(EMAIL_REGEX.test(email));
    }, [email])

    const onChangeHandler = (event) => {
        this.setState({
            selectedFile: event.target.files[0],
            loaded: 0,
        });
        console.log(event.target.files[0]);
    };
    
    const handle_picture = (event) => {

        const file = event.target.files[0];
        const maxSize = 5 * 1024 * 1024; // 5 MB

        if (file && file.size <= maxSize) {
            //console.log(file.type);
            setProfile_picture(file);
            setProfile_picture_name(file.name);
            setValidprofile_picture(true);
            setWrong_profile_picture(false);
        } else {
            setProfile_picture(null);
            setProfile_picture_name('');
            setValidprofile_picture(false);
            setWrong_profile_picture(true);
            alert("Choose a file format .jpg or .png with a maximum size of 5MB.");
        }
    }

    const handleSubmit = async (e) => {
        e.preventDefault();
        // if button enabled with JS hack
        const v1 = USER_REGEX.test(user);
        const v2 = PWD_REGEX.test(pwd);
        const v3 = NAME_REGEX.test(name)
        const v4 = NAME_REGEX.test(surname)
        const v5 = EMAIL_REGEX.test(email)
        if (!v1 || !v2 || !v3 || !v4 || !v5) {
            setErrMsg("Invalid Entry");
            return;
        }
        try {
            let response;
            if(validprofile_picture)
            {
                const reader = new FileReader();
                reader.readAsDataURL(profile_picture);
                let base64String;
                reader.onload = () => {
                    base64String = reader.result.split(",")[1];
                };
                setTimeout(async () => {
                response = await axios.post(REGISTER_URL,
                    JSON.stringify({ email: email, nickname: user, name: name, 
                        surname: surname, password: pwd, userType: isCreatorChecked?"Creator":"Viewer", AvatarImage: base64String }),
                    {
                        headers: { 'Content-Type': 'application/json' },
                        withCredentials: true //cred
                    }
                );
                }, 1000);
            }
            else
            {
                response = await axios.post(REGISTER_URL,
                    JSON.stringify({ email: email, nickname: user, name: name, 
                        surname: surname, password: pwd, userType: isCreatorChecked?"Creator":"Viewer", AvatarImage: "" }), //userType: "Simple"
                    {
                        headers: { 'Content-Type': 'application/json' },
                        withCredentials: true //cred
                    }
                );
            }
            //console.log(response?.data);
            //console.log(response?.accessToken);
            //console.log(JSON.stringify(response))
            setSuccess(true);
            setUser('');
            setPwd('');
            setMatchPwd('');
            setEmail('')
            setName('')
            setSurname('')
        } catch (err) {
            if (!err?.response) {
                setErrMsg('No Server Response');
            } else if (err.response?.status === 400) {
                setErrMsg('Registration Failed');
            } else if(err?.status === 409){
                setErrMsg('A user with this e-mail address already exists');
            } else {
                setErrMsg('Registration Failed')
            }
            errRef.current.focus();
        }
    }

    return (
        <section class="container-fluid justify-content-center" style={{marginTop:"200px"}}>
            <h1 style={{color: "white"}}>Upload your video</h1>
            
            <form onSubmit={handleSubmit}>
                <label htmlFor="name">
                    Title:
                    <FontAwesomeIcon icon={faCheck} className={validName ? "valid" : "hide"} />
                    <FontAwesomeIcon icon={faTimes} className={validName || !name ? "hide" : "invalid"} />
                </label>
                <input
                    type="text"
                    id="name"
                    ref={nameRef}
                    autoComplete="off"
                    onChange={(e) => setName(e.target.value)}
                    value={name}
                    required
                    aria-invalid={validName ? "false" : "true"}
                    aria-describedby="uidnote"
                    onFocus={() => setNameFocus(true)}
                    onBlur={() => setNameFocus(false)}
                />
                <p id="uidnote" className={nameFocus && name && !validName ? "instructions" : "offscreen"}>
                    <FontAwesomeIcon icon={faInfoCircle} />
                    3 to 18 characters.<br />
                    Must begin with a capital letter.<br />
                    Only letters allowed.
                </p>

                <label htmlFor="surname">
                    Description:
                    <FontAwesomeIcon icon={faCheck} className={validSurname ? "valid" : "hide"} />
                    <FontAwesomeIcon icon={faTimes} className={validSurname || !surname ? "hide" : "invalid"} />
                </label>
                <input
                    type="text"
                    id="surname"
                    ref={surnameRef}
                    autoComplete="off"
                    onChange={(e) => setSurname(e.target.value)}
                    value={surname}
                    required
                    aria-invalid={validSurname ? "false" : "true"}
                    aria-describedby="uidnote"
                    onFocus={() => setSurnameFocus(true)}
                    onBlur={() => setSurnameFocus(false)}
                />
                <p id="uidnote" className={surnameFocus && surname && !validSurname ? "instructions" : "offscreen"}>
                    <FontAwesomeIcon icon={faInfoCircle} />
                    3 to 18 characters.<br />
                    Must begin with a capital letter.<br />
                    Only letters allowed.
                </p>

                <label htmlFor="username">
                    Tags:
                    <FontAwesomeIcon icon={faCheck} className={validNickname ? "valid" : "hide"} />
                    <FontAwesomeIcon icon={faTimes} className={validNickname || !user ? "hide" : "invalid"} />
                </label>
                <input
                    type="text"
                    id="username"
                    ref={userRef}
                    autoComplete="off"
                    onChange={(e) => setUser(e.target.value)}
                    value={user}
                    required
                    aria-invalid={validNickname ? "false" : "true"}
                    aria-describedby="uidnote"
                    onFocus={() => setUserFocus(true)}
                    onBlur={() => setUserFocus(false)}
                />
                <p id="uidnote" className={userFocus && user && !validNickname ? "instructions" : "offscreen"}>
                    <FontAwesomeIcon icon={faInfoCircle} />
                    4 to 24 characters.<br />
                    Must begin with a letter.<br />
                    Letters, numbers, underscores, hyphens allowed.
                </p>

                <label htmlFor="profile_picture">
                    Thumbnail:
                    <FontAwesomeIcon icon={faCheck} className={validprofile_picture && profile_picture ? "valid" : "hide"} />
                    <FontAwesomeIcon icon={faTimes} className={!wrong_profile_picture ? "hide" : "invalid"} /> {/* validprofile_picture || !profile_picture */}
                </label>
                <input
                    type="file"
                    accept="image/*"
                    id="profile_picture"
                    //key={profile_picture}
                    onChange={handle_picture}
                    defaultValue={profile_picture_name}
                    //value={profile_picture_name}
                    //required
                    aria-invalid={!wrong_profile_picture ? "false" : "true"}//
                    aria-describedby="confirmnote"
                    onFocus={() => setProfile_pictureFocus(true)}
                    onBlur={() => setProfile_pictureFocus(false)}
                />
                <p id="confirmnote" className={!validprofile_picture ? "instructions" : "offscreen"}> {/*profile_pictureFocus && */ }
                    <FontAwesomeIcon icon={faInfoCircle} />
                    Must be image up to 5 MB!
                </p>

                <label htmlFor="video_file">
                    Video file:
                    <FontAwesomeIcon icon={faCheck} className={validprofile_picture && profile_picture ? "valid" : "hide"} />
                    <FontAwesomeIcon icon={faTimes} className={!wrong_profile_picture ? "hide" : "invalid"} /> {/* validprofile_picture || !profile_picture */}
                </label>
                <input
                    type="file"
                    accept="video/*"
                    id="video"
                    onChange={onChangeHandler}
                />
                <button onClick={handleSubmit}>Upload</button>
            </form>
        </section>
    )
}

export default AddVideo