import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";

export default function ResetPasswordForm() {
  const [email, setEmail] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [token, setToken] = useState(null);
  const [expiry, setExpiry] = useState(null);
  const [message, setMessage] = useState({ text: "", isError: false });
  const [loading, setLoading] = useState(false);
  const [isResetMode, setIsResetMode] = useState(false);

  const navigate = useNavigate();
  const location = useLocation();

  // Check URL for token parameters
  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const tokenParam = params.get("token");
    const emailParam = params.get("email");
    const expiryParam = params.get("expiry");

    if (tokenParam && emailParam && expiryParam) {
      setToken(tokenParam);
      setEmail(emailParam);
      setExpiry(expiryParam);
      setIsResetMode(true);
    }
  }, [location]);

  const handleRequestReset = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage({ text: "", isError: false });

    try {
      const response = await fetch(
        "https://localhost:7289/api/forgot-password",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ Email: email }),
        }
      );

      if (response.ok) {
        setMessage({
          text: "If your email is registered, you will receive a password reset link shortly.",
          isError: false,
        });
        setEmail("");
      } else {
        setMessage({
          text: "An error occurred. Please try again later.",
          isError: true,
        });
      }
    } catch (error) {
      setMessage({
        text: "Could not connect to server. Please try again later.",
        isError: true,
      });
    } finally {
      setLoading(false);
    }
  };

  const handleResetPassword = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage({ text: "", isError: false });

    // Validate passwords
    if (newPassword !== confirmPassword) {
      setMessage({ text: "Passwords don't match", isError: true });
      setLoading(false);
      return;
    }

    try {
      const response = await fetch(
        "https://localhost:7289/api/reset-password",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            Email: email,
            ForgotPasswordToken: token,
            ExpireTime: new Date(Number(expiry)),
            NewPassword: newPassword,
          }),
        }
      );

      if (response.ok) {
        setMessage({
          text: "Password reset successfully! You can now log in with your new password.",
          isError: false,
        });
        setTimeout(() => {
          navigate("/login");
        }, 3000);
      } else {
        const errorData = await response.json();
        setMessage({
          text:
            errorData.message ||
            "Failed to reset password. The link may be expired or invalid.",
          isError: true,
        });
      }
    } catch (error) {
      setMessage({
        text: "An error occurred. Please try again later.",
        isError: true,
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="reset-password-form-container">
      {message.text && (
        <div className={message.isError ? "error-message" : "success-message"}>
          {message.text}
        </div>
      )}

      {isResetMode ? (
        // Password reset form with token
        <form className="reset-password-form" onSubmit={handleResetPassword}>
          <h2>Create New Password</h2>
          <div className="form-group">
            <input
              type="password"
              className="form-input"
              placeholder="New Password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              required
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
              required
              disabled={loading}
            />
          </div>
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? "Resetting..." : "Reset Password"}
          </button>
        </form>
      ) : (
        // Request reset link form
        <form className="reset-password-form" onSubmit={handleRequestReset}>
          <h2>Reset Password</h2>
          <div className="form-group">
            <input
              type="email"
              className="form-input"
              placeholder="Enter your email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              disabled={loading}
            />
          </div>
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? "Sending..." : "Send Reset Link"}
          </button>
        </form>
      )}
    </div>
  );
}
