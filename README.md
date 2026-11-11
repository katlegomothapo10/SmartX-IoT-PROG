# Smart-X IoT Mesh Ecosystem

A hybrid Internet of Things (IoT) ecosystem designed to monitor, manage, and route data across a mesh network of ESP32 microcontrollers. This repository contains the complete implementation for Parts 1, 2, and 3.

## 🎥 Video Demo
*   **YouTube Demo Link:** [PASTE-YOUTUBE-LINK-HERE]

---

## 🚀 How to Compile and Run

### Prerequisites
- .NET 8.0 SDK (or higher)
- Visual Studio 2022

### Setup Instructions
1. Clone the repository: `git clone https://github.com/katlegomothapo10/SmartX-IoT-PROG.git`
2. Open `SmartX.sln` in Visual Studio.
3. Right-click the Solution in Solution Explorer -> **Configure Startup Projects**.
4. Select **Multiple startup projects** and set both `SmartX.API` and `SmartX.Client` to **Start**.
5. Press `F5` to compile and run.

### 🔐 Authentication
The system uses secure JWT (JSON Web Token) authentication.
- **Email:** Any valid email (e.g., `admin@smartx.com`)
- **Password:** Any password with 4+ characters (e.g., `1234`)

---

## 📸 Application Screenshots

### 1. Secure JWT Login & Main Menu
<img width="1917" height="738" alt="image" src="https://github.com/user-attachments/assets/29b0bc02-8d56-42dc-8439-e4f8a6023847" />

*The authenticated landing page showing all three active pillars.*

### 2. Network Topology & Graph Routing
<img width="1919" height="793" alt="image" src="https://github.com/user-attachments/assets/bd9a6ac5-bdcb-4e19-910e-9788cd22bdd4" />

*Showing BFS Traversal and Prim's Minimum Spanning Tree (MST) in action.*

---

pictures: <img width="1919" height="755" alt="image" src="https://github.com/user-attachments/assets/b35fd6f5-c42c-4eb6-8f8a-970b6706d829" />

<img width="1905" height="887" alt="image" src="https://github.com/user-attachments/assets/027c1053-056b-4bab-867c-96829d2f1771" />

<img width="1919" height="884" alt="image" src="https://github.com/user-attachments/assets/207aec8a-4039-4027-b5a1-7f41750bfa5b" />

<img width="1919" height="794" alt="image" src="https://github.com/user-attachments/assets/6adf854c-7622-4253-b93a-ba5d5f74f2b3" />

<img width="1238" height="898" alt="image" src="https://github.com/user-attachments/assets/e9d1f2af-a4e8-4462-b4ca-371a3e382b8b" />





## 🧠 Advanced Data Structures Implementation

### 1. Binary Search Tree (BST) - Historical Telemetry Queries
*   **Location:** `SmartX.API/Models/TelemetryTree.cs`
*   **Contribution to Efficiency:** When querying historical telemetry for a specific time range, iterating through a massive unsorted list of thousands of database records is \(O(n)\), which causes severe UI lag. By inserting telemetry into a BST (keyed on `Timestamp`), the `GetRange()` method prunes branches recursively. This achieves an average time complexity of \(O(\log n)\), allowing the dashboard to filter data ranges instantly.
*   **Code Example:**
```csharp
public void Insert(TelemetryRecord record) => Root = InsertRec(Root, record);
private TelemetryTreeNode InsertRec(TelemetryTreeNode? node, TelemetryRecord record) {
    if (node == null) return new TelemetryTreeNode(record);
    if (record.Timestamp < node.Timestamp) node.Left = InsertRec(node.Left, record);
    else if (record.Timestamp > node.Timestamp) node.Right = InsertRec(node.Right, record);
    return node;
}


nimising the battery drain of the ESP32 sensors.

