import axios from "axios";

/**
 * Creates a configured Axios instance with shared interceptors
 * @param {string} baseURL 
 */
const createAPIInstance = (baseURL) => {
  const instance = axios.create({
    baseURL: baseURL,
  });

  // Request Interceptor: Attach JWT token to headers if it exists
  instance.interceptors.request.use(
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

  // Response Interceptor: Handle global errors and standardize error format
  instance.interceptors.response.use(
    (response) => response,
    (error) => {
      const isAuthEndpoint = error?.config?.url?.includes("/auth/");
      
      // Redirect to login on 401 Unauthorized (except for auth endpoints)
      if (error?.response && error.response.status === 401 && !isAuthEndpoint) {
        localStorage.removeItem("token");
        window.location.href = "/login";
      }
      
      // Return a standard error object instead of throwing to prevent crashes
      return Promise.resolve({
        data: {
          success: false,
          message: error.response?.data?.message || error.message || "An unexpected network error occurred."
        }
      });
    }
  );

  return instance;
};

// Export specialized instances for each microservice
export const userAPI = createAPIInstance(process.env.REACT_APP_USER_SERVICE_URL || "http://localhost:5001/api");
export const rentalAPI = createAPIInstance(process.env.REACT_APP_RENTAL_SERVICE_URL || "http://localhost:5002/api");
export const notificationAPI = createAPIInstance(process.env.REACT_APP_NOTIFICATION_SERVICE_URL || "http://localhost:5003");

// Default export remains the User API for backward compatibility where generic 'api' is imported
export default userAPI;