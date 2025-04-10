import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState(""); // State to hold error message
  const [loading, setLoading] = useState(false); // State to hold loading status
  const navigate = useNavigate();

  // Check if the user is already logged in and redirect them to the dashboard
  useEffect(() => {
    const token = localStorage.getItem("accessToken");
    if (token) {
      navigate("/user-dashboard"); // Redirect if already logged in
    }
  }, [navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true); // Set loading to true when starting the request
    setError(""); // Reset error message

    // Check if username and password are empty
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
        body: JSON.stringify(data),
      });

      // Handle non-2xx responses
      if (!response.ok) {
        const errorData = await response.json();
        if (errorData.errors) {
          // Handle validation errors
          setError(Object.values(errorData.errors).flat().join(", "));
        } else {
          // Handle other errors
          setError(errorData || "Invalid username or password");
        }
        setLoading(false);
        return;
      }

      const responseData = await response.json();

      // If successful login, save tokens in localStorage and navigate to dashboard
      if (responseData && responseData.accessToken) {
        // Save tokens in localStorage
        localStorage.setItem("accessToken", responseData.accessToken);
        localStorage.setItem("refreshToken", responseData.refreshToken);

        // Store user ID if it's included in the response or decoded from the token
        const payload = JSON.parse(
          atob(responseData.accessToken.split(".")[1])
        );
        if (payload.nameid) {
          localStorage.setItem("userId", payload.nameid);
        }

        navigate("/user-dashboard");
      } else {
        setError("Login failed. Please try again.");
      }
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
