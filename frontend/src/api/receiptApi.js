import axiosClient from "./axiosClient";

export const receiptApi = {
  uploadForExpense: (expenseId, file) => {
    const form = new FormData();
    form.append("file", file);
    return axiosClient.post(`/receipts/expense/${expenseId}`, form, { headers: { "Content-Type": "multipart/form-data" } });
  },
  uploadForIncome: (incomeId, file) => {
    const form = new FormData();
    form.append("file", file);
    return axiosClient.post(`/receipts/income/${incomeId}`, form, { headers: { "Content-Type": "multipart/form-data" } });
  },
  getForExpense: (expenseId) => axiosClient.get(`/receipts/expense/${expenseId}`),
  getForIncome: (incomeId) => axiosClient.get(`/receipts/income/${incomeId}`),
  remove: (id) => axiosClient.delete(`/receipts/${id}`)
};
