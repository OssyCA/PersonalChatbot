// src/Utils/AuthUtils.js
// Remove isTokenExpired and refreshAccessToken functions since we won't have direct access to tokens

// Create an authenticated fetch that works with cookies
export const authFetch = async (url, options = {}) => {
  // Include credentials to allow cookies to be sent
  const authOptions = {
    ...options,
    credentials: "include", // This is crucial for cookies
    headers: {
      ...options.headers,
      "Content-Type": "application/json",
    },
  };

  // Make the request
  let response = await fetch(url, authOptions);

  // Handle 401 Unauthorized
  if (response.status === 401) {
    // Try to refresh token automatically by calling refresh endpoint
    const refreshResponse = await fetch(
      "https://localhost:7289/refresh-token",
      {
        method: "POST",
        credentials: "include",
      }
    );

    if (!refreshResponse.ok) {
      // Redirect to login if refresh failed
      window.location.href = "/login";
      throw new Error("Session expired. Please log in again.");
    }

    // Retry the original request
    return fetch(url, authOptions);
  }

  return response;
};

// Helper function to check if user is authenticated
export const isAuthenticated = async () => {
  try {
    const response = await fetch("https://localhost:7289/api/auth-test", {
      credentials: "include",
    });
    return response.ok;
  } catch (error) {
    console.error("Auth check error:", error);
    return false;
  }
};
