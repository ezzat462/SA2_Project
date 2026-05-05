import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import carService from "../services/carService";
import useFavorites from "../hooks/useFavorites";

export default function Home() {
  const [cars, setCars] = useState([]);
  const [loading, setLoading] = useState(true);
  const [sortOrder, setSortOrder] = useState("newest");
  const [searchText, setSearchText] = useState("");
  const [filterBrand, setFilterBrand] = useState("");
  const [filterType, setFilterType] = useState("");
  const [filterLocation, setFilterLocation] = useState("");
  const [filterMinPrice, setFilterMinPrice] = useState("");
  const [filterMaxPrice, setFilterMaxPrice] = useState("");
  const { user } = useAuth();
  const navigate = useNavigate();
  const { toggleFavorite, isFavorite } = useFavorites();

  const filteredCars = cars.filter((car) => {
    const matchesSearch =
      searchText === "" ||
      car.brand?.toLowerCase().includes(searchText.toLowerCase()) ||
      car.model?.toLowerCase().includes(searchText.toLowerCase()) ||
      car.location?.toLowerCase().includes(searchText.toLowerCase());

    const matchesBrand =
      filterBrand === "" || car.brand?.toLowerCase().includes(filterBrand.toLowerCase());

    const matchesType =
      filterType === "" || car.carType?.toLowerCase() === filterType.toLowerCase();

    const matchesLocation =
      filterLocation === "" || car.location?.toLowerCase().includes(filterLocation.toLowerCase());

    const matchesMin =
      filterMinPrice === "" || car.pricePerDay >= Number(filterMinPrice);

    const matchesMax =
      filterMaxPrice === "" || car.pricePerDay <= Number(filterMaxPrice);

    return matchesSearch && matchesBrand && matchesType && matchesLocation && matchesMin && matchesMax;
  });

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

      <div className="mb-5">
        <div className="d-flex justify-content-between align-items-center mb-3">
          <h2 className="fw-bold m-0 border-start border-primary border-4 ps-3">Available for rent</h2>
          <div className="dropdown">
            <button className="btn btn-outline-secondary dropdown-toggle" type="button" data-bs-toggle="dropdown">
              Sort Results
            </button>
            <ul className="dropdown-menu dropdown-menu-end shadow border-0 p-2">
              <li><button className="dropdown-item rounded" onClick={() => setSortOrder("price_asc")}>Price: Low to High</button></li>
              <li><button className="dropdown-item rounded" onClick={() => setSortOrder("price_desc")}>Price: High to Low</button></li>
              <li><hr className="dropdown-divider" /></li>
              <li><button className="dropdown-item rounded" onClick={() => setSortOrder("newest")}>Newest First</button></li>
            </ul>
          </div>
        </div>

        {/* Search + Filter Row */}
        <div
          className="p-3 rounded-4 d-flex flex-wrap gap-2 align-items-center"
          style={{
            background: "rgba(255,255,255,0.05)",
            backdropFilter: "blur(10px)",
            border: "1px solid rgba(255,255,255,0.1)",
          }}
        >
          {/* Search input */}
          <div className="input-group flex-grow-1" style={{ minWidth: "200px", maxWidth: "300px" }}>
            <span className="input-group-text bg-transparent border-secondary text-secondary">
              <i className="bi bi-search"></i>
            </span>
            <input
              type="text"
              className="form-control bg-transparent border-secondary text-white"
              placeholder="Search brand, model or city..."
              value={searchText}
              onChange={(e) => setSearchText(e.target.value)}
              style={{ color: "white" }}
            />
          </div>

          {/* Car Type dropdown */}
          <select
            className="form-select bg-transparent border-secondary"
            style={{ minWidth: "140px", maxWidth: "160px", color: filterType ? "white" : "#aaa" }}
            value={filterType}
            onChange={(e) => setFilterType(e.target.value)}
          >
            <option value="" style={{ background: "#1a1a2e" }}>All Types</option>
            <option value="Sedan" style={{ background: "#1a1a2e" }}>Sedan</option>
            <option value="SUV" style={{ background: "#1a1a2e" }}>SUV</option>
            <option value="Truck" style={{ background: "#1a1a2e" }}>Truck</option>
            <option value="Van" style={{ background: "#1a1a2e" }}>Van</option>
            <option value="Sports" style={{ background: "#1a1a2e" }}>Sports</option>
            <option value="Other" style={{ background: "#1a1a2e" }}>Other</option>
          </select>

          {/* Location input */}
          <input
            type="text"
            className="form-control bg-transparent border-secondary text-white"
            placeholder="📍 Location"
            style={{ minWidth: "130px", maxWidth: "160px" }}
            value={filterLocation}
            onChange={(e) => setFilterLocation(e.target.value)}
          />

          {/* Min price */}
          <input
            type="number"
            className="form-control bg-transparent border-secondary text-white"
            placeholder="$ Min"
            style={{ minWidth: "90px", maxWidth: "110px" }}
            value={filterMinPrice}
            onChange={(e) => setFilterMinPrice(e.target.value)}
          />

          {/* Max price */}
          <input
            type="number"
            className="form-control bg-transparent border-secondary text-white"
            placeholder="$ Max"
            style={{ minWidth: "90px", maxWidth: "110px" }}
            value={filterMaxPrice}
            onChange={(e) => setFilterMaxPrice(e.target.value)}
          />

          {/* Clear filters button — only show if any filter is active */}
          {(searchText || filterBrand || filterType || filterLocation || filterMinPrice || filterMaxPrice) && (
            <button
              className="btn btn-sm btn-outline-danger rounded-pill px-3"
              onClick={() => {
                setSearchText("");
                setFilterBrand("");
                setFilterType("");
                setFilterLocation("");
                setFilterMinPrice("");
                setFilterMaxPrice("");
              }}
            >
              ✕ Clear
            </button>
          )}

          {/* Results count */}
          <span className="ms-auto text-secondary small">
            {filteredCars.length} car{filteredCars.length !== 1 ? "s" : ""} found
          </span>
        </div>
      </div>

      {filteredCars.length === 0 ? (
        <div className="text-center py-5 bg-light rounded-4">
          <div className="display-1 mb-3">📭</div>
          <h3 className="text-muted">No cars match your search</h3>
          <p className="text-secondary">Try adjusting the filters or clearing the search.</p>
        </div>
      ) : (
        <div className="row g-4">
          {filteredCars.map((car) => (
            <div className="col-md-6 col-lg-4" key={car.id}>
              <div className="card h-100 border-0 shadow-sm rounded-4 overflow-hidden car-hover-card">
                <div className="position-relative">
                  <img
                    src={car.imageUrl || "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf"}
                    className="card-img-top"
                    alt={`${car.brand} ${car.model}`}
                    style={{ height: "220px", objectFit: "cover" }}
                  />
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      if (!user) {
                        navigate('/login', { state: { from: '/', message: 'Please log in to save favorites!' } });
                        return;
                      }
                      toggleFavorite(car);
                    }}
                    style={{
                      position: 'absolute',
                      top: '12px',
                      left: '12px',
                      background: 'transparent',
                      border: '1.5px solid rgba(255,255,255,0.5)',
                      borderRadius: '50%',
                      width: '36px',
                      height: '36px',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      cursor: 'pointer',
                      fontSize: '18px',
                      transition: 'transform 0.2s ease, background 0.2s ease',
                      zIndex: 10,
                    }}
                    onMouseEnter={e => e.currentTarget.style.transform = 'scale(1.15)'}
                    onMouseLeave={e => e.currentTarget.style.transform = 'scale(1)'}
                    title={isFavorite(car.id) ? 'Remove from favorites' : 'Add to favorites'}
                  >
                    <span style={{ color: isFavorite(car.id) ? '#ef4444' : 'white', fontSize: '18px', lineHeight: 1 }}>
                      {isFavorite(car.id) ? '❤' : '♡'}
                    </span>
                  </button>
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
        .form-control::placeholder {
          color: #888 !important;
        }
        .form-control:focus {
          background: rgba(255,255,255,0.08) !important;
          border-color: #3b82f6 !important;
          box-shadow: 0 0 0 0.2rem rgba(59,130,246,0.25) !important;
          color: white !important;
        }
        .form-select:focus {
          border-color: #3b82f6 !important;
          box-shadow: 0 0 0 0.2rem rgba(59,130,246,0.25) !important;
        }
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