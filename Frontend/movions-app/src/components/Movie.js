import React, {Component} from "react";
import "../assets/css/Card.css";
import MovieIcon from "../assets/images/movie_icon.svg";

export default class Movie extends Component {
    render() {
        return (
            <div className="card">
                <h1>{this.props.data.title}</h1>
                <img src={MovieIcon} alt="movie icon"/>
                <p>{this.props.data.description}</p>
            </div>
        );
    }
}
