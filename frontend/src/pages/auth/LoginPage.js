import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router-dom";
import { Box, Button, Card, CardContent, TextField, Typography, Alert } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../context/AuthContext";

export default function LoginPage() {
  const { t } = useTranslation();
  const { login } = useAuth();
  const navigate = useNavigate();
  const { register, handleSubmit, formState: { errors } } = useForm();
  const [apiError, setApiError] = useState(null);

  const onSubmit = async (data) => {
    setApiError(null);
    try {
      await login(data);
      navigate("/dashboard");
    } catch (err) {
      setApiError(err.response?.data?.message || "Login failed. Please check your credentials.");
    }
  };

  return (
    <Box sx={{ display: "flex", minHeight: "100vh", alignItems: "center", justifyContent: "center", bgcolor: "background.default", p: 2 }}>
      <Card sx={{ width: "100%", maxWidth: 420 }}>
        <CardContent sx={{ p: 3 }}>
          <Box sx={{ display: "flex", justifyContent: "center", mb: 2 }}>
            <img src="/logo192.png" alt="AgriLedger Logo" style={{ width: 80, height: 80, borderRadius: 16 }} />
          </Box>
          <Typography variant="h1" align="center" gutterBottom>{t("appName")}</Typography>
          <Typography align="center" color="text.secondary" sx={{ mb: 3 }}>{t("tagline")}</Typography>


          {apiError && <Alert severity="error" sx={{ mb: 2 }}>{apiError}</Alert>}

          <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
            <TextField
              fullWidth margin="normal" label={t("auth.email")} type="email"
              {...register("email", { required: true })}
              error={!!errors.email}
            />
            <TextField
              fullWidth margin="normal" label={t("auth.password")} type="password"
              {...register("password", { required: true })}
              error={!!errors.password}
            />
            <Button fullWidth type="submit" variant="contained" size="large" sx={{ mt: 2 }}>
              {t("auth.loginButton")}
            </Button>
          </Box>

          <Box sx={{ display: "flex", justifyContent: "space-between", mt: 2 }}>
            <Link to="/forgot-password">{t("auth.forgotPassword")}</Link>
            <Link to="/register">{t("auth.noAccount")}</Link>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}
