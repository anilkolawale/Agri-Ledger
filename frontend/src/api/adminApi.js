import axiosClient from "./axiosClient";

export const adminApi = {
  getStats: () => axiosClient.get("/admin/stats"),
  getUsers: () => axiosClient.get("/admin/users"),
  updateUserRole: (id, role) => axiosClient.put(`/admin/users/${id}/role`, { role }),
  deleteUser: (id) => axiosClient.delete(`/admin/users/${id}`)
};
