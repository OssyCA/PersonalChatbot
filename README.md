# JWT Authentication With Cookie-Based Implementation

## ⚠️ WORK IN PROGRESS ⚠️

This project is currently under active development and is not yet complete. Many features may be missing, incomplete, or contain bugs.

## Overview
---

This project demonstrates a full-stack implementation of JWT (JSON Web Token) authentication with a .NET Minimal API backend and React frontend. It features a secure cookie-based token storage approach, eliminating the security risks associated with storing tokens in localStorage.

## Project Structure
---

### Backend (.NET Minimal API)
- Located in the JwtMinimalAPI directory
- Built with .NET 9.0
- Implements JWT token authentication with refresh token mechanism
- Uses HttpOnly cookies for secure token storage
- Includes token refresh middleware that automatically refreshes tokens
- Features comprehensive error handling middleware
- Provides user registration and login endpoints
- Includes password change functionality
- Features a chatbot API endpoint using the Gemini API
- Implements email notification functionality
- Protects routes using JWT authorization

### Frontend (React)
- Located in the LoginJwt directory
- Built with React 19 and Vite
- Features login, registration, and chatbot interfaces
- Implements cookie-based authentication
- Includes automatic token refresh mechanism
- Uses protected routes for secure content
- Provides password change functionality
- Includes test functionality to verify API connectivity

## Key Features
---

- Secure Authentication: Uses HttpOnly cookies for token storage
- Refresh Token: Automatic refresh token mechanism
- Token Refresh Middleware: Proactively refreshes tokens
- Global Error Handling: Comprehensive error handling
- Chatbot Integration: Simple AI chatbot using Gemini API
- Email Notifications: Sends welcome emails to new users
- Password Management: Secure password changing
- Form Validation: Client and server-side validation
- Role-Based Authorization: Different access levels

## Getting Started
---

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 (for the backend API development)
- Visual Studio Code (for the React frontend development)
- Node.js (v18+)
- SQL Server (for the backend database)
- A Gemini API key for the chatbot functionality

### Backend Setup

1. Open the JwtMinimalAPI project in Visual Studio 2022
2. Create or update appsettings.json with the following structure:
```json
{
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
```

3. Alternatively, set up User Secrets for the ChatBot API key:
```
dotnet user-secrets set "ChatBotApiKey" "YOUR_GEMINI_API_KEY"
```

4. Run the migrations to create the database:
```
dotnet ef database update
```

5. Start the API using Visual Studio

### Frontend Setup

1. Open the LoginJwt directory in Visual Studio Code
2. Install dependencies:
```
npm install
```
3. Start the development server:
```
npm run dev
```

## API Endpoints
---

- /register: Create a new user account
- /login: Authenticate user and issue tokens
- /logout: Invalidate tokens and clear cookies
- /refresh-token: Get new tokens using a refresh token
- /change-password: Update user password
- /InputMessage/{inputMessage}: Send message to chatbot (protected)
- /api/GetUsers: Get list of users (admin only)
- /api/auth-test: Test authentication endpoint
- /api/public-test: Public test endpoint

## Security Notes
---

- Never commit real API keys, database connection strings, JWT secrets, or email credentials
- Use .NET User Secrets for development
- Gmail SMTP requires an "App Password" if you have 2FA enabled
- Use environment variables or secure configuration in production
