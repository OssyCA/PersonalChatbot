JWT Authentication Project
⚠️ Learning Project ⚠️

This project was created as a learning exercise to explore different approaches to JWT authentication in a full-stack application. It is not intended for production use without additional security considerations and improvements. The main goal was to understand the mechanics of JWT authentication, refresh tokens, and different storage strategies.
Overview
This project demonstrates a full-stack implementation of JWT (JSON Web Token) authentication with a .NET Minimal API backend and a React frontend. It includes token-based authentication with refresh tokens, user registration, login, and a protected chatbot application.
Branch-Specific Implementations
This repository contains multiple branches that showcase different approaches to JWT token storage:

Main Branch: Uses localStorage for token storage (simple but less secure)
Cookie-Branch: Implements HTTP-only cookies for token storage (more secure against XSS attacks)

Both implementations have their pros and cons:
LocalStorage Approach (Main Branch)

Pros: Simple to implement, works across all browsers, easy to access tokens on client
Cons: Vulnerable to XSS attacks as JavaScript can access the tokens

HttpOnly Cookies Approach (Cookie-Branch)

Pros: More secure against XSS attacks as JavaScript cannot access HttpOnly cookies
Cons: More complex to implement, potential CSRF vulnerabilities (requires additional mitigation)

Project Structure
The project consists of two main parts:
Backend (.NET Minimal API)

Located in the JwtMinimalAPI directory
Built with .NET 9.0
Implements JWT token authentication with refresh token mechanism
Features automatic token refresh middleware
Includes user registration and login endpoints
Provides password change functionality
Integrates a chatbot API endpoint that connects to the Gemini API
Implements email notification functionality for user registration
Protects routes using JWT authorization
Includes rate limiting for login attempts

Frontend (React)

Located in the LoginJwt directory
Built with React 19 and Vite
Includes login, registration, user dashboard, and chatbot pages
Implements client-side token refresh mechanism
Features protected routes using React Router
Includes password change functionality
Provides token testing functionality

Features

Authentication Flow:

User registration with email notification
User authentication with JWT tokens
Automatic token refresh mechanism
Protected API endpoints and frontend routes
Logout functionality that invalidates refresh tokens


User Management:

Registration with email and username
Password change functionality
Display of user information in dashboard


Chatbot Integration:

Integration with Google's Gemini API (requires an API key)
Protected chat interface
Real-time messaging


Security Features:

Password hashing
Token-based authentication
Refresh token rotation
Rate limiting on login attempts
Global exception handling



Technology Stack
Backend:

.NET 9.0
Entity Framework Core
SQL Server
JWT Authentication
Minimal API architecture
Rate limiting
Global exception middleware

Frontend:

React 19
Vite
React Router v7
Pure CSS (no external UI libraries)
Custom authentication utilities

Getting Started
Prerequisites

.NET 9.0 SDK
Node.js (v18+)
SQL Server (for the backend database)
A Gemini API key for the chatbot functionality
SMTP credentials for email notifications (Gmail setup included)

Backend Setup

Navigate to the JwtMinimalAPI directory
Create or update the following configuration files:
appsettings.json
Create this file with the following structure (replace with your own values):
json{
  "ChatBotApiKey": "GEMINI KEY",
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
    "Token": "YOUR_JWT_SECRET_KEY_SHOULD_BE_AT_LEAST_64_CHARACTERS_LONG_FOR_HMACSHA512",
    "Issuer": "YourApiJwt",
    "Audience": "YourApiJwtClient"
  },
  "GmailOptions": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Email": "your-email@example.com",
    "Password": "your-app-password"
  }
}

Set up User Secrets for the ChatBot API key (the ChatBot will not function without this):
dotnet user-secrets set "ChatBotApiKey" "YOUR_GEMINI_API_KEY"

Apply the database migrations to create the database:
dotnet ef database update

Start the API:
dotnet run


Frontend Setup

Navigate to the LoginJwt directory
Install dependencies:
npm install

Start the development server:
npm run dev


Implementation Details
JWT Authentication Flow
The authentication flow works as follows:

Registration: User registers with username, email, and password
Login: User logs in and receives an access token and refresh token
Token Storage:

Main branch: Tokens are stored in localStorage
Cookie branch: Tokens are stored in HTTP-only cookies


API Requests: Access token is sent with each request to protected endpoints

Main branch: Token sent via Authorization header
Cookie branch: Token sent automatically via cookies


Token Refresh: When access token expires, the system automatically refreshes it using the refresh token
Logout: On logout, the refresh token is invalidated on the server

Token Refresh Mechanism

Server-side middleware intercepts requests with expiring tokens and refreshes them automatically
Client-side utility (main branch: authFetch) handles token refresh when making API calls
Token refresh strategies differ between branches:

Main branch: Manual refresh using refresh token endpoint
Cookie branch: Automatic refresh via middleware



Password Security

Passwords are hashed using ASP.NET Core's PasswordHasher
Password validation ensures strong passwords with special characters, numbers, uppercase letters, etc.

Learning Outcomes
This project was created to learn and understand:

JWT authentication flow and implementation details
The pros and cons of different token storage strategies
Handling refresh tokens securely
Building a secure authentication system with .NET Minimal API
Creating protected routes in React
Implementing middleware for token refresh
Rate limiting to prevent brute force attacks
Best practices for password handling and validation

Important Notes

API Keys Required: The chatbot functionality will not work without a valid Google Gemini API key. You can obtain one from the Google AI Studio.
Security Considerations:

Never commit real API keys, database connection strings, JWT secrets, or email credentials to your repository
The project uses .NET User Secrets for development and should use environment variables or a secure configuration system in production
Gmail SMTP requires an "App Password" if you have 2FA enabled - never use your regular Gmail password
In a production environment, use more robust secret management and consider HTTPS for all communications



Future Enhancements

Add user role management
Implement password reset functionality
Add social login options
Enhance chatbot capabilities
Add admin dashboard
Implement user profile management
Add comprehensive logging and monitoring
Implement unit and integration tests
Explore other token storage mechanisms (e.g., Web Storage API, IndexedDB)
Implement CSRF protection for cookie-based authentication
