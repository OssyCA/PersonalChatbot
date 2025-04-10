import React, { useState } from "react";
import { authFetch } from "../Utils/AuthUtils";

const ChangePasswordForm = () => {
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [message, setMessage] = useState({ text: "", type: "" });
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage({ text: "", type: "" });

    // Validate inputs
    if (!oldPassword || !newPassword) {
      setMessage({ text: "Please fill in all fields", type: "error" });
      setLoading(false);
      return;
    }

    if (newPassword.length < 8) {
      setMessage({
        text: "New password must be at least 8 characters long",
        type: "error",
      });
      setLoading(false);
      return;
    }

    try {
      // Get username from localStorage
      const username = localStorage.getItem("username");

      if (!username) {
        setMessage({ text: "User information not found", type: "error" });
        setLoading(false);
        return;
      }

      // Notice the URL now has the leading slash
      const response = await authFetch(
        "https://localhost:7289/change-password",
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            UserName: username,
            OldPassword: oldPassword,
            NewPassword: newPassword,
          }),
        }
      );

      // Check status first before trying to parse JSON
      if (response.ok) {
        // For 204 No Content responses, there might not be JSON to parse
        setMessage({ text: "Password changed successfully", type: "success" });
        // Clear form fields
        setOldPassword("");
        setNewPassword("");
      } else {
        // Handle different error status codes
        if (response.status === 404) {
          setMessage({
            text: "Password change service not found. Contact administrator.",
            type: "error",
          });
        } else {
          // Try to parse error response as JSON, but handle case where it's not JSON
          try {
            const errorData = await response.json();
            setMessage({
              text:
                errorData ||
                "Failed to change password. Please check your old password.",
              type: "error",
            });
          } catch (jsonError) {
            // If response is not JSON, use status text
            setMessage({
              text: `Failed to change password: ${response.statusText}`,
              type: "error",
            });
          }
        }
      }
    } catch (error) {
      console.error("Error:", error);
      setMessage({
        text: "An error occurred while changing password",
        type: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="changePasswordContainer">
      <h2>Change Password</h2>

      {message.text && (
        <div className={`message ${message.type}`}>{message.text}</div>
      )}

      <form onSubmit={handleSubmit}>
        <input
          type="password"
          placeholder="Current password"
          className="input"
          value={oldPassword}
          onChange={(e) => setOldPassword(e.target.value)}
          disabled={loading}
        />
        <input
          type="password"
          placeholder="New password"
          className="input"
          value={newPassword}
          onChange={(e) => setNewPassword(e.target.value)}
          disabled={loading}
        />
        <button className="changeBtn" type="submit" disabled={loading}>
          {loading ? "Changing..." : "Change Password"}
        </button>
      </form>
    </div>
  );
};

export default ChangePasswordForm;
