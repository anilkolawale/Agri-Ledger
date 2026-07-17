import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box, Typography, Button, Paper, Table, TableHead, TableRow, TableCell, TableBody,
  IconButton, TextField, CircularProgress, TablePagination
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import AddIcon from "@mui/icons-material/Add";
import { useTranslation } from "react-i18next";
import { expenseApi } from "../../api/expenseApi";

const formatCurrency = (n) => `₹${Number(n || 0).toLocaleString("en-IN")}`;

export default function ExpensesListPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [result, setResult] = useState({ items: [], totalCount: 0 });
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const load = () => {
    setLoading(true);
    expenseApi.getPaged({ pageNumber: page + 1, pageSize, searchTerm: search || undefined })
      .then(({ data }) => setResult(data.data))
      .finally(() => setLoading(false));
  };

  // eslint-disable-next-line react-hooks/exhaustive-deps
  useEffect(load, [page, pageSize]);

  const handleSearch = (e) => {
    e.preventDefault();
    setPage(0);
    load();
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Delete this expense?")) return;
    await expenseApi.remove(id);
    load();
  };

  return (
    <Box>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2, flexWrap: "wrap", gap: 1 }}>
        <Typography variant="h1">{t("nav.expenses")}</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate("/expenses/new")}>
          {t("expense.addExpense")}
        </Button>
      </Box>

      <Box component="form" onSubmit={handleSearch} sx={{ display: "flex", gap: 1, mb: 2 }}>
        <TextField size="small" fullWidth placeholder={t("common.search")} value={search} onChange={(e) => setSearch(e.target.value)} />
        <Button type="submit" variant="outlined">{t("common.search")}</Button>
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
                <TableCell>Category</TableCell>
                <TableCell align="right">Amount</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {result.items.length === 0 && (
                <TableRow><TableCell colSpan={5} align="center">No expenses recorded yet.</TableCell></TableRow>
              )}
              {result.items.map((e) => (
                <TableRow key={e.id}>
                  <TableCell>{new Date(e.expenseDate).toLocaleDateString()}</TableCell>
                  <TableCell>{e.farmName}</TableCell>
                  <TableCell>{e.categoryName}</TableCell>
                  <TableCell align="right">{formatCurrency(e.amount)}</TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => navigate(`/expenses/${e.id}/edit`)}><EditIcon fontSize="small" /></IconButton>
                    <IconButton size="small" onClick={() => handleDelete(e.id)}><DeleteIcon fontSize="small" /></IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <TablePagination
            component="div"
            count={result.totalCount}
            page={page}
            onPageChange={(_, p) => setPage(p)}
            rowsPerPage={pageSize}
            onRowsPerPageChange={(e) => { setPageSize(parseInt(e.target.value, 10)); setPage(0); }}
          />
        </Paper>
      )}
    </Box>
  );
}
