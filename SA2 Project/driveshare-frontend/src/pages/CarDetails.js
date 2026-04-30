import React, { useState, useEffect, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import carService from '../services/carService';
import ratingService from '../services/ratingService';
import { ThemeContext } from '../context/ThemeContext';

export default function CarDetails() {
    const { id } = useParams();
    const navigate = useNavigate();
    const { theme } = useContext(ThemeContext);
    const [car, setCar] = useState(null);
    const [ratings, setRatings] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchDetails = async () => {
            try {
                const carRes = await carService.getById(id);
                if (carRes.data) setCar(carRes.data);

                const ratingRes = await ratingService.getRatingsForCar(id);
                if (ratingRes.data) setRatings(ratingRes.data);
            } catch (err) {
                console.error(err);
            } finally {
                setLoading(false);
            }
        };
        fetchDetails();
    }, [id]);

    if (loading) return <div>Loading...</div>;
    if (!car) return <div className="container mt-5">Car not found.</div>;

    const avgScore = ratings.length > 0 
        ? (ratings.reduce((acc, r) => acc + r.score, 0) / ratings.length).toFixed(1)
        : null;

    return (
        <div className="container mt-5" style={{ maxWidth: '800px' }}>
            <div className={`card shadow-sm border-0 mb-4 ${theme === 'dark' ? 'bg-dark text-white' : ''}`}>
                <img 
                    src={car.imageUrl || "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf"} 
                    alt={car.brand} 
                    className="card-img-top" 
                    style={{ height: '300px', objectFit: 'cover' }}
                />
                <div className="card-body p-4">
                    <div className="d-flex justify-content-between">
                        <h2>{car.brand} {car.model} ({car.year})</h2>
                        <h2>${car.pricePerDay}/day</h2>
                    </div>
                    <p className="text-muted"><i className="bi bi-geo-alt"></i> {car.location}</p>
                    <p>{car.description || "A beautiful car ready for your trip."}</p>
                    <button 
                        className="btn btn-primary btn-lg rounded-pill px-5 mt-3"
                        onClick={() => navigate(`/booking/${car.id}`)}
                    >
                        Book Now
                    </button>
                </div>
            </div>

            {/* Reviews Section */}
            <h4 className="border-bottom pb-2 mb-4">Reviews {avgScore && <span className="text-warning">⭐ {avgScore}</span>}</h4>
            {ratings.length === 0 ? (
                <p className="text-muted">No reviews yet for this car.</p>
            ) : (
                <div className="list-group list-group-flush mb-5">
                    {ratings.map(r => (
                        <div key={r.id} className={`list-group-item border-0 border-bottom py-3 ${theme === 'dark' ? 'bg-dark text-white' : ''}`}>
                            <div className="d-flex w-100 justify-content-between mb-1">
                                <h6 className="mb-0 fw-bold">{r.renterName}</h6>
                                <small className="text-muted">{new Date(r.createdAt).toLocaleDateString()}</small>
                            </div>
                            <div className="text-warning mb-2">
                                {'⭐'.repeat(r.score)}
                                <span className="text-muted ms-2" style={{ fontSize: '0.8rem' }}>({r.score}/5)</span>
                            </div>
                            <p className="mb-0">{r.feedback}</p>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}
