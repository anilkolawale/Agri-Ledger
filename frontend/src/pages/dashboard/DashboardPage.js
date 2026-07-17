import React, { useEffect, useState } from "react";
import { Grid, Typography, Paper, Table, TableHead, TableRow, TableCell, TableBody, CircularProgress, Box } from "@mui/material";
import { useTranslation } from "react-i18next";
import { dashboardApi } from "../../api/dashboardApi";
import SummaryCard from "../../components/common/SummaryCard";
import WeatherWidget from "../../components/common/WeatherWidget";

const formatCurrency = (n) => `₹${Number(n || 0).toLocaleString("en-IN")}`;

export default function DashboardPage() {
  const { t } = useTranslation();
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    dashboardApi.getSummary()
      .then(({ data }) => setSummary(data.data))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <Box sx={{ display: "flex", justifyContent: "center", mt: 6 }}><CircularProgress /></Box>;
  if (!summary) return null;

  const cards = [
    { title: t("dashboard.totalFarms"), value: summary.totalFarms },
    { title: t("dashboard.todaysExpense"), value: formatCurrency(summary.todaysExpenses) },
    { title: t("dashboard.monthlyExpense"), value: formatCurrency(summary.monthlyExpenses) },
    { title: t("dashboard.totalIncome"), value: formatCurrency(summary.totalIncome), color: "success.main" },
    { title: t("dashboard.totalExpense"), value: formatCurrency(summary.totalExpenses), color: "error.main" },
    { title: t("dashboard.netProfit"), value: formatCurrency(summary.netProfit), color: summary.netProfit >= 0 ? "success.main" : "error.main" },
    { title: "Crops Ready for Harvest", value: summary.cropsReadyForHarvest },
    { title: "Pending Labor Payments", value: summary.pendingLaborPayments },
    { title: "Low Inventory Items", value: summary.lowInventoryItems }
  ];

  return (
    <Box>
      <Typography variant="h1" gutterBottom>{t("dashboard.title")}</Typography>
      <Typography color="text.secondary" sx={{ mb: 2 }}>{t("dashboard.subtitle")}</Typography>

      {/* Weather Widget */}
      <WeatherWidget />

      <Grid container spacing={2}>
        {cards.map((c) => (
          <Grid item xs={6} sm={4} key={c.title}>
            <SummaryCard title={c.title} value={c.value} color={c.color} />
          </Grid>
        ))}
      </Grid>

      <Typography variant="h2" sx={{ mt: 4, mb: 1 }}>{t("dashboard.upcomingHarvests")}</Typography>
      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Farm</TableCell>
              <TableCell>Crop</TableCell>
              <TableCell>Expected Harvest Date</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {summary.upcomingHarvests.length === 0 && (
              <TableRow><TableCell colSpan={3} align="center">No upcoming harvests.</TableCell></TableRow>
            )}
            {summary.upcomingHarvests.map((h, i) => (
              <TableRow key={i}>
                <TableCell>{h.farmName}</TableCell>
                <TableCell>{h.cropName}</TableCell>
                <TableCell>{h.expectedHarvestDate ? new Date(h.expectedHarvestDate).toLocaleDateString() : "-"}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>
    </Box>
  );
}
