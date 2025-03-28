import React from "react";

const Register = () => {
  const [username, setUsername] = React.useState("");
  const [email, setEmail] = React.useState("");
  const [password, setPassword] = React.useState("");
  const handleSubmit = (e) => {
    e.preventDefault();

    const data = {
      UserName: username,
      Email: email,
      Password: password,
    };

    fetch("https://localhost:7289/register", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    })
      .then((response) => response.json()) // parse JSON from request response t
      .catch((error) => {
        console.error("Error:", error);
      });
  };
  return (
    <div className="form-box">
      <form className="form" method="POST" onSubmit={handleSubmit}>
        <span className="title">Sign up</span>
        <span className="subtitle">Create a free account with your email.</span>
        <div className="form-container">
          <input
            type="text"
            className="input"
            placeholder="Username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
          />
          <input
            type="email"
            className="input"
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          <input
            type="password"
            className="input"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
        <button>Sign up</button>
      </form>
      <div className="form-section">
        <p>
          <a href="/Login/">Have an account?</a>
        </p>
      </div>
    </div>
  );
};

export default Register;
