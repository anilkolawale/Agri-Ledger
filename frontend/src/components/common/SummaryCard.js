import React from "react";
import { Card, CardContent, Typography } from "@mui/material";

// Simple summary card used across the dashboard — per spec, "instead of charts,
// display simple summary cards" for a farmer-friendly, low-clutter overview.
export default function SummaryCard({ title, value, color }) {
  return (
    <Card sx={{ height: "100%" }}>
      <CardContent>
        <Typography variant="body2" color="text.secondary">{title}</Typography>
        <Typography variant="h5" sx={{ fontWeight: 700, color: color || "text.primary", mt: 0.5 }}>
          {value}
        </Typography>
      </CardContent>
    </Card>
  );
}
