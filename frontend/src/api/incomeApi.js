import axiosClient from "./axiosClient";

export const incomeApi = {
  getPaged: (params) => axiosClient.get("/incomes", { params }),
  getById: (id) => axiosClient.get(`/incomes/${id}`),
  create: (data) => axiosClient.post("/incomes", data),
  update: (id, data) => axiosClient.put(`/incomes/${id}`, data),
  remove: (id) => axiosClient.delete(`/incomes/${id}`)
};
