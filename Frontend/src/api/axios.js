import axios from "axios";

export default axios.create({
    //baseURL: 'http://localhost:7042'
    //baseURL: 'https://localhost:7042/zagorskim/VideIO/1.0.0'
    baseURL: 'https://io2test.azurewebsites.net/api/'
});