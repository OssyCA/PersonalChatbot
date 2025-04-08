import { Outlet, Navigate } from "react-router-dom";

const ProtectedRoute = () => {
  // Check if the user is authenticated
  const isAuthenticated = localStorage.getItem("accessToken") !== null;
  return isAuthenticated ? <Outlet /> : <Navigate to="/Login" />; // Redirect to login if not authenticated
};

export default ProtectedRoute;
