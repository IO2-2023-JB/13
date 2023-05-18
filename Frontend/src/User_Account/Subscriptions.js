import {useState, useEffect } from "react";
import AuthContext from "../context/AuthProvider";
import { useContext } from "react";
import axios from '../api/axios';
import '@fortawesome/fontawesome-svg-core/styles.css';
import { useNavigate, useLocation } from 'react-router-dom';

const SUBSCRIPTIONS_URL = '/subscribtions'; // change to p

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
          withCredentials: true 
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
            <h2 class="display-5" style={{marginLeft:"100px", textAlign: "left"}}> Your Subscriptions: </h2>
            <div style={{marginTop:"20px", 
                            color:"white", borderRadius:"15px", paddingBottom:"20px", paddingTop:"20px", backgroundColor:"#333333"}}>
                <div class ="mt-2 row justify-content-center">
                    <div style={{margin:"20px",
                                color:"white", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#222222", width:"300px", height:"300px"}}/>
                    <div style={{margin:"20px",
                                color:"white", borderRadius:"15px", paddingBottom:"20px", paddingTop:"0px", backgroundColor:"#222222", width:"300px", height:"300px"}}/>
                    
                    <ul style={{padding:"0px"}}>
                        {subscriptionsData.map(subscription => (
                            <section class="container-fluid justify-content-center" style={{marginTop:"20px", color:"white", borderRadius:"15px", 
                                    padding:"20px", backgroundColor:"#333333"}} onClick={() => handleSubscriptionClick(subscription.id)}>
                                <img key={subscription.avatarImage} src={subscription.avatarImage+"?time="+new Date()} alt="No avatar image"/>
                                <h2>Nickname:</h2>
                                <h2>{subscription.nickname}</h2>
                            </section>
                        )).reverse()}
                    </ul>
                </div>
            </div>
        </div>
    );
};
export default Subscriptions;