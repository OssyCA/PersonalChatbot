import React from "react";
import { useNavigate } from "react-router-dom";

const StartScreen = () => {
  const navigate = useNavigate();

  return (
    <div className="start-screen">
      <h1>Welcome to ChatBot Application</h1>
      <div className="buttons">
        <button className="startBtn" onClick={() => navigate("/Login")}>
          Login
        </button>
        <button
          className="startBtn"
          style={{ backgroundColor: "#4a5568" }}
          onClick={() => navigate("/register")}
        >
          Register
        </button>
      </div>
    </div>
  );
};

export default StartScreen;
