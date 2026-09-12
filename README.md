# Smart-X IoT Mesh Ecosystem

A hybrid IoT ecosystem for monitoring and managing distributed environments (hydroponic farms, smart grid installations, and automated utility trackers). This repository contains **Part 1** of the Portfolio of Evidence (PoE) — the data ingestion and validation gateway.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Key Features](#key-features)
- [Technical Concepts Demonstrated](#technical-concepts-demonstrated)
- [Project Structure](#project-structure)
- [Troubleshooting](#troubleshooting)

---

## Overview

The Smart-X system simulates thousands of ESP32 microcontrollers publishing telemetry data (floats, integers, and booleans) to a central gateway. This project implements:

- A **.NET Web API** for receiving and validating telemetry
- A **Blazor WebAssembly frontend** for real-time monitoring
- In-memory data storage with batching for high-throughput ingestion



**Tech Stack:**
- **Backend:** ASP.NET Core Minimal API (.NET 8/9/10)
- **Frontend:** Blazor WebAssembly
- **Data Storage:** In-memory (ConcurrentDictionary)
- **Styling:** Bootstrap 5 + custom CSS


## Prerequisites

Make sure you have the following installed:

| Tool | Version | Download |
|------|---------|----------|
| .NET SDK | 8.0 or higher | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| Visual Studio | 2022 (Community or higher) | [visualstudio.microsoft.com](https://visualstudio.microsoft.com/) |
| Git | Latest | [git-scm.com](https://git-scm.com/) |
| Docker *(optional)* | Latest | [docker.com](https://www.docker.com/) |

**Visual Studio workloads required:**
- ASP.NET and web development

---

## Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/YOUR-USERNAME/SmartX-IoT.git
cd SmartX-IoT
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

You should see: `Build succeeded.`

---

## Running the Application

### Option A: Visual Studio (Recommended)

1. Open `SmartX.sln` in Visual Studio
2. Right-click the **Solution** in Solution Explorer
3. Click **Configure Startup Projects...**
4. Select **Multiple startup projects**
5. Set both `SmartX.API` and `SmartX.Client` to **Start**
6. Set `SmartX.Client` to start **after** `SmartX.API` (use the arrows to reorder if needed)
7. Press **F5**

Two browser tabs will open:
- API Swagger UI at `https://localhost:5001/swagger`
- Client app at `https://localhost:5000`

### Option B: Command Line (Two Terminals)

**Terminal 1 — Start the API:**
```bash
cd SmartX.API
dotnet run
```
Note the port shown in the console (e.g. `https://localhost:5001`).

**Terminal 2 — Start the Client:**
```bash
cd SmartX.Client
dotnet run
```

> ⚠️ **Important:** If your API runs on a different port than `5001`, open `SmartX.Client/Program.cs` and change the `BaseAddress` to match.

### Option C: Docker (Optional)

Build and run the API:
```bash
docker build -t smartx-api -f SmartX.API/Dockerfile .
docker run -p 5001:5001 smartx-api
```

---

## API Endpoints

All endpoints are prefixed with `/api`.

### Sensors

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/sensors` | Register a new sensor |
| `GET` | `/api/sensors` | Get all registered sensors |
| `GET` | `/api/sensors/{macAddress}` | Get a specific sensor |
| `DELETE` | `/api/sensors/{macAddress}` | Delete a sensor |

### Telemetry

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/telemetry` | Ingest generic telemetry |
| `POST` | `/api/telemetry/float` | Ingest float telemetry |
| `POST` | `/api/telemetry/int` | Ingest integer telemetry |
| `POST` | `/api/telemetry/bool` | Ingest boolean telemetry |
| `GET` | `/api/telemetry/{macAddress}` | Get latest reading |
| `GET` | `/api/telemetry/{macAddress}/history` | Get reading history |
| `POST` | `/api/telemetry/bulk` | Simulate 200 random readings |

### Files & Deployment

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/sensors/{macAddress}/file` | Upload configuration file |
| `GET` | `/api/deployment/tree` | Get deployment tree |
| `POST` | `/api/deployment/validate` | Validate deployment tree |

### Dashboard

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/dashboard/stats` | Get dashboard statistics |

---

## Key Features

- ✅ **Sensor Registration** — register MAC address, location, category (Environmental / Power / Actuator)
- ✅ **Multi-Type Telemetry Ingestion** — floats, integers, and booleans handled uniformly via generics
- ✅ **File Upload** — attach configuration files or deployment photos to sensor profiles
- ✅ **Real-Time Dashboard** — auto-refreshes every 10 seconds with live sensor cards
- ✅ **Visual Hierarchy Alerts** — critical (red), warning (amber), and normal (green) severity banners
- ✅ **Bulk Simulation** — generate 200 telemetry packets in one click to prove performance under load
- ✅ **Recursive Deployment Validation** — validates nested device trees (Facility → Zone → Sub-Zone → Node)

---

## Technical Concepts Demonstrated

| Concept | Location | Purpose |
|---------|----------|---------|
| **Generics** | `TelemetryPacket<T>` | Handles `float`, `int`, `bool` without boxing/unboxing |
| **Operator Overloading** | `Sensor.cs` | Enables `sensor1 + sensor2`, `sensor1 - sensor2`, `sensor1 > sensor2` |
| **Jagged Arrays** | `SensorDataService._batchBuffer` | Buffers variable-sized telemetry batches |
| **Multi-Dimensional Arrays** | `SensorDataService._gridSummary` | Fixed-size rolling summary grid (100×3) |
| **Recursion** | `DeploymentValidator.ValidateNode` | Walks the deployment tree and validates every node |
| **Concurrent Collections** | `ConcurrentDictionary<string, Sensor>` | Thread-safe access under concurrent requests |

---

## Project Structure

```
SmartX-IoT/
├── SmartX.sln
├── README.md
├── .gitignore
├── SmartX.API/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Models/
│   │   ├── Sensor.cs
│   │   ├── TelemetryPacket.cs
│   │   └── DeploymentNode.cs
│   ├── Services/
│   │   ├── SensorDataService.cs
│   │   └── DeploymentValidator.cs
│   └── Utils/
│       └── DataSeeder.cs
└── SmartX.Client/
    ├── Program.cs
    ├── _Imports.razor
    ├── App.razor
    ├── Models/
    │   ├── Sensor.cs
    │   ├── TelemetryPacket.cs
    │   └── DashboardStats.cs
    ├── Services/
    │   └── ApiService.cs
    ├── Components/
    │   ├── AlertBanner.razor
    │   └── SensorCard.razor
    ├── Pages/
    │   ├── Index.razor
    │   ├── Dashboard.razor
    │   └── SensorRegistration.razor
    └── wwwroot/
        └── css/
            └── app.css
```

---

## Troubleshooting

### "Unable to connect to the API" in the client

- Make sure the API is running first
- Check the port in `SmartX.Client/Program.cs` matches the API's actual HTTPS port
- Accept the self-signed certificate warning in your browser (visit the API URL directly first)

### CORS errors in the browser console

- The API is configured with `AllowAnyOrigin()` for development. If you see CORS issues, ensure `app.UseCors("AllowBlazor")` is called **before** `app.MapX` calls in `Program.cs`.

### "Build succeeded" but nothing runs

- Right-click the solution → **Configure Startup Projects** → **Multiple startup projects** → set both to `Start`.

### Port already in use

- Kill the process on the port, or change the port in `SmartX.API/Properties/launchSettings.json`.

---

## Author

**Student:Katlego Mothapo
**Student Number:ST10442760
**Module:** PROG7312 
**Year:** 2026

---

## License

This project is submitted as academic work for the IIE. Not for redistribution.
