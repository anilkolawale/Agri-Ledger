import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { Link } from "react-router-dom";
import { Box, Button, Card, CardContent, TextField, Typography, Alert } from "@mui/material";
import { authApi } from "../../api/authApi";

export default function ForgotPasswordPage() {
  const { register, handleSubmit } = useForm();
  const [message, setMessage] = useState(null);

  const onSubmit = async (data) => {
    await authApi.forgotPassword(data);
    setMessage("If that email exists, a reset link has been sent.");
  };

  return (
    <Box sx={{ display: "flex", minHeight: "100vh", alignItems: "center", justifyContent: "center", p: 2 }}>
      <Card sx={{ width: "100%", maxWidth: 420 }}>
        <CardContent sx={{ p: 3 }}>
          <Typography variant="h1" gutterBottom>Forgot Password</Typography>
          {message && <Alert severity="success" sx={{ mb: 2 }}>{message}</Alert>}
          <Box component="form" onSubmit={handleSubmit(onSubmit)}>
            <TextField fullWidth margin="normal" label="Email" type="email" {...register("email", { required: true })} />
            <Button fullWidth type="submit" variant="contained" size="large" sx={{ mt: 2 }}>Send Reset Link</Button>
          </Box>
          <Box sx={{ textAlign: "center", mt: 2 }}><Link to="/login">Back to Login</Link></Box>
        </CardContent>
      </Card>
    </Box>
  );
}
