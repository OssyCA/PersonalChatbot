
# 🔐 JWT Authentication Project

⚠️ **Learning Project – Not for Production Use** ⚠️  
This project was created as a learning exercise to explore different approaches to JWT authentication in a full-stack environment. It is **not intended for production** without further security considerations and improvements.

---

## 📌 Overview

This full-stack project demonstrates **JWT (JSON Web Token) authentication** using:

- **.NET Minimal API (Backend)**
- **React with Vite (Frontend)**

It covers:
- User registration, login, and logout
- Token-based authentication with refresh tokens
- Protected chatbot interface using Google Gemini API
- Rate limiting and password hashing
- Two different JWT storage strategies across branches

---

## 🌱 Branch-Specific Implementations

| Branch        | Token Storage | Description |
|---------------|----------------|-------------|
| `main`        | `localStorage` | Simple to implement, vulnerable to XSS |
| `Cookie-Branch` | `HttpOnly Cookies` | More secure against XSS, but requires CSRF protection |

### 📊 Pros & Cons

**LocalStorage (Main Branch)**
- ✅ Simple to implement
- ✅ Works across all browsers
- ❌ Vulnerable to XSS

**HttpOnly Cookies (Cookie-Branch)**
- ✅ Secure against XSS
- ❌ Requires CSRF protection
- ❌ Slightly more complex to implement

---

## 🗂️ Project Structure

### Backend (.NET 9.0 – `JwtMinimalAPI`)
- JWT token + refresh token handling
- User registration, login, password change
- Gemini API integration for chatbot
- Email notifications (SMTP support)
- Rate limiting + exception handling

### Frontend (React 19 – `LoginJwt`)
- Login, registration, dashboard, and chatbot
- Token handling (localStorage or cookies depending on branch)
- Protected routes with React Router v7

---

## 🚀 Features

### ✅ Authentication Flow
- User registration with email notification
- JWT-based login and protected API access
- Automatic token refresh
- Logout with refresh token invalidation

### 👥 User Management
- Registration (username, email, password)
- Password change
- Dashboard with user info

### 🤖 Chatbot Integration
- Google Gemini API (API key required)
- Protected chat interface
- Real-time messaging

### 🔐 Security
- Password hashing
- JWT token handling
- Refresh token rotation
- Rate limiting for login
- Global exception middleware

---

## 🛠️ Tech Stack

### Backend
- .NET 9.0
- Entity Framework Core
- SQL Server
- JWT Authentication
- Minimal API architecture

### Frontend
- React 19
- Vite
- React Router v7
- Pure CSS (no UI libraries)

---

## 🧰 Getting Started

### ✅ Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/)
- [Node.js v18+](https://nodejs.org/)
- SQL Server
- Gemini API Key
- SMTP credentials (Gmail supported)

---

## ⚙️ Backend Setup

1. Navigate to the `JwtMinimalAPI` directory.
2. Create or update `appsettings.json`:
```json
{
  "ChatBotApiKey": "GEMINI KEY",
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

3. Set up User Secrets:
```bash
dotnet user-secrets set "ChatBotApiKey" "YOUR_GEMINI_API_KEY"
```

4. Apply database migrations:
```bash
dotnet ef database update
```

5. Start the API:
```bash
dotnet run
```

---

## ⚛️ Frontend Setup

1. Navigate to the `LoginJwt` directory.
2. Install dependencies:
```bash
npm install
```
3. Start the dev server:
```bash
npm run dev
```

---

## 🔄 JWT Flow (High-Level)

1. **Register** → Email + username + hashed password
2. **Login** → Receives access + refresh token
3. **Token Storage**
   - `main`: localStorage
   - `cookie-branch`: HttpOnly cookie
4. **Authenticated API calls**
   - `main`: Authorization header
   - `cookie-branch`: Automatic via browser
5. **Token Refresh**
   - Manual (main)
   - Middleware-based (cookie-branch)
6. **Logout** → Refresh token invalidation

---

## 🛡️ Password Security

- ASP.NET Core’s `PasswordHasher`
- Strong password enforcement
- Hashed passwords stored in DB

---

## 📚 Learning Outcomes

- JWT authentication flow
- Token storage strategies (localStorage vs cookies)
- Secure refresh token handling
- Rate limiting & exception handling
- Secure password validation
- Protected routes in React
- Middleware-based token refresh

---

## ⚠️ Important Notes

- Never commit secrets, API keys, or connection strings
- Use **User Secrets** during development
- Use **Environment Variables** in production
- Gmail SMTP with 2FA requires an **App Password**
- Use **HTTPS** in production environments

---

## 🌟 Future Enhancements

- User role management
- Password reset functionality
- Social login (Google, GitHub, etc.)
- Enhanced chatbot features
- Admin dashboard
- Unit & integration tests
- Logging & monitoring
- CSRF protection (cookie-branch)

---

## 📬 Contact

Feel free to fork, clone, or contribute to the project. This is a learning-focused repository, so improvements are always welcome!

---
