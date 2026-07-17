import React, { useEffect, useState } from "react";
import { Card, CardContent, Typography, Box, Grid, CircularProgress, Button, IconButton } from "@mui/material";
import MyLocationIcon from "@mui/icons-material/MyLocation";
import AirIcon from "@mui/icons-material/Air";
import WaterDropIcon from "@mui/icons-material/WaterDrop";
import GrainIcon from "@mui/icons-material/Grain";
import { useTranslation } from "react-i18next";
import axios from "axios";

// Open-Meteo Weather Codes Mapping
const WEATHER_MAPPING = {
  0: { labelEn: "Clear Sky", labelMr: "स्वच्छ आकाश", icon: "☀️" },
  1: { labelEn: "Mainly Clear", labelMr: "मुख्यतः स्वच्छ", icon: "🌤️" },
  2: { labelEn: "Partly Cloudy", labelMr: "अंशतः ढगाळ", icon: "⛅" },
  3: { labelEn: "Overcast", labelMr: "पूर्णपणे ढगाळ", icon: "☁️" },
  45: { labelEn: "Foggy", labelMr: "धुके", icon: "🌫️" },
  48: { labelEn: "Depositing Rime Fog", labelMr: "हिम धुके", icon: "🌫️" },
  51: { labelEn: "Light Drizzle", labelMr: "हलकी रिमझिम", icon: "🌧️" },
  53: { labelEn: "Moderate Drizzle", labelMr: "मध्यम रिमझिम", icon: "🌧️" },
  55: { labelEn: "Heavy Drizzle", labelMr: "जोरदार रिमझिम", icon: "🌧️" },
  61: { labelEn: "Light Rain", labelMr: "हलका पाऊस", icon: "🌧️" },
  63: { labelEn: "Moderate Rain", labelMr: "मध्यम पाऊस", icon: "🌦️" },
  65: { labelEn: "Heavy Rain", labelMr: "मुसळधार पाऊस", icon: "⛈️" },
  80: { labelEn: "Light Rain Showers", labelMr: "हलक्या पावसाच्या सरी", icon: "🌦️" },
  81: { labelEn: "Moderate Rain Showers", labelMr: "मध्यम पावसाच्या सरी", icon: "🌦️" },
  82: { labelEn: "Violent Rain Showers", labelMr: "अति मुसळधार पाऊस", icon: "⛈️" },
  95: { labelEn: "Thunderstorm", labelMr: "वादळी पाऊस", icon: "⛈️" },
  96: { labelEn: "Thunderstorm with Hail", labelMr: "गारांसह वादळी पाऊस", icon: "⛈️" },
  99: { labelEn: "Severe Thunderstorm with Hail", labelMr: "गारांसह तीव्र वादळी पाऊस", icon: "⛈️" }
};

const getWeatherInfo = (code, lang) => {
  const match = WEATHER_MAPPING[code] || { labelEn: "Cloudy", labelMr: "ढगाळ", icon: "☁️" };
  return {
    label: lang === "mr" ? match.labelMr : match.labelEn,
    icon: match.icon
  };
};

export default function WeatherWidget() {
  const { i18n } = useTranslation();
  const [weather, setWeather] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [coords, setCoords] = useState({ latitude: 19.9975, longitude: 73.7898 }); // Default Nashik, Maharashtra
  const [locationName, setLocationName] = useState("Nashik, MH");

  const fetchWeather = (lat, lon) => {
    setLoading(true);
    setError(null);
    const url = `https://api.open-meteo.com/v1/forecast?latitude=${lat}&longitude=${lon}&current=temperature_2m,relative_humidity_2m,apparent_temperature,precipitation,weather_code,wind_speed_10m&daily=weather_code,temperature_2m_max,temperature_2m_min&timezone=auto`;
    
    axios.get(url)
      .then((res) => {
        setWeather(res.data);
      })
      .catch((err) => {
        console.error("Error fetching weather:", err);
        setError(i18n.language === "mr" ? "हवामान डेटा लोड करण्यात अक्षम." : "Unable to load weather data.");
      })
      .finally(() => {
        setLoading(false);
      });
  };

  useEffect(() => {
    fetchWeather(coords.latitude, coords.longitude);
      // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [coords]);

  const requestLocation = () => {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (pos) => {
          setCoords({
            latitude: pos.coords.latitude,
            longitude: pos.coords.longitude
          });
          setLocationName(i18n.language === "mr" ? "माझे स्थान" : "My Location");
        },
        (err) => {
          console.warn("Geolocation error, using default Nashik coords", err);
        }
      );
    }
  };

  const getDayName = (dateStr) => {
    const date = new Date(dateStr);
    const options = { weekday: "short" };
    return date.toLocaleDateString(i18n.language === "mr" ? "mr-IN" : "en-US", options);
  };

  if (loading && !weather) {
    return (
      <Card sx={{ borderRadius: 3, mb: 2, display: "flex", justifyContent: "center", alignItems: "center", minHeight: 180 }}>
        <CircularProgress />
      </Card>
    );
  }

  if (error && !weather) {
    return (
      <Card sx={{ borderRadius: 3, mb: 2 }}>
        <CardContent>
          <Typography color="error">{error}</Typography>
          <Button startIcon={<MyLocationIcon />} onClick={requestLocation} sx={{ mt: 1 }}>
            {i18n.language === "mr" ? "पुन्हा प्रयत्न करा" : "Try Again"}
          </Button>
        </CardContent>
      </Card>
    );
  }

  const current = weather.current;
  const currentInfo = getWeatherInfo(current.weather_code, i18n.language);
  const isMarathi = i18n.language === "mr";

  return (
    <Card sx={{ 
      borderRadius: 3, 
      mb: 3, 
      background: "linear-gradient(135deg, #1e3c72 0%, #2a5298 100%)", 
      color: "#fff",
      boxShadow: "0 8px 32px 0 rgba(31, 38, 135, 0.2)"
    }}>
      <CardContent>
        <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
          <Box>
            <Typography variant="subtitle2" sx={{ opacity: 0.8, fontWeight: "bold" }}>
              {isMarathi ? "हवामान अंदाज" : "WEATHER FORECAST"}
            </Typography>
            <Typography variant="h6" sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
              {locationName}
              <IconButton size="small" onClick={requestLocation} sx={{ color: "#fff" }} title={isMarathi ? "माझे स्थान मिळवा" : "Get My Location"}>
                <MyLocationIcon fontSize="inherit" />
              </IconButton>
            </Typography>
          </Box>
          <Typography variant="h3" sx={{ fontWeight: "bold", display: "flex", alignSelf: "flex-start" }}>
            {Math.round(current.temperature_2m)}°C
          </Typography>
        </Box>

        <Box sx={{ display: "flex", alignItems: "center", gap: 1, mt: 1, mb: 2 }}>
          <Typography variant="h4" component="span" sx={{ lineHeight: 1 }}>{currentInfo.icon}</Typography>
          <Typography variant="body1" sx={{ fontWeight: 500 }}>
            {currentInfo.label} — {isMarathi ? "भासमान" : "Feels like"} {Math.round(current.apparent_temperature)}°C
          </Typography>
        </Box>

        <Grid container spacing={1} sx={{ borderTop: "1px solid rgba(255, 255, 255, 0.2)", pt: 2, pb: 1, mb: 1 }}>
          <Grid item xs={4} sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
            <AirIcon fontSize="small" sx={{ opacity: 0.8 }} />
            <Box>
              <Typography variant="caption" display="block" sx={{ opacity: 0.7, lineHeight: 1 }}>
                {isMarathi ? "वारा" : "Wind"}
              </Typography>
              <Typography variant="body2" sx={{ fontWeight: "bold" }}>
                {current.wind_speed_10m} km/h
              </Typography>
            </Box>
          </Grid>
          <Grid item xs={4} sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
            <WaterDropIcon fontSize="small" sx={{ opacity: 0.8 }} />
            <Box>
              <Typography variant="caption" display="block" sx={{ opacity: 0.7, lineHeight: 1 }}>
                {isMarathi ? "आर्द्रता" : "Humidity"}
              </Typography>
              <Typography variant="body2" sx={{ fontWeight: "bold" }}>
                {current.relative_humidity_2m}%
              </Typography>
            </Box>
          </Grid>
          <Grid item xs={4} sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
            <GrainIcon fontSize="small" sx={{ opacity: 0.8 }} />
            <Box>
              <Typography variant="caption" display="block" sx={{ opacity: 0.7, lineHeight: 1 }}>
                {isMarathi ? "पाऊस" : "Rain"}
              </Typography>
              <Typography variant="body2" sx={{ fontWeight: "bold" }}>
                {current.precipitation} mm
              </Typography>
            </Box>
          </Grid>
        </Grid>

        {/* 3 Day Forecast */}
        <Box sx={{ borderTop: "1px solid rgba(255, 255, 255, 0.2)", pt: 1.5 }}>
          <Grid container spacing={2} justifyContent="space-between">
            {weather.daily.time.slice(1, 4).map((time, idx) => {
              const forecastInfo = getWeatherInfo(weather.daily.weather_code[idx + 1], i18n.language);
              return (
                <Grid item xs={4} key={time} sx={{ textAlign: "center" }}>
                  <Typography variant="caption" sx={{ opacity: 0.8, textTransform: "capitalize", fontWeight: "bold" }}>
                    {getDayName(time)}
                  </Typography>
                  <Typography variant="h6" sx={{ my: 0.2, lineHeight: 1 }}>{forecastInfo.icon}</Typography>
                  <Typography variant="caption" display="block" sx={{ fontWeight: "bold" }}>
                    {Math.round(weather.daily.temperature_2m_max[idx + 1])}° / {Math.round(weather.daily.temperature_2m_min[idx + 1])}°
                  </Typography>
                </Grid>
              );
            })}
          </Grid>
        </Box>
      </CardContent>
    </Card>
  );
}
