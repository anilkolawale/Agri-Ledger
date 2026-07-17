import React, { useEffect, useState } from "react";
import { Box, Typography, Card, CardContent, Grid, FormControl, InputLabel, Select, MenuItem, CircularProgress, Stack, Paper, TableContainer, Table, TableHead, TableRow, TableCell, TableBody } from "@mui/material";
import StorefrontIcon from "@mui/icons-material/Storefront";
import TrendingUpIcon from "@mui/icons-material/TrendingUp";
import { useTranslation } from "react-i18next";
import { mandiApi } from "../../api/mandiApi";

// Mapping crop name to localized crop names and emojis
const CROP_METADATA = {
  Grapes: { emoji: "🍇", localizedMr: "द्राक्षे" },
  Pomegranate: { emoji: "🍎", localizedMr: "डाळिंब" },
  Banana: { emoji: "🍌", localizedMr: "केळी" },
  Drumstick: { emoji: "🌿", localizedMr: "शेवगा" },
  Sugarcane: { emoji: "🎋", localizedMr: "ऊस" },
  Ginger: { emoji: "🫚", localizedMr: "आले" }
};


const getCropDisplay = (cropName, lang) => {
  const meta = CROP_METADATA[cropName] || { emoji: "🌱", localizedMr: cropName };
  return {
    emoji: meta.emoji,
    name: lang === "mr" ? meta.localizedMr : cropName
  };
};

const marketDisplay = (marketName, lang) => {
  if (lang === "mr") {
    return marketName
      .replace("Nashik APMC", "नाशिक कृषी उत्पन्न बाजार समिती")
      .replace("Pune APMC", "पुणे कृषी उत्पन्न बाजार समिती")
      .replace("Mumbai APMC", "मुंबई कृषी उत्पन्न बाजार समिती")
      .replace("Nagpur APMC", "नागपूर कृषी उत्पन्न बाजार समिती");
  }
  return marketName;
};

export default function MandiPricesPage() {
  const { t, i18n } = useTranslation();
  const isMarathi = i18n.language === "mr";

  const [prices, setPrices] = useState([]);
  const [markets, setMarkets] = useState([]);
  const [crops, setCrops] = useState([]);

  const [selectedMarket, setSelectedMarket] = useState("");
  const [selectedCrop, setSelectedCrop] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Load initial dropdown options
    Promise.all([mandiApi.getMarkets(), mandiApi.getCrops()])
      .then(([resMarkets, resCrops]) => {
        setMarkets(resMarkets.data.data);
        setCrops(resCrops.data.data);
      })
      .catch((err) => console.error("Error loading dropdown data:", err));
  }, []);

  const loadPrices = () => {
    setLoading(true);
    mandiApi.getPrices(selectedMarket, selectedCrop)
      .then(({ data }) => setPrices(data.data))
      .catch((err) => console.error("Error fetching prices:", err))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    loadPrices();
  }, [selectedMarket, selectedCrop]);

  return (
    <Box sx={{ pb: 4 }}>
      <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 1 }}>
        <StorefrontIcon color="primary" sx={{ fontSize: "2rem" }} />
        <Typography variant="h1">{isMarathi ? "बाजार भाव (मंडी दर)" : "Mandi Crop Prices"}</Typography>
      </Stack>
      <Typography color="text.secondary" sx={{ mb: 3 }}>
        {isMarathi 
          ? "महाराष्ट्रातील प्रमुख कृषी उत्पन्न बाजार समित्यांमधील (APMC) पिकांचे दैनिक भाव पहा." 
          : "View daily market prices for major crops across APMC markets in Maharashtra."}
      </Typography>

      {/* Filter Bar */}
      <Paper sx={{ p: 2, mb: 3, borderRadius: 2 }}>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6}>
            <FormControl fullWidth size="small">
              <InputLabel id="market-filter-label">{isMarathi ? "बाजार समिती निवडा" : "Select Market"}</InputLabel>
              <Select
                labelId="market-filter-label"
                value={selectedMarket}
                label={isMarathi ? "बाजार समिती निवडा" : "Select Market"}
                onChange={(e) => setSelectedMarket(e.target.value)}
              >
                <MenuItem value=""><em>{isMarathi ? "सर्व बाजार" : "All Markets"}</em></MenuItem>
                {markets.map((m) => (
                  <MenuItem key={m} value={m}>{marketDisplay(m, i18n.language)}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} sm={6}>
            <FormControl fullWidth size="small">
              <InputLabel id="crop-filter-label">{isMarathi ? "पीक निवडा" : "Select Crop"}</InputLabel>
              <Select
                labelId="crop-filter-label"
                value={selectedCrop}
                label={isMarathi ? "पीक निवडा" : "Select Crop"}
                onChange={(e) => setSelectedCrop(e.target.value)}
              >
                <MenuItem value=""><em>{isMarathi ? "सर्व पिके" : "All Crops"}</em></MenuItem>
                {crops.map((c) => (
                  <MenuItem key={c} value={c}>{getCropDisplay(c, i18n.language).name}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
        </Grid>
      </Paper>

      {/* Prices Content */}
      {loading ? (
        <Box sx={{ display: "flex", justifyContent: "center", mt: 6 }}><CircularProgress /></Box>
      ) : (
        <Grid container spacing={2}>
          {prices.length === 0 ? (
            <Grid item xs={12}>
              <Paper sx={{ p: 4, textAlign: "center" }}>
                <Typography color="text.secondary">
                  {isMarathi ? "कोणतेही दर सापडले नाहीत." : "No prices found matching filters."}
                </Typography>
              </Paper>
            </Grid>
          ) : (
            prices.map((p) => {
              const cropInfo = getCropDisplay(p.cropName, i18n.language);
              const displayMarket = marketDisplay(p.marketName, i18n.language);
              return (
                <Grid item xs={12} sm={6} md={4} key={p.id}>
                  <Card sx={{ 
                    borderRadius: 2, 
                    boxShadow: "0 2px 8px rgba(0,0,0,0.08)", 
                    borderLeft: "5px solid #2e7d32",
                    "&:hover": { boxShadow: "0 4px 16px rgba(0,0,0,0.12)", transform: "translateY(-2px)" },
                    transition: "all 0.2s ease"
                  }}>
                    <CardContent>
                      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", mb: 1.5 }}>
                        <Box>
                          <Typography variant="h6" sx={{ display: "flex", alignItems: "center", gap: 1, fontWeight: "bold" }}>
                            <span style={{ fontSize: "1.4rem" }}>{cropInfo.emoji}</span> {cropInfo.name}
                          </Typography>
                          <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
                            {displayMarket}
                          </Typography>
                        </Box>
                        <Typography variant="caption" sx={{ bgcolor: "grey.100", p: "4px 8px", borderRadius: 1, color: "text.secondary" }}>
                          {new Date(p.priceDate).toLocaleDateString(isMarathi ? "mr-IN" : "en-US", { month: "short", day: "numeric" })}
                        </Typography>
                      </Box>

                      <Grid container spacing={1} sx={{ bgcolor: "grey.50", p: 1, borderRadius: 1.5, mb: 1 }}>
                        <Grid item xs={4} sx={{ textAlign: "center" }}>
                          <Typography variant="caption" color="text.secondary" display="block">
                            {isMarathi ? "किमान दर" : "Min Price"}
                          </Typography>
                          <Typography variant="body2" sx={{ fontWeight: "bold", color: "warning.dark" }}>
                            ₹{p.minPrice}
                          </Typography>
                        </Grid>
                        <Grid item xs={4} sx={{ textAlign: "center", borderLeft: "1px solid #e0e0e0", borderRight: "1px solid #e0e0e0" }}>
                          <Typography variant="caption" color="text.secondary" display="block">
                            {isMarathi ? "सरासरी दर" : "Modal Rate"}
                          </Typography>
                          <Typography variant="subtitle2" sx={{ fontWeight: "bold", color: "primary.main" }}>
                            ₹{p.modalPrice}
                          </Typography>
                        </Grid>
                        <Grid item xs={4} sx={{ textAlign: "center" }}>
                          <Typography variant="caption" color="text.secondary" display="block">
                            {isMarathi ? "कमाल दर" : "Max Price"}
                          </Typography>
                          <Typography variant="body2" sx={{ fontWeight: "bold", color: "success.dark" }}>
                            ₹{p.maxPrice}
                          </Typography>
                        </Grid>
                      </Grid>

                      <Typography variant="caption" color="text.secondary" sx={{ display: "flex", justifySelf: "flex-end", gap: 0.5, alignItems: "center" }}>
                        <TrendingUpIcon sx={{ fontSize: "0.9rem" }} />
                        {isMarathi ? `दर प्रति ${p.unit === "kg" ? "किलो" : p.unit}` : `Rates are per ${p.unit}`}
                      </Typography>

                    </CardContent>
                  </Card>
                </Grid>
              );
            })
          )}
        </Grid>
      )}
    </Box>
  );
}
