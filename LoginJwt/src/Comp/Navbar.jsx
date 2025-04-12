import { NavLink } from "react-router-dom";
import { useState } from "react";

import React from "react";

const Navbar = () => {
  return (
    <header>
      <nav>
        <ul>
          <li>
            <NavLink to="/">Home</NavLink>
          </li>
          <li>
            <NavLink to="/login">Login</NavLink>
          </li>
          <li>
            <NavLink to="/register">Register</NavLink>
          </li>
          <li>
            <NavLink to="/chatbot">ChatBot</NavLink>
          </li>
          <li>
            <NavLink to="/user-dashboard">User Dashboard</NavLink>
          </li>
        </ul>
      </nav>
    </header>
  );
};

export default Navbar;
