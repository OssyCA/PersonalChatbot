// Utility functions for handling authentication

// Check if the access token is expired
export const isTokenExpired = (token) => {
  if (!token) return true;

  try {
    // Get the expiration time from the token
    const payload = JSON.parse(atob(token.split(".")[1]));
    const expirationTime = payload.exp * 1000; // Convert to milliseconds

    // Check if the token is expired
    return Date.now() >= expirationTime;
  } catch (error) {
    console.error("Error parsing token:", error);
    return true; // If there's an error, consider the token expired
  }
};

// Refresh the access token using the refresh token
export const refreshAccessToken = async () => {
  const refreshToken = localStorage.getItem("refreshToken");
  const userId = localStorage.getItem("userId");

  if (!refreshToken || !userId) {
    // No refresh token or userId, user needs to log in again
    return false;
  }

  try {
    const response = await fetch("https://localhost:7289/refresh-token", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        UserId: userId,
        RefreshToken: refreshToken,
      }),
    });

    if (!response.ok) {
      // Refresh token is invalid or expired
      return false;
    }

    const data = await response.json();

    // Update the tokens in localStorage
    localStorage.setItem("accessToken", data.accessToken);
    localStorage.setItem("refreshToken", data.refreshToken);

    return true;
  } catch (error) {
    console.error("Error refreshing token:", error);
    return false;
  }
};

// Create an authenticated fetch that handles token refresh
export const authFetch = async (url, options = {}) => {
  let accessToken = localStorage.getItem("accessToken");
  let refreshToken = localStorage.getItem("refreshToken");

  // Check if the token is expired
  if (isTokenExpired(accessToken)) {
    // Try to refresh the token
    const refreshed = await refreshAccessToken();

    if (!refreshed) {
      // Redirect to login if refresh failed
      window.location.href = "/";
      localStorage.clear(); // Clear local storage to remove expired tokens
      throw new Error("Session expired. Please log in again.");
    }

    // Get the new access token
    accessToken = localStorage.getItem("accessToken");
    refreshToken = localStorage.getItem("refreshToken");
  }

  // Add the authorization header and refresh token header
  const authOptions = {
    ...options,
    headers: {
      ...options.headers,
      Authorization: `Bearer ${accessToken}`,
      "X-Refresh-Token": refreshToken,
    },
  };

  // Make the request
  let response = await fetch(url, authOptions);

  // Check for new tokens in response headers
  const newAccessToken = response.headers.get("X-Access-Token");
  const newRefreshToken = response.headers.get("X-Refresh-Token");

  // Update tokens if they were refreshed by the middleware
  if (newAccessToken && newRefreshToken) {
    localStorage.setItem("accessToken", newAccessToken);
    localStorage.setItem("refreshToken", newRefreshToken);
  }

  // Handle 401 Unauthorized
  if (response.status === 401) {
    // Try to refresh the token
    const refreshed = await refreshAccessToken();

    if (!refreshed) {
      // Redirect to login if refresh failed
      window.location.href = "/";
      throw new Error("Session expired. Please log in again.");
    }

    // Get the new access token
    accessToken = localStorage.getItem("accessToken");
    refreshToken = localStorage.getItem("refreshToken");

    // Retry the request with the new token
    authOptions.headers.Authorization = `Bearer ${accessToken}`;
    authOptions.headers["X-Refresh-Token"] = refreshToken;
    return fetch(url, authOptions);
  }

  return response;
};

// Helper function to get user info from JWT token
export const getUserInfoFromToken = () => {
  const token = localStorage.getItem("accessToken");
  if (!token) return null;

  try {
    const tokenParts = token.split(".");
    if (tokenParts.length !== 3) return null;

    const decodedPart = atob(tokenParts[1]);
    const tokenPayload = JSON.parse(decodedPart);

    return {
      userId:
        tokenPayload[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        ] || "",
      username:
        tokenPayload[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        ] || "",
      email:
        tokenPayload[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
        ] || "",
    };
  } catch (error) {
    console.error("Error parsing token:", error);
    return null;
  }
};
