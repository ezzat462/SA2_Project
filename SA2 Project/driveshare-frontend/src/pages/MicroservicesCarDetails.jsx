import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { rentalApiClient, userApiClient } from '../api/axiosClients';

const MicroservicesCarDetails = () => {
  const { id } = useParams();
  const [car, setCar] = useState(null);
  const [owner, setOwner] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchFullDetails = async () => {
      try {
        setLoading(true);
        
        // 1. Fetch Car details from RentalService
        const carData = await rentalApiClient.get(`/cars/${id}`);
        setCar(carData.data);

        // 2. Extract ownerId and fetch Owner details from UserService
        if (carData.data?.ownerId) {
          const ownerData = await userApiClient.get(`/admin/users/${carData.data.ownerId}`);
          // Note: Using /admin/users/id assuming an endpoint exists to get user by id
          setOwner(ownerData.data);
        }
      } catch (err) {
        console.error('Aggregation Error:', err);
        setError('Failed to load full car details.');
      } finally {
        setLoading(false);
      }
    };

    fetchFullDetails();
  }, [id]);

  if (loading) return <div className="text-center mt-5">Loading Car & Owner details...</div>;
  if (error) return <div className="alert alert-danger m-5">{error}</div>;
  if (!car) return <div className="container mt-5">Car not found.</div>;

  return (
    <div className="container mt-5">
      <div className="row">
        <div className="col-md-8">
          <div className="card shadow-lg border-0 overflow-hidden" style={{ borderRadius: '20px' }}>
            <img 
              src={car.imageUrl || "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf"} 
              alt={car.brand} 
              className="img-fluid"
              style={{ maxHeight: '400px', objectFit: 'cover' }}
            />
            <div className="card-body p-4">
              <h2 className="display-6 fw-bold mb-3">{car.brand} {car.model}</h2>
              <div className="d-flex gap-3 mb-3">
                <span className="badge bg-primary rounded-pill px-3 py-2">{car.type}</span>
                <span className="badge bg-info text-dark rounded-pill px-3 py-2">{car.transmission}</span>
                <span className="badge bg-secondary rounded-pill px-3 py-2">{car.year}</span>
              </div>
              <p className="lead text-muted">{car.description || "No description provided."}</p>
              <hr />
              <div className="d-flex justify-content-between align-items-center">
                <h3 className="text-primary mb-0">${car.pricePerDay} <small className="text-muted" style={{fontSize: '0.8rem'}}>/ day</small></h3>
                <button className="btn btn-primary btn-lg rounded-pill px-5">Book Now</button>
              </div>
            </div>
          </div>
        </div>

        <div className="col-md-4">
          <div className="card border-0 shadow-sm" style={{ borderRadius: '20px', background: '#f8f9fa' }}>
            <div className="card-body p-4 text-center">
              <h5 className="fw-bold mb-4">Meet the Owner</h5>
              {owner ? (
                <>
                  <div className="rounded-circle bg-primary text-white d-flex align-items-center justify-content-center mx-auto mb-3" style={{ width: '80px', height: '80px', fontSize: '2rem' }}>
                    {owner.fullName?.charAt(0)}
                  </div>
                  <h6 className="mb-1 fw-bold">{owner.fullName}</h6>
                  <p className="text-muted small mb-3">{owner.email}</p>
                  <div className="d-grid">
                    <button className="btn btn-outline-primary btn-sm rounded-pill">Contact Owner</button>
                  </div>
                </>
              ) : (
                <p className="text-muted italic">Owner details currently unavailable.</p>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default MicroservicesCarDetails;
