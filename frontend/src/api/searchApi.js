import axiosClient from "./axiosClient";

export const searchApi = {
  search: (q) => axiosClient.get("/search", { params: { q } })
};
