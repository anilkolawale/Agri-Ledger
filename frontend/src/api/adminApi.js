import axios from "axios";

const API_URL = process.env.REACT_APP_API_URL || "http://localhost:5000/api";

const getHeaders = () => ({
  headers: {
    Authorization: `Bearer ${localStorage.getItem("agriledger_token")}`
  }
});

export const adminApi = {
  getStats: () => axios.get(`${API_URL}/admin/stats`, getHeaders()),
  getUsers: () => axios.get(`${API_URL}/admin/users`, getHeaders()),
  updateUserRole: (id, role) => axios.put(`${API_URL}/admin/users/${id}/role`, { role }, getHeaders()),
  deleteUser: (id) => axios.delete(`${API_URL}/admin/users/${id}`, getHeaders())
};
