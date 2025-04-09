import React from "react";

const ChangePasswordForm = () => {
  return (
    <div className="changePasswordContainer">
      <h1>Change password</h1>
      <form action="POST">
        <input type="password" placeholder="oldpassword" className="input" />
        <input type="password" placeholder="new password" className="input" />
        <button className="changeBtn" type="submit">
          Change password
        </button>
      </form>
    </div>
  );
};

export default ChangePasswordForm;
