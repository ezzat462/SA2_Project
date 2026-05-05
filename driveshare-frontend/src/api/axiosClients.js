import axios from 'axios';

/**
 * Helper function to create an Axios instance with predefined configurations
 * @param {string} baseURL - The base URL for the service
 * @returns {AxiosInstance}
 */
const createInstance = (baseURL) => {
  const instance = axios.create({
    baseURL: baseURL,
    headers: {
      'Content-Type': 'application/json',
    },
  });

  // Request Interceptor: Automatically attach JWT Token to every request
  instance.interceptors.request.use(
    (config) => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    },
    (error) => {
      return Promise.reject(error);
    }
  );

  // Response Interceptor: Global error handling and data extraction
  instance.interceptors.response.use(
    (response) => {
      // Return the data directly to simplify usage in components/services
      return response.data;
    },
    (error) => {
      // Handle 401 Unauthorized (e.g., token expired)
      if (error.response && error.response.status === 401) {
        // Only redirect if not on login/register pages to avoid loops
        if (!window.location.pathname.includes('/login') && !window.location.pathname.includes('/register')) {
          localStorage.removeItem('token');
          window.location.href = '/login';
        }
      }
      
      // Return a rejected promise with the error message from the backend
      const errorMessage = error.response?.data?.message || 'Something went wrong';
      return Promise.reject(errorMessage);
    }
  );

  return instance;
};

// Export specialized clients for each Microservice
export const userApiClient = createInstance(process.env.REACT_APP_USER_SERVICE_URL);
export const rentalApiClient = createInstance(process.env.REACT_APP_RENTAL_SERVICE_URL);
