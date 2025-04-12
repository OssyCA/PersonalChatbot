import React, { useState, useEffect } from "react";
import TokenTestButton from "../Comp/TokenTestButton";
import { useNavigate } from "react-router-dom";
import ChangePasswordForm from "../Comp/ChangePasswordForm";

const UserDashboard = () => {
  // Establish basic state with correct field names
  const [user, setUser] = useState({
    userId: localStorage.getItem("userId") || "",
    username: localStorage.getItem("username") || "",
    email: "",
  });
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      await fetch("https://localhost:7289/logout", {
        method: "POST",
        credentials: "include",
      });
    } catch (error) {
      console.error("Logout error:", error);
    } finally {
      // Clear local storage and navigate to login page
      localStorage.clear();
      navigate("/");
    }
  };

  return (
    <div className="dashboard-container">
      <div className="dashboard-header">
        <h1>User Dashboard</h1>
        <button className="logout-button" onClick={handleLogout}>
          Sign Out
        </button>
      </div>

      <div className="user-info panel">
        <h2>User Information</h2>
        <div className="user-info-content">
          <p className="info-label">
            User ID: <span className="info-value">{user.userId}</span>
          </p>
          <p className="info-label">
            Email: <span className="info-value">{user.email}</span>
          </p>
          <p className="info-label">
            Username: <span className="info-value">{user.username}</span>
          </p>
        </div>
      </div>

      <ChangePasswordForm />

      <div className="dashboard-actions">
        <button
          className="dashboard-button"
          onClick={() => navigate("/chatbot")}
        >
          Open Chatbot
        </button>
      </div>

      <TokenTestButton />
    </div>
  );
};

export default UserDashboard;
