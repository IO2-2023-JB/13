import React, {Component} from "react";
import './Home.css';
import logo from './images/logo.png' // relative path to image 

export class Administrator extends Component{
    render(){
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
}