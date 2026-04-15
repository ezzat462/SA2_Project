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
    const isAuthEndpoint = error.config?.url?.includes("/auth/");
    if (error.response && error.response.status === 401 && !isAuthEndpoint) {
      // Only clear token and redirect if it's NOT a login/register call.
      // Auth endpoints return 401 on bad credentials — we let the caller handle that.
      localStorage.removeItem("token");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

export default API;