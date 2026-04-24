import { useState, useEffect } from "react";
import adminService from "../services/adminService";
import licenseService from "../services/licenseService";
import { toast } from 'react-toastify';

export default function AdminDashboard() {
  const [pendingLicenses, setPendingLicenses] = useState([]);
  const [pendingCars, setPendingCars] = useState([]);
  const [pendingOwners, setPendingOwners] = useState([]);
  const [loading, setLoading] = useState(true);

  // Backend base URL for images
  const API_URL = "http://localhost:5243";

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const licenseRes = await licenseService.getPendingLicenses();
      const carRes = await adminService.getPendingCars();
      const ownerRes = await adminService.getPendingOwners();
      if (licenseRes.success) setPendingLicenses(licenseRes.data);
      if (carRes.success) setPendingCars(carRes.data);
      if (ownerRes.success) setPendingOwners(ownerRes.data);
    } catch (error) {
      console.error("Failed to fetch admin data", error);
    } finally {
      setLoading(false);
    }
  };

  const handleVerifyLicense = async (id) => {
    const res = await licenseService.verifyLicense(id);
    if (res.success) {
      toast.success("License verified successfully!");
      setPendingLicenses(pendingLicenses.filter(l => l.id !== id));
    } else {
      toast.error(res.message || "Failed to verify license");
    }
  };

  const handleRejectLicense = async (id) => {
    const res = await licenseService.rejectLicense(id);
    if (res.success) {
      setPendingLicenses(pendingLicenses.filter(l => l.id !== id));
    }
  };

  const handleApproveCar = async (id) => {
    const res = await adminService.approveCar(id);
    if (res.success) {
      toast.success("Car listing approved!");
      setPendingCars(pendingCars.filter(c => c.id !== id));
    } else {
      toast.error(res.message || "Failed to approve car");
    }
  };

  // 1. ADD: Handle Reject Car logic
  const handleRejectCar = async (id) => {
    if (window.confirm("Are you sure you want to REJECT and DELETE this car post?")) {
      const res = await adminService.rejectCar(id);
      if (res.success) {
        setPendingCars(pendingCars.filter(c => c.id !== id));
      }
    }
  };

  const handleApproveOwner = async (id) => {
    const res = await adminService.updateOwnerStatus(id, 1);
    if (res.success) {
      setPendingOwners(pendingOwners.filter(o => o.id !== id));
    }
  };

  const handleRejectOwner = async (id) => {
    const res = await adminService.updateOwnerStatus(id, 2);
    if (res.success) {
      setPendingOwners(pendingOwners.filter(o => o.id !== id));
    }
  };

  if (loading) return (
    <div className="container mt-5 text-center">
      <div className="spinner-border text-primary" role="status">
        <span className="visually-hidden">Loading...</span>
      </div>
    </div>
  );

  return (
    <div className="container mt-5 py-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
         <h2 className="fw-bold border-start border-primary border-4 ps-3">Admin Management Hub</h2>
         <span className="badge bg-primary rounded-pill px-3 py-2">System Administrator</span>
      </div>

      <div className="row g-4">
        {/* Pending Licenses */}
        <div className="col-lg-5">
          <div className="card shadow-sm border-0 rounded-4 overflow-hidden">
            <div className="card-header bg-white py-3">
              <h5 className="m-0 fw-bold"><i className="bi bi-person-badge-fill me-2"></i>Pending Licenses</h5>
            </div>
            <div className="card-body p-0">
              {pendingLicenses.length === 0 ? (
                <div className="text-center py-5 text-muted">No pending license verifications.</div>
              ) : (
                <div className="list-group list-group-flush">
                  {pendingLicenses.map(license => (
                    <div key={license.id} className="list-group-item p-3">
                      <div className="d-flex gap-3 align-items-center">
                        <div className="flex-shrink-0">
                           <img 
                             src={`${API_URL}${license.licenseImageUrl}`}
                             alt="License" 
                             className="rounded border"
                             style={{ width: "100px", height: "65px", objectFit: "cover" }}
                           />
                        </div>
                        <div className="flex-grow-1">
                          <div className="d-flex justify-content-between align-items-center mb-1">
                            <strong>{license.user?.fullName || "User " + license.userId}</strong>
                            <span className="badge bg-warning text-dark">Pending</span>
                          </div>
                          <p className="small text-muted mb-0">{license.user?.email}</p>
                        </div>
                      </div>
                      <div className="d-flex justify-content-end gap-2 mt-3">
                        <a href={`${API_URL}${license.licenseImageUrl}`} target="_blank" rel="noreferrer" className="btn btn-outline-primary btn-sm rounded-pill px-3">View</a>
                        <button className="btn btn-success btn-sm rounded-pill px-3" onClick={() => handleVerifyLicense(license.id)}>Verify</button>
                        <button className="btn btn-danger btn-sm rounded-pill px-3" onClick={() => handleRejectLicense(license.id)}>Reject</button>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Pending Cars */}
        <div className="col-lg-7">
          <div className="card shadow-sm border-0 rounded-4 overflow-hidden">
            <div className="card-header bg-white py-3">
              <h5 className="m-0 fw-bold"><i className="bi bi-car-front-fill me-2"></i>Pending Car Listings</h5>
            </div>
            <div className="card-body p-0">
              {pendingCars.length === 0 ? (
                <div className="text-center py-5 text-muted">All cars have been reviewed. Good job!</div>
              ) : (
                <div className="list-group list-group-flush">
                  {pendingCars.map(car => (
                    <div key={car.id} className="list-group-item p-3">
                      <div className="d-flex gap-3 align-items-center">
                        {/* 2. ADD: Car Image Preview */}
                        <div className="car-preview flex-shrink-0">
                          <img 
                            src={car.imageUrl || "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf"} 
                            alt={car.brand} 
                            className="rounded-3 shadow-sm border"
                            style={{ width: "100px", height: "70px", objectFit: "cover" }}
                          />
                        </div>
                        
                        <div className="flex-grow-1">
                          <div className="d-flex justify-content-between">
                            <h6 className="mb-1 fw-bold">{car.brand} {car.model} ({car.year})</h6>
                            <span className="text-primary fw-bold">${car.pricePerDay}/day</span>
                          </div>
                          <p className="small text-muted mb-0">Owner: {car.ownerName} | Location: {car.location}</p>
                        </div>
                        
                        <div className="d-flex flex-column gap-2 ms-auto">
                          <button className="btn btn-success btn-sm rounded-pill px-3" onClick={() => handleApproveCar(car.id)}>Approve</button>
                          
                          {/* 3. ADD: Reject Button (Styled Red) */}
                          <button className="btn btn-outline-danger btn-sm rounded-pill px-3" onClick={() => handleRejectCar(car.id)}>Reject</button>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      </div>

      <div className="row g-4 mt-2">
        {/* Pending Car Owners */}
        <div className="col-lg-12">
          <div className="card shadow-sm border-0 rounded-4 overflow-hidden">
            <div className="card-header bg-white py-3">
              <h5 className="m-0 fw-bold"><i className="bi bi-briefcase-fill me-2"></i>Pending Car Owner Accounts</h5>
            </div>
            <div className="card-body p-0">
              {pendingOwners.length === 0 ? (
                <div className="text-center py-5 text-muted">No pending car owner approvals.</div>
              ) : (
                <div className="list-group list-group-flush">
                  {pendingOwners.map(owner => (
                    <div key={owner.id} className="list-group-item p-3 d-flex justify-content-between align-items-center">
                      <div>
                        <h6 className="mb-1 fw-bold">{owner.fullName}</h6>
                        <p className="small text-muted mb-0">{owner.email}</p>
                      </div>
                      <div className="d-flex gap-2">
                        <button className="btn btn-success btn-sm rounded-pill px-3" onClick={() => handleApproveOwner(owner.id)}>Approve Check</button>
                        <button className="btn btn-outline-danger btn-sm rounded-pill px-3" onClick={() => handleRejectOwner(owner.id)}>Reject</button>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
