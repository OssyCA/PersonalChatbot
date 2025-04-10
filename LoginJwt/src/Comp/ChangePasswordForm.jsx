import React, { useState } from "react";
import { authFetch } from "../Utils/AuthUtils";

const ChangePasswordForm = () => {
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [message, setMessage] = useState({ text: "", isError: false });
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();

    // Reset message
    setMessage({ text: "", isError: false });

    // Validate passwords
    if (!oldPassword || !newPassword || !confirmPassword) {
      setMessage({ text: "All fields are required", isError: true });
      return;
    }

    if (newPassword !== confirmPassword) {
      setMessage({ text: "New passwords don't match", isError: true });
      return;
    }

    // Set loading state
    setLoading(true);

    try {
      const userId = localStorage.getItem("userId");

      const response = await authFetch(
        "https://localhost:7289/change-password",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            UserId: userId,
            OldPassword: oldPassword,
            NewPassword: newPassword,
          }),
        }
      );

      if (response.ok) {
        setMessage({ text: "Password changed successfully!", isError: false });
        setOldPassword("");
        setNewPassword("");
        setConfirmPassword("");
      } else {
        const errorData = await response.json();
        setMessage({
          text: errorData.message || "Failed to change password",
          isError: true,
        });
      }
    } catch (error) {
      setMessage({
        text: "An error occurred. Please try again.",
        isError: true,
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="changePasswordContainer">
      <h1>Change Password</h1>

      {message.text && (
        <div className={message.isError ? "error-message" : "success-message"}>
          {message.text}
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <input
          type="password"
          placeholder="Current Password"
          className="input"
          value={oldPassword}
          onChange={(e) => setOldPassword(e.target.value)}
          disabled={loading}
        />
        <input
          type="password"
          placeholder="New Password"
          className="input"
          value={newPassword}
          onChange={(e) => setNewPassword(e.target.value)}
          disabled={loading}
        />
        <input
          type="password"
          placeholder="Confirm New Password"
          className="input"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
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
