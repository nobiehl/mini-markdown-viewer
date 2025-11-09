# Real-Time & Dynamic Data Examples

Chart.js examples showing realistic real-world data patterns and use cases.

---

## Server Performance Monitoring

**Use Case:** Real-time server metrics dashboard for DevOps teams.

```chart
{
  "type": "line",
  "data": {
    "labels": ["12:00", "12:05", "12:10", "12:15", "12:20", "12:25", "12:30", "12:35", "12:40", "12:45"],
    "datasets": [
      {
        "label": "CPU Usage (%)",
        "data": [45, 52, 48, 65, 72, 68, 58, 62, 55, 51],
        "borderColor": "rgb(255, 99, 132)",
        "backgroundColor": "rgba(255, 99, 132, 0.1)",
        "tension": 0.4,
        "fill": true,
        "borderWidth": 2
      },
      {
        "label": "Memory Usage (%)",
        "data": [62, 64, 66, 68, 70, 72, 71, 69, 67, 65],
        "borderColor": "rgb(54, 162, 235)",
        "backgroundColor": "rgba(54, 162, 235, 0.1)",
        "tension": 0.4,
        "fill": true,
        "borderWidth": 2
      },
      {
        "label": "Network I/O (%)",
        "data": [35, 42, 48, 55, 62, 58, 51, 48, 43, 38],
        "borderColor": "rgb(75, 192, 192)",
        "backgroundColor": "rgba(75, 192, 192, 0.1)",
        "tension": 0.4,
        "fill": true,
        "borderWidth": 2
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Production Server Metrics - Live Feed",
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
        "beginAtZero": true,
        "max": 100,
        "title": {
          "display": true,
          "text": "Usage (%)"
        }
      },
      "x": {
        "title": {
          "display": true,
          "text": "Time (5-minute intervals)"
        }
      }
    }
  }
}
```

---

## E-Commerce Sales Dashboard - Today's Activity

**Use Case:** Live sales tracking for online retail businesses.

```chart
{
  "type": "bar",
  "data": {
    "labels": ["00:00", "03:00", "06:00", "09:00", "12:00", "15:00", "18:00", "21:00", "23:00"],
    "datasets": [
      {
        "label": "Orders",
        "data": [12, 8, 15, 45, 78, 92, 125, 105, 68],
        "backgroundColor": "rgba(75, 192, 192, 0.7)",
        "borderColor": "rgb(75, 192, 192)",
        "borderWidth": 2,
        "yAxisID": "y"
      },
      {
        "label": "Revenue ($)",
        "data": [1250, 890, 1680, 5200, 8950, 11200, 15800, 13400, 7850],
        "type": "line",
        "borderColor": "rgb(255, 99, 132)",
        "backgroundColor": "rgba(255, 99, 132, 0.1)",
        "borderWidth": 3,
        "tension": 0.4,
        "fill": true,
        "yAxisID": "y1"
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Today's Sales Activity - Updated Every Hour",
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
        "type": "linear",
        "display": true,
        "position": "left",
        "title": {
          "display": true,
          "text": "Number of Orders"
        },
        "beginAtZero": true
      },
      "y1": {
        "type": "linear",
        "display": true,
        "position": "right",
        "title": {
          "display": true,
          "text": "Revenue ($)"
        },
        "beginAtZero": true,
        "grid": {
          "drawOnChartArea": false
        }
      }
    }
  }
}
```

---

## Social Media Analytics - Engagement Metrics

**Use Case:** Track social media post performance in real-time.

```chart
{
  "type": "line",
  "data": {
    "labels": ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"],
    "datasets": [
      {
        "label": "Likes",
        "data": [2450, 3120, 2890, 4250, 5180, 6890, 5420],
        "borderColor": "rgb(255, 99, 132)",
        "backgroundColor": "rgba(255, 99, 132, 0.2)",
        "tension": 0.4,
        "fill": true,
        "borderWidth": 3,
        "pointRadius": 6
      },
      {
        "label": "Comments",
        "data": [320, 428, 385, 562, 680, 825, 710],
        "borderColor": "rgb(54, 162, 235)",
        "backgroundColor": "rgba(54, 162, 235, 0.2)",
        "tension": 0.4,
        "fill": true,
        "borderWidth": 3,
        "pointRadius": 6
      },
      {
        "label": "Shares",
        "data": [185, 242, 218, 315, 392, 468, 405],
        "borderColor": "rgb(75, 192, 192)",
        "backgroundColor": "rgba(75, 192, 192, 0.2)",
        "tension": 0.4,
        "fill": true,
        "borderWidth": 3,
        "pointRadius": 6
      },
      {
        "label": "Saves",
        "data": [142, 189, 165, 238, 285, 352, 298],
        "borderColor": "rgb(255, 206, 86)",
        "backgroundColor": "rgba(255, 206, 86, 0.2)",
        "tension": 0.4,
        "fill": true,
        "borderWidth": 3,
        "pointRadius": 6
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "This Week's Social Media Engagement",
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
        "beginAtZero": true,
        "title": {
          "display": true,
          "text": "Engagement Count"
        }
      }
    }
  }
}
```

---

## IoT Sensor Data - Temperature & Humidity

**Use Case:** Monitor environmental sensors in real-time for smart buildings.

```chart
{
  "type": "line",
  "data": {
    "labels": ["00:00", "02:00", "04:00", "06:00", "08:00", "10:00", "12:00", "14:00", "16:00", "18:00", "20:00", "22:00"],
    "datasets": [
      {
        "label": "Temperature (°C)",
        "data": [18.5, 17.8, 17.2, 17.5, 19.2, 21.8, 24.5, 26.8, 27.2, 25.5, 22.8, 20.2],
        "borderColor": "rgb(255, 99, 132)",
        "backgroundColor": "rgba(255, 99, 132, 0.1)",
        "tension": 0.4,
        "fill": true,
        "borderWidth": 3,
        "yAxisID": "y"
      },
      {
        "label": "Humidity (%)",
        "data": [65, 68, 70, 72, 68, 62, 55, 48, 45, 52, 58, 62],
        "borderColor": "rgb(54, 162, 235)",
        "backgroundColor": "rgba(54, 162, 235, 0.1)",
        "tension": 0.4,
        "fill": true,
        "borderWidth": 3,
        "yAxisID": "y1"
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "Office Environment Monitoring - Last 24 Hours",
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
        "type": "linear",
        "display": true,
        "position": "left",
        "title": {
          "display": true,
          "text": "Temperature (°C)"
        },
        "min": 15,
        "max": 30
      },
      "y1": {
        "type": "linear",
        "display": true,
        "position": "right",
        "title": {
          "display": true,
          "text": "Humidity (%)"
        },
        "min": 40,
        "max": 80,
        "grid": {
          "drawOnChartArea": false
        }
      }
    }
  }
}
```

---

## Application Response Time Monitoring

**Use Case:** Track API performance and response times for SLA monitoring.

```chart
{
  "type": "bar",
  "data": {
    "labels": ["Authentication", "User Profile", "Search", "Checkout", "Payment", "Notification", "Analytics", "Reports"],
    "datasets": [
      {
        "label": "Avg Response Time (ms)",
        "data": [45, 120, 280, 380, 520, 95, 650, 1200],
        "backgroundColor": [
          "rgba(75, 192, 192, 0.8)",
          "rgba(75, 192, 192, 0.8)",
          "rgba(255, 206, 86, 0.8)",
          "rgba(255, 206, 86, 0.8)",
          "rgba(255, 159, 64, 0.8)",
          "rgba(75, 192, 192, 0.8)",
          "rgba(255, 159, 64, 0.8)",
          "rgba(255, 99, 132, 0.8)"
        ],
        "borderColor": [
          "rgb(75, 192, 192)",
          "rgb(75, 192, 192)",
          "rgb(255, 206, 86)",
          "rgb(255, 206, 86)",
          "rgb(255, 159, 64)",
          "rgb(75, 192, 192)",
          "rgb(255, 159, 64)",
          "rgb(255, 99, 132)"
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
        "text": "API Endpoint Performance - Last 15 Minutes",
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
          "label": "function(context) { return 'Response Time: ' + context.parsed.y + 'ms'; }"
        }
      }
    },
    "scales": {
      "y": {
        "beginAtZero": true,
        "title": {
          "display": true,
          "text": "Response Time (ms)"
        }
      }
    }
  }
}
```

---

## Stock Trading Dashboard - Intraday Price Action

**Use Case:** Financial market data visualization for traders.

```chart
{
  "type": "line",
  "data": {
    "labels": ["09:30", "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", "16:00"],
    "datasets": [
      {
        "label": "Stock Price ($)",
        "data": [152.50, 153.20, 152.80, 154.10, 155.40, 154.80, 156.20, 157.50, 156.90, 158.20, 159.10, 158.50, 160.20, 161.50],
        "borderColor": "rgb(75, 192, 192)",
        "backgroundColor": "rgba(75, 192, 192, 0.1)",
        "tension": 0.3,
        "fill": true,
        "borderWidth": 3,
        "pointRadius": 4
      },
      {
        "label": "Moving Average (20)",
        "data": [152.80, 153.10, 153.25, 153.60, 154.00, 154.30, 154.70, 155.20, 155.60, 156.10, 156.70, 157.20, 157.80, 158.40],
        "borderColor": "rgb(255, 159, 64)",
        "backgroundColor": "rgba(255, 159, 64, 0)",
        "tension": 0.4,
        "fill": false,
        "borderWidth": 2,
        "pointRadius": 0,
        "borderDash": [5, 5]
      }
    ]
  },
  "options": {
    "responsive": true,
    "plugins": {
      "title": {
        "display": true,
        "text": "TECH Inc. - Live Trading Data",
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
          "label": "function(context) { return context.dataset.label + ': $' + context.parsed.y.toFixed(2); }"
        }
      }
    },
    "scales": {
      "y": {
        "title": {
          "display": true,
          "text": "Price ($)"
        },
        "ticks": {
          "callback": "function(value) { return '$' + value.toFixed(2); }"
        }
      },
      "x": {
        "title": {
          "display": true,
          "text": "Trading Hours (EST)"
        }
      }
    }
  }
}
```

---

## Website Traffic Sources - Live Data

**Use Case:** Real-time traffic source breakdown for marketing teams.

```chart
{
  "type": "doughnut",
  "data": {
    "labels": ["Organic Search", "Direct Traffic", "Social Media", "Paid Ads", "Email Campaign", "Referral Links"],
    "datasets": [
      {
        "label": "Visitors",
        "data": [4250, 2890, 3120, 1850, 980, 1420],
        "backgroundColor": [
          "rgba(75, 192, 192, 0.9)",
          "rgba(54, 162, 235, 0.9)",
          "rgba(255, 99, 132, 0.9)",
          "rgba(255, 206, 86, 0.9)",
          "rgba(153, 102, 255, 0.9)",
          "rgba(255, 159, 64, 0.9)"
        ],
        "borderColor": [
          "rgb(75, 192, 192)",
          "rgb(54, 162, 235)",
          "rgb(255, 99, 132)",
          "rgb(255, 206, 86)",
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
        "text": "Current Traffic Sources - Last 24 Hours (14,510 total visitors)",
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
          "label": "function(context) { var total = context.dataset.data.reduce((a, b) => a + b, 0); var percentage = ((context.parsed / total) * 100).toFixed(1); return context.label + ': ' + context.parsed.toLocaleString() + ' (' + percentage + '%)'; }"
        }
      }
    }
  }
}
```

---

## Summary

These real-time examples demonstrate:

1. **Server Monitoring** - Live infrastructure performance tracking
2. **E-Commerce Dashboard** - Hourly sales and order metrics
3. **Social Media Analytics** - Weekly engagement patterns
4. **IoT Sensors** - Environmental monitoring with dual axes
5. **API Performance** - Response time monitoring with color-coded thresholds
6. **Stock Trading** - Intraday price movements with technical indicators
7. **Traffic Analytics** - Live visitor source distribution

Perfect for dashboards that update frequently and require real-time data visualization.
