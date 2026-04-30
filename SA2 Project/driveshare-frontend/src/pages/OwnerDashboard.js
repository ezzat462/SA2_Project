import { useState, useEffect, useContext } from "react";
import carService from "../services/carService";
import bookingService from "../services/bookingService";
import { AuthContext } from "../context/AuthContext";
import { toast } from 'react-toastify';

export default function OwnerDashboard() {
  const { user } = useContext(AuthContext);
  const today = new Date().toISOString().split("T")[0];
  const [myCars, setMyCars] = useState([]);
  const [bookings, setBookings] = useState([]);
  const [showSubmitModal, setShowSubmitModal] = useState(false);
  const [form, setForm] = useState({
    title: "",
    brand: "",
    model: "",
    year: new Date().getFullYear(),
    pricePerDay: "",
    location: "",
    description: "",
    imageUrl: "",
    type: 0, // Sedan
    transmission: 0, // Auto
    availableFrom: today,
    availableTo: ""
  });
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchMyCars();
    fetchBookings();
  }, []);

  const fetchMyCars = async () => {
    try {
      const res = await carService.getMyCars();
      if (res.success) setMyCars(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  const fetchBookings = async () => {
    try {
      const res = await bookingService.getIncomingBookings();
      if (res.success) setBookings(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  const handleAccept = async (id) => {
    const res = await bookingService.acceptBooking(id);
    if (res.success) {
      toast.success('Booking accepted! The renter has been notified.');
    } else {
      toast.error(res.message || 'Failed to accept booking.');
    }
    fetchBookings();
  };

  const handleReject = async (id) => {
    const res = await bookingService.rejectBooking(id);
    if (res.success) {
      toast.info('Booking rejected. The renter has been notified.');
    } else {
      toast.error(res.message || 'Failed to reject booking.');
    }
    fetchBookings();
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    // Map type and transmission to integers
    if (name === 'type' || name === 'transmission' || name === 'year') {
      setForm({ ...form, [name]: parseInt(value) });
    } else if (name === 'pricePerDay') {
      setForm({ ...form, [name]: parseFloat(value) });
    } else {
      setForm({ ...form, [name]: value });
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (new Date(form.availableTo) <= new Date(form.availableFrom)) {
      toast.warning("'Available To' date must be after 'Available From' date");
      return;
    }
    try {
      setLoading(true);
      const res = await carService.create(form);
      if (res.success) {
        setShowSubmitModal(true);
        setForm({
          title: "",
          brand: "", model: "", year: new Date().getFullYear(),
          pricePerDay: "", location: "", description: "", imageUrl: "",
          type: 0, transmission: 0,
          availableFrom: today, availableTo: ""
        });
        fetchMyCars();
      } else {
        toast.error(res.message || "Failed to post car");
      }
    } catch (error) {
      toast.error(error.response?.data?.message || "Failed to post car");
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateStr) => {
    if (!dateStr) return "N/A";
    return new Date(dateStr).toLocaleDateString();
  };

  if (user && (user.accountStatus === 0 || user.accountStatus === 'Pending')) {
    return (
      <div className="container mt-5 text-center">
        <h2 className="mb-4 fw-bold">Owner Dashboard</h2>
        <div className="alert alert-warning mt-4 p-5 d-inline-block shadow border-0 rounded-4" style={{ maxWidth: '600px' }}>
          <h4 className="alert-heading fw-bold mb-3">⏳ Waiting for Admin Approval</h4>
          <p className="mb-0 text-dark">Your account is currently under review by an administrator. You will be able to post listings and manage bookings once approved.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="container mt-5">
      <h2>Owner Dashboard</h2>
      
      <div className="row mt-4">
        <div className="col-md-4">
          <div className="card p-3 shadow-sm">
            <h4>Post a New Car</h4>
            <form onSubmit={handleSubmit}>
              <div className="mb-2">
                <label>Listing Title</label>
                <input name="title" className="form-control" value={form.title} onChange={handleChange} required placeholder="e.g. Luxury Sport Drive" />
              </div>
              <div className="row">
                <div className="col-md-6 mb-2">
                  <label>Brand</label>
                  <input name="brand" className="form-control" value={form.brand} onChange={handleChange} required />
                </div>
                <div className="col-md-6 mb-2">
                  <label>Model</label>
                  <input name="model" className="form-control" value={form.model} onChange={handleChange} required />
                </div>
              </div>
              <div className="row">
                <div className="col-md-6 mb-2">
                  <label>Type</label>
                  <select name="type" className="form-select" value={form.type} onChange={handleChange}>
                     <option value="0">Sedan</option>
                     <option value="1">SUV</option>
                     <option value="2">Truck</option>
                     <option value="3">Coupe</option>
                     <option value="4">Convertible</option>
                     <option value="5">Van</option>
                     <option value="6">Other</option>
                  </select>
                </div>
                <div className="col-md-6 mb-2">
                  <label>Transmission</label>
                  <select name="transmission" className="form-select" value={form.transmission} onChange={handleChange}>
                     <option value="0">Auto</option>
                     <option value="1">Manual</option>
                  </select>
                </div>
              </div>
              <div className="row">
                <div className="col-md-6 mb-2">
                  <label>Year</label>
                  <input name="year" type="number" className="form-control" value={form.year} onChange={handleChange} required />
                </div>
                <div className="col-md-6 mb-2">
                  <label>Price / Day</label>
                  <input name="pricePerDay" type="number" className="form-control" value={form.pricePerDay} onChange={handleChange} required />
                </div>
              </div>
              <div className="mb-2">
                <label>Location</label>
                <input name="location" className="form-control" value={form.location} onChange={handleChange} required />
              </div>
              <div className="mb-2">
                 <label>Description</label>
                 <textarea name="description" className="form-control" value={form.description} onChange={handleChange} rows="2"></textarea>
              </div>
              <div className="mb-2">
                <label>Image URL</label>
                <input name="imageUrl" className="form-control" value={form.imageUrl} onChange={handleChange} />
              </div>
              <div className="row">
                <div className="col-md-6 mb-2">
                  <label>Available From</label>
                  <input name="availableFrom" type="date" className="form-control" value={form.availableFrom} min={today} onChange={handleChange} required />
                </div>
                <div className="col-md-6 mb-3">
                  <label>Available To</label>
                  <input name="availableTo" type="date" className="form-control" value={form.availableTo} min={form.availableFrom} onChange={handleChange} required />
                </div>
              </div>
              <button className="btn btn-primary w-100 mt-2" disabled={loading}>
                {loading ? "Posting..." : "Post Car"}
              </button>
            </form>
          </div>
        </div>

        <div className="col-md-8">
          <h4>My Car Listings</h4>
          <div className="row">
            {myCars.length === 0 ? <p className="text-muted ms-3">You haven't posted any cars yet.</p> : (
              myCars.map(car => (
                <div className="col-md-6 mb-3" key={car.id}>
                  <div className="card h-100 shadow-sm">
                    <div className="card-body">
                      <h6 className="fw-bold">{car.title || `${car.brand} ${car.model}`}</h6>
                      <p className="small text-muted mb-1">{car.brand} {car.model} ({car.year})</p>
                      <p className="small text-muted mb-1">{car.location} | ${car.pricePerDay}/day</p>
                      <p className="small text-muted mb-1">Type: {['Sedan', 'SUV', 'Truck', 'Coupe', 'Convertible', 'Van', 'Other'][car.type || 0]}</p>
                      <p className="small text-muted mb-2">
                        Available: {formatDate(car.availableFrom)} to {formatDate(car.availableTo)}
                      </p>
                      <span className={`badge ${car.isApproved ? "bg-success" : "bg-warning text-dark"}`}>
                        {car.isApproved ? "Approved" : "Pending Approval"}
                      </span>
                    </div>
                  </div>
                </div>
              ))
            )}
          </div>

          <h4 className="mt-5 mb-3">Incoming Booking Requests</h4>
          <div className="card shadow-sm p-3">
            {bookings.length === 0 ? <p className="text-muted m-0">No incoming requests.</p> : (
              <div className="list-group list-group-flush">
                {bookings.map(b => (
                  <div key={b.id} className="list-group-item d-flex justify-content-between align-items-center py-3">
                    <div>
                      <h6 className="mb-0 fw-bold">{b.carBrand} {b.carModel} <span className="fw-normal">requested by</span> {b.renterName}</h6>
                      <small className="text-muted">
                        Dates: {formatDate(b.startDate)} to {formatDate(b.endDate)} • Total: ${b.totalPrice.toFixed(2)} • 
                        Status: <span className={`badge bg-${b.status === 'Pending' ? 'warning text-dark' : b.status === 'Accepted' ? 'success' : 'danger'}`}>{b.status}</span>
                      </small>
                    </div>
                    {b.status === 'Pending' && (
                      <div className="d-flex gap-2">
                        <button className="btn btn-sm btn-success rounded-pill px-3" onClick={() => handleAccept(b.id)}>Accept</button>
                        <button className="btn btn-sm btn-outline-danger rounded-pill px-3" onClick={() => handleReject(b.id)}>Reject</button>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Car Submission Success Modal */}
      {showSubmitModal && (
        <div className="modal show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content border-0 shadow-lg" style={{ borderRadius: '16px' }}>
              <div className="modal-body text-center p-5">
                <div style={{ fontSize: '3rem', marginBottom: '16px' }}>🎉</div>
                <h4 className="fw-bold mb-3" style={{ color: '#198754' }}>Listing Submitted!</h4>
                <p className="text-muted mb-4" style={{ fontSize: '0.95rem', lineHeight: '1.6' }}>
                  Your car listing has been submitted successfully and is currently under review by the admin.
                  You will be notified once it is approved.
                </p>
                <button
                  className="btn px-5 py-2 rounded-pill fw-semibold"
                  style={{ background: 'linear-gradient(45deg, #6a0dad, #9b30ff)', color: '#fff', border: 'none' }}
                  onClick={() => setShowSubmitModal(false)}
                >
                  Got It
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
