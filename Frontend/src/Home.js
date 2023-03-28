import React, {Component} from "react";
import './Home.css';
import logo from './images/logo.png' // relative path to image 

export class Home extends Component{
    render(){
        return(
            <div>
                <table>
                    <tr>
                        <img src={logo} alt="Italian Trulli" />
                    </tr>
                    <tr>
                        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="black" class="bi bi-search" viewBox="0 0 16 16" className="buttons">
                        <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
                        </svg>
                        <input type="text" placeholder="what are you looking for? ..." width="300" className="search-bars"/>
                    </tr>
                </table>
            </div>
        )
    }
}