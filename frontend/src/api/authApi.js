import axiosClient from "./axiosClient";

export const authApi = {
  login: (data) => axiosClient.post("/auth/login", data),
  register: (data) => axiosClient.post("/auth/register", data),
  forgotPassword: (data) => axiosClient.post("/auth/forgot-password", data),
  resetPassword: (data) => axiosClient.post("/auth/reset-password", data),
  changePassword: (data) => axiosClient.post("/auth/change-password", data),
  updateProfile: (data) => axiosClient.put("/auth/profile", data)
};
