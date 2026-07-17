import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Box, Typography, Button, Paper, Table, TableHead, TableRow, TableCell, TableBody, IconButton, CircularProgress, TablePagination } from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import AddIcon from "@mui/icons-material/Add";
import { useTranslation } from "react-i18next";
import { incomeApi } from "../../api/incomeApi";

const formatCurrency = (n) => `₹${Number(n || 0).toLocaleString("en-IN")}`;

export default function IncomeListPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [result, setResult] = useState({ items: [], totalCount: 0 });
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const load = () => {
    setLoading(true);
    incomeApi.getPaged({ pageNumber: page + 1, pageSize }).then(({ data }) => setResult(data.data)).finally(() => setLoading(false));
  };
  useEffect(load, [page, pageSize]);

  const handleDelete = async (id) => {
    if (!window.confirm("Delete this income record?")) return;
    await incomeApi.remove(id);
    load();
  };

  return (
    <Box>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2 }}>
        <Typography variant="h1">{t("nav.income")}</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate("/income/new")}>{t("income.addIncome")}</Button>
      </Box>

      {loading ? (
        <Box sx={{ display: "flex", justifyContent: "center", mt: 6 }}><CircularProgress /></Box>
      ) : (
        <Paper>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Date</TableCell>
                <TableCell>Farm</TableCell>
                <TableCell>Buyer</TableCell>
                <TableCell align="right">Total</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {result.items.length === 0 && (
                <TableRow><TableCell colSpan={5} align="center">No income recorded yet.</TableCell></TableRow>
              )}
              {result.items.map((i) => (
                <TableRow key={i.id}>
                  <TableCell>{new Date(i.saleDate).toLocaleDateString()}</TableCell>
                  <TableCell>{i.farmName}</TableCell>
                  <TableCell>{i.buyerName}</TableCell>
                  <TableCell align="right">{formatCurrency(i.totalAmount)}</TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => navigate(`/income/${i.id}/edit`)}><EditIcon fontSize="small" /></IconButton>
                    <IconButton size="small" onClick={() => handleDelete(i.id)}><DeleteIcon fontSize="small" /></IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <TablePagination
            component="div" count={result.totalCount} page={page} onPageChange={(_, p) => setPage(p)}
            rowsPerPage={pageSize} onRowsPerPageChange={(e) => { setPageSize(parseInt(e.target.value, 10)); setPage(0); }}
          />
        </Paper>
      )}
    </Box>
  );
}
