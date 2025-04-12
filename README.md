# JWT Authentication Project
---

## Overview

This project demonstrates a full-stack implementation of JWT (JSON Web Token) authentication with a .NET Minimal API backend and a React frontend. It includes token-based authentication with refresh tokens, user registration, login, and a protected chatbot application.

## Project Structure
---

The project consists of two main parts:

### Backend (.NET Minimal API)

- Located in the `JwtMinimalAPI` directory
- Built with .NET 9.0
- Implements JWT token authentication with refresh token mechanism
- Features automatic token refresh middleware
- Includes user registration and login endpoints
- Provides password change functionality
- Integrates a chatbot API endpoint that connects to the Gemini API
- Implements email notification functionality for user registration
- Protects routes using JWT authorization
- Includes rate limiting for login attempts

### Frontend (React)

- Located in the `LoginJwt` directory
- Built with React 19 and Vite
- Includes login, registration, user dashboard, and chatbot pages
- Implements client-side token refresh mechanism
- Features protected routes using React Router
- Includes password change functionality
- Provides token testing functionality

## Features
---

- **Authentication Flow**:
  - User registration with email notification
  - User authentication with JWT tokens
  - Automatic token refresh mechanism
  - Protected API endpoints and frontend routes
  - Logout functionality that invalidates refresh tokens

- **User Management**:
  - Registration with email and username
  - Password change functionality
  - Display of user information in dashboard

- **Chatbot Integration**:
  - Integration with Google's Gemini API (requires an API key)
  - Protected chat interface
  - Real-time messaging

- **Security Features**:
  - Password hashing
  - Token-based authentication
  - Refresh token rotation
  - Rate limiting on login attempts
  - Global exception handling

## Technology Stack
---

### Backend:

- .NET 9.0
- Entity Framework Core
- SQL Server
- JWT Authentication
- Minimal API architecture
- Rate limiting
- Global exception middleware

### Frontend:

- React 19
- Vite
- React Router v7
- Pure CSS (no external UI libraries)
- Custom authentication utilities

## Getting Started
---

### Prerequisites


- .NET 9.0 SDK
- Node.js (v18+)
- SQL Server (for the backend database)
- A Gemini API key for the chatbot functionality
- SMTP credentials for email notifications (Gmail setup included)

### Backend Setup


1. Navigate to the JwtMinimalAPI directory
2. Create or update the following configuration files:

   #### appsettings.json
   Create this file with the following structure (replace with your own values):
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
   ```

3. Set up User Secrets for the ChatBot API key (the ChatBot will not function without this):
   ```
   dotnet user-secrets set "ChatBotApiKey" "YOUR_GEMINI_API_KEY"
   ```

4. Apply the database migrations to create the database:
   ```
   dotnet ef database update
   ```

5. Start the API:
   ```
   dotnet run
   ```

### Frontend Setup


1. Navigate to the LoginJwt directory
2. Install dependencies:
   ```
   npm install
   ```
3. Start the development server:
   ```
   npm run dev
   ```

## Implementation Details
---

### JWT Authentication Flow


The authentication flow works as follows:

1. **Registration**: User registers with username, email, and password
2. **Login**: User logs in and receives an access token and refresh token
3. **Token Storage**: Tokens are stored in localStorage on the client
4. **API Requests**: Access token is sent with each request to protected endpoints
5. **Token Refresh**: When access token expires, the system automatically refreshes it using the refresh token
6. **Logout**: On logout, the refresh token is invalidated on the server

### Token Refresh Mechanism


- **Server-side middleware** intercepts requests with expiring tokens and refreshes them automatically
- **Client-side utility** (`authFetch`) handles token refresh when making API calls
- New tokens are returned via response headers

### Password Security


- Passwords are hashed using ASP.NET Core's `PasswordHasher`
- Password validation ensures strong passwords with special characters, numbers, uppercase letters, etc.

## Important Notes
---

1. **API Keys Required**: The chatbot functionality will not work without a valid Google Gemini API key. You can obtain one from the [Google AI Studio](https://makersuite.google.com/app/apikey).

2. **Security Considerations**:
   - Never commit real API keys, database connection strings, JWT secrets, or email credentials to your repository
   - The project uses .NET User Secrets for development and should use environment variables or a secure configuration system in production
   - Gmail SMTP requires an "App Password" if you have 2FA enabled - never use your regular Gmail password
   - In a production environment, use more robust secret management and consider HTTPS for all communications

## Future Enhancements
---

- Add user role management
- Implement password reset functionality
- Add social login options
- Enhance chatbot capabilities
- Add admin dashboard
- Implement user profile management
- Add comprehensive logging and monitoring
- Implement unit and integration tests
