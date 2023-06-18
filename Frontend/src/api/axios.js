import axios from "axios";

export default axios.create({
    //baseURL: 'http://localhost:7042'
    baseURL: 'https://io2.azurewebsites.net/api/'
});