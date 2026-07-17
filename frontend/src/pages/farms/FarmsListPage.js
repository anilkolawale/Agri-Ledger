import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box, Typography, Button, Card, CardContent, Grid, IconButton, CircularProgress
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import AddIcon from "@mui/icons-material/Add";
import { useTranslation } from "react-i18next";
import { farmApi } from "../../api/farmApi";

export default function FarmsListPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [farms, setFarms] = useState([]);
  const [loading, setLoading] = useState(true);

  const load = () => {
    setLoading(true);
    farmApi.getAll().then(({ data }) => setFarms(data.data)).finally(() => setLoading(false));
  };

  useEffect(load, []);

  const handleDelete = async (id) => {
    if (!window.confirm("Delete this farm? Its crops and records stay in your history.")) return;
    await farmApi.remove(id);
    load();
  };

  return (
    <Box>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2 }}>
        <Typography variant="h1">{t("nav.farms")}</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate("/farms/new")}>
          {t("farm.addFarm")}
        </Button>
      </Box>

      {loading ? (
        <Box sx={{ display: "flex", justifyContent: "center", mt: 6 }}><CircularProgress /></Box>
      ) : (
        <Grid container spacing={2}>
          {farms.length === 0 && (
            <Grid item xs={12}><Typography color="text.secondary">No farms yet. Add your first farm to get started.</Typography></Grid>
          )}
          {farms.map((farm) => (
            <Grid item xs={12} sm={6} key={farm.id}>
              <Card>
                <CardContent>
                  <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
                    <Box>
                      <Typography variant="h2">{farm.name}</Typography>
                      {farm.location && <Typography color="text.secondary">{farm.location}</Typography>}
                      {farm.areaInAcres && <Typography variant="body2">{farm.areaInAcres} acres</Typography>}
                      <Typography variant="body2" color="text.secondary">{farm.cropCount} crop(s)</Typography>
                    </Box>
                    <Box>
                      <IconButton onClick={() => navigate(`/farms/${farm.id}/edit`)}><EditIcon /></IconButton>
                      <IconButton onClick={() => handleDelete(farm.id)}><DeleteIcon /></IconButton>
                    </Box>
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}
    </Box>
  );
}
