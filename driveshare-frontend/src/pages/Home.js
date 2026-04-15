import { useState, useEffect } from "react";
import carService from "../services/carService";

export default function Home() {
  const [cars, setCars] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchCars();
  }, []);

  const fetchCars = async () => {
    try {
      const response = await carService.getAll();
      if (response.success) {
        setCars(response.data.items || response.data);
      }
    } catch (error) {
      console.error("Error fetching cars", error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div className="container mt-5 text-center">Loading cars...</div>;

  return (
    <div className="container mt-5">
      <div className="jumbotron text-white p-5 rounded shadow mb-5" style={{ background: "linear-gradient(45deg, #1e3a8a, #3b82f6)" }}>
        <h1>Find Your Perfect Ride 🚗</h1>
        <p className="lead">Rent directly from owners and save more.</p>
      </div>

      <h2 className="mb-4 text-center">Available Cars </h2>

      {cars.length === 0 ? (
        <div className="text-center mt-5">
          <p className="text-muted">No cars available for rent right now. Check back later!</p>
        </div>
      ) : (
        <div className="row">
          {cars.map((car) => (
            <div className="col-md-4" key={car.id}>
              <div className="card mb-4 shadow-sm h-100 car-card">
                <img
                  src={car.imageUrl || "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf"}
                  className="card-img-top"
                  alt={`${car.brand} ${car.model}`}
                  style={{ height: "200px", objectFit: "cover" }}
                />
                <div className="card-body">
                  <h5 className="card-title">{car.brand} {car.model}</h5>
                  <p className="card-text text-muted mb-2">
                    📍 {car.location} | 📅 {car.year}
                  </p>
                  <p className="card-text fw-bold text-primary mb-3">
                    💲 {car.pricePerDay} / day
                  </p>
                  <button className="btn btn-primary w-100">
                    View Details
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}