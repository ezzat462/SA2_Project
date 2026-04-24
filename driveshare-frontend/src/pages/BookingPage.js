import React, { useState, useEffect, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import bookingService from '../services/bookingService';
import licenseService from '../services/licenseService';
import carService from '../services/carService';
import { ThemeContext } from '../context/ThemeContext';
import { toast } from 'react-toastify';

export default function BookingPage() {
    const { carId } = useParams();
    const navigate = useNavigate();
    const { theme } = useContext(ThemeContext);
    
    const [car, setCar] = useState(null);
    const [licenseVerified, setLicenseVerified] = useState(false);
    const [loading, setLoading] = useState(true);
    const [startDate, setStartDate] = useState('');
    const [endDate, setEndDate] = useState('');
    const [totalPrice, setTotalPrice] = useState(0);
    const [message, setMessage] = useState('');
    const [bookingSuccess, setBookingSuccess] = useState(false);

    useEffect(() => {
        const init = async () => {
            try {
                const licRes = await licenseService.getMyLicense();
                if (licRes.data?.status !== 'Verified') {
                    setMessage('Please upload and verify your driver license before booking.');
                    setTimeout(() => navigate('/profile/license'), 3000);
                    return;
                }
                setLicenseVerified(true);

                const carRes = await carService.getById(carId);
                if (carRes.data) {
                    setCar(carRes.data);
                }
            } catch (err) {
                console.error(err);
            } finally {
                setLoading(false);
            }
        };
        init();
    }, [carId, navigate]);

    useEffect(() => {
        if (startDate && endDate && car) {
            const start = new Date(startDate);
            const end = new Date(endDate);
            let days = Math.ceil((end - start) / (1000 * 60 * 60 * 24));
            if (days <= 0) days = 1;
            setTotalPrice(days * car.pricePerDay);
        } else {
            setTotalPrice(0);
        }
    }, [startDate, endDate, car]);

    const handleBooking = async (e) => {
        e.preventDefault();
        setMessage('');
        try {
            const res = await bookingService.createBooking({
                carPostId: parseInt(carId),
                startDate,
                endDate
            });
            if (res.success) {
                setBookingSuccess(true);
            } else {
                setMessage(res.message || 'Error creating booking.');
            }
        } catch (err) {
            setMessage(err.response?.data?.message || 'Error creating booking.');
        }
    };

    if (loading) return <div>Loading...</div>;

    if (!licenseVerified) {
        return (
            <div className="container mt-5 text-center">
                <div className="alert alert-warning">{message}</div>
                <p>Redirecting to license page...</p>
            </div>
        );
    }

    if (!car) return <div>Car not found</div>;

    if (bookingSuccess) {
        return (
            <div className="container mt-5" style={{ maxWidth: '600px' }}>
                <div 
                    className="card shadow-sm border-0 p-5 text-center"
                    style={{ borderRadius: '16px' }}
                >
                    <div style={{ fontSize: '3.5rem', marginBottom: '16px' }}>🎉</div>
                    <h3 className="fw-bold mb-3" style={{ color: '#198754' }}>Booking Request Sent!</h3>
                    <p className="text-muted mb-2" style={{ fontSize: '0.95rem', lineHeight: '1.6' }}>
                        Your rental request for <strong>{car.brand} {car.model}</strong> has been submitted successfully.
                    </p>
                    <p className="text-muted mb-4" style={{ fontSize: '0.9rem' }}>
                        The car owner has been notified and will review your request.
                        You'll receive a notification once they accept or decline.
                    </p>
                    <div className="d-flex gap-3 justify-content-center">
                        <button
                            className="btn px-4 py-2 rounded-pill fw-semibold"
                            style={{ background: 'linear-gradient(45deg, #6a0dad, #9b30ff)', color: '#fff', border: 'none' }}
                            onClick={() => navigate('/renter/bookings')}
                        >
                            View My Bookings
                        </button>
                        <button
                            className="btn btn-outline-secondary px-4 py-2 rounded-pill"
                            onClick={() => navigate('/')}
                        >
                            Browse More Cars
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="container mt-5" style={{ maxWidth: '600px' }}>
            <div 
                className={`card shadow-sm p-4 ${theme === 'dark' ? 'bg-dark text-white' : ''}`}
                style={{ borderRadius: "15px", backdropFilter: "blur(10px)", background: theme === 'dark' ? 'rgba(33, 37, 41, 0.8)' : 'rgba(255, 255, 255, 0.8)' }}
            >
                <h3 className="mb-3">Book {car.brand} {car.model}</h3>
                <p className="text-muted">Price per day: ${car.pricePerDay}</p>
                <form onSubmit={handleBooking}>
                    <div className="mb-3">
                        <label className="form-label">Start Date</label>
                        <input 
                            type="date" 
                            className="form-control" 
                            value={startDate}
                            min={new Date(car.availableFrom).toISOString().split('T')[0]}
                            max={new Date(car.availableTo).toISOString().split('T')[0]}
                            onChange={(e) => setStartDate(e.target.value)} 
                            required 
                        />
                    </div>
                    <div className="mb-3">
                        <label className="form-label">End Date</label>
                        <input 
                            type="date" 
                            className="form-control" 
                            value={endDate}
                            min={startDate || new Date(car.availableFrom).toISOString().split('T')[0]}
                            max={new Date(car.availableTo).toISOString().split('T')[0]}
                            onChange={(e) => setEndDate(e.target.value)} 
                            required 
                        />
                    </div>
                    {totalPrice > 0 && (
                        <div className="alert alert-info py-2">
                            <strong>Total Price:</strong> ${totalPrice.toFixed(2)}
                        </div>
                    )}
                    {message && <div className="alert alert-danger py-2">{message}</div>}
                    <button 
                        type="submit" 
                        className="btn w-100 placeholder-wave"
                        style={{ background: "linear-gradient(45deg, #6a0dad, #9b30ff)", color: "white", border: "none" }}
                    >
                        Request Booking
                    </button>
                </form>
            </div>
        </div>
    );
}
