import axiosClient from "./axiosClient";

export const inventoryApi = {
  getAll: (farmId) => axiosClient.get("/inventory", { params: farmId ? { farmId } : {} }),
  getById: (id) => axiosClient.get(`/inventory/${id}`),
  create: (data) => axiosClient.post("/inventory", data),
  update: (id, data) => axiosClient.put(`/inventory/${id}`, data),
  adjust: (id, quantityDelta) => axiosClient.post(`/inventory/${id}/adjust`, { quantityDelta }),
  remove: (id) => axiosClient.delete(`/inventory/${id}`)
};
