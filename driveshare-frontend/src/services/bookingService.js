import { rentalAPI as api } from "./api";

const bookingService = {
  createBooking: async (bookingData) => {
    const response = await api.post("/bookings", bookingData);
    return response.data;
  },

  acceptBooking: async (id) => {
    const response = await api.put(`/bookings/${id}/accept`);
    return response.data;
  },

  rejectBooking: async (id) => {
    const response = await api.put(`/bookings/${id}/reject`);
    return response.data;
  },

  getMyBookings: async () => {
    const response = await api.get("/bookings/my");
    return response.data;
  },

  getIncomingBookings: async () => {
    const response = await api.get("/bookings/incoming");
    return response.data;
  },
};

export default bookingService;
