import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  // Check if user is already logged in
  useEffect(() => {
    // Try to access auth test endpoint to check authentication status
    fetch("https://localhost:7289/api/auth-test", {
      credentials: "include",
    })
      .then((response) => {
        if (response.ok) {
          navigate("/user-dashboard");
        }
      })
      .catch((error) => {
        console.error("Auth check error:", error);
      });
  }, [navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    if (!username.trim() || !password.trim()) {
      setError("Username and password are required");
      setLoading(false);
      return;
    }

    const data = {
      UserName: username,
      Password: password,
    };

    try {
      const response = await fetch("https://localhost:7289/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include", // Important for cookies
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        const errorData = await response.json();
        if (errorData.errors) {
          setError(Object.values(errorData.errors).flat().join(", "));
        } else {
          setError(errorData || "Invalid username or password");
        }
        setLoading(false);
        return;
      }

      const userData = await response.json();

      // We're using cookies now, but we can store non-sensitive user data
      // like name and ID in localStorage for UI purposes
      if (userData.userId) {
        localStorage.setItem("userId", userData.userId);
      }
      if (userData.username) {
        localStorage.setItem("username", userData.username);
      }

      navigate("/user-dashboard");
    } catch (error) {
      console.error("Error:", error);
      setError("An unexpected error occurred. Please try again later.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="form-box">
      <form className="form" onSubmit={handleSubmit}>
        <span className="title">Sign in</span>
        <span className="subtitle">Access your account</span>

        {error && <div className="error-message">{error}</div>}

        <div className="form-container">
          <input
            type="text"
            className="input"
            placeholder="Username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            disabled={loading}
          />
          <input
            type="password"
            className="input"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            disabled={loading}
          />
        </div>
        <button type="submit" disabled={loading}>
          {loading ? "Signing in..." : "Sign in"}
        </button>
      </form>
      <div className="form-section">
        <p>
          <a href="#">Forgot password?</a>
        </p>
        <p>
          <a href="/register">Don't have an account?</a>
        </p>
      </div>
    </div>
  );
};

export default Login;
