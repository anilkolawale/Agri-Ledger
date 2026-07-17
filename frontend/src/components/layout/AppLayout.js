import React, { useState } from "react";
import { Outlet, useNavigate, useLocation } from "react-router-dom";
import {
  AppBar, Toolbar, Typography, Drawer, List, ListItem, ListItemButton, 
  ListItemIcon, ListItemText, Box, Fab, IconButton, Menu, MenuItem,
  Divider, useMediaQuery, useTheme
} from "@mui/material";
import DashboardIcon from "@mui/icons-material/Dashboard";
import AgricultureIcon from "@mui/icons-material/Agriculture";
import GrassIcon from "@mui/icons-material/Grass";
import ReceiptLongIcon from "@mui/icons-material/ReceiptLong";
import PaidIcon from "@mui/icons-material/Paid";
import AddIcon from "@mui/icons-material/Add";
import LanguageIcon from "@mui/icons-material/Language";
import LogoutIcon from "@mui/icons-material/Logout";
import MenuIcon from "@mui/icons-material/Menu";
import GroupsIcon from "@mui/icons-material/Groups";
import StorageIcon from "@mui/icons-material/Storage";
import StorefrontIcon from "@mui/icons-material/Storefront";
import BarChartIcon from "@mui/icons-material/BarChart";
import AdminPanelSettingsIcon from "@mui/icons-material/AdminPanelSettings";
import PersonIcon from "@mui/icons-material/Person";

import { useTranslation } from "react-i18next";

import { useAuth } from "../../context/AuthContext";
import GlobalSearch from "../common/GlobalSearch";

const DRAWER_WIDTH = 240;

const ALL_NAV_ITEMS = [
  { path: "/dashboard", labelKey: "nav.dashboard", icon: <DashboardIcon /> },
  { path: "/farms", labelKey: "nav.farms", icon: <AgricultureIcon /> },
  { path: "/crops", labelKey: "nav.crops", icon: <GrassIcon /> },
  { path: "/expenses", labelKey: "nav.expenses", icon: <ReceiptLongIcon /> },
  { path: "/income", labelKey: "nav.income", icon: <PaidIcon /> },
  { path: "/labor", labelKey: "nav.labor", icon: <GroupsIcon /> },
  { path: "/inventory", labelKey: "nav.inventory", icon: <StorageIcon /> },
  { path: "/reports", labelKey: "nav.reports", icon: <BarChartIcon /> },
  { path: "/profile", labelKey: "nav.profile", icon: <PersonIcon /> }
];



export default function AppLayout() {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const location = useLocation();
  const { logout, user } = useAuth();
  const theme = useTheme();
  
  // Use MUI breakpoint media query to detect screen size
  const isDesktop = useMediaQuery(theme.breakpoints.up("md"));
  const [mobileOpen, setMobileOpen] = useState(false);
  const [anchorEl, setAnchorEl] = useState(null);

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const handleNavigation = (path) => {
    navigate(path);
    if (!isDesktop) {
      setMobileOpen(false);
    }
  };

  const navItems = [...ALL_NAV_ITEMS];
  if (user?.role === "Admin") {
    navItems.push({ path: "/admin", labelKey: "nav.admin", icon: <AdminPanelSettingsIcon /> });
  }

  const drawerContent = (
    <Box sx={{ height: "100%", display: "flex", flexDirection: "column", bgcolor: "#fafafa" }}>
      <Box sx={{ p: 2, display: "flex", flexDirection: "column", alignItems: "center", bgcolor: "primary.main", color: "#fff" }}>
        <Box sx={{ mb: 1 }}>
          <img src="/logo192.png" alt="AgriLedger Logo" style={{ width: 64, height: 64, borderRadius: 12, backgroundColor: "#fff", padding: 4 }} />
        </Box>
        <Typography variant="h6" sx={{ fontWeight: "bold" }}>
          {t("appName")}
        </Typography>
        <Typography variant="caption" sx={{ opacity: 0.8, textTransform: "uppercase", mt: 0.5, fontWeight: "bold" }}>
          {t("tagline")}
        </Typography>
      </Box>

      <Divider />
      <List sx={{ flexGrow: 1, py: 1 }}>
        {navItems.map((item) => {
          const isActive = location.pathname.startsWith(item.path);
          return (
            <ListItem key={item.path} disablePadding sx={{ px: 1, my: 0.5 }}>

              <ListItemButton
                onClick={() => handleNavigation(item.path)}
                sx={{
                  borderRadius: 2,
                  bgcolor: isActive ? "primary.light" : "transparent",
                  color: isActive ? "primary.contrastText" : "text.primary",
                  "&:hover": {
                    bgcolor: isActive ? "primary.light" : "grey.100"
                  },
                  "& .MuiListItemIcon-root": {
                    color: isActive ? "primary.contrastText" : "text.secondary"
                  }
                }}
              >
                <ListItemIcon sx={{ minWidth: 40 }}>
                  {item.icon}
                </ListItemIcon>
                <ListItemText 
                  primary={t(item.labelKey)} 
                  primaryTypographyProps={{ fontSize: "0.95rem", fontWeight: isActive ? "bold" : "medium" }}
                />
              </ListItemButton>
            </ListItem>
          );
        })}
      </List>
      <Divider />
      <List sx={{ p: 1 }}>
        <ListItem disablePadding sx={{ px: 1 }}>
          <ListItemButton 
            onClick={() => { logout(); navigate("/login"); }}
            sx={{ borderRadius: 2, color: "error.main", "& .MuiListItemIcon-root": { color: "error.main" } }}
          >
            <ListItemIcon sx={{ minWidth: 40 }}>
              <LogoutIcon />
            </ListItemIcon>
            <ListItemText primary={t("nav.logout")} primaryTypographyProps={{ fontSize: "0.95rem", fontWeight: "bold" }} />
          </ListItemButton>
        </ListItem>
      </List>
    </Box>
  );

  return (
    <Box sx={{ display: "flex", minHeight: "100vh" }}>
      {/* AppBar */}
      <AppBar 
        position="fixed" 
        sx={{ 
          zIndex: (theme) => theme.zIndex.drawer + 1,
          width: isDesktop ? `calc(100% - ${DRAWER_WIDTH}px)` : "100%",
          ml: isDesktop ? `${DRAWER_WIDTH}px` : 0,
          boxShadow: "0 1px 10px rgba(0,0,0,0.05)"
        }}
      >
        <Toolbar>
          {!isDesktop && (
            <IconButton
              color="inherit"
              aria-label="open drawer"
              edge="start"
              onClick={handleDrawerToggle}
              sx={{ mr: 2 }}
            >
              <MenuIcon />
            </IconButton>
          )}
          <Typography variant="h1" sx={{ flexGrow: 1, fontSize: "1.2rem", fontWeight: "bold" }}>
            {isDesktop ? t("appName") : ""}
          </Typography>
          
          <GlobalSearch />

          <IconButton color="inherit" onClick={(e) => setAnchorEl(e.currentTarget)}>
            <LanguageIcon />
          </IconButton>
          <Menu anchorEl={anchorEl} open={!!anchorEl} onClose={() => setAnchorEl(null)}>
            <MenuItem onClick={() => { i18n.changeLanguage("en"); setAnchorEl(null); }}>English</MenuItem>
            <MenuItem onClick={() => { i18n.changeLanguage("mr"); setAnchorEl(null); }}>मराठी</MenuItem>
          </Menu>
        </Toolbar>
      </AppBar>

      {/* Responsive Drawer navigation sidebar */}
      <Box
        component="nav"
        sx={{ width: isDesktop ? DRAWER_WIDTH : 0, flexShrink: 0 }}
      >
        {isDesktop ? (
          <Drawer
            variant="permanent"
            open
            sx={{
              "& .MuiDrawer-paper": { width: DRAWER_WIDTH, boxSizing: "border-box", borderRight: "1px solid #e0e0e0" }
            }}
          >
            {drawerContent}
          </Drawer>
        ) : (
          <Drawer
            variant="temporary"
            open={mobileOpen}
            onClose={handleDrawerToggle}
            ModalProps={{ keepMounted: true }} // Better open performance on mobile
            sx={{
              "& .MuiDrawer-paper": { width: DRAWER_WIDTH, boxSizing: "border-box" }
            }}
          >
            {drawerContent}
          </Drawer>
        )}
      </Box>

      {/* Main Content Area */}
      <Box 
        component="main" 
        sx={{ 
          flexGrow: 1, 
          p: 3, 
          width: isDesktop ? `calc(100% - ${DRAWER_WIDTH}px)` : "100%",
          mt: 8 // spacing for AppBar
        }}
      >
        <Box sx={{ maxWidth: 960, mx: "auto" }}>
          <Outlet />
        </Box>
      </Box>

      {/* Floating Add Expense Button */}
      <Fab
        color="secondary"
        aria-label="add expense"
        sx={{ position: "fixed", bottom: 20, right: 20, zIndex: 1000 }}
        onClick={() => navigate("/expenses/new")}
      >
        <AddIcon />
      </Fab>
    </Box>
  );
}
