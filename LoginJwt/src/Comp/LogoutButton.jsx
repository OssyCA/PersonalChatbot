import React from "react";
import { useNavigate } from "react-router-dom";

const LogoutButton = () => {
  const navigate = useNavigate();
  const handleLogout = async () => {
    try {
      await fetch("https://localhost:7289/logout", {
        method: "POST",
        credentials: "include",
      });
    } catch (error) {
      console.error("Logout error:", error);
    } finally {
      localStorage.clear();
      navigate("/");
    }
  };

  return (
    <button onClick={handleLogout} className="btn btn-sm btn-danger">
      Sign out
    </button>
  );
};

export default LogoutButton;
