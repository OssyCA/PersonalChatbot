import React from "react";
import { useNavigate } from "react-router-dom";

//WHAT TO DO:
// Add login or register button
// Add styling

const StartScreen = () => {
  const navigate = useNavigate();

  return (
    <div>
      <button className="startBtn" onClick={() => navigate("/Login")}>
        LOG IN
      </button>
    </div>
  );
};

export default StartScreen;
