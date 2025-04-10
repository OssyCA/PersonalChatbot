import React from "react";
import { Navigate, Outlet } from "react-router-dom";

const ProtectedRoute = () => {
  // Simple check for username in localStorage
  const isLoggedIn = !!localStorage.getItem("username");

  // If not logged in, redirect to login
  if (!isLoggedIn) {
    return <Navigate to="/login" />;
  }

  // If logged in, render the protected route
  return <Outlet />;
};

export default ProtectedRoute;
