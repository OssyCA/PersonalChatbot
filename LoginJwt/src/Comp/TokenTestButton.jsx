// src/Comp/TokenTestButton.jsx
import React, { useState } from "react";
import { authFetch } from "../Utils/AuthUtils";

const TokenTestButton = () => {
  const [messages, setMessages] = useState([]);
  const [loading, setLoading] = useState(false);

  const addMessage = (text, isError = false) => {
    setMessages((prev) => [
      ...prev,
      { text, isError, time: new Date().toLocaleTimeString() },
    ]);
  };

  const clearMessages = () => {
    setMessages([]);
  };

  const testPublicEndpoint = async () => {
    setLoading(true);
    addMessage("Testing public endpoint...");

    try {
      const response = await fetch("https://localhost:7289/api/public-test");
      const data = await response.json();

      if (response.ok) {
        addMessage(`Public endpoint success: ${data.message}`);
      } else {
        addMessage(`Public endpoint failed: ${response.status}`, true);
      }
    } catch (error) {
      addMessage(`Error: ${error.message}`, true);
    } finally {
      setLoading(false);
    }
  };

  const testAuthEndpoint = async () => {
    setLoading(true);
    addMessage("Testing auth endpoint...");

    try {
      const response = await authFetch("https://localhost:7289/api/auth-test");

      if (response.ok) {
        const data = await response.json();
        addMessage(`Auth endpoint success: ${data.message}`);
      } else {
        addMessage(`Auth endpoint failed: ${response.status}`, true);
      }
    } catch (error) {
      addMessage(`Error: ${error.message}`, true);
    } finally {
      setLoading(false);
    }
  };

  const testRefreshToken = async () => {
    setLoading(true);
    addMessage("Testing token refresh...");

    try {
      const response = await fetch("https://localhost:7289/refresh-token", {
        method: "POST",
        credentials: "include",
      });

      if (response.ok) {
        addMessage("Token refresh successful");
      } else {
        addMessage(`Token refresh failed: ${response.status}`, true);
      }
    } catch (error) {
      addMessage(`Error: ${error.message}`, true);
    } finally {
      setLoading(false);
    }
  };

  const checkCookies = () => {
    addMessage("Checking cookies (see console)");
  };

  return (
    <div className="token-test-container">
      <h2>API Tests</h2>

      <div className="test-buttons">
        <button
          onClick={testPublicEndpoint}
          disabled={loading}
          className="token-test-button"
        >
          Test Public API
        </button>
        <button
          onClick={testAuthEndpoint}
          disabled={loading}
          className="token-test-button"
        >
          Test Auth API
        </button>
        <button
          onClick={testRefreshToken}
          disabled={loading}
          className="token-test-button"
        >
          Test Token Refresh
        </button>
        <button
          onClick={checkCookies}
          disabled={loading}
          className="token-test-button"
        >
          Check Cookies
        </button>
        <button onClick={clearMessages} className="token-test-button">
          Clear Log
        </button>
      </div>

      <div className="test-log">
        {messages.map((msg, index) => (
          <div
            key={index}
            className={`log-entry ${msg.isError ? "error" : "success"}`}
          >
            <span className="log-time">[{msg.time}]</span> {msg.text}
          </div>
        ))}
      </div>
    </div>
  );
};

export default TokenTestButton;
