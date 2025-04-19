import LoginForm from "../Forms/LoginForm";

const Login = () => {
  return (
    <div className="form-box">
      <LoginForm />
      <div className="form-section">
        <p>
          <a href="/resetpassword">Forgot password?</a>
        </p>
        <p>
          <a href="/register">Don't have an account?</a>
        </p>
      </div>
    </div>
  );
};

export default Login;
