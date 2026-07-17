import React, { useEffect, useState } from "react";
import { Box, Typography, Button, List, ListItem, ListItemText, IconButton, Link } from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import CameraAltIcon from "@mui/icons-material/CameraAlt";
import { receiptApi } from "../../api/receiptApi";

// Camera/photo/bill upload widget used on Expense and Income forms — matches the
// spec's "capture photos, upload bills/invoices, preview/download/delete" requirement.
export default function ReceiptManager({ ownerType, ownerId }) {
  const [receipts, setReceipts] = useState([]);
  const [uploading, setUploading] = useState(false);

  const load = () => {
    const getFn = ownerType === "expense" ? receiptApi.getForExpense : receiptApi.getForIncome;
    getFn(ownerId).then(({ data }) => setReceipts(data.data));
  };

  useEffect(() => { if (ownerId) load(); }, [ownerId]);

  const handleUpload = async (e) => {
    const file = e.target.files?.[0];
    if (!file) return;
    setUploading(true);
    try {
      const uploadFn = ownerType === "expense" ? receiptApi.uploadForExpense : receiptApi.uploadForIncome;
      await uploadFn(ownerId, file);
      load();
    } finally {
      setUploading(false);
      e.target.value = "";
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Delete this receipt?")) return;
    await receiptApi.remove(id);
    load();
  };

  if (!ownerId) return null;

  return (
    <Box sx={{ mt: 2 }}>
      <Typography variant="subtitle2" gutterBottom>Receipts / Bills</Typography>
      <List dense>
        {receipts.map((r) => (
          <ListItem key={r.id} secondaryAction={
            <IconButton edge="end" size="small" onClick={() => handleDelete(r.id)}><DeleteIcon fontSize="small" /></IconButton>
          }>
            <ListItemText primary={<Link href={r.fileUrl} target="_blank" rel="noreferrer">{r.fileName}</Link>} secondary={`${(r.fileSizeBytes / 1024).toFixed(0)} KB`} />
          </ListItem>
        ))}
      </List>
      <Button component="label" variant="outlined" size="small" startIcon={<CameraAltIcon />} disabled={uploading}>
        {uploading ? "Uploading..." : "Upload Photo / Bill"}
        <input type="file" hidden accept=".jpg,.jpeg,.png,.pdf" onChange={handleUpload} />
      </Button>
    </Box>
  );
}
