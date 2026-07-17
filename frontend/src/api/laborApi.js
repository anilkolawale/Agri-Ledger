import axiosClient from "./axiosClient";

export const laborApi = {
  getAll: (params) => axiosClient.get("/labor", { params }),
  getById: (id) => axiosClient.get(`/labor/${id}`),
  create: (data) => axiosClient.post("/labor", data),
  update: (id, data) => axiosClient.put(`/labor/${id}`, data),
  remove: (id) => axiosClient.delete(`/labor/${id}`)
};
