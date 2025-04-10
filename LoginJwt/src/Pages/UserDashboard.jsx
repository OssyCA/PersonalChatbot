import React, { useState, useEffect } from "react";
import TokenTestButton from "../Comp/TokenTestButton";
import { useNavigate } from "react-router-dom";
import ChangePasswordForm from "../Comp/ChangePasswordForm";

const UserDashboard = () => {
  // Establish basic state with correct field names
  const [user, setUser] = useState({
    userId: "",
    email: "",
    username: "",
  });
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.clear();
    navigate("/");
  };

  // Use useEffect to load user data from token
  useEffect(() => {
    try {
      // Get token
      const token = localStorage.getItem("accessToken");

      // Check if token exists
      if (!token) {
        console.log("No token found");
        navigate("/login");
        return;
      }

      // Split the token string correctly (JWT consists of 3 parts separated by dots)
      const tokenParts = token.split(".");
      // Check if token has the right format
      if (tokenParts.length !== 3) {
        console.log("Invalid token format");
        return;
      }

      // Decode the payload part (second part) of the token
      const decodedPart = atob(tokenParts[1]);

      const tokenPayload = JSON.parse(decodedPart);

      // Update user state with data from token
      const updatedUser = {
        userId:
          tokenPayload[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
          ] || "",
        username:
          tokenPayload[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
          ] || "",
        email:
          tokenPayload[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
          ] || "",
      };
      localStorage.setItem(
        "username",
        tokenPayload[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        ]
      );
      setUser(updatedUser);
    } catch (error) {
      console.error(error); // Show full error details
    }
  }, []); // once it mounts

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
