// src/Utils/AuthUtils.js
export const authFetch = async (url, options = {}) => {
  const authOptions = {
    ...options,
    credentials: "include", // For cookies
    headers: {
      ...(options.headers || {}),
      "Content-Type": "application/json",
    },
  };

  try {
    // First attempt
    let response = await fetch(url, authOptions);

    // NOT NEEDED WITH MIDDLEWERE
    // // If unauthorized, try refresh once
    // if (response.status === 401) {
    //   console.log("Unauthorized, attempting token refresh...");

    //   const refreshResponse = await fetch(
    //     "https://localhost:7289/refresh-token",
    //     {
    //       method: "POST",
    //       credentials: "include",
    //     }
    //   );

    //   if (refreshResponse.ok) {
    //     console.log("Token refreshed, retrying original request");
    //     // Retry original request
    //     return fetch(url, authOptions);
    //   } else {
    //     console.log("Refresh failed, returning 401");
    //     // If refresh failed, we're truly unauthorized
    //     return response;
    //   }
    // }

    return response;
  } catch (error) {
    console.error("Auth fetch error:", error);
    throw error;
  }
};
