import {useRef, useState, useEffect } from "react"
import AuthContext from "../context/AuthProvider"
import useAuth from '../hooks/useAuth';
import { useContext } from "react";
import axios from '../api/axios';
import {faCheck, faTimes, faInfoCircle  } from "@fortawesome/free-solid-svg-icons"
import { FontAwesomeIcon} from "@fortawesome/react-fontawesome"
import '@fortawesome/fontawesome-svg-core/styles.css';
import { config } from '@fortawesome/fontawesome-svg-core';
import logo from '../images/happy.jpg'

config.autoAddCss = false;

const PROFILE_URL = '/user';
const USER_REGEX = /^[A-z][A-z0-9-_]{3,23}$/;
const NAME_REGEX = /^[A-Z][a-z]{2,17}$/;
const EMAIL_REGEX = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

const ProfilePage = () => {

  const { setAuth } = useAuth();
  const { auth } = useContext(AuthContext);
  const [errMsg, setErrMsg] = useState('');
  const errRef = useRef();

  const [success, setSuccess] = useState(false);

  const [data, setData] = useState(null);

useEffect(() => {
  axios.get(PROFILE_URL + "?id=" + auth?.id, {
    headers: { 
      'Content-Type': 'application/json',
      "Authorization" : `Bearer ${auth?.accessToken}`
    },
    withCredentials: true 
  })
  .then(response => {
    //console.log("success");
    console.log(JSON.stringify(response?.data));
    setData(response?.data);
  })
  .catch(error => {
    console.log("error: ", error);
  });
}, [auth?.accessToken, auth?.id]);

const [userData, setUserData] = useState({
  firstName: "asd",
  lastName: "",
  nickname: "",
  email: "",
  accountBalance: 0,
});

useEffect(() => {
  if (data) {
    setUserData({
      firstName: data?.name,
      lastName: data?.surname,
      nickname: data?.nickname,
      email: data?.email,
      accountBalance: data?.accountBalance,
    });
  }
}, [data]);


const userRef = useRef();
const nameRef = useRef();
const surnameRef = useRef();
const emailRef = useRef();
const [user, setUser] = useState('');
const [validNickname, setValidNickname] = useState(false);
const [userFocus, setUserFocus] = useState(false)
const [name, setName] = useState("");
const [validName, setValidName] = useState(false);
const [nameFocus, setNameFocus] = useState(false)
const [surname, setSurname] = useState('');
const [validSurname, setValidSurname] = useState(false);
const [surnameFocus, setSurnameFocus] = useState(false)
const [email, setEmail] = useState('');
const [validEmail, setValidEmail] = useState(false);
const [emailFocus, setEmailFocus] = useState(false);

useEffect(() => {
  setName(userData.firstName);
}, [userData.firstName]);
useEffect(() => {
  setSurname(userData.lastName);
}, [userData.lastName]);
useEffect(() => {
  setUser(userData.nickname);
}, [userData.nickname]);
useEffect(() => {
  setEmail(userData.email);
}, [userData.email]);

  useEffect(() => {
    setValidNickname(USER_REGEX.test(user));
  }, [user])

  useEffect(() => {
      setErrMsg('');
  }, [user])

  useEffect(() => {
      setValidName(NAME_REGEX.test(name));
  }, [name])

  useEffect(() => {
      setValidSurname(NAME_REGEX.test(surname));
  }, [surname])

  useEffect(() => {
      setValidEmail(EMAIL_REGEX.test(email));
  }, [email])

  const [editMode, setEditMode] = useState(false);

  const handleEditClick = () => {
    setEditMode(true);
  };

  const handleCancelClick = () => {
    setEditMode(false);
    setName(userData.firstName);
    setSurname(userData.lastName);
    setUser(userData.nickname);
    setEmail(userData.email);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const v1 = USER_REGEX.test(user);
    const v2 = NAME_REGEX.test(name)
    const v3 = NAME_REGEX.test(surname)
    const v4 = EMAIL_REGEX.test(email)
    if (!v1 || !v2 || !v3 || !v4) {
        setErrMsg("Invalid Entry");
        return;
    }
    try {
        const response = await axios.put(PROFILE_URL,
            JSON.stringify({
              id: auth?.id,
              email: email, 
              nickname: user, 
              name: name, 
              surname: surname,
              accountBalance: userData.accountBalance,
              userType: auth?.roles == "Viewer" ? 1 : (auth?.roles == "Creator" ? 2 : 3)
            }),
            {
                headers: { 
                  'Content-Type': 'application/json',
                  'Authorization': `Bearer ${auth?.accessToken}`
                },
                withCredentials: true //cred
            }
        );
        //console.log(response?.data);
        //console.log(response?.accessToken);
        //console.log(JSON.stringify(response))
        setSuccess(true);
        //clear state and controlled inputs
        //need value attrib on inputs for this
        setUser('');
        setEmail('')
        setName('')
        setSurname('')
        axios.get(PROFILE_URL + "?id=" + auth?.id, {
          headers: { 
            'Content-Type': 'application/json',
            "Authorization" : `Bearer ${auth?.accessToken}`
          },
          withCredentials: true 
        })
        .then(response => {
          //console.log("success");
          console.log(JSON.stringify(response?.data));
          setData(response?.data);
        })
        .catch(error => {
          console.log("error: ", error);
        });
        handleCancelClick();
    } catch (err) {
        if (!err?.response) {
            setErrMsg('No Server Response');
        } else if (err.response?.status === 400) {
            if(err.response.data && typeof err.response.data === 'object' && err.response.data.DuplicateUserName)
                setErrMsg(err.response.data && typeof err.response.data === 'object' ? err.response.data.DuplicateUserName : 'Registration Failed');
            else
                setErrMsg(err.response.data && typeof err.response.data === 'object' ? err.response.data.DuplicateEmail  : 'Registration Failed');
        } else {
            setErrMsg('Data Change Failed')
        }
        errRef.current.focus();
    }
}

return (
  <div style={{marginTop: "200px"}} class="container">
    {!editMode ? (
      <div class="row">
        <h1 class="display-1">{userData.nickname}</h1>
        <div class ="mt-2 row">
          <div class="col-sm">
            <h2>Data</h2>
            <section>
              <label>Name:</label>
              <div>{userData.firstName}</div>
              <label>Surname:</label>
              <div>{userData.lastName}</div>
              <label>Nickname:</label>
              <div>{userData.nickname}</div>
              <label>Email:</label>
              <div>{userData.email}</div>
              <div>
                <button onClick={handleEditClick}>Edit</button>
              </div>
            </section>
          </div>
          <div class="col-sm">
            <h2>Picture</h2>
            <section>
              <img src = {logo} alt="Italien"/>
              <label>Description:</label>
              <div>Let's go!</div>
              <label>Likes:</label>
              <div>72</div>
            </section>
          </div>
          <div class="col-sm">
          <h2>Videos</h2>
            <section>
              <ul style={{padding:"0px"}}>
                <li style={{listStyleType: "none"}}>
              <div className="box" style={{width:"300px", height:"170px"}}>
                <div className="box2" style={{width:"280px", height:"150px"}}>
                    <table>
                        <tr>
                        <div className="movie_title" style={{width:"280px", height:"50px", fontSize:"10px", marginTop:"0"}}>
                            Test video title for non-existing video
                        </div>
                        </tr>
                        <tr>
                        <div className="movie_thumbnail" style={{width:"280px", height:"100px"}}>
                            <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="white" class="bi bi-play-circle-fill" viewBox="0 0 16 16">
                                <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                            </svg>
                        </div>
                        </tr>
                    </table>
                    </div> 
                </div>
                </li>
                <li style={{listStyleType: "none"}}>
              <div className="box" style={{width:"300px", height:"170px"}}>
                <div className="box2" style={{width:"280px", height:"150px"}}>
                    <table>
                        <tr>
                        <div className="movie_title" style={{width:"280px", height:"50px", fontSize:"10px", marginTop:"0"}}>
                            Test video title for non-existing video
                        </div>
                        </tr>
                        <tr>
                        <div className="movie_thumbnail" style={{width:"280px", height:"100px"}}>
                            <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="white" class="bi bi-play-circle-fill" viewBox="0 0 16 16">
                                <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                            </svg>
                        </div>
                        </tr>
                    </table>
                    </div> 
                </div>
                </li>
                <li style={{listStyleType: "none"}}>
              <div className="box" style={{width:"300px", height:"170px"}}>
                <div className="box2" style={{width:"280px", height:"150px"}}>
                    <table>
                        <tr>
                        <div className="movie_title" style={{width:"280px", height:"50px", fontSize:"10px", marginTop:"0"}}>
                            Test video title for non-existing video
                        </div>
                        </tr>
                        <tr>
                        <div className="movie_thumbnail" style={{width:"280px", height:"100px"}}>
                            <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="white" class="bi bi-play-circle-fill" viewBox="0 0 16 16">
                                <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                            </svg>
                        </div>
                        </tr>
                    </table>
                    </div> 
                </div>
                </li>
                <li style={{listStyleType: "none"}}>
              <div className="box" style={{width:"300px", height:"170px"}}>
                <div className="box2" style={{width:"280px", height:"150px"}}>
                    <table>
                        <tr>
                        <div className="movie_title" style={{width:"280px", height:"50px", fontSize:"10px", marginTop:"0"}}>
                            Test video title for non-existing video
                        </div>
                        </tr>
                        <tr>
                        <div className="movie_thumbnail" style={{width:"280px", height:"100px"}}>
                            <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="white" class="bi bi-play-circle-fill" viewBox="0 0 16 16">
                                <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"/>
                            </svg>
                        </div>
                        </tr>
                    </table>
                    </div> 
                </div>
                </li>
              </ul>
            </section>
          </div>
        </div>
      </div>
    ) : (
      <div style={{marginTop: "200px"}} class="col-xs-1" align="center"> 
        <h1>Change Data</h1>
        <section>
            <p ref={errRef} className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">{errMsg}</p>
            <form onSubmit={handleSubmit}>
                        <label htmlFor="name">
                            Name:
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
                            Surname:
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
                            Nickname:
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

                        <label htmlFor="email">
                            Email:
                            <FontAwesomeIcon icon={faCheck} className={validEmail ? "valid" : "hide"} />
                            <FontAwesomeIcon icon={faTimes} className={validEmail || !email ? "hide" : "invalid"} />
                        </label>
                        <input
                            type="text"
                            id="email"
                            ref={emailRef}
                            autoComplete="off"
                            onChange={(e) => setEmail(e.target.value)}
                            value={email}
                            required
                            aria-invalid={validEmail ? "false" : "true"}
                            aria-describedby="uidnote"
                            onFocus={() => setEmailFocus(true)}
                            onBlur={() => setEmailFocus(false)}
                        />
                        <p id="uidnote" className={emailFocus && email && !validEmail ? "instructions" : "offscreen"}>
                            <FontAwesomeIcon icon={faInfoCircle} />
                            Must be valid email address.<br />
                        </p>

                        <button disabled={!validNickname || !validName || !validSurname || !validEmail ? true : false}>Submit</button>
                    </form>
                    {/* <button disabled={!validNickname || !validName || !validSurname || !validEmail ? true : false} onClick={handleSubmitClick}>Submit</button> */}
                    <button onClick={handleCancelClick}>Cancel</button>
        </section>
      </div>
    )}
    </div>
);


};

export default ProfilePage;