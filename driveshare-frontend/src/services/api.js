import axios from "axios";

const API = axios.create({
  baseURL: "http://localhost:5243/api",
});

// Request Interceptor: Attach JWT token to headers if it exists
API.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response Interceptor: Handle global errors like 401 Unauthorized
API.interceptors.response.use(
  (response) => response,
  (error) => {
    const isAuthEndpoint = error?.config?.url?.includes("/auth/");
    if (error?.response && error.response.status === 401 && !isAuthEndpoint) {
      localStorage.removeItem("token");
      window.location.href = "/login";
    }
    
    // Safely swallow errors to prevent Uncaught Runtime Errors
    return Promise.resolve({
      data: {
        success: false,
        message: error.response?.data?.message || error.message || "An unexpected network error occurred."
      }
    });
  }
);

export default API;