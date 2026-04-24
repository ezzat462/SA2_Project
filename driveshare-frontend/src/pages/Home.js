import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import carService from "../services/carService";

export default function Home() {
  const [cars, setCars] = useState([]);
  const [loading, setLoading] = useState(true);
  const [sortOrder, setSortOrder] = useState("newest");
  const { user } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    fetchCars();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [sortOrder]);

  const fetchCars = async () => {
    try {
      setLoading(true);
      const response = await carService.getAll({ sortOrder });
      if (response.success) {
        setCars(response.data.items || response.data);
      }
    } catch (error) {
      console.error("Error fetching cars:", error);
      // Dummy data for fallback if service fails
      const dummyCars = [
        { id: 1, brand: "Tesla", model: "Model S", location: "San Francisco", year: 2023, pricePerDay: 150, imageUrl: "https://images.unsplash.com/photo-1560958089-b8a1929cea89" },
        { id: 2, brand: "BMW", model: "M4", location: "Los Angeles", year: 2022, pricePerDay: 120, imageUrl: "https://images.unsplash.com/photo-1555215695-3004980ad54e" },
        { id: 3, brand: "Audi", model: "RS6", location: "New York", year: 2023, pricePerDay: 180, imageUrl: "https://images.unsplash.com/photo-1606152424101-ad2f9bc9b8b0" },
      ];
      setCars(dummyCars);
    } finally {
      setLoading(false);
    }
  };

  const handleBookNow = (carId) => {
    if (!user) {
      navigate("/login", { state: { from: "/", message: "Please log in to book a ride!" } });
    } else {
      navigate(`/booking/${carId}`);
    }
  };

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center vh-100">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading cars...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="container py-5 mt-4">
      {/* Hero Section */}
      <div className="hero-section text-center p-5 rounded-4 shadow-lg mb-5" 
           style={{ 
             background: "linear-gradient(135deg, #1e3a8a 0%, #3b82f6 100%)",
             color: "#fff",
             position: "relative",
             overflow: "hidden" 
           }}>
        <div style={{ position: "relative", zIndex: 2 }}>
          <h1 className="display-4 fw-bold mb-3 mt-4">Find Your Perfect Drive 🚗</h1>
          <p className="lead fs-4 opacity-75 mb-4">The ultimate marketplace for peer-to-peer car sharing.</p>
          <div className="d-flex justify-content-center gap-3 mb-4">
             <span className="badge bg-light text-dark px-3 py-2 rounded-pill shadow-sm">Verified Owners</span>
             <span className="badge bg-light text-dark px-3 py-2 rounded-pill shadow-sm">Premium Fleet</span>
             <span className="badge bg-light text-dark px-3 py-2 rounded-pill shadow-sm">24/7 Support</span>
          </div>
        </div>
        {/* Decorative backdrop elements */}
        <div style={{ position: "absolute", top: "-20%", right: "-10%", width: "300px", height: "300px", background: "rgba(255,255,255,0.1)", borderRadius: "50%" }}></div>
        <div style={{ position: "absolute", bottom: "-10%", left: "-10%", width: "200px", height: "200px", background: "rgba(255,255,255,0.05)", borderRadius: "50%" }}></div>
      </div>

      <div className="d-flex justify-content-between align-items-center mb-5">
        <h2 className="fw-bold m-0 border-start border-primary border-4 ps-3">Available for rent</h2>
        <div className="dropdown">
          <button className="btn btn-outline-secondary dropdown-toggle" type="button" id="filterDropdown" data-bs-toggle="dropdown" aria-expanded="false">
            Filter Results
          </button>
          <ul className="dropdown-menu dropdown-menu-end shadow border-0 p-2" aria-labelledby="filterDropdown">
            <li><button className="dropdown-item rounded" onClick={() => setSortOrder("price_asc")}>Price: Low to High</button></li>
            <li><button className="dropdown-item rounded" onClick={() => setSortOrder("price_desc")}>Price: High to Low</button></li>
            <li><hr className="dropdown-divider" /></li>
            <li><button className="dropdown-item rounded" onClick={() => setSortOrder("newest")}>Newest First</button></li>
          </ul>
        </div>
      </div>

      {cars.length === 0 ? (
        <div className="text-center py-5 bg-light rounded-4">
          <div className="display-1 mb-3">📭</div>
          <h3 className="text-muted">No cars available right now</h3>
          <p className="text-secondary">Please check back later or try adjusting your search filters.</p>
        </div>
      ) : (
        <div className="row g-4">
          {cars.map((car) => (
            <div className="col-md-6 col-lg-4" key={car.id}>
              <div className="card h-100 border-0 shadow-sm rounded-4 overflow-hidden car-hover-card">
                <div className="position-relative">
                  <img
                    src={car.imageUrl || "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf"}
                    className="card-img-top"
                    alt={`${car.brand} ${car.model}`}
                    style={{ height: "220px", objectFit: "cover" }}
                  />
                  <div className="position-absolute top-0 end-0 m-3">
                    <span className="badge bg-white text-primary fw-bold shadow-sm p-2 rounded-pill px-3">
                      NEW
                    </span>
                  </div>
                </div>
                
                <div className="card-body p-4">
                  <div className="d-flex justify-content-between align-items-start mb-2">
                    <div>
                      <h5 className="card-title fw-bold mb-1 fs-4">{car.brand} {car.model}</h5>
                      <p className="text-muted small mb-0">
                        <i className="bi bi-geo-alt-fill me-1"></i> {car.location} | {car.year}
                      </p>
                    </div>
                    <div className="text-end">
                       <span className="text-primary fw-bold fs-4">${car.pricePerDay}</span>
                       <span className="text-muted small">/day</span>
                    </div>
                  </div>
                  
                  <div className="d-flex mt-3 mb-4 text-muted small gap-3">
                    <span><i className="bi bi-person me-1"></i> 5 Seats</span>
                    <span><i className="bi bi-gear me-1"></i> Auto</span>
                    <span><i className="bi bi-fuel-pump me-1"></i> Electric</span>
                  </div>

                  <div className="d-grid gap-2">
                    <button 
                      onClick={() => handleBookNow(car.id)}
                      className="btn btn-primary rounded-pill py-2 fw-bold shadow-sm"
                    >
                      Book Now
                    </button>
                    <button 
                      className="btn btn-light rounded-pill py-2 text-primary border-0"
                      onClick={() => navigate(`/cars/${car.id}`)}
                    >
                      View Details
                    </button>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Styled JSX (Optional if no global CSS is provided) */}
      <style>{`
        .car-hover-card {
          transition: transform 0.3s ease, box-shadow 0.3s ease;
        }
        .car-hover-card:hover {
          transform: translateY(-8px);
          box-shadow: 0 1rem 3rem rgba(0,0,0,0.12) !important;
        }
        .hero-section {
          background-size: cover;
          background-position: center;
        }
      `}</style>
    </div>
  );
}