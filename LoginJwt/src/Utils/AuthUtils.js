// src/Utils/AuthUtils.js
export const authFetch = async (url, options = {}) => {
  // Make sure credentials are included
  const authOptions = {
    ...options,
    credentials: "include", // This is crucial for cookies
    headers: {
      ...(options.headers || {}),
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
          headers: {
            "Content-Type": "application/json",
          },
        }
      );

      if (!refreshResponse.ok) {
        console.error("Failed to refresh token", await refreshResponse.text());
        // Return the original error response
        return response;
      }

      console.log("Token refreshed successfully, retrying original request");
      // Retry the original request with the same options
      return fetch(url, authOptions);
    }

    return response;
  } catch (error) {
    console.error("authFetch error:", error);
    throw error;
  }
};
