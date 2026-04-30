import axios from 'axios';

const createInstance = (baseURL) => {
  const instance = axios.create({
    baseURL: baseURL,
    headers: {
      'Content-Type': 'application/json',
    },
  });

  // Request Interceptor for Authentication
  instance.interceptors.request.use(
    (config) => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    },
    (error) => Promise.reject(error)
  );

  // Response Interceptor for Global Error Handling
  instance.interceptors.response.use(
    (response) => response.data, // Simplify response data access
    (error) => {
      if (error.response && error.response.status === 401) {
        // Clear auth and redirect on unauthorized (except for login/register)
        if (!error.config.url.includes('/auth/')) {
          localStorage.removeItem('token');
          window.location.href = '/login';
        }
      }
      return Promise.reject(error.response?.data || error.message);
    }
  );

  return instance;
};

export const userApiClient = createInstance(process.env.REACT_APP_USER_SERVICE_URL);
export const rentalApiClient = createInstance(process.env.REACT_APP_RENTAL_SERVICE_URL);
