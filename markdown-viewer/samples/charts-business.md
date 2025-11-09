# Business Dashboard Examples

Professional Chart.js visualizations for business intelligence, KPIs, and executive dashboards.

---

## Revenue Dashboard - Multi-Line Comparison

**Use Case:** Executive dashboard showing revenue streams and growth trends.

```chart
{
  "type": "line",
  "data": {
    "labels": ["Q1 2023", "Q2 2023", "Q3 2023", "Q4 2023", "Q1 2024", "Q2 2024", "Q3 2024"],
    "datasets": [
      {
        "label": "Product Revenue",
        "data": [2500000, 2800000, 3200000, 3800000, 4100000, 4500000, 4900000],
        "borderColor": "rgb(75, 192, 192)",
        "backgroundColor": "rgba(75, 192, 192, 0.1)",
        "tension": 0.3,
        "fill": true,
        "borderWidth": 3
      },
      {
        "label": "Service Revenue",
        "data": [1200000, 1400000, 1600000, 1900000, 2100000, 2400000, 2700000],
        "borderColor": "rgb(255, 99, 132)",
        "backgroundColor": "rgba(255, 99, 132, 0.1)",
        "tension": 0.3,
        "fill": true,
        "borderWidth": 3
      },
      {
        "label": "Subscription Revenue",
        "data": [800000, 950000, 1100000, 1300000, 1500000, 1750000, 2000000],
        "borderColor": "rgb(153, 102, 255)",
        "backgroundColor": "rgba(153, 102, 255, 0.1)",
        "tension": 0.3,
        "fill": true,
        "borderWidth": 3
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Revenue Streams Analysis - Last 7 Quarters",
        "font": {
          "size": 20,
          "weight": "bold"
        }
      },
      "legend": {
        "display": true,
        "position": "top"
      },
      "tooltip": {
        "mode": "index",
        "intersect": false,
        "callbacks": {
          "label": "function(context) { return context.dataset.label + ': $' + context.parsed.y.toLocaleString(); }"
        }
      }
    },
    "scales": {
      "y": {
        "beginAtZero": true,
        "ticks": {
          "callback": "function(value) { return '$' + (value / 1000000) + 'M'; }"
        }
      }
    },
    "interaction": {
      "mode": "nearest",
      "axis": "x",
      "intersect": false
    }
  }
}
```

---

## Sales Performance by Region

**Use Case:** Compare sales performance across different geographical regions.

```chart
{
  "type": "bar",
  "data": {
    "labels": ["North America", "Europe", "Asia Pacific", "Latin America", "Middle East", "Africa"],
    "datasets": [
      {
        "label": "2023 Sales",
        "data": [5800000, 4200000, 3900000, 1800000, 2100000, 1200000],
        "backgroundColor": "rgba(54, 162, 235, 0.7)",
        "borderColor": "rgb(54, 162, 235)",
        "borderWidth": 2
      },
      {
        "label": "2024 Sales (YTD)",
        "data": [6500000, 4800000, 4700000, 2200000, 2500000, 1500000],
        "backgroundColor": "rgba(75, 192, 192, 0.7)",
        "borderColor": "rgb(75, 192, 192)",
        "borderWidth": 2
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Regional Sales Performance Comparison",
        "font": {
          "size": 20,
          "weight": "bold"
        }
      },
      "legend": {
        "display": true,
        "position": "top"
      },
      "tooltip": {
        "callbacks": {
          "label": "function(context) { return context.dataset.label + ': $' + context.parsed.y.toLocaleString(); }"
        }
      }
    },
    "scales": {
      "y": {
        "beginAtZero": true,
        "ticks": {
          "callback": "function(value) { return '$' + (value / 1000000) + 'M'; }"
        }
      }
    }
  }
}
```

---

## Key Performance Indicators (KPI) Dashboard

**Use Case:** Track multiple KPIs in a single view for executive reporting.

```chart
{
  "type": "bar",
  "data": {
    "labels": ["Revenue Growth", "Customer Retention", "Market Share", "Employee Satisfaction", "Product Quality", "Brand Awareness"],
    "datasets": [
      {
        "label": "Target %",
        "data": [25, 90, 35, 85, 95, 60],
        "backgroundColor": "rgba(201, 203, 207, 0.5)",
        "borderColor": "rgb(201, 203, 207)",
        "borderWidth": 2
      },
      {
        "label": "Actual %",
        "data": [28, 92, 32, 88, 97, 65],
        "backgroundColor": "rgba(75, 192, 192, 0.8)",
        "borderColor": "rgb(75, 192, 192)",
        "borderWidth": 2
      }
    ]
  },
  "options": {
    "indexAxis": "y",
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Q3 2024 KPI Performance vs Targets",
        "font": {
          "size": 20,
          "weight": "bold"
        }
      },
      "legend": {
        "display": true,
        "position": "top"
      },
      "tooltip": {
        "callbacks": {
          "label": "function(context) { return context.dataset.label + ': ' + context.parsed.x + '%'; }"
        }
      }
    },
    "scales": {
      "x": {
        "beginAtZero": true,
        "max": 100,
        "ticks": {
          "callback": "function(value) { return value + '%'; }"
        }
      }
    }
  }
}
```

---

## Monthly Profit & Loss Statement

**Use Case:** Financial reporting showing revenue, costs, and profit margins.

```chart
{
  "type": "bar",
  "data": {
    "labels": ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
    "datasets": [
      {
        "label": "Revenue",
        "data": [850000, 920000, 880000, 1050000, 1120000, 980000, 1200000, 1150000, 1080000, 1300000, 1250000, 1450000],
        "backgroundColor": "rgba(75, 192, 192, 0.7)",
        "borderColor": "rgb(75, 192, 192)",
        "borderWidth": 2,
        "order": 2
      },
      {
        "label": "Costs",
        "data": [620000, 680000, 650000, 750000, 800000, 720000, 850000, 820000, 780000, 920000, 890000, 1050000],
        "backgroundColor": "rgba(255, 99, 132, 0.7)",
        "borderColor": "rgb(255, 99, 132)",
        "borderWidth": 2,
        "order": 2
      },
      {
        "label": "Profit",
        "data": [230000, 240000, 230000, 300000, 320000, 260000, 350000, 330000, 300000, 380000, 360000, 400000],
        "type": "line",
        "borderColor": "rgb(255, 206, 86)",
        "backgroundColor": "rgba(255, 206, 86, 0.2)",
        "borderWidth": 3,
        "tension": 0.4,
        "fill": false,
        "order": 1
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "2024 Monthly Profit & Loss Statement",
        "font": {
          "size": 20,
          "weight": "bold"
        }
      },
      "legend": {
        "display": true,
        "position": "top"
      },
      "tooltip": {
        "mode": "index",
        "intersect": false,
        "callbacks": {
          "label": "function(context) { return context.dataset.label + ': $' + context.parsed.y.toLocaleString(); }"
        }
      }
    },
    "scales": {
      "y": {
        "beginAtZero": true,
        "ticks": {
          "callback": "function(value) { return '$' + (value / 1000) + 'K'; }"
        }
      }
    }
  }
}
```

---

## Customer Acquisition Cost by Channel

**Use Case:** Marketing ROI analysis showing cost efficiency across channels.

```chart
{
  "type": "doughnut",
  "data": {
    "labels": ["Social Media", "Search Ads", "Email Marketing", "Content Marketing", "Referrals", "Direct Sales"],
    "datasets": [
      {
        "label": "Cost per Acquisition",
        "data": [45, 89, 23, 67, 12, 105],
        "backgroundColor": [
          "rgba(255, 99, 132, 0.8)",
          "rgba(54, 162, 235, 0.8)",
          "rgba(255, 206, 86, 0.8)",
          "rgba(75, 192, 192, 0.8)",
          "rgba(153, 102, 255, 0.8)",
          "rgba(255, 159, 64, 0.8)"
        ],
        "borderColor": [
          "rgb(255, 99, 132)",
          "rgb(54, 162, 235)",
          "rgb(255, 206, 86)",
          "rgb(75, 192, 192)",
          "rgb(153, 102, 255)",
          "rgb(255, 159, 64)"
        ],
        "borderWidth": 2,
        "hoverOffset": 15
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Customer Acquisition Cost by Channel ($)",
        "font": {
          "size": 20,
          "weight": "bold"
        }
      },
      "legend": {
        "display": true,
        "position": "right"
      },
      "tooltip": {
        "callbacks": {
          "label": "function(context) { return context.label + ': $' + context.parsed; }"
        }
      }
    }
  }
}
```

---

## Employee Productivity Metrics

**Use Case:** HR dashboard tracking employee performance across departments.

```chart
{
  "type": "radar",
  "data": {
    "labels": ["Task Completion", "Quality Score", "Collaboration", "Innovation", "Time Management", "Customer Feedback"],
    "datasets": [
      {
        "label": "Engineering",
        "data": [92, 88, 85, 95, 87, 90],
        "backgroundColor": "rgba(54, 162, 235, 0.3)",
        "borderColor": "rgb(54, 162, 235)",
        "borderWidth": 3,
        "pointBackgroundColor": "rgb(54, 162, 235)",
        "pointBorderColor": "#fff",
        "pointHoverBackgroundColor": "#fff",
        "pointHoverBorderColor": "rgb(54, 162, 235)",
        "pointRadius": 5
      },
      {
        "label": "Sales",
        "data": [88, 85, 92, 80, 90, 95],
        "backgroundColor": "rgba(255, 99, 132, 0.3)",
        "borderColor": "rgb(255, 99, 132)",
        "borderWidth": 3,
        "pointBackgroundColor": "rgb(255, 99, 132)",
        "pointBorderColor": "#fff",
        "pointHoverBackgroundColor": "#fff",
        "pointHoverBorderColor": "rgb(255, 99, 132)",
        "pointRadius": 5
      },
      {
        "label": "Marketing",
        "data": [85, 90, 88, 92, 84, 87],
        "backgroundColor": "rgba(75, 192, 192, 0.3)",
        "borderColor": "rgb(75, 192, 192)",
        "borderWidth": 3,
        "pointBackgroundColor": "rgb(75, 192, 192)",
        "pointBorderColor": "#fff",
        "pointHoverBackgroundColor": "#fff",
        "pointHoverBorderColor": "rgb(75, 192, 192)",
        "pointRadius": 5
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Department Performance Metrics (0-100)",
        "font": {
          "size": 20,
          "weight": "bold"
        }
      },
      "legend": {
        "display": true,
        "position": "top"
      }
    },
    "scales": {
      "r": {
        "beginAtZero": true,
        "max": 100,
        "ticks": {
          "stepSize": 20,
          "backdropColor": "rgba(255, 255, 255, 0.8)"
        }
      }
    }
  }
}
```

---

## Summary

These business-focused examples demonstrate:

1. **Revenue Analysis** - Multi-stream revenue tracking with growth trends
2. **Regional Performance** - Geographical sales comparison
3. **KPI Dashboard** - Target vs actual performance metrics
4. **P&L Statement** - Financial overview with mixed chart types
5. **Marketing ROI** - Cost efficiency across acquisition channels
6. **HR Metrics** - Employee and department performance tracking

Perfect for executive dashboards, board presentations, and stakeholder reports.
