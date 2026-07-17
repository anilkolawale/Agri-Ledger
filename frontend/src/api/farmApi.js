import axiosClient from "./axiosClient";

export const farmApi = {
  getAll: () => axiosClient.get("/farms"),
  getById: (id) => axiosClient.get(`/farms/${id}`),
  create: (data) => axiosClient.post("/farms", data),
  update: (id, data) => axiosClient.put(`/farms/${id}`, data),
  remove: (id) => axiosClient.delete(`/farms/${id}`)
};
