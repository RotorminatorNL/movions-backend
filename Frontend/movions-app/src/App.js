import {BrowserRouter as Router, Route, NavLink} from "react-router-dom";
import Home from "./views/Home.js";
import MovieCollection from "./views/MovieCollection";

function App() {
    return (
        <Router>
            <div>
                <h1>Simple but it works.</h1>
                <nav>
                    <ul className="header">
                        <li>
                            <NavLink to="/">Home</NavLink>
                        </li>
                        <li>
                            <NavLink to="/MovieCollection">MovieCollection</NavLink>
                        </li>
                    </ul>
                </nav>
                <div className="content">
                    <Route exact path="/">
                        <Home/>
                    </Route>
                    <Route path="/MovieCollection">
                        <MovieCollection/>
                    </Route>
                </div>
            </div>
        </Router>
    );
}

export default App;
