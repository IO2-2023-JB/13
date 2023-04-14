import React from 'react';
import {useRef, useState, useEffect } from "react"
import axios from './api/axios';
const VIDEO_URL = '/video';

export default class AddVideo extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            selectedFile: null
        };
    }
    onChangeHandler = (event) => {
        this.setState({
            selectedFile: event.target.files[0],
            loaded: 0,
        });
        console.log(event.target.files[0]);
    };

    handleSubmit = (event) => {
        event.preventDefault();
        const formData = new FormData();
        const { selectedFile } = this.state;
        formData.append('title', 'test');
        formData.append('description', 'test');
        formData.append('thumbnail', {  position: 0,
                                        readTimeout: 0,
                                        writeTimeout: 0 });
        formData.append('tags', ["xD"]);
        formData.append('inputFile', selectedFile);
        fetch(VIDEO_URL, {
            method: 'POST',
            body: formData,
        });
    };

    render() {
        return (
            <section class="container-fluid justify-content-center" style={{marginTop:"200px"}}>
                <h1 style={{color: "white"}}>Upload your video</h1>
                <input
                    style ={{marginTop:"100px"}}
                    type="file"
                    accept="video/*"
                    id="video"
                    onChange={this.onChangeHandler}
                />

                <button onClick={this.handleSubmit}>Upload</button>
            </section>
        )
    }
}
