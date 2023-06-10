import {useState, useEffect } from "react";
import AuthContext from "../context/AuthProvider";
import { useContext } from "react";
import axios from '../api/axios';
import '@fortawesome/fontawesome-svg-core/styles.css';
import { useNavigate, useLocation, NavLink } from 'react-router-dom';

const SUBSCRIPTIONS_URL = '/subscriptions';

const Subscriptions = () => {
    const { auth } = useContext(AuthContext);
    const navigate = useNavigate();
    const location = useLocation();

    const [subscriptionsData, setSubscriptionsData] = useState([]);

    useEffect(() => {
      localStorage.setItem("lastVisitedPage", location.pathname);
    });

    useEffect(() => {
        axios.get(SUBSCRIPTIONS_URL + "?id=" + auth?.id, {
          headers: { 
            'Content-Type': 'application/json',
            "Authorization" : `Bearer ${auth?.accessToken}`
          },
          withCredentials: false 
        })
        .then(response => {
            setSubscriptionsData(response?.data?.subscriptions);
        })
        .catch(error => {
          console.log("error: ", error);
        });
    }, [auth?.accessToken, auth?.id]);
    
    const handleSubscriptionClick = (id) => {
        navigate(`/creatorprofile/${id}`);
    }

    return(
        <div style={{marginTop: "200px"}}>
            <nav className='navbar fixed-bottom navbar-expand-sm m-0 bg-dark'>
                <ul className='navbar-nav mx-2' style={{ justifyContent: "center" }}>
                    <li className='nav-item m-1' style={{ justifyContent: "center" }}>
                        <NavLink className="btn btn-outline-light" to='/subscriptions'>
                            Creators
                        </NavLink>
                    </li>
                    <li className='nav-item m-1' style={{ justifyContent: "center" }}>
                        <NavLink className="btn btn-outline-light" to='/subscriptionsvideos'>
                            Videos
                        </NavLink>
                    </li>
                </ul>
            </nav>
            <h2 class="display-5" style={{textAlign: "center"}}> Your Subscriptions: </h2>
            <div style={{marginTop:"20px", marginLeft:"200px", marginRight:"200px", marginBottom:"200px", color:"white", borderRadius:"15px", 
                paddingBottom:"20px", paddingTop:"20px", backgroundColor:"#333333"}}>
                <ul style={{padding:"0px", display:"inline"}}>
                    {subscriptionsData.map(subscription => (
                        <li class="mx-5" style={{marginTop:"20px", color:"white", borderRadius:"15px", cursor: "pointer",
                                padding:"20px", backgroundColor:"#222222"}} onClick={() => handleSubscriptionClick(subscription.id)}>
                            <div class="row">
                                <div class="col-sm">
                                    <img key={subscription.avatarImage} width="200px" height="200px" src={subscription.avatarImage+"?time="+new Date()} alt="No avatar image"/>
                                </div>
                                <div class="col-sm">
                                    <h4>Nickname:</h4>
                                    <h1 style={{marginTop:"30px"}}>{subscription.nickname}</h1>
                                </div>
                            </div>
                        </li>
                    )).reverse()}
                </ul>
            </div>
        </div>
    );
};
export default Subscriptions;