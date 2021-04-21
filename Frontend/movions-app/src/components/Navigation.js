import React, {Component} from "react";
import {NavLink} from "react-router-dom";
import "../assets/css/Navigation.css";
import "../assets/css/NavBarBurger.css";
import "../assets/css/NavBarOptions.css";
import Logo from "../assets/images/logo_movions.svg";

export default class Navigation extends Component {
    navBarOpen() {
        document
            .getElementById("navbar-burger")
            .classList
            .add("is-active");
        document
            .getElementById("navbar-menu")
            .classList
            .add("is-active");
    }

    navBarClose() {
        document
            .getElementById("navbar-burger")
            .classList
            .remove("is-active");
        document
            .getElementById("navbar-menu")
            .classList
            .remove("is-active");
    }

    navBarController = () => {
        let navbarBurger = document.getElementById("navbar-burger");
        let navbarMenu = document.getElementById("navbar-menu");

        if (navbarBurger.classList.contains("is-active") || navbarMenu.classList.contains("is-active")) {
            this.navBarClose();
        } else {
            this.navBarOpen();
        }
    }

    render() {
        return (
            <nav role="navigation" aria-label="main navigation" className="navbar">
                <div className="background"></div>
                <div className="container">
                    <div className="navbar-brand">
                        <NavLink to="/"><img src={Logo} alt="Logo"/>Movions</NavLink>
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
                        <div className="navbar-options" onClick={this.navBarClose}>
                            <NavLink exact to="/" activeClassName="selected">Home</NavLink>
                            <NavLink to="/MovieCollection" activeClassName="selected">MovieCollection</NavLink>
                        </div>
                    </div>
                </div>
            </nav>
        )
    }
}