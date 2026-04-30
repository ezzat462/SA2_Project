import api from "./api";

const licenseService = {
  uploadLicense: async (file) => {
    const formData = new FormData();
    formData.append("file", file);
    const response = await api.post("/license/upload", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    return response.data;
  },

  getMyLicense: async () => {
    const response = await api.get("/license/my");
    return response.data;
  },

  getPendingLicenses: async () => {
    const response = await api.get("/admin/license/pending");
    return response.data;
  },

  verifyLicense: async (id) => {
    const response = await api.put(`/admin/license/${id}/verify`);
    return response.data;
  },

  rejectLicense: async (id) => {
    const response = await api.put(`/admin/license/${id}/reject`);
    return response.data;
  },
};

export default licenseService;
