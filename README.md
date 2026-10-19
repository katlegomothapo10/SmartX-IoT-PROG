# Smart-X IoT Mesh Ecosystem (PROG7311)

A hybrid Internet of Things (IoT) ecosystem designed to monitor and manage distributed environments. This repository contains the complete implementation for **Part 1 (Data Ingestion)** and **Part 2 (Real-Time Processing and Device Management)**.

## 🛠️ Prerequisites & Setup
- **.NET 8.0 SDK** (or higher)
- **Visual Studio 2022**
- To run the project:
  1. Clone the repository.
  2. Open `SmartX.sln`.
  3. Set both `SmartX.API` and `SmartX.Client` as Multiple Startup Projects.
  4. Press `F5`.

## 🔐 Authentication
The system features a secure login portal to restrict unauthorized access.
- **Email:** Enter any valid email format.
- **Password:** Must be **4 or more characters**.
- Click **Secure Login**.

---

## 🧪 How to Test Part 2 Implementations

### 1. Testing Queues & Priority Queues (15 Marks)
The system uses a standard `Queue<TelemetryMessage>` for FIFO telemetry ingestion and a `PriorityQueue<TelemetryMessage, int>` for critical alerts.
**Step-by-step to simulate a high-priority alert:**
1. Log in and navigate to the **Command Stream** tab.
2. Look at the "System Metrics" panel on the left. Note the "Priority Queue" count.
3. Click the red **"🔥 High-Priority Alert"** button in the Test Simulator panel.
4. The system enqueues a critical message (Priority 0) which bypasses the normal queue.
5. Watch the "Priority Queue" metric increment instantly, and a new "High Temp Alert" insight appear in the Predictive Insights panel.

### 2. Testing Stacks (Undo Functionality)
The system uses a `Stack<CommandRequest>` to store command history for a LIFO (Last-In, First-Out) undo feature.
**Step-by-step to test Undo:**
1. In the Manual Override panel, enter a MAC Address (e.g., `AA:BB:CC:DD:EE:01`), select "Turn On", and click **Execute Command**.
2. The command appears in the History table below.
3. Click the yellow **"↩ Undo Last Action"** button.
4. The last command is popped off the stack, marked as Undone, and visually struck through in the history table.

### 3. Testing Dictionaries & File Parsing (15 Marks)
The system uses a `Dictionary<string, Sensor>` to store devices, allowing instant O(1) lookups by MAC address without looping.
**Step-by-step to test Dictionary lookup & File Upload:**
1. Go to the **Data Ingestion** tab.
2. Click **Register Sensor** and upload a `.txt` file containing a list of MAC addresses (one per line, e.g., `AA:BB:CC:DD:EE:99`).
3. The system reads the file, performs instant dictionary lookups to verify uniqueness, and populates the dashboard grid.

### 4. Testing Sets (Unique Disconnects) (10 Marks)
The system uses a `HashSet<string>` to track disconnected nodes, preventing duplicate alerts from processing twice.
**Step-by-step to test Sets:**
1. Go to the **Command Stream** tab.
2. Click the **"🔌 Device Disconnect"** button three times.
3. Check the "System Metrics" panel. The "Disconnected" count will only be **1**. The HashSet recognizes the device is already in the set and ignores duplicate alerts.

### 5. Predictive Action & Recommendation Engine (30 Marks)
The system records user search history and matches it against timeline telemetry to proactively suggest actions.
**Step-by-step to test Predictive Insights:**
1. In the Manual Override panel, type `Pump` in the MAC address box and execute a command.
2. The engine records this in your `UserSearchHistory` list.
3. Based on this pattern, the green **"Automated Insights"** panel will dynamically generate a "Pump Action Required" suggestion.
4. If a critical alert is triggered (as in Step 1), the engine will also proactively suggest resolving the high temperature.

---

## 🐳 Docker Support
This project includes a `Dockerfile` and `docker-compose.yml`. 
To run the API in a container:
```bash
docker-compose up --build
