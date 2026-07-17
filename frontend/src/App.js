import React from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { ThemeProvider, CssBaseline } from "@mui/material";
import theme from "./theme/theme";
import { AuthProvider } from "./context/AuthContext";
import ErrorBoundary from "./components/common/ErrorBoundary";
import ProtectedRoute from "./routes/ProtectedRoute";
import AppLayout from "./components/layout/AppLayout";

import LoginPage from "./pages/auth/LoginPage";
import RegisterPage from "./pages/auth/RegisterPage";
import ForgotPasswordPage from "./pages/auth/ForgotPasswordPage";

import DashboardPage from "./pages/dashboard/DashboardPage";
import FarmsListPage from "./pages/farms/FarmsListPage";
import FarmFormPage from "./pages/farms/FarmFormPage";
import CropsListPage from "./pages/crops/CropsListPage";
import CropFormPage from "./pages/crops/CropFormPage";
import ExpensesListPage from "./pages/expenses/ExpensesListPage";
import ExpenseFormPage from "./pages/expenses/ExpenseFormPage";
import IncomeListPage from "./pages/income/IncomeListPage";
import IncomeFormPage from "./pages/income/IncomeFormPage";
import LaborListPage from "./pages/labor/LaborListPage";
import LaborFormPage from "./pages/labor/LaborFormPage";
import InventoryListPage from "./pages/inventory/InventoryListPage";
import InventoryFormPage from "./pages/inventory/InventoryFormPage";
import ReportsPage from "./pages/reports/ReportsPage";
import AdminPanelPage from "./pages/admin/AdminPanelPage";
import ProfilePage from "./pages/profile/ProfilePage";




export default function App() {
  return (
    <ErrorBoundary>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <AuthProvider>
          <BrowserRouter>
          <Routes>
            {/* Public routes */}
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/forgot-password" element={<ForgotPasswordPage />} />

            {/* Protected routes - require a valid JWT session */}
            <Route element={<ProtectedRoute />}>
              <Route element={<AppLayout />}>
                <Route path="/dashboard" element={<DashboardPage />} />

                <Route path="/farms" element={<FarmsListPage />} />
                <Route path="/farms/new" element={<FarmFormPage />} />
                <Route path="/farms/:id/edit" element={<FarmFormPage />} />

                <Route path="/crops" element={<CropsListPage />} />
                <Route path="/crops/new" element={<CropFormPage />} />
                <Route path="/crops/:id/edit" element={<CropFormPage />} />

                <Route path="/expenses" element={<ExpensesListPage />} />
                <Route path="/expenses/new" element={<ExpenseFormPage />} />
                <Route path="/expenses/:id/edit" element={<ExpenseFormPage />} />

                <Route path="/income" element={<IncomeListPage />} />
                <Route path="/income/new" element={<IncomeFormPage />} />
                <Route path="/income/:id/edit" element={<IncomeFormPage />} />

                <Route path="/labor" element={<LaborListPage />} />
                <Route path="/labor/new" element={<LaborFormPage />} />
                <Route path="/labor/:id/edit" element={<LaborFormPage />} />

                <Route path="/inventory" element={<InventoryListPage />} />
                <Route path="/inventory/new" element={<InventoryFormPage />} />
                <Route path="/inventory/:id/edit" element={<InventoryFormPage />} />

                <Route path="/reports" element={<ReportsPage />} />
                <Route path="/admin" element={<AdminPanelPage />} />
                <Route path="/profile" element={<ProfilePage />} />



              </Route>
            </Route>


            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            <Route path="*" element={<Navigate to="/dashboard" replace />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </ThemeProvider>
    </ErrorBoundary>
  );
}
