import React from "react";

const AdminDashboard = () => {
  return (
    <div className="dashboard-container">
      <div className="dashboard-header">
        <h1>Admin Dashboard</h1>
        <button className="btn btn-danger" onClick={() => alert("Sign Out")}>
          Sign Out
        </button>
      </div>

      <div className="admin-info">
        <h2>Admin Information</h2>
        <p>Welcome to the Admin Dashboard!</p>
      </div>

      {/* Placeholder for admin-specific features */}
      <div className="admin-features">
        <h3>Admin Features</h3>
        <p>Manage users, view reports, and more.</p>
      </div>
    </div>
  );
};

export default AdminDashboard;
