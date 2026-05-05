import { rentalAPI as API } from "./api";

const carService = {
  getAll: async (params = {}) => {
    const response = await API.get("/cars", { params });
    return response.data;
  },

  getById: async (id) => {
    const response = await API.get(`/cars/${id}`);
    return response.data;
  },

  create: async (carData) => {
    const response = await API.post("/cars", carData);
    return response.data;
  },

  update: async (id, carData) => {
    const response = await API.put(`/cars/${id}`, carData);
    return response.data;
  },

  getMyCars: async () => {
    const response = await API.get("/cars/my-cars");
    return response.data;
  }
};

export default carService;
