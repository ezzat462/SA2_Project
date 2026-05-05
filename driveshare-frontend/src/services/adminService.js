import { userAPI, rentalAPI } from "./api";

const adminService = {
  // UserService Actions (Port 5001)
  getPendingUsers: async () => {
    const response = await userAPI.get("/admin/users/pending");
    return response.data;
  },

  getPendingOwners: async () => {
    const response = await userAPI.get("/admin/owners/pending");
    return response.data;
  },

  updateOwnerStatus: async (id, status) => {
    const response = await userAPI.put(`/admin/owners/${id}/status`, status, {
        headers: { 'Content-Type': 'application/json' }
    });
    return response.data;
  },

  approveUser: async (id) => {
    const response = await userAPI.put(`/admin/users/${id}/approve`);
    return response.data;
  },

  rejectUser: async (id) => {
    const response = await userAPI.put(`/admin/users/${id}/reject`);
    return response.data;
  },

  // RentalService Actions (Port 5002)
  getPendingCars: async () => {
    const response = await rentalAPI.get("/cars/admin/pending");
    return response.data;
  },

  approveCar: async (id) => {
    const response = await rentalAPI.put(`/cars/admin/${id}/approve`);
    return response.data;
  },

  rejectCar: async (id) => {
    const response = await rentalAPI.put(`/cars/admin/${id}/reject`);
    return response.data;
  }
};

export default adminService;
