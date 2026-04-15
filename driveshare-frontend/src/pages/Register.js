import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import authService from "../services/authService";
import { useAuth } from "../context/AuthContext";

export default function Register() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [form, setForm] = useState({
    fullName: "",
    email: "",
    password: "",
    role: "Renter",
    adminSecretCode: ""
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleChange = (e) => {
    setForm({
      ...form,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    if (!form.fullName || !form.email || !form.password) {
      setError("Please fill all fields");
      return;
    }

    try {
      setLoading(true);
      const response = await authService.register(form);
      if (response.success) {
        login(response.data.token);
        alert("Account created successfully 🎉");
        navigate("/");
      } else {
        setError(response.message || "Registration failed");
      }
    } catch (err) {
      console.error(err);
      if (!err.response) {
        setError("Network error: Could not reach the server.");
      } else if (err.response.data?.errors && typeof err.response.data.errors === 'object') {
        const errorMessages = Object.values(err.response.data.errors).flat().join(", ");
        setError(errorMessages || "Registration failed due to invalid data");
      } else {
        setError(err.response?.data?.message || "Registration failed");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="d-flex justify-content-center align-items-center vh-100">
      <div className="card p-4 shadow" style={{ width: "400px" }}>
        
        <h3 className="text-center mb-3">Sign Up</h3>

        {error && <div className="alert alert-danger">{error}</div>}

        <form onSubmit={handleSubmit}>

          <div className="mb-3">
            <label>Full Name</label>
            <input
              name="fullName"
              className="form-control"
              placeholder="Enter your name"
              onChange={handleChange}
              disabled={loading}
            />
          </div>

          <div className="mb-3">
            <label>Email</label>
            <input
              name="email"
              type="email"
              className="form-control"
              placeholder="Enter your email"
              onChange={handleChange}
              disabled={loading}
            />
          </div>

          <div className="mb-3">
            <label>Password</label>
            <input
              name="password"
              type="password"
              className="form-control"
              placeholder="Enter password"
              onChange={handleChange}
              disabled={loading}
            />
          </div>

          <div className="mb-3">
            <label>Account Type</label>
            <select
              name="role"
              className="form-control"
              onChange={handleChange}
              disabled={loading}
            >
              <option value="Renter">Renter</option>
              <option value="CarOwner">Car Owner</option>
              <option value="Admin">Admin</option>
            </select>
          </div>

          {form.role === "Admin" && (
            <div className="mb-3">
              <label>Admin Secret Code</label>
              <input
                name="adminSecretCode"
                type="password"
                className="form-control"
                placeholder="Enter admin secret code"
                onChange={handleChange}
                disabled={loading}
              />
            </div>
          )}

          <button className="btn primary-btn w-100" disabled={loading}>
            {loading ? "Signing up..." : "Sign Up"}
          </button>

        </form>

        <p className="text-center mt-3">
          Already have an account?{" "}
          <Link to="/login" className="fw-bold text-decoration-none">
            Sign In
          </Link>
        </p>

      </div>
    </div>
  );
}