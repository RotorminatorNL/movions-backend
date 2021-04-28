import React, {Component} from "react";
import GetMovies from "../api/endpoints/GetMovies";
import Movie from "../components/Movie";
import "../assets/css/MovieCollection.css";

export default class MovieCollection extends Component {
    constructor(props) {
        super(props);

        this.state = {
            movies: []
        };
    }

    async componentDidMount() {
        this.setState({movies: await GetMovies()});
    }

    render() {
        return (
            <div className="movie-collection">
                {this
                    .state
                    .movies
                    .map(movie => (<Movie key={movie.id} data={movie}/>))}
                {this
                    .state
                    .movies
                    .map(movie => (<Movie key={movie.id} data={movie}/>))}
                {this
                    .state
                    .movies
                    .map(movie => (<Movie key={movie.id} data={movie}/>))}
                {this
                    .state
                    .movies
                    .map(movie => (<Movie key={movie.id} data={movie}/>))}
                {this
                    .state
                    .movies
                    .map(movie => (<Movie key={movie.id} data={movie}/>))}
            </div>
        );
    }
}