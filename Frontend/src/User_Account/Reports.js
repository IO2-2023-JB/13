import {useState, useEffect } from "react";
import AuthContext from "../context/AuthProvider";
import { useContext } from "react";
import axios from '../api/axios';
import '@fortawesome/fontawesome-svg-core/styles.css';
import { useLocation } from 'react-router-dom';

const TICKET_LIST = "/ticket/list";

const Reports = () => {
    const { auth } = useContext(AuthContext);
    const location = useLocation();

    const [ticketsData, setTicketsData] = useState([]);

    useEffect(() => {
      localStorage.setItem("lastVisitedPage", location.pathname);
    });

    useEffect(() => {
        axios.get(TICKET_LIST, {
            headers: { 
              'Content-Type': 'application/json',
              "Authorization" : `Bearer ${auth?.accessToken}`
            },
            withCredentials: true 
          })
          .then(response => {
            setTicketsData(response?.data);
            console.log(response?.data);
          })
          .catch(error => {
            console.log("error: ", error);
        });
    }, [auth?.accessToken, auth?.id])

    return(
        <div style={{marginTop: "200px"}}>
            <h2 class="display-5" style={{textAlign: "center"}}> Your Reports: </h2>
        </div>
    );
};
export default Reports;