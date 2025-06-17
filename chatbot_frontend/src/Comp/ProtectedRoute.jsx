import { useEffect, useState } from "react";
import { Navigate, Outlet } from "react-router-dom";

const ProtectedRoute = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const verifyAuth = async () => {
      try {
        const response = await fetch("https://localhost:7289/api/auth-test", {
          method: "GET",
          credentials: "include", // Include cookies in the request
        });

        if (response.ok) {
          setIsAuthenticated(true);
        } else {
          setIsAuthenticated(false);
        }
      } catch (error) {
        console.error("Error checking authentication:", error);
        setIsAuthenticated(false);
      } finally {
        setIsLoading(false);
      }
    };
    verifyAuth(); // Call the function to verify authentication
  }, []); // runs once on mount

  if (isLoading) {
    return <div>Loading...</div>; // Show a loading state while checking authentication
  }

  // if authenticated, render the child routes
  return isAuthenticated ? (
    <Outlet />
  ) : (
    <Navigate to="/login" replace /> // Redirect to login if not authenticated
  );
};

export default ProtectedRoute;
