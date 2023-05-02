import { useRef, useState, useEffect } from 'react';
import jwt_decode  from 'jwt-decode';
import axios from '../api/axios';
import useAuth from '../hooks/useAuth';
import { useNavigate, useLocation } from 'react-router-dom'
import {cookies} from '../App'

const LOGIN_URL = '/login';

const Login = () => {
    const { setAuth } = useAuth();

    const navigate = useNavigate();
    const location = useLocation();
    const from = location.state?.from?.pathname || "/home";

    const emailRef = useRef();
    const errRef = useRef();

    const [email, setUser] = useState('');
    const [pwd, setPwd] = useState('');
    const [errMsg, setErrMsg] = useState('');

    useEffect(() => {
        localStorage.setItem("lastVisitedPage", location.pathname);
      })

    useEffect(() => {
        emailRef.current.focus();
    }, [])

    useEffect(() => {
        setErrMsg('');
    }, [email, pwd])

    const handleSubmit = async (e) => {
        e.preventDefault();
        try{
            const response = await axios.post(LOGIN_URL, 
                JSON.stringify({email: email, password: pwd}),
                {
                    headers: { 'Content-Type': 'application/json'},
                    withCredentials: true //cred
                }
            );
            const token = response?.data?.token;
            const payload = jwt_decode(token);
            const roles = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
            const id = payload["sub"];
            const accessToken = token;
            setAuth({user: email, pwd, roles, accessToken, id});
            cookies.set("accessToken", accessToken, { expires: new Date(payload["exp"] * 1000)});
            setUser('');
            setPwd('');
            navigate(from, {replace: true});

        }catch(err){
            if(!err?.response) {
                setErrMsg('No Server Response')
            } else if(err.response?.status === 400) {
                setErrMsg('Login Failed');
            } else if(err.response?.status === 404){
                setErrMsg('Account does not exist');
            } else if(err.response?.status === 401 ){
                setErrMsg('Incorrect password');
            } else {
                setErrMsg('Login Failed');
            }
            errRef.current.focus();
        }
    }

    return (
        <section class="container-fluid justify-content-center" style={{marginTop:"200px", color: "white"}}>
            <p ref={errRef} className={errMsg ? "errmsg" : 
            "offscreen"} aria-live="assertive">{errMsg}</p>
            <h1>Sign In</h1>
            <form onSubmit={handleSubmit}>
                <label htmlFor="email">Email:</label>
                <input 
                    type = "text"
                    id="email"
                    ref={emailRef}
                    autoComplete="off"
                    onChange={(e) => setUser(e.target.value)}
                    value={email}
                    required
                />

                <label htmlFor="password">Password:</label>
                <input 
                    type = "password"
                    id="password"
                    onChange={(e) => setPwd(e.target.value)}
                    value={pwd}
                    required
                />
                <button>Sign In</button>
            </form>
            <p>
                Need an Account?<br />
                <span className="line">
                    {/*put router link here*/}
                    <a href="/register">Sign Up</a>
                </span>
            </p>
        </section>
    )
}

export default Login