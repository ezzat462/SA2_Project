import { useState, useEffect } from "react";
import carService from "../services/carService";

export default function OwnerDashboard() {
  const today = new Date().toISOString().split("T")[0];
  const [myCars, setMyCars] = useState([]);
  const [form, setForm] = useState({
    brand: "",
    model: "",
    year: new Date().getFullYear(),
    pricePerDay: "",
    location: "",
    description: "",
    imageUrl: "",
    availableFrom: today,
    availableTo: ""
  });
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchMyCars();
  }, []);

  const fetchMyCars = async () => {
    const res = await carService.getMyCars();
    if (res.success) setMyCars(res.data);
  };

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (new Date(form.availableTo) <= new Date(form.availableFrom)) {
      alert("'Available To' date must be after 'Available From' date");
      return;
    }
    try {
      setLoading(true);
      const res = await carService.create(form);
      if (res.success) {
        alert("Car posted! Waiting for admin approval.");
        setForm({
          brand: "", model: "", year: new Date().getFullYear(),
          pricePerDay: "", location: "", description: "", imageUrl: "",
          availableFrom: today, availableTo: ""
        });
        fetchMyCars();
      }
    } catch (error) {
      alert(error.response?.data?.message || "Failed to post car");
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateStr) => {
    if (!dateStr) return "N/A";
    return new Date(dateStr).toLocaleDateString();
  };

  return (
    <div className="container mt-5">
      <h2>Owner Dashboard</h2>
      
      <div className="row mt-4">
        <div className="col-md-4">
          <div className="card p-3 shadow-sm">
            <h4>Post a New Car</h4>
            <form onSubmit={handleSubmit}>
              <div className="mb-2">
                <label>Brand</label>
                <input name="brand" className="form-control" value={form.brand} onChange={handleChange} required />
              </div>
              <div className="mb-2">
                <label>Model</label>
                <input name="model" className="form-control" value={form.model} onChange={handleChange} required />
              </div>
              <div className="mb-2">
                <label>Year</label>
                <input name="year" type="number" className="form-control" value={form.year} onChange={handleChange} required />
              </div>
              <div className="mb-2">
                <label>Price / Day</label>
                <input name="pricePerDay" type="number" className="form-control" value={form.pricePerDay} onChange={handleChange} required />
              </div>
              <div className="mb-2">
                <label>Location</label>
                <input name="location" className="form-control" value={form.location} onChange={handleChange} required />
              </div>
              <div className="mb-2">
                <label>Image URL</label>
                <input name="imageUrl" className="form-control" value={form.imageUrl} onChange={handleChange} />
              </div>
              <div className="mb-2">
                <label>Available From</label>
                <input name="availableFrom" type="date" className="form-control" value={form.availableFrom} min={today} onChange={handleChange} required />
              </div>
              <div className="mb-3">
                <label>Available To</label>
                <input name="availableTo" type="date" className="form-control" value={form.availableTo} min={form.availableFrom} onChange={handleChange} required />
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
                      <h6>{car.brand} {car.model} ({car.year})</h6>
                      <p className="small text-muted mb-1">{car.location} | ${car.pricePerDay}/day</p>
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
        </div>
      </div>
    </div>
  );
}
