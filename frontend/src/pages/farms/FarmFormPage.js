import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import { useNavigate, useParams } from "react-router-dom";
import { Box, Button, Card, CardContent, TextField, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { farmApi } from "../../api/farmApi";

export default function FarmFormPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = !!id;
  const { register, handleSubmit, reset } = useForm();

  useEffect(() => {
    if (isEdit) {
      farmApi.getById(id).then(({ data }) => reset(data.data));
    }
  }, [id, isEdit, reset]);

  const onSubmit = async (data) => {
    const payload = { ...data, areaInAcres: data.areaInAcres ? Number(data.areaInAcres) : null };
    if (isEdit) await farmApi.update(id, payload);
    else await farmApi.create(payload);
    navigate("/farms");
  };

  return (
    <Card sx={{ maxWidth: 480, mx: "auto" }}>
      <CardContent>
        <Typography variant="h1" gutterBottom>{isEdit ? t("common.edit") : t("farm.addFarm")}</Typography>
        <Box component="form" onSubmit={handleSubmit(onSubmit)}>
          <TextField fullWidth margin="normal" label={t("farm.farmName")} {...register("name", { required: true })} />
          <TextField fullWidth margin="normal" label={t("farm.area")} type="number" inputProps={{ step: "0.1" }} {...register("areaInAcres")} />
          <TextField fullWidth margin="normal" label={t("farm.location")} {...register("location")} />
          <TextField fullWidth margin="normal" label="Notes" multiline rows={3} {...register("notes")} />
          <Box sx={{ display: "flex", gap: 1, mt: 2 }}>
            <Button type="submit" variant="contained" fullWidth>{t("common.save")}</Button>
            <Button variant="outlined" fullWidth onClick={() => navigate("/farms")}>{t("common.cancel")}</Button>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
}
