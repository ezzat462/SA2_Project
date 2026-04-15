import API from "./api";

const adminService = {
  getPendingUsers: async () => {
    const response = await API.get("/admin/users/pending");
    return response.data;
  },

  approveUser: async (id) => {
    const response = await API.put(`/admin/users/${id}/approve`);
    return response.data;
  },

  rejectUser: async (id) => {
    const response = await API.put(`/admin/users/${id}/reject`);
    return response.data;
  },

  getPendingCars: async () => {
    const response = await API.get("/admin/cars/pending");
    return response.data;
  },

  approveCar: async (id) => {
    const response = await API.put(`/admin/cars/${id}/approve`);
    return response.data;
  },

  rejectCar: async (id) => {
    const response = await API.put(`/admin/cars/${id}/reject`);
    return response.data;
  }
};

export default adminService;
