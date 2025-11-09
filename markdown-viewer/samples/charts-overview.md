# Chart.js Overview - All Chart Types

This document showcases all major Chart.js chart types with impressive, colorful examples.

---

## Line Chart - Monthly Website Traffic

**Use Case:** Track trends over time, perfect for analytics dashboards.

```chart
{
  "type": "line",
  "data": {
    "labels": ["January", "February", "March", "April", "May", "June", "July", "August"],
    "datasets": [
      {
        "label": "Unique Visitors",
        "data": [12500, 19300, 15800, 22400, 28900, 32100, 29800, 35600],
        "borderColor": "rgb(75, 192, 192)",
        "backgroundColor": "rgba(75, 192, 192, 0.2)",
        "tension": 0.4,
        "fill": true
      },
      {
        "label": "Page Views",
        "data": [45000, 58000, 51000, 72000, 89000, 98000, 91000, 105000],
        "borderColor": "rgb(255, 99, 132)",
        "backgroundColor": "rgba(255, 99, 132, 0.2)",
        "tension": 0.4,
        "fill": true
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Website Traffic Analytics - 2024",
        "font": {
          "size": 18
        }
      },
      "legend": {
        "display": true,
        "position": "top"
      },
      "tooltip": {
        "mode": "index",
        "intersect": false
      }
    },
    "scales": {
      "y": {
        "beginAtZero": true,
        "ticks": {
          "callback": "function(value) { return value.toLocaleString(); }"
        }
      }
    }
  }
}
```

---

## Bar Chart - Product Sales Comparison

**Use Case:** Compare values across categories, ideal for sales reports.

```chart
{
  "type": "bar",
  "data": {
    "labels": ["Product A", "Product B", "Product C", "Product D", "Product E", "Product F"],
    "datasets": [
      {
        "label": "Q1 Sales",
        "data": [65000, 59000, 80000, 81000, 56000, 72000],
        "backgroundColor": "rgba(255, 99, 132, 0.8)",
        "borderColor": "rgb(255, 99, 132)",
        "borderWidth": 2
      },
      {
        "label": "Q2 Sales",
        "data": [78000, 68000, 95000, 89000, 62000, 85000],
        "backgroundColor": "rgba(54, 162, 235, 0.8)",
        "borderColor": "rgb(54, 162, 235)",
        "borderWidth": 2
      },
      {
        "label": "Q3 Sales",
        "data": [82000, 71000, 102000, 94000, 68000, 91000],
        "backgroundColor": "rgba(255, 206, 86, 0.8)",
        "borderColor": "rgb(255, 206, 86)",
        "borderWidth": 2
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Quarterly Product Sales Comparison ($)",
        "font": {
          "size": 18
        }
      },
      "legend": {
        "display": true,
        "position": "top"
      }
    },
    "scales": {
      "y": {
        "beginAtZero": true,
        "ticks": {
          "callback": "function(value) { return '$' + value.toLocaleString(); }"
        }
      }
    }
  }
}
```

---

## Pie Chart - Market Share Distribution

**Use Case:** Show proportional data, perfect for market analysis.

```chart
{
  "type": "pie",
  "data": {
    "labels": ["Company A", "Company B", "Company C", "Company D", "Others"],
    "datasets": [
      {
        "label": "Market Share",
        "data": [35, 25, 18, 12, 10],
        "backgroundColor": [
          "rgba(255, 99, 132, 0.9)",
          "rgba(54, 162, 235, 0.9)",
          "rgba(255, 206, 86, 0.9)",
          "rgba(75, 192, 192, 0.9)",
          "rgba(153, 102, 255, 0.9)"
        ],
        "borderColor": [
          "rgb(255, 99, 132)",
          "rgb(54, 162, 235)",
          "rgb(255, 206, 86)",
          "rgb(75, 192, 192)",
          "rgb(153, 102, 255)"
        ],
        "borderWidth": 2
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Smartphone Market Share 2024 (%)",
        "font": {
          "size": 18
        }
      },
      "legend": {
        "display": true,
        "position": "right"
      },
      "tooltip": {
        "callbacks": {
          "label": "function(context) { return context.label + ': ' + context.parsed + '%'; }"
        }
      }
    }
  }
}
```

---

## Doughnut Chart - Budget Allocation

**Use Case:** Similar to pie charts but with a center hole, great for financial breakdowns.

```chart
{
  "type": "doughnut",
  "data": {
    "labels": ["Development", "Marketing", "Operations", "Sales", "R&D", "Administration"],
    "datasets": [
      {
        "label": "Budget Allocation",
        "data": [300000, 250000, 180000, 220000, 150000, 100000],
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
        "hoverOffset": 10
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Annual Budget Allocation - $1.2M Total",
        "font": {
          "size": 18
        }
      },
      "legend": {
        "display": true,
        "position": "bottom"
      },
      "tooltip": {
        "callbacks": {
          "label": "function(context) { return context.label + ': $' + context.parsed.toLocaleString(); }"
        }
      }
    }
  }
}
```

---

## Radar Chart - Technology Skills Assessment

**Use Case:** Compare multiple variables across different categories, ideal for performance reviews.

```chart
{
  "type": "radar",
  "data": {
    "labels": ["JavaScript", "Python", "SQL", "Docker", "Cloud Services", "Security", "Testing", "DevOps"],
    "datasets": [
      {
        "label": "Developer A",
        "data": [95, 70, 85, 90, 75, 80, 88, 82],
        "backgroundColor": "rgba(255, 99, 132, 0.3)",
        "borderColor": "rgb(255, 99, 132)",
        "borderWidth": 3,
        "pointBackgroundColor": "rgb(255, 99, 132)",
        "pointBorderColor": "#fff",
        "pointHoverBackgroundColor": "#fff",
        "pointHoverBorderColor": "rgb(255, 99, 132)"
      },
      {
        "label": "Developer B",
        "data": [80, 92, 78, 85, 88, 75, 82, 90],
        "backgroundColor": "rgba(54, 162, 235, 0.3)",
        "borderColor": "rgb(54, 162, 235)",
        "borderWidth": 3,
        "pointBackgroundColor": "rgb(54, 162, 235)",
        "pointBorderColor": "#fff",
        "pointHoverBackgroundColor": "#fff",
        "pointHoverBorderColor": "rgb(54, 162, 235)"
      },
      {
        "label": "Team Average",
        "data": [85, 80, 80, 85, 80, 78, 85, 85],
        "backgroundColor": "rgba(75, 192, 192, 0.3)",
        "borderColor": "rgb(75, 192, 192)",
        "borderWidth": 3,
        "pointBackgroundColor": "rgb(75, 192, 192)",
        "pointBorderColor": "#fff",
        "pointHoverBackgroundColor": "#fff",
        "pointHoverBorderColor": "rgb(75, 192, 192)"
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Technical Skills Assessment (0-100)",
        "font": {
          "size": 18
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
          "stepSize": 20
        }
      }
    }
  }
}
```

---

## Polar Area Chart - Customer Satisfaction Scores

**Use Case:** Show multivariate data with both angle and radius representing values.

```chart
{
  "type": "polarArea",
  "data": {
    "labels": ["Product Quality", "Customer Service", "Delivery Speed", "Price Value", "User Experience", "Brand Trust"],
    "datasets": [
      {
        "label": "Satisfaction Score",
        "data": [92, 88, 78, 85, 90, 94],
        "backgroundColor": [
          "rgba(255, 99, 132, 0.7)",
          "rgba(54, 162, 235, 0.7)",
          "rgba(255, 206, 86, 0.7)",
          "rgba(75, 192, 192, 0.7)",
          "rgba(153, 102, 255, 0.7)",
          "rgba(255, 159, 64, 0.7)"
        ],
        "borderColor": [
          "rgb(255, 99, 132)",
          "rgb(54, 162, 235)",
          "rgb(255, 206, 86)",
          "rgb(75, 192, 192)",
          "rgb(153, 102, 255)",
          "rgb(255, 159, 64)"
        ],
        "borderWidth": 2
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Customer Satisfaction Survey Results (0-100)",
        "font": {
          "size": 18
        }
      },
      "legend": {
        "display": true,
        "position": "right"
      },
      "tooltip": {
        "callbacks": {
          "label": "function(context) { return context.label + ': ' + context.parsed.r + '/100'; }"
        }
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

This overview demonstrates the six primary Chart.js chart types:

1. **Line Chart** - Best for showing trends and changes over time
2. **Bar Chart** - Perfect for comparing discrete categories
3. **Pie Chart** - Ideal for showing parts of a whole
4. **Doughnut Chart** - Similar to pie, with a modern aesthetic
5. **Radar Chart** - Excellent for multivariate data comparison
6. **Polar Area Chart** - Combines features of pie and radar charts

Each chart type serves specific visualization needs. Choose based on your data structure and the story you want to tell.
