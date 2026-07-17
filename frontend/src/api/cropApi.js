import axiosClient from "./axiosClient";

export const cropApi = {
  getAll: (farmId) => axiosClient.get("/crops", { params: farmId ? { farmId } : {} }),
  getById: (id) => axiosClient.get(`/crops/${id}`),
  create: (data) => axiosClient.post("/crops", data),
  update: (id, data) => axiosClient.put(`/crops/${id}`, data),
  remove: (id) => axiosClient.delete(`/crops/${id}`)
};
