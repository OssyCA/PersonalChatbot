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

  // Use useEffect to load user data from token
  useEffect(() => {
    try {
      // Get token
      const token = localStorage.getItem("accessToken");

      // 4. Check if token exists
      if (!token) {
        console.log("No token found");
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

      setUser(updatedUser);
    } catch (error) {
      console.error(error); // Show full error details
    }
  }, []); // once it mounts

  return (
    <>
      <div className="dashboard-container">
        <h1>User Dashboard</h1>
        <div className="user-info">
          <h2>User Information</h2>
          {/* Make sure the correct field names are used in JSX */}
          <p className="info-label">
            <span className="info-value">Username: </span> {user.userId}
          </p>
          <p className="info-label">
            <span className="info-value">User Email: </span>
            {user.email}
          </p>
          <p className="info-label">
            <span className="info-value">User Name: </span> {user.username}
          </p>
        </div>
        <ChangePasswordForm />
        <button
          className="dashboard-button"
          onClick={() => navigate("/chatbot")}
        >
          Chat bot
        </button>
        <TokenTestButton />
      </div>
    </>
  );
};

export default UserDashboard;
