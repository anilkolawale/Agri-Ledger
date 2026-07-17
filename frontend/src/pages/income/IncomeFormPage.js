import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate, useParams } from "react-router-dom";
import { Box, Button, Card, CardContent, TextField, Typography, MenuItem } from "@mui/material";
import { useTranslation } from "react-i18next";
import { incomeApi } from "../../api/incomeApi";
import { farmApi } from "../../api/farmApi";
import { cropApi } from "../../api/cropApi";
import ReceiptManager from "../../components/common/ReceiptManager";

const PAYMENT_STATUSES = ["Pending", "Partial", "Paid"];

export default function IncomeFormPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = !!id;
  const { register, handleSubmit, watch, reset } = useForm();
  const [farms, setFarms] = useState([]);
  const [crops, setCrops] = useState([]);
  const selectedFarmId = watch("farmId");

  useEffect(() => {
    farmApi.getAll().then(({ data }) => setFarms(data.data));
    if (isEdit) {
      incomeApi.getById(id).then(({ data }) => reset({ ...data.data, saleDate: data.data.saleDate?.slice(0, 10) }));
    }
  }, [id, isEdit, reset]);

  useEffect(() => {
    if (selectedFarmId) cropApi.getAll(selectedFarmId).then(({ data }) => setCrops(data.data));
  }, [selectedFarmId]);

  const onSubmit = async (data) => {
    const payload = {
      ...data,
      farmId: Number(data.farmId),
      cropId: data.cropId ? Number(data.cropId) : null,
      quantity: Number(data.quantity),
      pricePerUnit: Number(data.pricePerUnit),
      paymentStatus: PAYMENT_STATUSES.indexOf(data.paymentStatus)
    };
    if (isEdit) await incomeApi.update(id, payload);
    else await incomeApi.create(payload);
    navigate("/income");
  };

  return (
    <Card sx={{ maxWidth: 480, mx: "auto" }}>
      <CardContent>
        <Typography variant="h1" gutterBottom>{t("income.addIncome")}</Typography>
        <Box component="form" onSubmit={handleSubmit(onSubmit)}>
          <TextField select fullWidth margin="normal" label="Farm" {...register("farmId", { required: true })} defaultValue="">
            {farms.map((f) => <MenuItem key={f.id} value={f.id}>{f.name}</MenuItem>)}
          </TextField>
          <TextField select fullWidth margin="normal" label="Crop (optional)" {...register("cropId")} defaultValue="">
            <MenuItem value="">None</MenuItem>
            {crops.map((c) => <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>)}
          </TextField>
          <TextField fullWidth margin="normal" label={t("income.buyerName")} {...register("buyerName", { required: true })} />
          <TextField fullWidth margin="normal" label={t("income.quantity")} type="number" inputProps={{ step: "0.01" }} {...register("quantity", { required: true })} />
          <TextField fullWidth margin="normal" label="Unit" defaultValue="kg" {...register("unit")} />
          <TextField fullWidth margin="normal" label={t("income.pricePerUnit")} type="number" inputProps={{ step: "0.01" }} {...register("pricePerUnit", { required: true })} />
          <TextField fullWidth margin="normal" label="Sale Date" type="date" InputLabelProps={{ shrink: true }} {...register("saleDate", { required: true })} />
          <TextField select fullWidth margin="normal" label="Payment Status" defaultValue="Pending" {...register("paymentStatus")}>
            {PAYMENT_STATUSES.map((s) => <MenuItem key={s} value={s}>{s}</MenuItem>)}
          </TextField>
          <TextField fullWidth margin="normal" label="Notes" multiline rows={3} {...register("notes")} />
          {isEdit && <ReceiptManager ownerType="income" ownerId={Number(id)} />}
          <Box sx={{ display: "flex", gap: 1, mt: 2 }}>
            <Button type="submit" variant="contained" fullWidth>{t("common.save")}</Button>
            <Button variant="outlined" fullWidth onClick={() => navigate("/income")}>{t("common.cancel")}</Button>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
}
