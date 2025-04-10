import React, { useState } from "react";

const TokenTestButton = () => {
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);
  const [status, setStatus] = useState(""); // "success" or "error"

  const testTokenRefresh = async () => {
    setMessage("Testing token refresh...");
    setStatus("");
    setLoading(true);

    try {
      // Call refresh token endpoint directly
      const response = await fetch("https://localhost:7289/refresh-token", {
        method: "POST",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
        },
      });

      console.log("Refresh response status:", response.status);

      if (response.ok) {
        setMessage("✅ Token refresh successful!");
        setStatus("success");
      } else {
        const errorText = await response.text();
        setMessage(
          `❌ Token refresh failed: ${response.status} ${response.statusText}`
        );
        setStatus("error");
        console.error("Refresh error details:", errorText);
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
        onClick={testTokenRefresh}
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
