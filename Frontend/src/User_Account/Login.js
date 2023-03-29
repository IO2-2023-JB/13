import { useRef, useState, useEffect, useContext } from 'react';
import AuthContext from "../context/AuthProvider"
import jwt_decode  from 'jwt-decode';
import axios from '../api/axios';
import useAuth from '../hooks/useAuth';
import {Link, useNavigate, useLocation} from 'react-router-dom'

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
            //console.log(JSON.stringify(response?.data));
            const token = response?.data;
            const textEncoder = new TextEncoder();
            //const secretArray = textEncoder.encode('TajnyKlucz128bit');
            const payload = jwt_decode(token);
            const roles = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
            console.log(roles);
            const accessToken = token;
            setAuth({user: email, pwd, roles, accessToken});
            setUser('');
            setPwd('');
            navigate(from, {replace: true});

        }catch(err){
            if(!err?.response) {
                setErrMsg('Account with this email does not exist')
                //setErrMsg('No Server Response');
            } else if(err.response?.status === 400) {
                setErrMsg('Wrong email or password');
            } else {
                setErrMsg('Login Failed');
            }
            errRef.current.focus();
        }
    }

    return (
        <section>
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