import API from "./api";

const notificationService = {
  getMyNotifications: async () => {
    const response = await API.get("/notifications");
    return response.data;
  },

  markAsRead: async (id) => {
    const response = await API.post(`/notifications/${id}/read`);
    return response.data;
  },

  markAllAsRead: async () => {
    const response = await API.post("/notifications/read-all");
    return response.data;
  }
};

export default notificationService;
