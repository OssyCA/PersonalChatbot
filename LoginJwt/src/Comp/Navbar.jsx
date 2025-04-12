import { NavLink } from "react-router-dom";
import { useState, useEffect } from "react";

const Navbar = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  useEffect(() => {
    // Check if user is logged in
    const username = localStorage.getItem("username");
    setIsLoggedIn(!!username);
  }, []);

  return (
    <header>
      <nav>
        <ul className="nav-list">
          <li className="nav-item">
            <NavLink
              to="/"
              className={({ isActive }) =>
                isActive ? "nav-link active" : "nav-link"
              }
            >
              Home
            </NavLink>
          </li>

          {!isLoggedIn && (
            <>
              <li className="nav-item">
                <NavLink
                  to="/login"
                  className={({ isActive }) =>
                    isActive ? "nav-link active" : "nav-link"
                  }
                >
                  Login
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink
                  to="/register"
                  className={({ isActive }) =>
                    isActive ? "nav-link active" : "nav-link"
                  }
                >
                  Register
                </NavLink>
              </li>
            </>
          )}

          {isLoggedIn && (
            <>
              <li className="nav-item">
                <NavLink
                  to="/chatbot"
                  className={({ isActive }) =>
                    isActive ? "nav-link active" : "nav-link"
                  }
                >
                  ChatBot
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink
                  to="/user-dashboard"
                  className={({ isActive }) =>
                    isActive ? "nav-link active" : "nav-link"
                  }
                >
                  Dashboard
                </NavLink>
              </li>
            </>
          )}
        </ul>
      </nav>
    </header>
  );
};

export default Navbar;
