import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Box, Typography, Button, Paper, Table, TableHead, TableRow, TableCell, TableBody, IconButton, Chip, CircularProgress } from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import AddIcon from "@mui/icons-material/Add";
import { laborApi } from "../../api/laborApi";

const formatCurrency = (n) => `₹${Number(n || 0).toLocaleString("en-IN")}`;
const PAYMENT_STATUSES = ["Pending", "Partial", "Paid"];
const STATUS_COLORS = ["warning", "info", "success"];

export default function LaborListPage() {
  const navigate = useNavigate();
  const [labors, setLabors] = useState([]);
  const [loading, setLoading] = useState(true);

  const load = () => {
    setLoading(true);
    laborApi.getAll().then(({ data }) => setLabors(data.data)).finally(() => setLoading(false));
  };
  useEffect(load, []);

  const handleDelete = async (id) => {
    if (!window.confirm("Delete this labor entry?")) return;
    await laborApi.remove(id);
    load();
  };

  return (
    <Box>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2 }}>
        <Typography variant="h1">Labor</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate("/labor/new")}>Add Labor Entry</Button>
      </Box>

      {loading ? (
        <Box sx={{ display: "flex", justifyContent: "center", mt: 6 }}><CircularProgress /></Box>
      ) : (
        <Paper>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Worker</TableCell>
                <TableCell>Farm</TableCell>
                <TableCell>Task</TableCell>
                <TableCell>Date</TableCell>
                <TableCell align="right">Wage</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {labors.length === 0 && (
                <TableRow><TableCell colSpan={7} align="center">No labor entries yet.</TableCell></TableRow>
              )}
              {labors.map((l) => (
                <TableRow key={l.id}>
                  <TableCell>{l.workerName}</TableCell>
                  <TableCell>{l.farmName}</TableCell>
                  <TableCell>{l.task}</TableCell>
                  <TableCell>{new Date(l.workDate).toLocaleDateString()}</TableCell>
                  <TableCell align="right">{formatCurrency(l.dailyWage)}</TableCell>
                  <TableCell><Chip size="small" label={PAYMENT_STATUSES[l.paymentStatus]} color={STATUS_COLORS[l.paymentStatus] || "default"} /></TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => navigate(`/labor/${l.id}/edit`)}><EditIcon fontSize="small" /></IconButton>
                    <IconButton size="small" onClick={() => handleDelete(l.id)}><DeleteIcon fontSize="small" /></IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </Paper>
      )}
    </Box>
  );
}
