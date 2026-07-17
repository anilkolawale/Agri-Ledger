import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router-dom";
import { Box, Button, Card, CardContent, TextField, Typography, Alert, MenuItem } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../context/AuthContext";

export default function RegisterPage() {
  const { t } = useTranslation();
  const { register: registerUser } = useAuth();
  const navigate = useNavigate();
  const { register, handleSubmit, watch, formState: { errors } } = useForm();
  const [apiError, setApiError] = useState(null);

  const onSubmit = async (data) => {
    setApiError(null);
    try {
      const { confirmPassword, ...payload } = data;
      await registerUser(payload);
      navigate("/dashboard");
    } catch (err) {
      setApiError(err.response?.data?.message || "Registration failed.");
    }
  };

  return (
    <Box sx={{ display: "flex", minHeight: "100vh", alignItems: "center", justifyContent: "center", bgcolor: "background.default", p: 2 }}>
      <Card sx={{ width: "100%", maxWidth: 420 }}>
        <CardContent sx={{ p: 3 }}>
          <Typography variant="h1" align="center" gutterBottom>{t("auth.register")}</Typography>
          {apiError && <Alert severity="error" sx={{ mb: 2 }}>{apiError}</Alert>}

          <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
            <TextField fullWidth margin="normal" label={t("auth.fullName")} {...register("fullName", { required: true })} error={!!errors.fullName} />
            <TextField fullWidth margin="normal" label={t("auth.email")} type="email" {...register("email", { required: true })} error={!!errors.email} />
            <TextField fullWidth margin="normal" label={t("auth.phoneNumber")} {...register("phoneNumber", { required: true })} error={!!errors.phoneNumber} />
            <TextField fullWidth margin="normal" label={t("auth.password")} type="password" {...register("password", { required: true, minLength: 6 })} error={!!errors.password} />
            <TextField
              fullWidth margin="normal" label={t("auth.confirmPassword")} type="password"
              {...register("confirmPassword", { validate: (v) => v === watch("password") || "Passwords do not match" })}
              error={!!errors.confirmPassword}
              helperText={errors.confirmPassword?.message}
            />
            <TextField select fullWidth margin="normal" label="Language" defaultValue="en" {...register("preferredLanguage")}>
              <MenuItem value="en">English</MenuItem>
              <MenuItem value="mr">मराठी</MenuItem>
            </TextField>
            <Button fullWidth type="submit" variant="contained" size="large" sx={{ mt: 2 }}>
              {t("auth.registerButton")}
            </Button>
          </Box>

          <Box sx={{ textAlign: "center", mt: 2 }}>
            <Link to="/login">{t("auth.haveAccount")}</Link>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}
