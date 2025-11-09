# Data Science Visualizations

Advanced Chart.js examples for data analysis, machine learning, and scientific research.

---

## Scatter Plot - Correlation Analysis

**Use Case:** Analyze relationships between two continuous variables, identify patterns and outliers.

```chart
{
  "type": "scatter",
  "data": {
    "datasets": [
      {
        "label": "High Performance Users",
        "data": [
          {"x": 25, "y": 85},
          {"x": 32, "y": 92},
          {"x": 28, "y": 88},
          {"x": 35, "y": 95},
          {"x": 30, "y": 90},
          {"x": 38, "y": 98},
          {"x": 33, "y": 93},
          {"x": 36, "y": 96},
          {"x": 40, "y": 99},
          {"x": 29, "y": 87}
        ],
        "backgroundColor": "rgba(75, 192, 192, 0.7)",
        "borderColor": "rgb(75, 192, 192)",
        "pointRadius": 6,
        "pointHoverRadius": 8
      },
      {
        "label": "Medium Performance Users",
        "data": [
          {"x": 18, "y": 65},
          {"x": 22, "y": 72},
          {"x": 20, "y": 68},
          {"x": 25, "y": 75},
          {"x": 23, "y": 70},
          {"x": 19, "y": 67},
          {"x": 21, "y": 69},
          {"x": 24, "y": 73},
          {"x": 26, "y": 76},
          {"x": 27, "y": 78}
        ],
        "backgroundColor": "rgba(255, 206, 86, 0.7)",
        "borderColor": "rgb(255, 206, 86)",
        "pointRadius": 6,
        "pointHoverRadius": 8
      },
      {
        "label": "Low Performance Users",
        "data": [
          {"x": 8, "y": 45},
          {"x": 12, "y": 52},
          {"x": 10, "y": 48},
          {"x": 15, "y": 58},
          {"x": 13, "y": 55},
          {"x": 9, "y": 47},
          {"x": 11, "y": 50},
          {"x": 14, "y": 56},
          {"x": 16, "y": 60},
          {"x": 17, "y": 62}
        ],
        "backgroundColor": "rgba(255, 99, 132, 0.7)",
        "borderColor": "rgb(255, 99, 132)",
        "pointRadius": 6,
        "pointHoverRadius": 8
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "User Engagement vs Conversion Rate Correlation",
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
          "label": "function(context) { return context.dataset.label + ': (' + context.parsed.x + ' hours, ' + context.parsed.y + '% conversion)'; }"
        }
      }
    },
    "scales": {
      "x": {
        "type": "linear",
        "position": "bottom",
        "title": {
          "display": true,
          "text": "Weekly Usage Hours"
        },
        "beginAtZero": true
      },
      "y": {
        "title": {
          "display": true,
          "text": "Conversion Rate (%)"
        },
        "beginAtZero": true,
        "max": 100
      }
    }
  }
}
```

---

## Bubble Chart - Multi-Dimensional Analysis

**Use Case:** Visualize three dimensions of data simultaneously (x, y, and size).

```chart
{
  "type": "bubble",
  "data": {
    "datasets": [
      {
        "label": "North America",
        "data": [
          {"x": 85000, "y": 4.2, "r": 25},
          {"x": 92000, "y": 4.5, "r": 30},
          {"x": 78000, "y": 3.8, "r": 20},
          {"x": 105000, "y": 4.8, "r": 35}
        ],
        "backgroundColor": "rgba(255, 99, 132, 0.6)",
        "borderColor": "rgb(255, 99, 132)",
        "borderWidth": 2
      },
      {
        "label": "Europe",
        "data": [
          {"x": 72000, "y": 4.0, "r": 22},
          {"x": 88000, "y": 4.4, "r": 28},
          {"x": 65000, "y": 3.6, "r": 18},
          {"x": 95000, "y": 4.6, "r": 32}
        ],
        "backgroundColor": "rgba(54, 162, 235, 0.6)",
        "borderColor": "rgb(54, 162, 235)",
        "borderWidth": 2
      },
      {
        "label": "Asia Pacific",
        "data": [
          {"x": 55000, "y": 3.5, "r": 28},
          {"x": 68000, "y": 4.1, "r": 35},
          {"x": 48000, "y": 3.2, "r": 24},
          {"x": 75000, "y": 4.3, "r": 38}
        ],
        "backgroundColor": "rgba(75, 192, 192, 0.6)",
        "borderColor": "rgb(75, 192, 192)",
        "borderWidth": 2
      },
      {
        "label": "Latin America",
        "data": [
          {"x": 42000, "y": 3.8, "r": 15},
          {"x": 52000, "y": 4.2, "r": 18},
          {"x": 38000, "y": 3.4, "r": 12},
          {"x": 58000, "y": 4.4, "r": 20}
        ],
        "backgroundColor": "rgba(255, 206, 86, 0.6)",
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
        "text": "Regional Market Analysis: Salary vs Satisfaction vs Team Size",
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
          "label": "function(context) { var label = context.dataset.label || ''; if (label) { label += ': '; } label += 'Salary: $' + context.parsed.x.toLocaleString() + ', Satisfaction: ' + context.parsed.y + '/5, Team Size: ' + context.raw.r; return label; }"
        }
      }
    },
    "scales": {
      "x": {
        "title": {
          "display": true,
          "text": "Average Salary ($)"
        },
        "ticks": {
          "callback": "function(value) { return '$' + (value / 1000) + 'K'; }"
        }
      },
      "y": {
        "title": {
          "display": true,
          "text": "Employee Satisfaction (1-5)"
        },
        "min": 3,
        "max": 5
      }
    }
  }
}
```

---

## Mixed Chart - Time Series with Predictions

**Use Case:** Combine historical data with forecasts and confidence intervals.

```chart
{
  "type": "line",
  "data": {
    "labels": ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
    "datasets": [
      {
        "label": "Actual Data",
        "data": [1200, 1350, 1280, 1500, 1650, 1580, 1720, 1850, null, null, null, null],
        "borderColor": "rgb(75, 192, 192)",
        "backgroundColor": "rgba(75, 192, 192, 0.2)",
        "borderWidth": 3,
        "tension": 0.4,
        "fill": false,
        "pointRadius": 5
      },
      {
        "label": "Predicted Data",
        "data": [null, null, null, null, null, null, null, 1850, 1920, 2050, 2180, 2300],
        "borderColor": "rgb(255, 159, 64)",
        "backgroundColor": "rgba(255, 159, 64, 0.2)",
        "borderWidth": 3,
        "borderDash": [10, 5],
        "tension": 0.4,
        "fill": false,
        "pointRadius": 5
      },
      {
        "label": "Upper Confidence",
        "data": [null, null, null, null, null, null, null, 1900, 2020, 2180, 2340, 2480],
        "borderColor": "rgba(255, 99, 132, 0.3)",
        "backgroundColor": "rgba(255, 99, 132, 0.1)",
        "borderWidth": 1,
        "borderDash": [5, 5],
        "tension": 0.4,
        "fill": "+1",
        "pointRadius": 0
      },
      {
        "label": "Lower Confidence",
        "data": [null, null, null, null, null, null, null, 1800, 1820, 1920, 2020, 2120],
        "borderColor": "rgba(255, 99, 132, 0.3)",
        "backgroundColor": "rgba(255, 99, 132, 0.1)",
        "borderWidth": 1,
        "borderDash": [5, 5],
        "tension": 0.4,
        "fill": false,
        "pointRadius": 0
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Sales Forecast with 95% Confidence Interval",
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
        "intersect": false
      }
    },
    "scales": {
      "y": {
        "beginAtZero": false,
        "title": {
          "display": true,
          "text": "Sales Units"
        }
      }
    }
  }
}
```

---

## Distribution Analysis - Histogram Style

**Use Case:** Show frequency distribution of continuous data.

```chart
{
  "type": "bar",
  "data": {
    "labels": ["0-10", "10-20", "20-30", "30-40", "40-50", "50-60", "60-70", "70-80", "80-90", "90-100"],
    "datasets": [
      {
        "label": "Frequency Distribution",
        "data": [45, 120, 280, 450, 680, 720, 580, 380, 180, 65],
        "backgroundColor": [
          "rgba(255, 99, 132, 0.8)",
          "rgba(255, 159, 64, 0.8)",
          "rgba(255, 205, 86, 0.8)",
          "rgba(75, 192, 192, 0.8)",
          "rgba(54, 162, 235, 0.8)",
          "rgba(153, 102, 255, 0.8)",
          "rgba(201, 203, 207, 0.8)",
          "rgba(255, 99, 132, 0.8)",
          "rgba(255, 159, 64, 0.8)",
          "rgba(255, 205, 86, 0.8)"
        ],
        "borderColor": "rgb(75, 192, 192)",
        "borderWidth": 1
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Test Score Distribution (N=3,500 Students)",
        "font": {
          "size": 20,
          "weight": "bold"
        }
      },
      "legend": {
        "display": false
      },
      "tooltip": {
        "callbacks": {
          "label": "function(context) { return 'Students: ' + context.parsed.y; }"
        }
      }
    },
    "scales": {
      "x": {
        "title": {
          "display": true,
          "text": "Score Range"
        }
      },
      "y": {
        "beginAtZero": true,
        "title": {
          "display": true,
          "text": "Number of Students"
        }
      }
    }
  }
}
```

---

## Multi-Variate Time Series Comparison

**Use Case:** Track multiple related metrics over time for pattern analysis.

```chart
{
  "type": "line",
  "data": {
    "labels": ["Week 1", "Week 2", "Week 3", "Week 4", "Week 5", "Week 6", "Week 7", "Week 8"],
    "datasets": [
      {
        "label": "Model Accuracy (%)",
        "data": [78.5, 82.3, 85.1, 87.8, 89.2, 90.5, 91.3, 92.1],
        "borderColor": "rgb(75, 192, 192)",
        "backgroundColor": "rgba(75, 192, 192, 0.1)",
        "yAxisID": "y",
        "tension": 0.4,
        "borderWidth": 3,
        "pointRadius": 5
      },
      {
        "label": "Training Loss",
        "data": [0.85, 0.68, 0.52, 0.41, 0.35, 0.28, 0.23, 0.19],
        "borderColor": "rgb(255, 99, 132)",
        "backgroundColor": "rgba(255, 99, 132, 0.1)",
        "yAxisID": "y1",
        "tension": 0.4,
        "borderWidth": 3,
        "pointRadius": 5
      },
      {
        "label": "Validation Loss",
        "data": [0.90, 0.75, 0.61, 0.52, 0.47, 0.42, 0.39, 0.36],
        "borderColor": "rgb(255, 159, 64)",
        "backgroundColor": "rgba(255, 159, 64, 0.1)",
        "yAxisID": "y1",
        "tension": 0.4,
        "borderWidth": 3,
        "pointRadius": 5
      },
      {
        "label": "F1 Score",
        "data": [0.72, 0.78, 0.82, 0.85, 0.87, 0.89, 0.90, 0.91],
        "borderColor": "rgb(153, 102, 255)",
        "backgroundColor": "rgba(153, 102, 255, 0.1)",
        "yAxisID": "y2",
        "tension": 0.4,
        "borderWidth": 3,
        "pointRadius": 5
      }
    ]
  },
  "options": {
    "responsive": true,
    "interaction": {
      "mode": "index",
      "intersect": false
    },
    "plugins": {
      "title": {
        "display": true,
        "text": "Machine Learning Model Training Progress",
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
      "y": {
        "type": "linear",
        "display": true,
        "position": "left",
        "title": {
          "display": true,
          "text": "Accuracy (%)"
        },
        "min": 70,
        "max": 100
      },
      "y1": {
        "type": "linear",
        "display": true,
        "position": "right",
        "title": {
          "display": true,
          "text": "Loss"
        },
        "min": 0,
        "max": 1,
        "grid": {
          "drawOnChartArea": false
        }
      },
      "y2": {
        "type": "linear",
        "display": false,
        "min": 0,
        "max": 1
      }
    }
  }
}
```

---

## Summary

These data science examples showcase:

1. **Scatter Plot** - Correlation analysis between continuous variables
2. **Bubble Chart** - Three-dimensional data visualization
3. **Time Series Forecasting** - Predictions with confidence intervals
4. **Distribution Analysis** - Frequency distributions and histograms
5. **Multi-Variate Tracking** - Machine learning model performance metrics

Perfect for research papers, data analysis reports, and ML model dashboards.
