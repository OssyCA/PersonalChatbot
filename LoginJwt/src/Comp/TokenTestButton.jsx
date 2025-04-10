import React, { useState } from "react";
import { authFetch } from "../Utils/AuthUtils";

const TokenTestButton = () => {
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);
  const [status, setStatus] = useState(""); // "success" or "error"

  const testAuthUtils = async () => {
    setMessage("Testing token refresh with AuthUtils...");
    setStatus("");
    setLoading(true);

    try {
      // Get current tokens for comparison
      const oldAccessToken = localStorage.getItem("accessToken");
      const oldRefreshToken = localStorage.getItem("refreshToken");

      if (!oldAccessToken || !oldRefreshToken) {
        setMessage("❌ Missing tokens in localStorage");
        setStatus("error");
        setLoading(false);
        return;
      }

      // Get new tokens after request
      const newAccessToken = localStorage.getItem("accessToken");
      const newRefreshToken = localStorage.getItem("refreshToken");

      if (response.ok) {
        // Check if tokens were refreshed
        if (
          oldAccessToken !== newAccessToken ||
          oldRefreshToken !== newRefreshToken
        ) {
          setMessage("✅ Request succeeded and tokens were refreshed!");
          setStatus("success");
        } else {
          setMessage(
            "✅ Request succeeded but tokens weren't refreshed (not expired yet)"
          );
          setStatus("success");
        }
      } else {
        setMessage(
          `❌ Request failed: ${response.status} ${response.statusText}`
        );
        setStatus("error");
      }
    } catch (error) {
      setMessage(`❌ Error: ${error.message}`);
      setStatus("error");
      console.error("Test error:", error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="token-test-container">
      <button
        onClick={testAuthUtils}
        className="token-test-button"
        disabled={loading}
      >
        {loading ? "Testing..." : "Test Token Refresh"}
      </button>

      {message && (
        <div className={`message-display ${status ? status : ""}`}>
          {message}
        </div>
      )}
    </div>
  );
};

export default TokenTestButton;
