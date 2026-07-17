import axiosClient from "./axiosClient";

export const mandiApi = {
  getPrices: (market, crop) => {
    const params = {};
    if (market) params.market = market;
    if (crop) params.crop = crop;
    return axiosClient.get("/mandi/prices", { params });
  },
  getMarkets: () => axiosClient.get("/mandi/markets"),
  getCrops: () => axiosClient.get("/mandi/crops")
};
