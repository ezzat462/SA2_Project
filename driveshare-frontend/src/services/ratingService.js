import { rentalAPI as api } from "./api";

const ratingService = {
  createRating: async (ratingData) => {
    const response = await api.post("/ratings", ratingData);
    return response.data;
  },

  getRatingsForCar: async (carId) => {
    const response = await api.get(`/cars/${carId}/ratings`);
    return response.data;
  },
};

export default ratingService;
