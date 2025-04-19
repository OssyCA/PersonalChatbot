export default function ResetPasswordForm() {
  return (
    <div className="reset-password-form-container">
      <h2>Reset Password</h2>
      <form className="reset-password-form">
        <div className="form-group">
          <input
            type="email"
            className="form-input"
            placeholder="Enter your email"
            required
          />
        </div>
        <button type="submit" className="btn btn-primary">
          Send Reset Link
        </button>
      </form>
    </div>
  );
}
