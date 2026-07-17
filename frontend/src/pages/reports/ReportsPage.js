import React, { useEffect, useState } from "react";
import {
  Box, Typography, Paper, Table, TableHead, TableRow, TableCell, TableBody,
  MenuItem, TextField, Button, CircularProgress, Grid
} from "@mui/material";
import DownloadIcon from "@mui/icons-material/Download";
import { reportsApi } from "../../api/reportsApi";
import FinanceChart from "../../components/common/FinanceChart";
import { useTranslation } from "react-i18next";

// One screen for all the spec's tabular reports, with search/filter (date range)
// and Excel/PDF export — matches "easy-to-read tables ... Search, Filters, Sorting,
// Pagination, Excel Export, PDF Export".
const REPORT_TYPES = [
  { key: "expenses", label: "Expenses", fetch: reportsApi.expenses },
  { key: "income", label: "Income", fetch: reportsApi.income },
  { key: "crop-profit", label: "Crop-wise Profit", fetch: reportsApi.cropProfit },
  { key: "category-expense", label: "Category-wise Expenses", fetch: reportsApi.categoryExpense },
  { key: "farm-expense", label: "Farm-wise Expenses", fetch: reportsApi.farmExpense },
  { key: "farm-income", label: "Farm-wise Income", fetch: reportsApi.farmIncome },
  { key: "labor-payments", label: "Labor Payments", fetch: reportsApi.laborPayments },
  { key: "inventory", label: "Inventory Report", fetch: reportsApi.inventory }
];

export default function ReportsPage() {
  const { i18n } = useTranslation();
  const isMarathi = i18n.language === "mr";

  const [reportKey, setReportKey] = useState("expenses");
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");
  const [rows, setRows] = useState([]);
  const [chartData, setChartData] = useState([]);
  const [loading, setLoading] = useState(false);

  const currentReport = REPORT_TYPES.find((r) => r.key === reportKey);

  const loadChartData = () => {
    Promise.all([
      reportsApi.expenses({ from: from || undefined, to: to || undefined }),
      reportsApi.income({ from: from || undefined, to: to || undefined })
    ]).then(([expRes, incRes]) => {
      const expenses = expRes.data.data;
      const incomes = incRes.data.data;

      // Group by YYYY-MM
      const monthlyMap = {};

      expenses.forEach((e) => {
        const date = new Date(e.expenseDate);
        if (isNaN(date)) return;
        const key = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}`;
        if (!monthlyMap[key]) monthlyMap[key] = { expense: 0, income: 0, dateObj: date };
        monthlyMap[key].expense += e.amount;
      });

      incomes.forEach((i) => {
        const date = new Date(i.saleDate);
        if (isNaN(date)) return;
        const key = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}`;
        if (!monthlyMap[key]) monthlyMap[key] = { expense: 0, income: 0, dateObj: date };
        monthlyMap[key].income += i.totalAmount;
      });

      // Sort keys chronologically
      const sortedKeys = Object.keys(monthlyMap).sort();
      const chartDataList = sortedKeys.map((key) => {
        const item = monthlyMap[key];
        const monthLabel = item.dateObj.toLocaleDateString(i18n.language === "mr" ? "mr-IN" : "en-US", { month: "short" });
        return {
          label: `${monthLabel} ${String(item.dateObj.getFullYear()).slice(-2)}`,
          expense: item.expense,
          income: item.income
        };
      });

      setChartData(chartDataList);
    }).catch((err) => console.error("Error loading chart data:", err));
  };

  const load = () => {
    setLoading(true);
    currentReport.fetch({ from: from || undefined, to: to || undefined })
      .then(({ data }) => setRows(data.data))
      .finally(() => setLoading(false));

    loadChartData();
  };

  // eslint-disable-next-line react-hooks/exhaustive-deps
  useEffect(load, [reportKey]);

  const handleExport = async (format) => {
    const { data } = await reportsApi.export(reportKey, format, { from: from || undefined, to: to || undefined });
    const url = window.URL.createObjectURL(new Blob([data]));
    const link = document.createElement("a");
    link.href = url;
    link.download = `${reportKey}-report.${format === "pdf" ? "pdf" : "xlsx"}`;
    link.click();
    window.URL.revokeObjectURL(url);
  };

  const columns = rows.length > 0 ? Object.keys(rows[0]) : [];

  return (
    <Box>
      <Typography variant="h1" gutterBottom>{isMarathi ? "अहवाल" : "Reports"}</Typography>

      {/* Comparison Chart */}
      <FinanceChart data={chartData} />

      <Grid container spacing={2} sx={{ mb: 2 }}>
        <Grid item xs={12} sm={4}>
          <TextField select fullWidth label={isMarathi ? "अहवाल निवडा" : "Report"} value={reportKey} onChange={(e) => setReportKey(e.target.value)}>
            {REPORT_TYPES.map((r) => (
              <MenuItem key={r.key} value={r.key}>
                {isMarathi ? translateReportLabel(r.key) : r.label}
              </MenuItem>
            ))}
          </TextField>
        </Grid>
        <Grid item xs={6} sm={3}>
          <TextField fullWidth label={isMarathi ? "या तारखेपासून" : "From"} type="date" InputLabelProps={{ shrink: true }} value={from} onChange={(e) => setFrom(e.target.value)} />
        </Grid>
        <Grid item xs={6} sm={3}>
          <TextField fullWidth label={isMarathi ? "या तारखेपर्यंत" : "To"} type="date" InputLabelProps={{ shrink: true }} value={to} onChange={(e) => setTo(e.target.value)} />
        </Grid>
        <Grid item xs={12} sm={2}>
          <Button fullWidth variant="contained" onClick={load} sx={{ height: "100%" }}>{isMarathi ? "लागू करा" : "Apply"}</Button>
        </Grid>
      </Grid>

      <Box sx={{ display: "flex", gap: 1, mb: 2 }}>
        <Button startIcon={<DownloadIcon />} variant="outlined" onClick={() => handleExport("excel")}>
          {isMarathi ? "एक्सेल निर्यात" : "Export Excel"}
        </Button>
        <Button startIcon={<DownloadIcon />} variant="outlined" onClick={() => handleExport("pdf")}>
          {isMarathi ? "पीडीएफ निर्यात" : "Export PDF"}
        </Button>
      </Box>

      {loading ? (
        <Box sx={{ display: "flex", justifyContent: "center", mt: 6 }}><CircularProgress /></Box>
      ) : (
        <Paper sx={{ overflowX: "auto" }}>
          <Table size="small">
            <TableHead>
              <TableRow>
                {columns.map((c) => <TableCell key={c}>{splitCamel(c)}</TableCell>)}
              </TableRow>
            </TableHead>
            <TableBody>
              {rows.length === 0 && (
                <TableRow><TableCell colSpan={Math.max(columns.length, 1)} align="center">{isMarathi ? "निवडलेल्या फिल्टरसाठी कोणताही डेटा नाही." : "No data for the selected filters."}</TableCell></TableRow>
              )}
              {rows.map((row, idx) => (
                <TableRow key={idx}>
                  {columns.map((c) => <TableCell key={c}>{formatCell(row[c], isMarathi)}</TableCell>)}
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </Paper>
      )}
    </Box>
  );
}

function translateReportLabel(key) {
  switch (key) {
    case "expenses": return "खर्च अहवाल";
    case "income": return "उत्पन्न अहवाल";
    case "crop-profit": return "पीक निहाय नफा";
    case "category-expense": return "श्रेणी निहाय खर्च";
    case "farm-expense": return "शेत निहाय खर्च";
    case "farm-income": return "शेत निहाय उत्पन्न";
    case "labor-payments": return "मजूर देयके";
    case "inventory": return "इन्व्हेंटरी अहवाल";
    default: return key;
  }
}

function splitCamel(s) {
  return s.replace(/([A-Z])/g, " $1").replace(/^./, (c) => c.toUpperCase()).trim();
}

function formatCell(value, isMarathi) {
  if (typeof value === "boolean") return value ? (isMarathi ? "होय" : "Yes") : (isMarathi ? "नाही" : "No");
  if (typeof value === "string" && /^\d{4}-\d{2}-\d{2}T/.test(value)) return new Date(value).toLocaleDateString(isMarathi ? "mr-IN" : "en-US");
  return value ?? "-";
}

