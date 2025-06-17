import React from "react";
import { useNavigate } from "react-router-dom";

const StartScreen = () => {
  const navigate = useNavigate();

  return (
    <div className="start-screen">
      <h1>Welcome to ChatBot Application</h1>
      <p>Your intelligent AI assistant for seamless conversations</p>

      <div className="btn-group">
        <button
          className="btn btn-primary btn-lg"
          onClick={() => navigate("/login")}
        >
          Sign In
        </button>
        <button
          className="btn btn-outline btn-lg"
          onClick={() => navigate("/register")}
        >
          Create Account
        </button>
      </div>
    </div>
  );
};

export default StartScreen;
