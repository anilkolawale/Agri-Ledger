import React, { useState } from "react";
import { Box, Paper, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";

export default function FinanceChart({ data = [] }) {
  const { i18n } = useTranslation();
  const [hoveredBar, setHoveredBar] = useState(null);

  if (!data || data.length === 0) {
    return (
      <Box sx={{ p: 4, textAlign: "center" }}>
        <Typography color="text.secondary">
          {i18n.language === "mr" ? "तुलना करण्यासाठी कोणताही डेटा उपलब्ध नाही." : "No data available for chart comparison."}
        </Typography>
      </Box>
    );
  }

  // Chart layout parameters
  const width = 600;
  const height = 300;
  const paddingLeft = 60;
  const paddingRight = 20;
  const paddingTop = 30;
  const paddingBottom = 40;

  const chartWidth = width - paddingLeft - paddingRight;
  const chartHeight = height - paddingTop - paddingBottom;

  // Find max value in dataset for scale
  const maxVal = Math.max(
    ...data.map((d) => Math.max(d.expense || 0, d.income || 0)),
    1000 // default min height
  );

  // Round max value up to a neat interval (e.g. 5000, 10000, etc.)
  const magnitude = Math.pow(10, Math.floor(Math.log10(maxVal)));
  const step = magnitude / 2 > 0 ? magnitude / 2 : 100;
  const roundedMax = Math.ceil(maxVal / step) * step;

  // X scale mapping
  const numItems = data.length;
  const barSpacing = chartWidth / numItems;
  const barGroupWidth = barSpacing * 0.7; // 70% of spacing is bars
  const singleBarWidth = barGroupWidth / 2 * 0.8; // gap between expense/income bars

  // Y scale mapping helper
  const getY = (val) => chartHeight - (val / roundedMax) * chartHeight;

  // Generate grid values
  const numGridLines = 4;
  const gridValues = Array.from({ length: numGridLines + 1 }, (_, i) => (roundedMax / numGridLines) * i);

  return (
    <Paper sx={{ p: 3, borderRadius: 3, boxShadow: "0 4px 20px rgba(0,0,0,0.06)", mb: 3 }}>
      <Typography variant="h6" gutterBottom sx={{ fontWeight: "bold" }}>
        {i18n.language === "mr" ? "मासिक उत्पन्न विरुद्ध खर्च तुलना" : "Monthly Income vs Expense Comparison"}
      </Typography>

      <Box sx={{ position: "relative", width: "100%", overflowX: "auto" }}>
        <svg 
          viewBox={`0 0 ${width} ${height}`} 
          width="100%" 
          height="100%"
          style={{ minWidth: 500, overflow: "visible" }}
        >
          {/* Grid lines & Y axis labels */}
          {gridValues.map((val) => {
            const y = paddingTop + getY(val);
            return (
              <g key={val}>
                <line 
                  x1={paddingLeft} 
                  y1={y} 
                  x2={width - paddingRight} 
                  y2={y} 
                  stroke="#e0e0e0" 
                  strokeWidth={1}
                  strokeDasharray={val === 0 ? "none" : "4 4"}
                />
                <text 
                  x={paddingLeft - 8} 
                  y={y + 4} 
                  textAnchor="end" 
                  fill="#757575" 
                  fontSize={11}
                  fontFamily="sans-serif"
                >
                  ₹{val.toLocaleString("en-IN")}
                </text>
              </g>
            );
          })}

          {/* Bar Groups */}
          {data.map((item, idx) => {
            const groupX = paddingLeft + (idx * barSpacing) + (barSpacing - barGroupWidth) / 2;

            // Income bar (green)
            const incomeVal = item.income || 0;
            const incomeH = (incomeVal / roundedMax) * chartHeight;
            const incomeX = groupX;
            const incomeY = paddingTop + getY(incomeVal);

            // Expense bar (red)
            const expenseVal = item.expense || 0;
            const expenseH = (expenseVal / roundedMax) * chartHeight;
            const expenseX = groupX + singleBarWidth + 4;
            const expenseY = paddingTop + getY(expenseVal);

            return (
              <g key={item.label}>
                {/* Income Bar */}
                <rect
                  x={incomeX}
                  y={incomeY}
                  width={singleBarWidth}
                  height={Math.max(incomeH, 2)} // min height 2px for visibility
                  fill="#2e7d32"
                  rx={3}
                  style={{ cursor: "pointer", transition: "all 0.2s" }}
                  onMouseEnter={(e) => setHoveredBar({
                    x: incomeX + singleBarWidth / 2,
                    y: incomeY - 10,
                    text: `${i18n.language === "mr" ? "उत्पन्न" : "Income"}: ₹${incomeVal.toLocaleString("en-IN")}`
                  })}
                  onMouseLeave={() => setHoveredBar(null)}
                />

                {/* Expense Bar */}
                <rect
                  x={expenseX}
                  y={expenseY}
                  width={singleBarWidth}
                  height={Math.max(expenseH, 2)}
                  fill="#d32f2f"
                  rx={3}
                  style={{ cursor: "pointer", transition: "all 0.2s" }}
                  onMouseEnter={(e) => setHoveredBar({
                    x: expenseX + singleBarWidth / 2,
                    y: expenseY - 10,
                    text: `${i18n.language === "mr" ? "खर्च" : "Expense"}: ₹${expenseVal.toLocaleString("en-IN")}`
                  })}
                  onMouseLeave={() => setHoveredBar(null)}
                />

                {/* Month Label */}
                <text
                  x={groupX + singleBarWidth + 2}
                  y={height - paddingBottom + 18}
                  textAnchor="middle"
                  fill="#424242"
                  fontSize={12}
                  fontWeight="bold"
                  fontFamily="sans-serif"
                >
                  {item.label}
                </text>
              </g>
            );
          })}

          {/* Simple Tooltip inside SVG */}
          {hoveredBar && (
            <g>
              <rect
                x={hoveredBar.x - 70}
                y={hoveredBar.y - 25}
                width={140}
                height={22}
                fill="rgba(0, 0, 0, 0.8)"
                rx={4}
              />
              <text
                x={hoveredBar.x}
                y={hoveredBar.y - 10}
                textAnchor="middle"
                fill="#fff"
                fontSize={10.5}
                fontWeight="bold"
                fontFamily="sans-serif"
              >
                {hoveredBar.text}
              </text>
            </g>
          )}
        </svg>
      </Box>

      {/* Legend */}
      <Box sx={{ display: "flex", justifyContent: "center", gap: 3, mt: 2 }}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
          <Box sx={{ width: 16, height: 16, bgcolor: "#2e7d32", borderRadius: 0.5 }} />
          <Typography variant="body2" sx={{ fontWeight: 500 }}>
            {i18n.language === "mr" ? "उत्पन्न (Income)" : "Income"}
          </Typography>
        </Box>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
          <Box sx={{ width: 16, height: 16, bgcolor: "#d32f2f", borderRadius: 0.5 }} />
          <Typography variant="body2" sx={{ fontWeight: 500 }}>
            {i18n.language === "mr" ? "खर्च (Expense)" : "Expense"}
          </Typography>
        </Box>
      </Box>
    </Paper>
  );
}
