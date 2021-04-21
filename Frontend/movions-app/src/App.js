import {BrowserRouter as Router, Route} from "react-router-dom";
import "./assets/css/App.css";
import Navigation from "./components/Navigation";
import Home from "./views/Home";
import MovieCollection from "./views/MovieCollection";

function App() {
    return (
        <Router>
            <div>
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
            </div>
        </Router>
    );
}

export default App;
