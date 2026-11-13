Smart-X IoT Mesh Ecosystem
PROG7312 – Final Portfolio of Evidence (PoE)

YouTube Demonstration: https://youtu.be/cJ-JQjNSEB0?feature=shared

A hybrid Internet of Things (IoT) ecosystem designed to monitor, manage, and analyse data across a simulated mesh network of ESP32 microcontrollers.

The Smart-X IoT Mesh Ecosystem integrates advanced data structures and algorithms with a modern web application architecture. It combines a Blazor WebAssembly frontend, an ASP.NET Core Minimal API backend, and Docker support to provide a centralised platform for telemetry monitoring, device management, command execution, and network topology analysis.

The project demonstrates the practical application of Binary Search Trees, Graphs, Breadth-First Search, Prim's Minimum Spanning Tree, Queues, Priority Queues, Stacks, HashSets, and Dictionaries across three integrated development components.

📌 Project Overview

Smart-X is organised into three primary functional modules:

Module	Description
Data Ingestion	Collects, processes, and displays telemetry data from connected IoT devices.
Command Stream	Supports device control, command history, priority-based processing, and predictive insights.
Network Topology	Visualises mesh connectivity and implements graph traversal and minimum spanning tree algorithms.
Technology Stack
Frontend: Blazor WebAssembly
Backend: ASP.NET Core Minimal API
Programming Language: C#
Framework: .NET 8
Authentication: JWT (JSON Web Tokens)
API Documentation: Swagger / OpenAPI
Containerisation: Docker and Docker Compose
Data Structures: Trees, Graphs, Queues, Stacks, HashSets, and Dictionaries

🚀 Installation and Execution
Prerequisites

Ensure the following software is installed before running the application:

.NET 8.0 SDK or later
Visual Studio 2022
ASP.NET and web development workload
Docker Desktop (optional, for containerised execution)
Git
1. Clone the Repository

Open a terminal and execute:

bash
git clone https://github.com/katlegomothapo10/SmartX-IoT-PROG.git


Navigate into the project directory:

bash
cd SmartX-IoT-PROG

2. Open the Solution
Open SmartX.sln in Visual Studio 2022.
Allow Visual Studio to restore the NuGet packages.
Wait for the solution to finish loading.
3. Configure Startup Projects

To run the frontend and backend simultaneously:

Right-click the solution in Solution Explorer.
Select Configure Startup Projects.
Choose Multiple startup projects.
Set the following projects to Start:
SmartX.API
SmartX.Client
Apply the configuration.
4. Build and Run

Press F5 or select Start Debugging.

Visual Studio will build and launch both applications.

🔐 Authentication

The application includes JWT-based authentication to secure communication between the Blazor frontend and backend API.

For the demonstration environment, the login interface accepts:

Field	Demonstration Credentials
Email	admin@smartx.com
Password	1234

Note: These are demonstration credentials based on the current project implementation. The authentication validation should not be treated as production-grade identity management. Production deployments should use properly configured user accounts, password hashing, credential validation, and securely managed JWT signing keys.

📸 Application Screenshots

The following screenshots demonstrate the implemented functionality across the Smart-X ecosystem.

1. Secure Login and Main Menu

<img width="1917" height="738" alt="Secure JWT Login and Main Menu" src="https://github.com/user-attachments/assets/29b0bc02-8d56-42dc-8439-e4f8a6023847" />

The authenticated landing page provides access to the three primary application modules: Data Ingestion, Command Stream, and Network Topology. Navigation is handled asynchronously to support smooth transitions between application features.

2. Main Dashboard – Data Ingestion

<img width="1919" height="755" alt="Main Dashboard" src="https://github.com/user-attachments/assets/b35fd6f5-c42c-4eb6-8f8a-970b6706d829" />

The main dashboard provides an overview of the Smart-X ecosystem and acts as the entry point for accessing the application's monitoring and management functionality.

3. Telemetry Dashboard and Live Health Gauge

<img width="1905" height="887" alt="Telemetry Dashboard" src="https://github.com/user-attachments/assets/027c1053-056b-4bab-867c-96829d2f1771" />

Displays telemetry information, sensor activity, mesh health indicators, and detected anomalies. A custom CSS-based gauge provides a visual representation of the network's reported health status.

4. Command Dashboard and Predictive Insights

<img width="1919" height="884" alt="Command Dashboard" src="https://github.com/user-attachments/assets/207aec8a-4039-4027-b5a1-7f41750bfa5b" />

Provides manual device override controls, command execution history, Stack-based undo functionality, and an automated predictive insights engine that analyses historical user search patterns.

5. Network Topology and Graph Routing

<img width="1919" height="794" alt="Topology Dashboard" src="https://github.com/user-attachments/assets/6adf854c-7622-4253-b93a-ba5d5f74f2b3" />
<img width="581" height="379" alt="image" src="https://github.com/user-attachments/assets/12b72a8c-9e96-4700-b91f-f02463ec0cbb" />
<img width="581" height="434" alt="image" src="https://github.com/user-attachments/assets/00714656-ebc8-4054-9d40-23c5a66c23c7" />


Visualises the IoT mesh network and demonstrates graph-based connectivity analysis using Breadth-First Search (BFS) and Prim's Minimum Spanning Tree (MST) algorithm.

6. Backend API – Swagger Documentation

<img width="1238" height="898" alt="Swagger API" src="https://github.com/user-attachments/assets/e9d1f2af-a4e8-4462-b4ca-371a3e382b8b" />

The ASP.NET Core Minimal API exposes the backend functionality through documented HTTP endpoints. Swagger provides an interactive interface for inspecting and testing the available API operations.

7. Docker Container Execution

<img width="1919" height="931" alt="Docker Terminal 1" src="https://github.com/user-attachments/assets/1919e1b1-e396-48ec-af9b-c17eae02f22e" />

Terminal output demonstrating the Docker image build and container execution process.

<img width="1919" height="585" alt="Docker Terminal 2" src="https://github.com/user-attachments/assets/3c5797f0-8a1d-4bb9-b9ec-2bb199be1043" />

Background service logs demonstrating priority-based telemetry processing, including the [CRITICAL BYPASS] processing message.

8. Docker Desktop

<img width="1518" height="852" alt="Docker Desktop" src="https://github.com/user-attachments/assets/8d89d344-6af7-4e4b-827e-86a6b9f08995" />

Docker Desktop displaying the running Smart-X API container.

🧠 Advanced Data Structures and Algorithms

A central objective of the Smart-X project is to demonstrate how appropriate data structures and algorithms improve the efficiency, organisation, and processing of IoT network information.

The following sections outline their implementation and practical application.

1. Binary Search Tree (BST) – Historical Telemetry Queries

Implementation: SmartX.API/Models/TelemetryTree.cs

Purpose:

The Binary Search Tree stores historical telemetry records using their timestamps as ordering keys.

Instead of searching through every telemetry record sequentially, the application uses the tree structure to navigate towards the relevant time range and eliminate unnecessary branches.

Efficiency:

Average insertion: O(log n)
Average search: O(log n)
Range queries: O(log n + k) under favourable tree conditions, where k represents the number of returned records.
Worst-case operations: O(n) if the tree becomes unbalanced.

Implementation example:

csharp
public void Insert(TelemetryRecord record) =>
    Root = InsertRec(Root, record);

private TelemetryTreeNode? InsertRec(
    TelemetryTreeNode? node,
    TelemetryRecord record)
{
    if (node == null)
        return new TelemetryTreeNode(record);

    if (record.Timestamp < node.Timestamp)
        node.Left = InsertRec(node.Left, record);
    else if (record.Timestamp > node.Timestamp)
        node.Right = InsertRec(node.Right, record);

    return node;
}


Practical application: Enables efficient retrieval of historical sensor readings within selected date and time ranges.

2. Graph – IoT Mesh Network Topology

Implementation: SmartX.API/Models/MeshGraph.cs

Purpose:

The mesh network is represented using an adjacency list implemented through a Dictionary<string, MeshNode>.

Each node represents an IoT device, while its edges represent connections to neighbouring devices. Edge weights represent the connection cost used by the routing algorithm.

An adjacency list is particularly useful for sparse networks because it stores existing connections rather than maintaining a complete two-dimensional adjacency matrix.

Efficiency:

Node lookup: O(1) average.
Edge insertion: O(1) average, excluding any additional collection operations.
Space complexity: O(V + E).

Implementation example:

csharp
public void AddEdge(
    string sourceMac,
    string targetMac,
    int weight)
{
    Nodes[sourceMac].Edges.Add(
        new MeshEdge
        {
            TargetMac = targetMac,
            Weight = weight
        });

    Nodes[targetMac].Edges.Add(
        new MeshEdge
        {
            TargetMac = sourceMac,
            Weight = weight
        });
}


Practical application: Represents the relationships between connected IoT devices and provides the foundation for network traversal and route optimisation.

3. Breadth-First Search (BFS) – Network Connectivity

Implementation: MeshGraph.BFS()

Purpose:

Breadth-First Search traverses the mesh network systematically, exploring neighbouring nodes level by level.

The implementation uses:

Queue<string> to maintain the traversal order.
HashSet<string> to track visited nodes and prevent duplicate processing.

Efficiency:

O(V + E), where:

V represents the number of vertices (devices).
E represents the number of edges (connections).

Implementation example:

csharp
while (queue.Count > 0)
{
    var current = queue.Dequeue();

    visited.Add(current);

    foreach (var edge in Nodes[current].Edges)
    {
        if (!seen.Contains(edge.TargetMac))
        {
            seen.Add(edge.TargetMac);
            queue.Enqueue(edge.TargetMac);
        }
    }
}


Practical application: Identifies reachable devices within the mesh and helps determine which nodes may be affected by a gateway or connection failure.

4. Prim's Algorithm – Minimum Spanning Tree (MST)

Implementation: MeshGraph.GetMinimumSpanningTree()

Purpose:

Prim's algorithm calculates a Minimum Spanning Tree for a connected, weighted, undirected graph.

The implementation uses a PriorityQueue to select the lowest-weight available edge connecting the existing tree to an unvisited node.

This produces a spanning tree that connects all reachable nodes while minimising the total edge weight.

Efficiency:

With an adjacency list and binary-heap-based priority queue, the typical complexity is O(E log V).

Implementation example:

csharp
var pq = new PriorityQueue<
    (string Source, MeshEdge Edge), int>();

while (pq.Count > 0 && visited.Count < Nodes.Count)
{
    var (source, edge) = pq.Dequeue();

    if (visited.Contains(edge.TargetMac))
        continue;

    visited.Add(edge.TargetMac);
    mst.Add(edge);
}


Practical application: Demonstrates how a weighted network can be connected using a minimum-cost spanning structure. The edge weights represent the project's selected connection metric; they should not automatically be interpreted as real-world battery consumption without an appropriate energy model.

5. Queues and Priority Queues – Telemetry Processing

Implementation: SmartX.API/Services/RealTimeCommandService.cs

Purpose:

The system uses queue-based processing to organise incoming telemetry messages.

Two processing mechanisms are implemented:

Standard Queue: Uses First-In, First-Out (FIFO) processing for normal telemetry messages.
Priority Queue: Gives critical alerts precedence over standard messages.

This separation helps ensure that important events, such as severe power anomalies, are handled ahead of routine telemetry.

Implementation example:

csharp
TelemetryQueue.Enqueue(msg);

if (msg.Priority == 0)
{
    CriticalAlertQueue.Enqueue(msg, 0);
}


Practical application: Demonstrates priority-based message handling and reduces the likelihood of critical alerts being delayed behind routine telemetry.

6. Stacks – Command History and Undo Functionality

Implementation: RealTimeCommandService.CommandUndoStack

Purpose:

A Stack follows the Last-In, First-Out (LIFO) principle.

The application uses this structure to maintain a history of manual device commands. When an operator requests an undo operation, the most recently stored command is retrieved first.

Efficiency:

Push: O(1) amortised.
Pop: O(1).
Peek: O(1).

Implementation example:

csharp
public CommandRequest? UndoLastCommand()
{
    if (CommandUndoStack.Count > 0)
    {
        var cmd = CommandUndoStack.Pop();

        cmd.IsUndone = true;

        return cmd;
    }

    return null;
}


Practical application: Provides a mechanism for tracking and reversing the most recently issued command within the application's command history.

7. HashSets – Duplicate Disconnect Detection

Implementation: RealTimeCommandService.DisconnectedNodes

Purpose:

A HashSet stores unique values and prevents duplicate device identifiers from being added.

The system uses this property to track disconnected IoT devices and avoid repeatedly processing the same disconnect notification.

Efficiency:

Add: O(1) average.
Contains: O(1) average.
Remove: O(1) average.

Implementation example:

csharp
public bool HandleDisconnect(string mac)
{
    return DisconnectedNodes.Add(mac);
}


Practical application: Helps prevent duplicate disconnect events from unnecessarily increasing error counts or triggering repeated processing.

8. Dictionaries – Device Registry and Lookup

Implementation: SensorDataService.DeviceRegistry

Purpose:

A Dictionary maps each sensor's MAC address to its corresponding device information.

This allows the application to retrieve registered devices directly using their unique identifiers rather than searching through an entire collection.

Efficiency:

Average lookup complexity: O(1).

Worst-case lookup complexity: O(n).

Practical application: Provides efficient device identification and supports the processing of incoming telemetry messages.

🤖 Predictive Action Engine

Implementation: SmartX.API/Services/PredictiveEngine.cs

The Predictive Action Engine analyses historical user search activity to identify recurring command patterns and generate automated recommendations.

How It Works

The engine maintains a nested dictionary:

csharp
Dictionary<string, Dictionary<string, int>>


This structure records user search patterns and the frequency of associated actions.

When a user searches for a device or command, the engine evaluates previously recorded activity to identify potential patterns.

For example:

A user searches for pump.
The user selects the TurnOn command.
The engine records the interaction.
The user repeats the same action.
On a subsequent search, the engine can generate an automated insight based on the recorded behaviour.

The dashboard displays recommendations alongside a confidence indicator derived from the stored interaction history.

Purpose: Demonstrates how dictionaries and historical usage patterns can be combined to provide context-aware recommendations within an IoT management application.

🧪 System Testing and Demonstration Guide

The following test scenarios demonstrate the main functional requirements and data structure implementations.

Test 1: Graph Construction and Prim's Algorithm

Objective: Verify mesh graph construction, connectivity, and Minimum Spanning Tree generation.

Navigate to Network Topology.
Click Generate Mesh Connections.
Enter the following starting node:

AA:BB:CC:DD:EE:01

Select Calculate MST.
Inspect the resulting minimum spanning tree.
Enter the same MAC address in the BFS input.
Click Run BFS.
Verify that reachable network nodes are displayed.

Expected result: The application generates a weighted mesh graph and displays the results of Prim's algorithm and BFS traversal.

Test 2: Binary Search Tree

Objective: Verify historical telemetry insertion and date-range querying.

Navigate to the Dashboard.
Click Simulate Load to generate telemetry records.
Navigate to Network Topology.
Select the date range Today.
Click Query Tree.

Expected result: The application retrieves telemetry records matching the selected date range using the Binary Search Tree.

Test 3: Queue and Priority Queue Processing

Objective: Verify priority-based telemetry handling.

Navigate to Command Stream.
Click Simulate High-Priority Alert.
Inspect the backend API terminal output.
Look for the following log message:

[CRITICAL BYPASS]

Expected result: The critical alert is routed through the priority-processing mechanism and handled ahead of standard queued telemetry.

Test 4: HashSet Duplicate Detection

Objective: Verify unique disconnect tracking.

Remain in Command Stream.
Click Simulate Device Disconnect.
Repeat the operation for the same device.
Inspect the disconnect metrics.

Expected result: The HashSet prevents the same device identifier from being added repeatedly.

Test 5: Predictive Action Engine

Objective: Verify the recording and analysis of user command patterns.

Navigate to Command Stream.
Search for pump.
Select TurnOn.
Execute the command.
Repeat the same search and command.
Search for pump again without executing the command.
Inspect the Automated Insights section.

Expected result: The Predictive Action Engine identifies the previously recorded interaction pattern and displays a recommendation based on the available historical data.

Test 6: Stack-Based Undo Functionality

Objective: Verify Last-In, First-Out command history.

Navigate to Command Stream.
Execute a manual device command.
Execute another command.
Select the Undo operation.
Inspect the command history.

Expected result: The most recently recorded command is retrieved first from the Stack and marked as undone.

🐳 Docker Support

Smart-X includes Docker configuration files to support containerised execution of the backend API.

Running the Application with Docker

Ensure Docker Desktop is installed and running.

From the project directory, execute:

bash
docker-compose up --build


This command builds the configured Docker images and starts the services defined in docker-compose.yml.

To run the containers in detached mode:

bash
docker-compose up --build -d


To inspect running containers:

bash
docker-compose ps


To view application logs:

bash
docker-compose logs -f


To stop the containers:

bash
docker-compose down


Note: Ensure that the Docker configuration, exposed ports, and application environment variables are correctly configured before deployment.

📂 Project Structure

The primary solution components are organised as follows:

text
SmartX-IoT-PROG/
│
├── SmartX.API/
│   ├── Models/
│   │   ├── TelemetryTree.cs
│   │   └── MeshGraph.cs
│   │
│   ├── Services/
│   │   ├── RealTimeCommandService.cs
│   │   ├── PredictiveEngine.cs
│   │   └── SensorDataService.cs
│   │
│   └── Program.cs
│
├── SmartX.Client/
│   └── Blazor WebAssembly frontend
│
├── SmartX.sln
├── Dockerfile
├── docker-compose.yml
└── README.md


Note: This is a high-level representation of the main project components. Additional files and directories may exist in the repository.

🎯 Project Outcomes

The Smart-X IoT Mesh Ecosystem demonstrates the integration of theoretical computer science concepts into a practical software engineering project.

The implementation highlights the following outcomes:

Data Management: Efficient organisation and retrieval of historical telemetry using Binary Search Trees and Dictionaries.
Network Analysis: Representation and traversal of connected IoT devices using Graphs and BFS.
Route Optimisation: Application of Prim's algorithm to calculate minimum spanning trees.
Message Processing: Queue and Priority Queue implementations for managing telemetry and critical alerts.
Command Management: Stack-based command history and undo functionality.
Duplicate Prevention: HashSet-based tracking of unique device disconnect events.
Predictive Insights: Analysis of historical user interactions to generate automated recommendations.
Application Security: JWT-based authentication for protected API operations.
Deployment: Docker-based backend containerisation.
👨‍💻 Repository Information
Detail	Information
Project	Smart-X IoT Mesh Ecosystem
Module	PROG7311
Assessment	Final Portfolio of Evidence
Language	C#
Framework	.NET 8
Frontend	Blazor WebAssembly
Backend	ASP.NET Core Minimal API
Repository	SmartX-IoT-PROG
🏁 Conclusion

The Smart-X IoT Mesh Ecosystem brings together multiple advanced data structures, graph algorithms, and modern application development technologies to create an integrated IoT monitoring and management platform.

Through its three main modules, the project demonstrates how theoretical concepts such as tree traversal, graph optimisation, priority-based processing, and collection management can be applied to practical software engineering challenges.

The completed implementation provides a foundation for further development of intelligent IoT monitoring systems, with opportunities for physical ESP32 integration, persistent telemetry storage, enhanced security, and more advanced predictive analytics.

Smart-X – Connecting Devices, Processing Intelligence, and Optimising the IoT Network.
