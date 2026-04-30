import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import AuthProvider from "./context/AuthContext";
import { NotificationProvider } from "./context/NotificationContext";
import Home from "./pages/Home";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Navbar from "./components/Navbar";
import ProtectedRoute from "./components/ProtectedRoute";
import AdminDashboard from "./pages/AdminDashboard";
import OwnerDashboard from "./pages/OwnerDashboard";
import BookingPage from "./pages/BookingPage";
import RenterDashboard from "./pages/RenterDashboard";
import LicensePage from "./pages/LicensePage";
import CarDetails from "./pages/CarDetails";
import Favorites from "./pages/Favorites";
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

function App() {
  return (
    <AuthProvider>
      <NotificationProvider>
        <ToastContainer position="top-right" autoClose={5000} pauseOnHover theme="colored" />
        <Router>
          <Navbar />

          <Routes>
            {/* Public Routes */}
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/cars/:id" element={<CarDetails />} />

            {/* Protected Routes: Admin Only */}
            <Route element={<ProtectedRoute allowedRoles={["Admin"]} />}>
              <Route path="/admin" element={<AdminDashboard />} />
            </Route>

            {/* Protected Routes: Owner Only */}
            <Route element={<ProtectedRoute allowedRoles={["CarOwner", "Admin"]} />}>
              <Route path="/owner" element={<OwnerDashboard />} />
            </Route>

            {/* Protected Routes: User */}
            <Route element={<ProtectedRoute allowedRoles={["CarOwner", "Admin", "User", "Renter"]} />}>
              <Route path="/booking/:carId" element={<BookingPage />} />
              <Route path="/renter/bookings" element={<RenterDashboard />} />
              <Route path="/profile/license" element={<LicensePage />} />
              <Route path="/favorites" element={<Favorites />} />
            </Route>
          </Routes>
        </Router>
      </NotificationProvider>
    </AuthProvider>
  );
}

export default App;