import React, { useEffect, useState } from "react";
import {
  Box, Grid, Card, CardContent, Typography, Table, TableBody, TableCell,
  TableContainer, TableHead, TableRow, Paper, Button, TextField, Select,
  MenuItem, IconButton, Dialog, DialogTitle, DialogContent, DialogActions,
  CircularProgress, Alert, Chip, InputAdornment
} from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import DeleteIcon from "@mui/icons-material/Delete";
import SupervisedUserCircleIcon from "@mui/icons-material/SupervisedUserCircle";
import AgricultureIcon from "@mui/icons-material/Agriculture";
import GrassIcon from "@mui/icons-material/Grass";
import AccountBalanceWalletIcon from "@mui/icons-material/AccountBalanceWallet";
import TrendingDownIcon from "@mui/icons-material/TrendingDown";
import { useTranslation } from "react-i18next";
import { adminApi } from "../../api/adminApi";
import { useAuth } from "../../context/AuthContext";

export default function AdminPanelPage() {
  const { t } = useTranslation();
  const { user: currentUser } = useAuth();
  const [stats, setStats] = useState(null);
  const [users, setUsers] = useState([]);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Dialog State
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);
      const [statsRes, usersRes] = await Promise.all([
        adminApi.getStats(),
        adminApi.getUsers()
      ]);
      setStats(statsRes.data.data);
      setUsers(usersRes.data.data);
    } catch (err) {
      setError(err.response?.data?.message || "Failed to load admin dashboard data.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleRoleChange = async (userId, newRole) => {
    try {
      setError(null);
      await adminApi.updateUserRole(userId, newRole);
      setUsers(users.map(u => u.id === userId ? { ...u, role: newRole } : u));
    } catch (err) {
      setError(err.response?.data?.message || "Failed to update user role.");
    }
  };

  const handleDeleteClick = (userToDelete) => {
    setSelectedUser(userToDelete);
    setDeleteConfirmOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!selectedUser) return;
    try {
      setError(null);
      await adminApi.deleteUser(selectedUser.id);
      setUsers(users.filter(u => u.id !== selectedUser.id));
      setDeleteConfirmOpen(false);
      setSelectedUser(null);
    } catch (err) {
      setError(err.response?.data?.message || "Failed to delete user.");
    }
  };

  const filteredUsers = users.filter(u => {
    const fullName = u.fullName || "";
    const email = u.email || "";
    const phoneNumber = u.phoneNumber || "";
    return (
      fullName.toLowerCase().includes(search.toLowerCase()) ||
      email.toLowerCase().includes(search.toLowerCase()) ||
      phoneNumber.includes(search)
    );
  });

  if (loading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", minHeight: "60vh" }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ pb: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: "bold", mb: 3, color: "primary.main" }}>
        {t("admin.title", "System Administration")}
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {/* Stats Cards */}
      {stats && (
        <Grid container spacing={3} sx={{ mb: 4 }}>
          <Grid item xs={12} sm={6} md={2.4}>
            <Card sx={{ bgcolor: "primary.light", color: "primary.contrastText" }}>
              <CardContent sx={{ display: "flex", alignItems: "center", gap: 2 }}>
                <SupervisedUserCircleIcon sx={{ fontSize: 40 }} />
                <Box>
                  <Typography variant="h5" sx={{ fontWeight: "bold" }}>{stats.totalUsers ?? 0}</Typography>
                  <Typography variant="body2">{t("admin.totalUsers", "Total Users")}</Typography>
                </Box>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} sm={6} md={2.4}>
            <Card sx={{ bgcolor: "info.light", color: "info.contrastText" }}>
              <CardContent sx={{ display: "flex", alignItems: "center", gap: 2 }}>
                <AgricultureIcon sx={{ fontSize: 40 }} />
                <Box>
                  <Typography variant="h5" sx={{ fontWeight: "bold" }}>{stats.totalFarms ?? 0}</Typography>
                  <Typography variant="body2">{t("admin.totalFarms", "Total Farms")}</Typography>
                </Box>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} sm={6} md={2.4}>
            <Card sx={{ bgcolor: "success.light", color: "success.contrastText" }}>
              <CardContent sx={{ display: "flex", alignItems: "center", gap: 2 }}>
                <GrassIcon sx={{ fontSize: 40 }} />
                <Box>
                  <Typography variant="h5" sx={{ fontWeight: "bold" }}>{stats.totalCrops ?? 0}</Typography>
                  <Typography variant="body2">{t("admin.totalCrops", "Total Crops")}</Typography>
                </Box>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} sm={6} md={2.4}>
            <Card sx={{ bgcolor: "warning.light", color: "warning.contrastText" }}>
              <CardContent sx={{ display: "flex", alignItems: "center", gap: 2 }}>
                <AccountBalanceWalletIcon sx={{ fontSize: 40 }} />
                <Box>
                  <Typography variant="h5" sx={{ fontWeight: "bold" }}>₹{(stats.totalIncomes ?? 0).toLocaleString()}</Typography>
                  <Typography variant="body2">{t("admin.totalIncome", "System Income")}</Typography>
                </Box>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} sm={6} md={2.4}>
            <Card sx={{ bgcolor: "error.light", color: "error.contrastText" }}>
              <CardContent sx={{ display: "flex", alignItems: "center", gap: 2 }}>
                <TrendingDownIcon sx={{ fontSize: 40 }} />
                <Box>
                  <Typography variant="h5" sx={{ fontWeight: "bold" }}>₹{(stats.totalExpenses ?? 0).toLocaleString()}</Typography>
                  <Typography variant="body2">{t("admin.totalExpense", "System Expense")}</Typography>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      )}

      {/* User Management Section */}
      <Paper sx={{ p: 3, borderRadius: 2 }}>
        <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", flexWrap: "wrap", gap: 2, mb: 3 }}>
          <Typography variant="h6" sx={{ fontWeight: "bold" }}>
            {t("admin.userManagement", "User Management")}
          </Typography>
          <TextField
            size="small"
            placeholder={t("admin.searchPlaceholder", "Search by name, email...")}
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon sx={{ color: "text.secondary" }} />
                </InputAdornment>
              )
            }}
            sx={{ width: { xs: "100%", sm: 300 } }}
          />
        </Box>

        <TableContainer>
          <Table>
            <TableHead>
              <TableRow sx={{ bgcolor: "grey.100" }}>
                <TableCell sx={{ fontWeight: "bold" }}>{t("admin.fullName", "Full Name")}</TableCell>
                <TableCell sx={{ fontWeight: "bold" }}>{t("admin.email", "Email")}</TableCell>
                <TableCell sx={{ fontWeight: "bold" }}>{t("admin.phone", "Phone")}</TableCell>
                <TableCell sx={{ fontWeight: "bold" }}>{t("admin.role", "Role")}</TableCell>
                <TableCell sx={{ fontWeight: "bold" }}>{t("admin.farms", "Farms Count")}</TableCell>
                <TableCell sx={{ fontWeight: "bold" }}>{t("admin.joined", "Joined Date")}</TableCell>
                <TableCell sx={{ fontWeight: "bold", textAlign: "right" }}>{t("admin.actions", "Actions")}</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filteredUsers.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={7} align="center" sx={{ py: 4 }}>
                    {t("admin.noUsersFound", "No users found")}
                  </TableCell>
                </TableRow>
              ) : (
                filteredUsers.map((u) => (
                  <TableRow key={u.id} sx={{ "&:hover": { bgcolor: "action.hover" } }}>
                    <TableCell sx={{ fontWeight: "medium" }}>{u.fullName}</TableCell>
                    <TableCell>{u.email}</TableCell>
                    <TableCell>{u.phoneNumber || "-"}</TableCell>
                    <TableCell>
                      {u.id === currentUser?.id ? (
                        <Chip label={u.role} color="primary" size="small" />
                      ) : (
                        <Select
                          size="small"
                          value={u.role || "Farmer"}
                          onChange={(e) => handleRoleChange(u.id, e.target.value)}
                          sx={{ minWidth: 100, height: 32 }}
                        >
                          <MenuItem value="Farmer">Farmer</MenuItem>
                          <MenuItem value="Admin">Admin</MenuItem>
                        </Select>
                      )}
                    </TableCell>
                    <TableCell>{u.farmsCount ?? 0}</TableCell>
                    <TableCell>{u.createdAt ? new Date(u.createdAt).toLocaleDateString() : "-"}</TableCell>
                    <TableCell align="right">
                      {u.id !== currentUser?.id && (
                        <IconButton color="error" size="small" onClick={() => handleDeleteClick(u)}>
                          <DeleteIcon />
                        </IconButton>
                      )}
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteConfirmOpen} onClose={() => setDeleteConfirmOpen(false)}>
        <DialogTitle sx={{ fontWeight: "bold" }}>
          {t("admin.deleteTitle", "Delete User?")}
        </DialogTitle>
        <DialogContent>
          <Typography>
            {t("admin.deleteConfirm", "Are you sure you want to delete user")} <strong>{selectedUser?.fullName}</strong> ({selectedUser?.email})? {t("admin.deleteWarning", "This will soft-delete their account.")}
          </Typography>
        </DialogContent>
        <DialogActions sx={{ p: 2 }}>
          <Button onClick={() => setDeleteConfirmOpen(false)} variant="outlined">
            {t("common.cancel", "Cancel")}
          </Button>
          <Button onClick={handleConfirmDelete} variant="contained" color="error">
            {t("common.delete", "Delete")}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
