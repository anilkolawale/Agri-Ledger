import { createTheme } from "@mui/material/styles";

// Farm-friendly, high-contrast theme: green primary, large touch targets for mobile-first use.
const theme = createTheme({
  palette: {
    primary: { main: "#2e7d32" },
    secondary: { main: "#f9a825" },
    background: { default: "#f5f7f5" }
  },
  typography: {
    fontFamily: `"Roboto", "Segoe UI", sans-serif`,
    h1: { fontSize: "1.6rem", fontWeight: 700 },
    h2: { fontSize: "1.3rem", fontWeight: 600 }
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: { borderRadius: 10, textTransform: "none", fontWeight: 600, minHeight: 44 }
      }
    },
    MuiTextField: {
      defaultProps: { size: "medium" }
    },
    MuiCard: {
      styleOverrides: { root: { borderRadius: 14 } }
    }
  }
});

export default theme;
