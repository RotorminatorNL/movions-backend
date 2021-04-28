import React, {Component} from "react";
import "../assets/css/Card.css";
import MovieIcon from "../assets/images/movie_icon.svg";

export default class Movie extends Component {
    openCard(cardElement, cardContent) {
        cardElement
            .classList
            .add("open");
        cardContent
            .classList
            .add("open");
    }

    closeCard(e) {
        if(e.classList.value === "card-element open"){
            e.classList.remove("open");
            e.children[0].classList.remove("open");
        }
    }

    cardController = (e) => {
        console.log(e)

        let clickElement = e.target;

        if(clickElement.closest(".card-element.open") === null){
            let cardElement = clickElement.closest(".card-element");
            let cardContent = cardElement.children[0];
            
            if (!cardElement.classList.contains("open")) {
                this.openCard(cardElement, cardContent);
            }
        } else {
            if(clickElement.classList.value === "card-element open") {
                this.closeCard(clickElement);
            }
        }
    }

    render() {
        return (
            <div className="card-element" id={this.props.id} onMouseUp={this.cardController}>
                <div className="card-content">
                    <h1>{this.props.data.title}</h1>
                    <img src={MovieIcon} alt="movie icon"/>
                    <p>{this.props.data.description}</p>
                </div>
            </div>
        );
    }
}
