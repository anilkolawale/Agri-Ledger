import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Box, Typography, Button, Paper, Table, TableHead, TableRow, TableCell, TableBody, IconButton, Chip, CircularProgress } from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import AddIcon from "@mui/icons-material/Add";
import { useTranslation } from "react-i18next";
import { cropApi } from "../../api/cropApi";

const STATUS_LABELS = ["Planned", "Planted", "Growing", "Ready for Harvest", "Harvested", "Failed"];

export default function CropsListPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [crops, setCrops] = useState([]);
  const [loading, setLoading] = useState(true);

  const load = () => {
    setLoading(true);
    cropApi.getAll().then(({ data }) => setCrops(data.data)).finally(() => setLoading(false));
  };
  useEffect(load, []);

  const handleDelete = async (id) => {
    if (!window.confirm("Delete this crop?")) return;
    await cropApi.remove(id);
    load();
  };

  return (
    <Box>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2 }}>
        <Typography variant="h1">{t("nav.crops")}</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate("/crops/new")}>Add Crop</Button>
      </Box>

      {loading ? (
        <Box sx={{ display: "flex", justifyContent: "center", mt: 6 }}><CircularProgress /></Box>
      ) : (
        <Paper>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Crop</TableCell>
                <TableCell>Farm</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Expected Harvest</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {crops.length === 0 && (
                <TableRow><TableCell colSpan={5} align="center">No crops yet.</TableCell></TableRow>
              )}
              {crops.map((c) => (
                <TableRow key={c.id}>
                  <TableCell>{c.name}{c.variety ? ` (${c.variety})` : ""}</TableCell>
                  <TableCell>{c.farmName}</TableCell>
                  <TableCell><Chip size="small" label={STATUS_LABELS[c.status]} /></TableCell>
                  <TableCell>{c.expectedHarvestDate ? new Date(c.expectedHarvestDate).toLocaleDateString() : "-"}</TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => navigate(`/crops/${c.id}/edit`)}><EditIcon fontSize="small" /></IconButton>
                    <IconButton size="small" onClick={() => handleDelete(c.id)}><DeleteIcon fontSize="small" /></IconButton>
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
