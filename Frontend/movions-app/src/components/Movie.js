import React, {Component} from "react";
import "../assets/css/Movie.css";

export default class Movie extends Component {
    render() {
        return (
            <div className="card">
				<h1>{this.props.data.title}</h1>
				<p>{this.props.data.description}</p>
			</div>
        );
    }
}
