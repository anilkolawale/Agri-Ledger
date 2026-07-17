import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate, useParams } from "react-router-dom";
import { Box, Button, Card, CardContent, TextField, Typography, MenuItem } from "@mui/material";
import { farmApi } from "../../api/farmApi";
import { laborApi } from "../../api/laborApi";

const PAYMENT_STATUSES = ["Pending", "Partial", "Paid"];

export default function LaborFormPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = !!id;
  const { register, handleSubmit, reset } = useForm();
  const [farms, setFarms] = useState([]);

  useEffect(() => {
    farmApi.getAll().then(({ data }) => setFarms(data.data));
    if (isEdit) {
      laborApi.getById(id).then(({ data }) => reset({ ...data.data, workDate: data.data.workDate?.slice(0, 10) }));
    }
  }, [id, isEdit, reset]);

  const onSubmit = async (data) => {
    const payload = { ...data, farmId: Number(data.farmId), dailyWage: Number(data.dailyWage), paymentStatus: Number(data.paymentStatus) };
    if (isEdit) await laborApi.update(id, payload);
    else await laborApi.create(payload);
    navigate("/labor");
  };

  return (
    <Card sx={{ maxWidth: 480, mx: "auto" }}>
      <CardContent>
        <Typography variant="h1" gutterBottom>{isEdit ? "Edit Labor Entry" : "Add Labor Entry"}</Typography>
        <Box component="form" onSubmit={handleSubmit(onSubmit)}>
          <TextField select fullWidth margin="normal" label="Farm" {...register("farmId", { required: true })} defaultValue="">
            {farms.map((f) => <MenuItem key={f.id} value={f.id}>{f.name}</MenuItem>)}
          </TextField>
          <TextField fullWidth margin="normal" label="Worker Name" {...register("workerName", { required: true })} />
          <TextField fullWidth margin="normal" label="Task" {...register("task", { required: true })} />
          <TextField fullWidth margin="normal" label="Date" type="date" InputLabelProps={{ shrink: true }} {...register("workDate", { required: true })} />
          <TextField fullWidth margin="normal" label="Daily Wage" type="number" inputProps={{ step: "0.01" }} {...register("dailyWage", { required: true })} />
          <TextField select fullWidth margin="normal" label="Payment Status" defaultValue={0} {...register("paymentStatus")}>
            {PAYMENT_STATUSES.map((s, i) => <MenuItem key={s} value={i}>{s}</MenuItem>)}
          </TextField>
          <TextField fullWidth margin="normal" label="Notes" multiline rows={3} {...register("notes")} />
          <Box sx={{ display: "flex", gap: 1, mt: 2 }}>
            <Button type="submit" variant="contained" fullWidth>Save</Button>
            <Button variant="outlined" fullWidth onClick={() => navigate("/labor")}>Cancel</Button>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
}
