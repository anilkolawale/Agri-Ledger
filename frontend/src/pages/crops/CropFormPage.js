import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate, useParams } from "react-router-dom";
import { Box, Button, Card, CardContent, TextField, Typography, MenuItem } from "@mui/material";
import { farmApi } from "../../api/farmApi";
import { cropApi } from "../../api/cropApi";

const STATUS_LABELS = ["Planned", "Planted", "Growing", "Ready for Harvest", "Harvested", "Failed"];

export default function CropFormPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = !!id;
  const { register, handleSubmit, reset } = useForm();
  const [farms, setFarms] = useState([]);

  useEffect(() => {
    farmApi.getAll().then(({ data }) => setFarms(data.data));
    if (isEdit) {
      cropApi.getById(id).then(({ data }) => reset({
        ...data.data,
        plantDate: data.data.plantDate?.slice(0, 10),
        expectedHarvestDate: data.data.expectedHarvestDate?.slice(0, 10)
      }));
    }
  }, [id, isEdit, reset]);

  const onSubmit = async (data) => {
    const payload = { ...data, farmId: Number(data.farmId), status: Number(data.status) };
    if (isEdit) await cropApi.update(id, payload);
    else await cropApi.create(payload);
    navigate("/crops");
  };

  return (
    <Card sx={{ maxWidth: 480, mx: "auto" }}>
      <CardContent>
        <Typography variant="h1" gutterBottom>{isEdit ? "Edit Crop" : "Add Crop"}</Typography>
        <Box component="form" onSubmit={handleSubmit(onSubmit)}>
          <TextField select fullWidth margin="normal" label="Farm" {...register("farmId", { required: true })} defaultValue="">
            {farms.map((f) => <MenuItem key={f.id} value={f.id}>{f.name}</MenuItem>)}
          </TextField>
          <TextField fullWidth margin="normal" label="Crop Name" {...register("name", { required: true })} />
          <TextField fullWidth margin="normal" label="Variety" {...register("variety")} />
          <TextField fullWidth margin="normal" label="Plant Date" type="date" InputLabelProps={{ shrink: true }} {...register("plantDate")} />
          <TextField fullWidth margin="normal" label="Expected Harvest Date" type="date" InputLabelProps={{ shrink: true }} {...register("expectedHarvestDate")} />
          <TextField select fullWidth margin="normal" label="Status" defaultValue={0} {...register("status")}>
            {STATUS_LABELS.map((s, i) => <MenuItem key={s} value={i}>{s}</MenuItem>)}
          </TextField>
          <TextField fullWidth margin="normal" label="Notes" multiline rows={3} {...register("notes")} />
          <Box sx={{ display: "flex", gap: 1, mt: 2 }}>
            <Button type="submit" variant="contained" fullWidth>Save</Button>
            <Button variant="outlined" fullWidth onClick={() => navigate("/crops")}>Cancel</Button>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
}
