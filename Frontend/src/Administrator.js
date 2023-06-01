import React, {Component} from "react";
import './Home.css';
import { useState, useEffect } from "react";
import axios from './api/axios';
import AuthContext from "./context/AuthProvider";
import { useContext } from "react";
import { useLocation } from 'react-router-dom';

const TICKET_LIST = "/ticket/list";

const Administrator = () => {

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
        <div class="col-xs-1"style={{marginTop:"200px"}}>
            <div class="row mt-5">
                <h1 class="display-3 mx-5">Reported users</h1>
                <section>
                </section>
            </div>
            <div class="row mt-5">
                <h1 class="display-3 mx-5">Reported videos</h1>
                <section>
                    
                </section>
            </div>
            <div class="row mt-5">
                <h1 class="display-3 mx-5">Requests for support ticket</h1>
                <section>
                </section>
            </div>
            
        </div>
    )
}
export default Administrator;