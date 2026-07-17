import axios from "axios";

const axiosClient = axios.create({
  baseURL:
    process.env.REACT_APP_API_BASE_URL ||
    "https://agri-ledger.onrender.com/api",
});

// Attach JWT token
axiosClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("agriledger_token");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

// Handle unauthorized responses
axiosClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("agriledger_token");
      window.location.href = "/login";
    }

    return Promise.reject(error);
  }
);

export default axiosClient;