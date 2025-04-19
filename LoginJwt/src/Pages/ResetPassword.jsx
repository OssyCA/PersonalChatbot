import React from "react";
import ResetPasswordForm from "../Forms/ResetPasswordForm";

const ResetPassword = () => {
  return (
    <>
      <div className="reset-password-page">
        <h1>Reset Password</h1>
        <ResetPasswordForm />
        <p className="reset-password-note">
          Please enter your email to receive a password reset link.
        </p>
      </div>
    </>
  );
};

export default ResetPassword;
