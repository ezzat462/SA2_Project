import { Link, NavLink, useNavigate } from "react-router-dom";
import { useContext } from "react";
import { AuthContext } from "../context/AuthContext";
import { ThemeContext } from "../context/ThemeContext";
import NotificationBell from "./NotificationBell";
import useFavorites from '../hooks/useFavorites';

export default function Navbar() {
  const { user, logout } = useContext(AuthContext);
  const { theme, toggleTheme } = useContext(ThemeContext);
  const navigate = useNavigate();
  const { favorites } = useFavorites();
  const favCount = favorites.length;

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <nav
      className={`navbar navbar-expand-lg px-4 ${theme === 'dark' ? 'navbar-dark bg-dark' : 'navbar-light bg-light'}`}
      style={{
        borderBottom: theme === 'dark' ? "1px solid #6a0dad" : "1px solid #dee2e6"
      }}
    >
      <div className="container-fluid">
        {/* Logo */}
        <Link className="navbar-brand fw-bold" to="/">
          <span style={{ color: "#9b30ff" }}>🚗Drive</span>Share
        </Link>

        {/* Hamburger for mobile */}
        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span className="navbar-toggler-icon"></span>
        </button>

        <div className="collapse navbar-collapse justify-content-end" id="navbarNav">
          <ul className="navbar-nav align-items-center gap-2">

            {/* Theme Toggle */}
            <li className="nav-item">
              <button className="btn btn-outline-secondary btn-sm rounded-circle" onClick={toggleTheme}>
                {theme === "dark" ? "☀️" : "🌙"}
              </button>
            </li>

            {user && (
              <>
                <li className="nav-item">
                  <NavLink className="nav-link" to="/">Home</NavLink>
                </li>
                {user.role === "Admin" && (
                  <li className="nav-item">
                    <NavLink className="nav-link" to="/admin">Admin Dash</NavLink>
                  </li>
                )}
                {user.role === "CarOwner" && (
                  <li className="nav-item">
                    <NavLink className="nav-link" to="/owner">Owner Dash</NavLink>
                  </li>
                )}
                {user.role === "Renter" && (
                  <li className="nav-item">
                    <NavLink className="nav-link" to="/renter/bookings">My Bookings</NavLink>
                  </li>
                )}
                <li className="nav-item d-flex align-items-center me-3 ms-2">
                  <NavLink
                    to="/favorites"
                    className="nav-link d-flex align-items-center gap-1"
                  >
                    Favorites
                    {favCount > 0 && (
                      <span style={{
                        background: '#ef4444',
                        color: 'white',
                        borderRadius: '999px',
                        padding: '1px 7px',
                        fontSize: '11px',
                        fontWeight: 'bold',
                        marginLeft: '2px',
                      }}>{favCount}</span>
                    )}
                  </NavLink>
                </li>
                <li className="nav-item d-flex align-items-center">
                  <NotificationBell />
                </li>
              </>
            )}

            {!user ? (
              <>
                <li className="nav-item">
                  <Link to="/login" className="btn btn-outline-primary btn-sm mx-1">Sign In</Link>
                </li>
                <li className="nav-item">
                  <Link
                    to="/register"
                    className="btn btn-sm"
                    style={{
                      background: "linear-gradient(45deg, #6a0dad, #9b30ff)",
                      color: "white",
                      border: "none"
                    }}
                  >
                    Sign Up
                  </Link>
                </li>
              </>
            ) : (
              <li className="nav-item dropdown">
                <span className="nav-link dropdown-toggle" style={{ cursor: 'pointer' }} id="navbarDropdown" role="button" data-bs-toggle="dropdown">
                  {user.unique_name || user.email}
                </span>
                <ul className="dropdown-menu dropdown-menu-end shadow">
                  <li><button className="dropdown-item text-danger" onClick={handleLogout}>Logout</button></li>
                </ul>
              </li>
            )}
          </ul>
        </div>
      </div>
    </nav>
  );
}