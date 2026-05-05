import { notificationAPI } from "./api";

const notificationService = {
  getMyNotifications: async () => {
    const response = await notificationAPI.get("/api/notifications");
    return response.data;
  },

  markAsRead: async (id) => {
    const response = await notificationAPI.post(`/api/notifications/${id}/read`);
    return response.data;
  },

  markAllAsRead: async () => {
    const response = await notificationAPI.post("/api/notifications/read-all");
    return response.data;
  }
};

export default notificationService;
