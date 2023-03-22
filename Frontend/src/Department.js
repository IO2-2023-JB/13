import React, {Component} from "react";
import { variables } from "./Variables";

export class Department extends Component{
    constructor(props) {
        super(props);
        this.state = { forecasts: [], loading: true };
      }
    refreshList(){
        fetch(variables.API_URL + 'WeatherForecast')
        .then(response => response.json())
        .then(data =>{
            this.setState({forecasts: data, loading: false})
        })
    }
    componentDidMount(){
        this.refreshList();
    }
    static renderForecastsTable(forecasts) {
        return (
          <table className='table table-striped' aria-labelledby="tabelLabel">
            <thead>
              <tr>
                <th>Date</th>
                <th>Temp. (C)</th>
                <th>Temp. (F)</th>
                <th>Summary</th>
              </tr>
            </thead>
            <tbody>
              {forecasts.map(forecast =>
                <tr key={forecast.date}>
                  <td>{forecast.date}</td>
                  <td>{forecast.temperatureC}</td>
                  <td>{forecast.temperatureF}</td>
                  <td>{forecast.summary}</td>
                </tr>
              )}
            </tbody>
          </table>
        );
      }
    render(){
        let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Department.renderForecastsTable(this.state.forecasts);
        return(
            <div>
                <h3>This is department page</h3>
                <h1 id="tabelLabel" >Weather forecast</h1>
                <p>This component demonstrates fetching data from the server.</p>
                {contents}
            </div>
        )
    }
}