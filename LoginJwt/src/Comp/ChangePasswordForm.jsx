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
    <div className="change-password-container">
      <h2>Change Password</h2>

      {message.text && (
        <div className={message.isError ? "error-message" : "success-message"}>
          {message.text}
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <input
            type="password"
            className="form-input"
            placeholder="Current Password"
            value={oldPassword}
            onChange={(e) => setOldPassword(e.target.value)}
            disabled={loading}
          />
        </div>

        <div className="form-group">
          <input
            type="password"
            className="form-input"
            placeholder="New Password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            disabled={loading}
          />
        </div>

        <div className="form-group">
          <input
            type="password"
            className="form-input"
            placeholder="Confirm New Password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            disabled={loading}
          />
        </div>

        <button className="btn btn-primary" type="submit" disabled={loading}>
          {loading ? (
            <>
              <span className="spinner"></span>
              Changing...
            </>
          ) : (
            "Change Password"
          )}
        </button>
      </form>
    </div>
  );
};

export default ChangePasswordForm;
