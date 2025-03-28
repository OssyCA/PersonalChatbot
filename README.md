# JWT Authentication Project

## ⚠️ WORK IN PROGRESS ⚠️

This project is currently under active development and is not yet complete. Many features may be missing, incomplete, or contain bugs.

## Overview

This project demonstrates a full-stack implementation of JWT (JSON Web Token) authentication with a .NET Minimal API backend and a React frontend. It includes token-based authentication with refresh tokens, user registration, and a protected chatbot application.

## Project Structure

The project consists of two main parts:

### Backend (.NET Minimal API)
- Located in the `JwtMinimalAPI` directory
- Uses .NET 9.0
- Implements JWT token authentication with refresh token mechanism
- Includes user registration and login endpoints
- Features a chatbot API endpoint that uses Gemini API
- Includes email notification functionality
- Protected routes using JWT authorization

### Frontend (React)
- Located in the `LoginJwt` directory
- Built with React 19 and Vite
- Includes login, registration, and chatbot pages
- Implements token refresh mechanism
- Protected routes

## Features

- User registration with email notification
- User authentication with JWT tokens
- Token refresh mechanism
- Protected API endpoints
- Simple chatbot functionality
- Input validation

## Current Status

- Basic authentication flow is implemented
- Chatbot functionality is partially implemented
- UI is minimal and requires improvements
- Error handling needs enhancement
- Testing is incomplete

## Todo List

- [ ] Improve error handling throughout the application
- [ ] Add comprehensive form validation in the frontend
- [ ] Enhance the UI/UX design
- [ ] Add user profile management
- [ ] Implement password reset functionality
- [ ] Add comprehensive testing
- [ ] Improve security measures
- [ ] Add admin functionality
- [ ] Enhance chatbot capabilities
- [ ] Add documentation

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Node.js (v18+)
- SQL Server (for the backend database)

### Backend Setup

1. Navigate to the JwtMinimalAPI directory
2. Update the connection string in `appsettings.json` to point to your SQL Server instance
3. Run the migrations to create the database:
   ```
   dotnet ef database update
   ```
4. Start the API:
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

## Notes

This project is intended for learning purposes and may not follow all best practices for a production application. Security measures and error handling are still being developed.

## License

This project is open source and available under the [MIT License](LICENSE).
