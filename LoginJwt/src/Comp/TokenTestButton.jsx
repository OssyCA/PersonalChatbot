import React, { useState } from "react";
import { authFetch } from "../Utils/AuthUtils";

const TokenTestButton = () => {
  const [message, setMessage] = useState("");

  const testAuthUtils = async () => {
    setMessage("Testing token refresh with AuthUtils...");

    try {
      // Get current tokens for comparison
      const oldAccessToken = localStorage.getItem("accessToken");
      const oldRefreshToken = localStorage.getItem("refreshToken");

      if (!oldAccessToken || !oldRefreshToken) {
        setMessage("❌ Missing tokens in localStorage");
        return;
      }

      // Make a request using authFetch which handles token refresh
      const response = await authFetch("https://localhost:7289/api/auth-test", {
        method: "GET",
      });

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
        } else {
          setMessage(
            "✅ Request succeeded but tokens weren't refreshed (not expired yet)"
          );
        }
      } else {
        setMessage(
          `❌ Request failed: ${response.status} ${response.statusText}`
        );
      }
    } catch (error) {
      setMessage(`❌ Error: ${error.message}`);
      console.error("Test error:", error);
    }
  };

  return (
    <div className="token-test-container">
      <button onClick={testAuthUtils} className="token-test-button">
        Test Token Refresh
      </button>

      {message && <div className="message-display">{message}</div>}
    </div>
  );
};

export default TokenTestButton;
