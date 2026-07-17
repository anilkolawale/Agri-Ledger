import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Dialog, DialogTitle, DialogContent, TextField, List, ListItemButton,
  ListItemText, Typography, IconButton, CircularProgress, Box
} from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import { searchApi } from "../../api/searchApi";

// Implements the spec's "Global Search" across farm, crop, buyer, worker,
// category and expense description in a single lightweight dialog.
export default function GlobalSearch() {
  const navigate = useNavigate();
  const [open, setOpen] = useState(false);
  const [term, setTerm] = useState("");
  const [results, setResults] = useState(null);
  const [loading, setLoading] = useState(false);

  const runSearch = async (value) => {
    setTerm(value);
    if (!value || value.length < 2) { setResults(null); return; }
    setLoading(true);
    try {
      const { data } = await searchApi.search(value);
      setResults(data.data);
    } finally {
      setLoading(false);
    }
  };

  const goTo = (path) => {
    setOpen(false);
    navigate(path);
  };

  const sections = results ? [
    { title: "Farms", items: results.farms, path: (id) => `/farms` },
    { title: "Crops", items: results.crops, path: (id) => `/crops` },
    { title: "Expenses", items: results.expenses, path: (id) => `/expenses/${id}/edit` },
    { title: "Income", items: results.incomes, path: (id) => `/income/${id}/edit` },
    { title: "Labor", items: results.labors, path: (id) => `/labor` }
  ].filter((s) => s.items.length > 0) : [];

  return (
    <>
      <IconButton color="inherit" onClick={() => setOpen(true)}><SearchIcon /></IconButton>
      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>Search</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus fullWidth placeholder="Search farms, crops, buyers, workers, categories..."
            value={term} onChange={(e) => runSearch(e.target.value)} sx={{ mb: 2 }}
          />
          {loading && <Box sx={{ display: "flex", justifyContent: "center", py: 2 }}><CircularProgress size={24} /></Box>}
          {!loading && results && sections.length === 0 && (
            <Typography color="text.secondary">No results found.</Typography>
          )}
          <List dense>
            {sections.map((section) => (
              <Box key={section.title} sx={{ mb: 1 }}>
                <Typography variant="overline" color="text.secondary">{section.title}</Typography>
                {section.items.map((item) => (
                  <ListItemButton key={item.id} onClick={() => goTo(section.path(item.id))}>
                    <ListItemText primary={item.title} secondary={item.subtitle} />
                  </ListItemButton>
                ))}
              </Box>
            ))}
          </List>
        </DialogContent>
      </Dialog>
    </>
  );
}
