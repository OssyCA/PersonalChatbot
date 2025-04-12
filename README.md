# JWT Authentication Project

## ⚠️ WORK IN PROGRESS ⚠️

This project is currently under active development and is not yet complete. Many features may be missing, incomplete, or contain bugs.

## Overview

This project demonstrates a full-stack implementation of JWT (JSON Web Token) authentication with a .NET Minimal API backend and a React frontend. It includes token-based authentication with refresh tokens, user registration, and a protected chatbot application.

Overview
This project demonstrates a full-stack implementation of JWT (JSON Web Token) authentication with a .NET Minimal API backend and React frontend. It features a secure cookie-based token storage approach, eliminating the security risks associated with storing tokens in localStorage.
Project Structure
The project consists of two main parts:
Backend (.NET Minimal API)

Located in the JwtMinimalAPI directory
Built with .NET 9.0
Implements JWT token authentication with refresh token mechanism
Uses HttpOnly cookies for secure token storage
Includes token refresh middleware that automatically refreshes tokens about to expire
Features comprehensive error handling middleware
Provides user registration and login endpoints
Includes password change functionality
Features a chatbot API endpoint using the Gemini API
Implements email notification functionality on user registration
Protects routes using JWT authorization
Implements admin-specific routes with role-based authorization

Frontend (React)

Located in the LoginJwt directory
Built with React 19 and Vite
Features login, registration, and chatbot interfaces
Implements cookie-based authentication
Includes automatic token refresh mechanism
Uses protected routes for secure content
Provides password change functionality
Includes test functionality to verify API connectivity

Key Features

Secure Authentication: Uses HttpOnly cookies for token storage, preventing XSS attacks
Refresh Token: Implements automatic refresh token mechanism to maintain sessions
Token Refresh Middleware: Server-side middleware that proactively refreshes tokens near expiration
Global Error Handling: Comprehensive error handling throughout the application
Chatbot Integration: Simple AI chatbot using the Gemini API
Email Notifications: Sends welcome emails to newly registered users
Password Management: Allows users to change their passwords securely
Form Validation: Client and server-side validation for user inputs
Role-Based Authorization: Different access levels for regular users and administrators

Security Considerations

HttpOnly Cookies: Prevents client-side JavaScript from accessing tokens
Secure Flag: Ensures cookies are only sent over HTTPS
SameSite Policy: Configured to prevent CSRF attacks
Token Validation: Comprehensive token validation on the server
Password Hashing: Secure password storage using ASP.NET Identity's password hasher
Password Validation: Enforces strong password requirements
Rate Limiting: Prevents brute force attacks on login endpoints

Getting Started
Prerequisites

.NET 9.0 SDK
Visual Studio 2022 (for the backend API development)
Visual Studio Code (for the React frontend development)
Node.js (v18+)
SQL Server (for the backend database)
A Gemini API key for the chatbot functionality

Backend Setup

Open the JwtMinimalAPI project in Visual Studio 2022
Create or update the following configuration files:
appsettings.json
Create this file with the following structure (replace with your own values):
json{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "UserDatabase": "Data Source=YOUR_SERVER;Database=YOUR_DATABASE;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"
  },
  "Appsettings": {
    "Token": "YOUR_JWT_SECRET_KEY_GOES_HERE",
    "Issuer": "YourApiJwt",
    "Audience": "YourApiJwtClient"
  },
  "GmailOptions": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Email": "your-email@example.com",
    "Password": "your-app-password"
  },
  "ChatBotApiKey": "YOUR_GEMINI_API_KEY"
}

Alternatively, you can set up User Secrets for the ChatBot API key:
dotnet user-secrets set "ChatBotApiKey" "YOUR_GEMINI_API_KEY"

Run the migrations to create the database:
dotnet ef database update

Start the API:
dotnet run


Frontend Setup

Open the LoginJwt directory in Visual Studio Code
Install dependencies:
npm install

Start the development server:
npm run dev


Technical Details
Authentication Flow

Registration: User registers with username, email, and password
Email Verification: Welcome email is sent to the user
Login: User logs in with username and password
Token Generation: Server generates JWT token and refresh token
Cookie Storage: Tokens are stored in HttpOnly cookies
Automatic Refresh: Tokens are refreshed automatically when near expiration
Logout: Cookies are cleared and server invalidates the refresh token

API Endpoints

/register: Create a new user account
/login: Authenticate user and issue tokens
/logout: Invalidate tokens and clear cookies
/refresh-token: Get new tokens using a refresh token
/change-password: Update user password
/InputMessage/{inputMessage}: Send message to chatbot (protected)
/api/GetUsers: Get list of users (admin only)
/api/auth-test: Test endpoint for authentication
/api/public-test: Public test endpoint

Security Notes

Never commit real API keys, database connection strings, JWT secrets, or email credentials to your repository
The project uses .NET User Secrets for development and should use environment variables or a secure configuration system in production
Gmail SMTP requires an "App Password" if you have 2FA enabled - never use your regular Gmail password
In a production environment, you would want to use more robust secret management

Future Enhancements

Add comprehensive unit and integration tests
Implement email verification for new accounts
Add password reset functionality
Enhance the UI/UX design
Implement more admin functionality
Add user profile management
Expand chatbot capabilities
Add logging and monitoring
