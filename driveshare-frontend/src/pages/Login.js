import { useState } from "react";
import { useAuth } from "../context/AuthContext";
import { Link, useNavigate } from "react-router-dom";
import authService from "../services/authService";
import { jwtDecode } from "jwt-decode";

export default function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    if (!email || !password) {
      setError("Please fill all fields");
      return;
    }

    try {
      setLoading(true);
      const response = await authService.login(email, password);
      // Backend returns wrapped ApiResponse, so we need data.data.token
      if (response.success) {
          login(response.data.token);

          // Decode token to determine role-based redirect
          const decoded = jwtDecode(response.data.token);
          const roleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
          const role = decoded[roleClaim] || decoded.role;

          if (role === "Admin") navigate("/admin");
          else if (role === "CarOwner") navigate("/owner");
          else navigate("/");
      } else {
        setError(response.message || "Login failed");
      }
    } catch (err) {
      // Backend returns 401 with ApiResponse body: { success: false, message: "..." }
      setError(
        err.response?.data?.message ||
        err.response?.data?.Message ||
        "Login failed. Please check your credentials."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="d-flex justify-content-center align-items-center vh-100">
      <div className="card p-4 shadow" style={{ width: "350px" }}>
        
        <h3 className="text-center mb-3">Sign In</h3>

        {error && <div className="alert alert-danger">{error}</div>}

        <form onSubmit={handleSubmit}>
          
          <div className="mb-3">
            <label>Email</label>
            <input
              type="email"
              className="form-control"
              placeholder="Enter email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              disabled={loading}
            />
          </div>

          <div className="mb-3">
            <label>Password</label>
            <input
              type="password"
              className="form-control"
              placeholder="Enter password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              disabled={loading}
            />
          </div>

          <button className="btn primary-btn w-100" disabled={loading}>
            {loading ? "Signing in..." : "Sign In"}
          </button>
        </form>

        <p className="text-center mt-3">
          Don't have an account?{" "}
          <Link to="/register" className="fw-bold text-decoration-none">
            Sign Up
          </Link>
        </p>

      </div>
    </div>
  );
}