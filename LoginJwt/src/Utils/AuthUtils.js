// src/Utils/AuthUtils.js
// Remove isTokenExpired and refreshAccessToken functions since we won't have direct access to tokens

// Create an authenticated fetch that works with cookies
export const authFetch = async (url, options = {}) => {
  // Make sure credentials are included
  const authOptions = {
    ...options,
    credentials: "include", // This is crucial for cookies
    headers: {
      ...options.headers,
      "Content-Type": "application/json",
    },
  };

  try {
    // Make the request
    let response = await fetch(url, authOptions);

    // Handle 401 Unauthorized
    if (response.status === 401) {
      console.log("Unauthorized, attempting to refresh token...");

      // Try to refresh token automatically
      const refreshResponse = await fetch(
        "https://localhost:7289/refresh-token",
        {
          method: "POST",
          credentials: "include",
        }
      );

      if (!refreshResponse.ok) {
        console.error("Failed to refresh token", await refreshResponse.text());
        // Don't redirect immediately, just return the error response
        return response;
      }

      console.log("Token refreshed successfully, retrying original request");
      // Retry the original request
      return fetch(url, authOptions);
    }

    return response;
  } catch (error) {
    console.error("authFetch error:", error);
    throw error;
  }
};
