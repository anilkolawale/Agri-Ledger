import React, { useState } from "react";
import {
  Box, Card, CardContent, Typography, TextField, Button, Grid,
  Select, MenuItem, InputLabel, FormControl, Alert, CircularProgress,
  Avatar, CardHeader
} from "@mui/material";
import PersonIcon from "@mui/icons-material/Person";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../context/AuthContext";
import { authApi } from "../../api/authApi";

export default function ProfilePage() {
  const { t, i18n } = useTranslation();
  const { user, updateUser } = useAuth();

  // Local Form State (populated from AuthContext user object)
  const [fullName, setFullName] = useState(user?.fullName || "");
  const [phoneNumber, setPhoneNumber] = useState(user?.phoneNumber || "");
  const [address, setAddress] = useState(user?.address || "");
  const [preferredLanguage, setPreferredLanguage] = useState(user?.preferredLanguage || "en");

  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setSuccess(false);
    setError(null);

    try {
      // API call to update profile in backend DB
      await authApi.updateProfile({
        fullName,
        phoneNumber,
        preferredLanguage,
        address
      });

      // Update AuthContext session state & local storage
      updateUser({
        fullName,
        phoneNumber,
        preferredLanguage,
        address
      });

      // Hot-reload language settings in i18n
      i18n.changeLanguage(preferredLanguage);
      setSuccess(true);
    } catch (err) {
      setError(err.response?.data?.message || "Failed to update profile. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ pb: 4, maxWidth: 600, mx: "auto" }}>
      <Typography variant="h4" sx={{ fontWeight: "bold", mb: 3, color: "primary.main" }}>
        {t("profile.title", "User Profile")}
      </Typography>

      {success && (
        <Alert severity="success" sx={{ mb: 3 }}>
          {t("profile.saveSuccess", "Profile updated successfully!")}
        </Alert>
      )}
      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}

      <Card sx={{ borderRadius: 2 }}>
        <CardHeader
          avatar={
            <Avatar sx={{ bgcolor: "primary.main", width: 56, height: 56 }}>
              <PersonIcon sx={{ fontSize: 32 }} />
            </Avatar>
          }
          title={user?.fullName}
          subheader={user?.email}
          titleTypographyProps={{ variant: "h6", fontWeight: "bold" }}
          sx={{ borderBottom: "1px solid #e0e0e0", py: 2 }}
        />

        <CardContent sx={{ p: 3 }}>
          <Box component="form" onSubmit={handleSubmit} noValidate>
            <Grid container spacing={3}>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  disabled
                  label={t("auth.email")}
                  value={user?.email || ""}
                  helperText={t("profile.emailReadonly", "Email cannot be changed")}
                />
              </Grid>

              <Grid item xs={12}>
                <TextField
                  fullWidth
                  required
                  label={t("auth.fullName")}
                  value={fullName}
                  onChange={(e) => setFullName(e.target.value)}
                />
              </Grid>

              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label={t("auth.phoneNumber")}
                  value={phoneNumber}
                  onChange={(e) => setPhoneNumber(e.target.value)}
                />
              </Grid>

              <Grid item xs={12}>
                <TextField
                  fullWidth
                  multiline
                  rows={3}
                  label={t("profile.address", "Address")}
                  value={address}
                  onChange={(e) => setAddress(e.target.value)}
                />
              </Grid>

              <Grid item xs={12}>
                <FormControl fullWidth>
                  <InputLabel id="lang-select-label">{t("profile.language", "Preferred Language")}</InputLabel>
                  <Select
                    labelId="lang-select-label"
                    label={t("profile.language", "Preferred Language")}
                    value={preferredLanguage}
                    onChange={(e) => setPreferredLanguage(e.target.value)}
                  >
                    <MenuItem value="en">English</MenuItem>
                    <MenuItem value="mr">मराठी</MenuItem>
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12}>
                <Button
                  fullWidth
                  type="submit"
                  variant="contained"
                  size="large"
                  disabled={loading || !fullName.trim()}
                  sx={{ mt: 1 }}
                >
                  {loading ? <CircularProgress size={24} /> : t("common.save")}
                </Button>
              </Grid>
            </Grid>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}
