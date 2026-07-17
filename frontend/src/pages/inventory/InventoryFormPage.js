import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate, useParams } from "react-router-dom";
import { Box, Button, Card, CardContent, TextField, Typography, MenuItem } from "@mui/material";
import { farmApi } from "../../api/farmApi";
import { inventoryApi } from "../../api/inventoryApi";

const TYPE_LABELS = ["Seeds", "Fertilizer", "Pesticides", "Equipment", "Packaging Material", "Other"];

export default function InventoryFormPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = !!id;
  const { register, handleSubmit, reset } = useForm();
  const [farms, setFarms] = useState([]);

  useEffect(() => {
    farmApi.getAll().then(({ data }) => setFarms(data.data));
    if (isEdit) inventoryApi.getById(id).then(({ data }) => reset(data.data));
  }, [id, isEdit, reset]);

  const onSubmit = async (data) => {
    const payload = {
      ...data,
      farmId: data.farmId ? Number(data.farmId) : null,
      type: Number(data.type),
      quantityAvailable: Number(data.quantityAvailable),
      reorderThreshold: Number(data.reorderThreshold)
    };
    if (isEdit) await inventoryApi.update(id, payload);
    else await inventoryApi.create(payload);
    navigate("/inventory");
  };

  return (
    <Card sx={{ maxWidth: 480, mx: "auto" }}>
      <CardContent>
        <Typography variant="h1" gutterBottom>{isEdit ? "Edit Inventory Item" : "Add Inventory Item"}</Typography>
        <Box component="form" onSubmit={handleSubmit(onSubmit)}>
          <TextField select fullWidth margin="normal" label="Farm (optional)" {...register("farmId")} defaultValue="">
            <MenuItem value="">All Farms</MenuItem>
            {farms.map((f) => <MenuItem key={f.id} value={f.id}>{f.name}</MenuItem>)}
          </TextField>
          <TextField fullWidth margin="normal" label="Item Name" {...register("name", { required: true })} />
          <TextField select fullWidth margin="normal" label="Type" defaultValue={0} {...register("type")}>
            {TYPE_LABELS.map((t, i) => <MenuItem key={t} value={i}>{t}</MenuItem>)}
          </TextField>
          <TextField fullWidth margin="normal" label="Quantity Available" type="number" inputProps={{ step: "0.01" }} {...register("quantityAvailable", { required: true })} />
          <TextField fullWidth margin="normal" label="Unit" defaultValue="kg" {...register("unit")} />
          <TextField fullWidth margin="normal" label="Reorder Threshold" type="number" inputProps={{ step: "0.01" }} {...register("reorderThreshold", { required: true })} />
          <TextField fullWidth margin="normal" label="Notes" multiline rows={3} {...register("notes")} />
          <Box sx={{ display: "flex", gap: 1, mt: 2 }}>
            <Button type="submit" variant="contained" fullWidth>Save</Button>
            <Button variant="outlined" fullWidth onClick={() => navigate("/inventory")}>Cancel</Button>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
}
