import axiosClient from "./axiosClient";

export const expenseApi = {
  getPaged: (params) => axiosClient.get("/expenses", { params }),
  getById: (id) => axiosClient.get(`/expenses/${id}`),
  create: (data) => axiosClient.post("/expenses", data),
  update: (id, data) => axiosClient.put(`/expenses/${id}`, data),
  remove: (id) => axiosClient.delete(`/expenses/${id}`),
  getCategories: () => axiosClient.get("/expenses/categories"),
  createCategory: (name) => axiosClient.post("/expenses/categories", JSON.stringify(name), {
    headers: { "Content-Type": "application/json" }
  })
};
