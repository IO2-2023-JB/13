import React, {Component} from "react";
import './Home.css';
import logo from './images/logo.png' // relative path to image 

export class Home extends Component{
    render(){
        return(
            <div>
                <table>
                    <tr>
                        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="black" class="bi bi-search" viewBox="0 0 16 16" className="buttons">
                            <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
                        </svg>

                        <input type="text" placeholder="what are you looking for? ..." width="300" className="search-bars"/>
                    </tr>
                    <tr>
                        <img src={logo} alt="Italian Trulli" width="850" />
                    </tr>
                    <tr>
                        <svg xmlns="http://www.w3.org/2000/svg" width="80" height="80" fill="black" class="bi bi-arrow-down-circle-fill" viewBox="0 0 16 16" className="buttons2">
                            <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM8.5 4.5a.5.5 0 0 0-1 0v5.793L5.354 8.146a.5.5 0 1 0-.708.708l3 3a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V4.5z"/>
                        </svg>
                    </tr>
                    <tr>
                        <div className="box">
                            <div className="box2">
                            <table>
                                <tr>
                                <div className="movie_title">
                                    Test video title for non-existing video
                                </div>
                                </tr>
                                <tr>
                                <div className="movie_thumbnail">

                                </div>
                                </tr>
                            </table>
                            </div>   
                        </div>
                        <div className="box">
                        <div className="box2">
                            <table>
                                <tr>
                                <div className="movie_title">
                                    Test video title for non-existing video
                                </div>
                                </tr>
                                <tr>
                                <div className="movie_thumbnail">

                                </div>
                                </tr>
                            </table>
                            </div> 
                        </div>
                        <div className="box">
                        <div className="box2">
                            <table>
                                <tr>
                                <div className="movie_title">
                                    Test video title for non-existing video
                                </div>
                                </tr>
                                <tr>
                                <div className="movie_thumbnail">

                                </div>
                                </tr>
                            </table>
                            </div> 
                        </div>
                        <div className="box">
                        <div className="box2">
                            <table>
                                <tr>
                                <div className="movie_title">
                                    Test video title for non-existing video
                                </div>
                                </tr>
                                <tr>
                                <div className="movie_thumbnail">

                                </div>
                                </tr>
                            </table>
                            </div> 
                        </div>
                    </tr>
                </table>
            </div>
        )
    }
}