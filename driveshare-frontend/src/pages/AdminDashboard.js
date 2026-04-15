import { useState, useEffect } from "react";
import adminService from "../services/adminService";

export default function AdminDashboard() {
  const [pendingUsers, setPendingUsers] = useState([]);
  const [pendingCars, setPendingCars] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const userRes = await adminService.getPendingUsers();
      const carRes = await adminService.getPendingCars();
      if (userRes.success) setPendingUsers(userRes.data);
      if (carRes.success) setPendingCars(carRes.data);
    } catch (error) {
      console.error("Failed to fetch admin data", error);
    } finally {
      setLoading(false);
    }
  };

  const handleApproveUser = async (id) => {
    if (await adminService.approveUser(id)) {
      setPendingUsers(pendingUsers.filter(u => u.id !== id));
    }
  };

  const handleApproveCar = async (id) => {
    if (await adminService.approveCar(id)) {
      setPendingCars(pendingCars.filter(c => c.id !== id));
    }
  };

  if (loading) return <div className="container mt-5">Loading...</div>;

  return (
    <div className="container mt-5">
      <h2 className="mb-4">Admin Dashboard</h2>

      <div className="row">
        <div className="col-md-6">
          <div className="card shadow-sm p-3 mb-4">
            <h4>Pending Licenses</h4>
            <hr />
            {pendingUsers.length === 0 ? <p className="text-muted">No pending users.</p> : (
              <ul className="list-group">
                {pendingUsers.map(user => (
                  <li key={user.id} className="list-group-item d-flex justify-content-between align-items-center">
                    <div>
                      <strong>{user.fullName}</strong> ({user.email})<br />
                      <a href={`http://localhost:5000${user.licenseImageUrl}`} target="_blank" rel="noreferrer">View License</a>
                    </div>
                    <div>
                      <button className="btn btn-success btn-sm me-2" onClick={() => handleApproveUser(user.id)}>Approve</button>
                    </div>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>

        <div className="col-md-6">
          <div className="card shadow-sm p-3 mb-4">
            <h4>Pending Cars</h4>
            <hr />
            {pendingCars.length === 0 ? <p className="text-muted">No pending cars.</p> : (
              <ul className="list-group">
                {pendingCars.map(car => (
                  <li key={car.id} className="list-group-item d-flex justify-content-between align-items-center">
                    <div>
                      <strong>{car.brand} {car.model}</strong> ({car.year})<br />
                      Owner: {car.ownerName} | ${car.pricePerDay}/day
                    </div>
                    <div>
                      <button className="btn btn-success btn-sm me-2" onClick={() => handleApproveCar(car.id)}>Approve</button>
                    </div>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
