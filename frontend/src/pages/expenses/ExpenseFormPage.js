import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate, useParams } from "react-router-dom";
import { Box, Button, Card, CardContent, TextField, Typography, MenuItem } from "@mui/material";
import { useTranslation } from "react-i18next";
import { expenseApi } from "../../api/expenseApi";
import { farmApi } from "../../api/farmApi";
import { cropApi } from "../../api/cropApi";
import ReceiptManager from "../../components/common/ReceiptManager";

const PAYMENT_METHODS = ["Cash", "UPI", "Bank", "Cheque", "Other"];

// Fields mirror the "Add Expense" mockup: Farm, Crop, Category, Amount, Date, Time,
// Payment Method, Description (+ Receipt upload documented as a follow-up module).
export default function ExpenseFormPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = !!id;
  const { register, handleSubmit, watch, reset } = useForm();
  const [farms, setFarms] = useState([]);
  const [crops, setCrops] = useState([]);
  const [categories, setCategories] = useState([]);
  const selectedFarmId = watch("farmId");

  useEffect(() => {
    farmApi.getAll().then(({ data }) => setFarms(data.data));
    expenseApi.getCategories().then(({ data }) => setCategories(data.data));
    if (isEdit) {
      expenseApi.getById(id).then(({ data }) => reset({
        ...data.data,
        expenseDate: data.data.expenseDate?.slice(0, 10)
      }));
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
      expenseCategoryId: Number(data.expenseCategoryId),
      amount: Number(data.amount),
      paymentMethod: PAYMENT_METHODS.indexOf(data.paymentMethod)
    };
    if (isEdit) await expenseApi.update(id, payload);
    else await expenseApi.create(payload);
    navigate("/expenses");
  };

  return (
    <Card sx={{ maxWidth: 480, mx: "auto" }}>
      <CardContent>
        <Typography variant="h1" gutterBottom>{t("expense.addExpense")}</Typography>
        <Box component="form" onSubmit={handleSubmit(onSubmit)}>
          <TextField select fullWidth margin="normal" label="Farm" {...register("farmId", { required: true })} defaultValue="">
            {farms.map((f) => <MenuItem key={f.id} value={f.id}>{f.name}</MenuItem>)}
          </TextField>
          <TextField select fullWidth margin="normal" label="Crop (optional)" {...register("cropId")} defaultValue="">
            <MenuItem value="">None</MenuItem>
            {crops.map((c) => <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>)}
          </TextField>
          <TextField select fullWidth margin="normal" label={t("expense.category")} {...register("expenseCategoryId", { required: true })} defaultValue="">
            {categories.map((c) => <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>)}
          </TextField>
          <TextField fullWidth margin="normal" label={t("expense.amount")} type="number" inputProps={{ step: "0.01" }} {...register("amount", { required: true })} />
          <TextField fullWidth margin="normal" label={t("expense.date")} type="date" InputLabelProps={{ shrink: true }} {...register("expenseDate", { required: true })} />
          <TextField fullWidth margin="normal" label="Time" type="time" InputLabelProps={{ shrink: true }} {...register("expenseTime")} />
          <TextField select fullWidth margin="normal" label={t("expense.paymentMethod")} defaultValue="Cash" {...register("paymentMethod")}>
            {PAYMENT_METHODS.map((m) => <MenuItem key={m} value={m}>{m}</MenuItem>)}
          </TextField>
          <TextField fullWidth margin="normal" label="Description" multiline rows={3} {...register("description")} />
          {isEdit && <ReceiptManager ownerType="expense" ownerId={Number(id)} />}
          {!isEdit && (
            <Typography variant="caption" color="text.secondary" sx={{ display: "block", mt: 1 }}>
              Save this expense first, then reopen it to attach a photo or bill.
            </Typography>
          )}
          <Box sx={{ display: "flex", gap: 1, mt: 2 }}>
            <Button type="submit" variant="contained" fullWidth>{t("common.save")}</Button>
            <Button variant="outlined" fullWidth onClick={() => navigate("/expenses")}>{t("common.cancel")}</Button>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
}
