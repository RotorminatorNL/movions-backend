import React, {Component} from "react";
import {NavLink} from "react-router-dom";
import "../assets/css/Navigation.css";

export default class Navigation extends Component {
    navBarController() {
        let navbarBurger = document.getElementById("navbar-burger");
        let navbarMenu = document.getElementById("navbar-menu");

        if(navbarBurger.classList.contains("is-active") || navbarMenu.classList.contains("is-active")){
            navbarBurger.classList.remove("is-active");
            navbarMenu.classList.remove("is-active");
        }else{
            navbarBurger.classList.add("is-active");
            navbarMenu.classList.add("is-active");
        }
    }

    render() {
        return (
            <div className="container">
                <nav role="navigation" aria-label="main navigation" className="navbar">
                    <div className="navbar-brand">
                        <button
                            id="navbar-burger"
                            aria-label="menu"
                            className="navbar-burger burger"
                            onClick={this.navBarController}>
                            <span aria-hidden="true"></span>
                            <span aria-hidden="true"></span>
                            <span aria-hidden="true"></span>
                        </button>
                    </div>
                    <div id="navbar-menu" className="navbar-menu">
                        <div className="navbar-options">
                            <NavLink to="/">Home</NavLink>
                            <NavLink to="/MovieCollection">MovieCollection</NavLink>
                        </div>
                    </div>
                </nav>
            </div>
        )
    }
}