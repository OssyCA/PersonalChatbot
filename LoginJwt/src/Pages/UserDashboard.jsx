import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import TokenTestButton from "../Comp/TokenTestButton";
import ChangePasswordForm from "../Comp/ChangePasswordForm";
import LogoutButton from "../Comp/LogoutButton";

const UserDashboard = () => {
  // Establish basic state with correct field names
  const [user, setUser] = useState({
    userId: localStorage.getItem("userId") || "",
    username: localStorage.getItem("username") || "",
    email: localStorage.getItem("email") || "",
  });
  const navigate = useNavigate();

  return (
    <div className="dashboard-container">
      <div className="dashboard-header">
        <h1>User Dashboard</h1>
        <LogoutButton />
      </div>

      <div className="user-info">
        <h2>User Information</h2>
        <div className="user-info-content">
          <p className="info-label">
            User ID: <span className="info-value">{user.userId}</span>
          </p>
          <p className="info-label">
            Username: <span className="info-value">{user.username}</span>
          </p>
          <p className="info-label">
            Email: <span className="info-value">{user.email}</span>
          </p>
        </div>
      </div>

      <ChangePasswordForm />

      <div className="dashboard-actions">
        <button
          className="btn btn-primary btn-lg"
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
