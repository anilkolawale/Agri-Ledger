import axiosClient from "./axiosClient";

export const reportsApi = {
  expenses: (params) => axiosClient.get("/reports/expenses", { params }),
  income: (params) => axiosClient.get("/reports/income", { params }),
  cropProfit: (params) => axiosClient.get("/reports/crop-profit", { params }),
  categoryExpense: (params) => axiosClient.get("/reports/category-expense", { params }),
  farmExpense: (params) => axiosClient.get("/reports/farm-expense", { params }),
  farmIncome: (params) => axiosClient.get("/reports/farm-income", { params }),
  laborPayments: (params) => axiosClient.get("/reports/labor-payments", { params }),
  inventory: () => axiosClient.get("/reports/inventory"),
  profitAndLoss: (params) => axiosClient.get("/reports/profit-loss", { params }),
  export: (reportType, format, params) =>
    axiosClient.get(`/reports/export/${reportType}`, { params: { ...params, format }, responseType: "blob" })
};
