import { useRef, useState, useEffect, useContext } from 'react';
import AuthContext from "../context/AuthProvider"

import axios from '../api/axios';

const LOGIN_URL = '/login';

const Login = () => {
    const { setAuth } = useContext(AuthContext);
    const emailRef = useRef();
    const errRef = useRef();

    const [email, setUser] = useState('');
    const [pwd, setPwd] = useState('');
    const [errMsg, setErrMsg] = useState('');
    const [success, setSuccess] = useState(false);

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
            console.log(JSON.stringify(response?.data));
            const accessToken = response?.data?.accessToken;
            const roles = response?.data?.roles;
            setAuth({email: email, pwd, roles, accessToken});
            setUser('');
            setPwd('');
            setSuccess(true);

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
        <>
        {success ? (
            <section>
                <h1>You are logged in!</h1>
                <br />
                <p>
                    <a href="/home">Go to Home</a>
                </p>
            </section>
        ) : (
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
        )}
        </>
    )
}

export default Login