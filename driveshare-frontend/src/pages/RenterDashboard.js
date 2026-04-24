import React, { useState, useEffect, useContext } from 'react';
import bookingService from '../services/bookingService';
import ratingService from '../services/ratingService';
import { ThemeContext } from '../context/ThemeContext';

export default function RenterDashboard() {
    const { theme } = useContext(ThemeContext);
    const [bookings, setBookings] = useState([]);
    const [loading, setLoading] = useState(true);
    const [selectedBooking, setSelectedBooking] = useState(null);
    const [score, setScore] = useState(5);
    const [feedback, setFeedback] = useState('');
    const [ratingMessage, setRatingMessage] = useState('');

    useEffect(() => {
        fetchBookings();
    }, []);

    const fetchBookings = async () => {
        try {
            const res = await bookingService.getMyBookings();
            if (res.data) setBookings(res.data);
        } catch (err) {
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleRate = async (e) => {
        e.preventDefault();
        try {
            await ratingService.createRating({
                bookingId: selectedBooking.id,
                score,
                feedback
            });
            setRatingMessage('Thanks for your feedback!');
            setTimeout(() => {
                setSelectedBooking(null);
                setRatingMessage('');
            }, 2000);
            fetchBookings(); // Refresh bookings to know it's rated (backend usually checks if rating exists, but for UI we might need to rely on success here)
        } catch (err) {
            setRatingMessage(err.response?.data?.message || 'Error submitting rating.');
        }
    };

    if (loading) return <div>Loading...</div>;

    return (
        <div className="container mt-5">
            <h2 className="mb-4">My Bookings</h2>
            <div className="row g-4">
                {bookings.map(b => (
                    <div className="col-12 col-md-6 mb-3" key={b.id}>
                        <div 
                            className={`card shadow-sm h-100 ${theme === 'dark' ? 'bg-dark text-white' : ''}`}
                            style={{ borderRadius: "15px", backdropFilter: "blur(10px)", background: theme === 'dark' ? 'rgba(33, 37, 41, 0.8)' : 'rgba(255, 255, 255, 0.8)' }}
                        >
                            <div className="card-body">
                                <h5>{b.carBrand} {b.carModel}</h5>
                                <p className="mb-1"><strong>From:</strong> {new Date(b.startDate).toLocaleDateString()} <strong>To:</strong> {new Date(b.endDate).toLocaleDateString()}</p>
                                <p className="mb-2"><strong>Total Price:</strong> ${b.totalPrice.toFixed(2)}</p>
                                
                                <span className={`badge bg-${b.status === 'Completed' ? 'success' : b.status === 'Accepted' ? 'primary' : b.status === 'Rejected' ? 'danger' : 'warning'}`}>
                                    {b.status}
                                </span>

                                {b.status === 'Completed' && (
                                    <button 
                                        className="btn btn-sm btn-outline-info ms-3"
                                        onClick={() => setSelectedBooking(b)}
                                    >
                                        ⭐ Rate this car
                                    </button>
                                )}
                            </div>
                        </div>
                    </div>
                ))}
            </div>

            {/* Rating Modal */}
            {selectedBooking && (
                <div className="modal show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
                    <div className="modal-dialog modal-dialog-centered">
                        <div className={`modal-content ${theme === 'dark' ? 'bg-dark text-white' : ''}`}>
                            <div className="modal-header">
                                <h5 className="modal-title">Rate {selectedBooking.carBrand}</h5>
                                <button type="button" className="btn-close" onClick={() => setSelectedBooking(null)}></button>
                            </div>
                            <div className="modal-body">
                                {ratingMessage && <div className="alert alert-info">{ratingMessage}</div>}
                                <form onSubmit={handleRate}>
                                    <div className="mb-3">
                                        <label className="form-label">Score (1-5)</label>
                                        <select className="form-control" value={score} onChange={e => setScore(Number(e.target.value))}>
                                            {[5, 4, 3, 2, 1].map(n => <option key={n} value={n}>{n} Stars</option>)}
                                        </select>
                                    </div>
                                    <div className="mb-3">
                                        <label className="form-label">Feedback</label>
                                        <textarea className="form-control" rows="3" value={feedback} onChange={e => setFeedback(e.target.value)}></textarea>
                                    </div>
                                    <button type="submit" className="btn btn-primary w-100 placeholder-wave align-items-center">Submit Rating</button>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
