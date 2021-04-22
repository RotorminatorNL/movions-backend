import {BrowserRouter as Router, Route} from "react-router-dom";
import Navigation from "./components/Navigation";
import Footer from "./components/Footer";
import Home from "./views/Home";
import MovieCollection from "./views/MovieCollection";
import "./assets/css/App.css";

function App() {
    return (
        <Router>
            <div className="app">
                <div className="background"></div>
                <Navigation />
                <div className="container">
                    <div className="content">
                        <Route exact path="/">
                            <Home/>
                        </Route>
                        <Route path="/MovieCollection">
                            <MovieCollection/>
                        </Route>
                    </div>
                </div>
                <Footer />
            </div>
        </Router>
    );
}

export default App;
