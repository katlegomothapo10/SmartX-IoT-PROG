Smart-X IoT Mesh Ecosystem
Intelligent IoT Network Management, Graph Routing & Real-Time Telemetry Monitoring

<p align="center">

A Hybrid Internet of Things (IoT) Ecosystem Powered by ESP32 Microcontrollers, Advanced Data Structures and Intelligent Network Algorithms.

</p>

1. Project Overview

Smart-X IoT Mesh Ecosystem is a hybrid Internet of Things (IoT) platform developed to demonstrate the integration of embedded systems, network communication, data structures, graph algorithms and modern software engineering principles.

The system is designed around a network of ESP32 microcontrollers that communicate within an IoT mesh environment. It provides a centralised platform for monitoring network activity, managing connected devices, analysing telemetry data and exploring efficient network-routing strategies.

Rather than treating IoT devices as isolated components, Smart-X brings them together into an interconnected ecosystem where network information can be monitored, processed and visualised through a central application.

The project combines practical IoT concepts with algorithmic problem-solving to demonstrate how computer science principles can be applied to real-world distributed systems.

This repository contains the complete implementation of Parts 1, 2 and 3 of the Smart-X project.

2. Project Demonstration

🎥 Video Demonstration: Watch the Smart-X System Demo

The demonstration showcases the application's functionality, including authentication, network topology visualisation, graph traversal, network optimisation and telemetry management.

3. Core System Features

Smart-X integrates several key functionalities into one application.

Feature	Description
Authentication	JWT-based authentication for accessing the application.
IoT Network Management	Monitoring and managing connected ESP32 devices.
Network Topology	Visual representation of relationships between network nodes.
Breadth-First Search (BFS)	Traversal of the network to explore connected devices.
Prim's Minimum Spanning Tree	Identification of a minimum-cost network spanning structure.
Telemetry Management	Collection and processing of historical sensor information.
Binary Search Tree	Efficient organisation and retrieval of timestamp-based telemetry records.
Network Visualisation	Graphical representation of network structures and algorithmic operations.
4. Technology Stack

The application combines embedded hardware, backend development and desktop application technologies.

Technology	Purpose
C#	Core application logic and backend development.
.NET 8	Application framework and runtime.
ASP.NET Core Web API	Backend services and communication.
Visual Studio 2022	Development and debugging environment.
ESP32	Embedded IoT microcontroller platform.
JSON Web Tokens (JWT)	Authentication and token-based access.
Graph Data Structures	Modelling IoT network connections.
Binary Search Tree	Organising historical telemetry data.
5. System Architecture

Smart-X follows a hybrid architecture that separates device-level communication, backend processing and application-level interaction.

Architectural Components

1. IoT Device Layer

The device layer consists of ESP32 microcontrollers responsible for interacting with sensors and participating in the IoT network.

2. Backend API Layer

The ASP.NET Core Web API provides the central processing layer. It handles application requests, processes information and exposes functionality to the client application.

3. Client Application Layer

The client provides the user interface through which users can authenticate, interact with the ecosystem, monitor network information and view algorithmic results.

4. Data Structures and Algorithms Layer

This layer provides the underlying computational logic for managing network relationships, traversing graph structures, optimising connectivity and retrieving historical telemetry.

High-Level Architecture
text
       ESP32 IoT DEVICES
      /       |        \
   Node 1   Node 2    Node 3
      \       |        /
       IoT MESH NETWORK
              |
              v
     SMART-X BACKEND API
        ASP.NET Core
              |
       Data Processing
              |
      DATA STRUCTURES
       /           \
   Graphs          BST
      |              |
  BFS / Prim     Telemetry
       \           /
        \         /
         v       v
      SMART-X CLIENT
       User Interface
              |
       Monitoring &
       Visualisation

6. Advanced Data Structures and Algorithms

A major objective of Smart-X is to demonstrate how advanced data structures and algorithms can improve the organisation, processing and management of information in an IoT environment.

6.1 Binary Search Tree (BST) – Historical Telemetry Management

Implementation: SmartX.API/Models/TelemetryTree.cs

The Binary Search Tree is implemented to organise historical telemetry records according to their timestamps.

Instead of relying exclusively on sequential searches through an unsorted collection, the BST organises records into a hierarchical structure.

This allows the application to eliminate entire branches of the tree when searching for records within a specified timestamp range.

How it works:

Each node stores a telemetry record.
Records with earlier timestamps are inserted into the left subtree.
Records with later timestamps are inserted into the right subtree.
Duplicate timestamps are not inserted by the current comparison logic.
Range queries use timestamp comparisons to avoid exploring irrelevant branches.

Performance Analysis:

Operation	Average Complexity	Worst Case
Insertion	O(log n)	O(n)
Search	O(log n)	O(n)
Range Query	O(log n + k)*	O(n)

k represents the number of matching records returned, assuming a balanced tree and efficient range traversal.

Why this matters:

IoT environments can generate substantial volumes of telemetry data. Efficiently retrieving historical sensor information is important when analysing device behaviour, monitoring network performance and displaying historical trends.

The BST demonstrates how an appropriate data structure can reduce unnecessary comparisons during timestamp-based queries.

Implementation Example:

csharp
public void Insert(TelemetryRecord record)
    => Root = InsertRec(Root, record);

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

6.2 Breadth-First Search (BFS) – Network Traversal

Algorithm: Breadth-First Search

BFS is used to explore the IoT network by visiting connected nodes level by level.

Starting from a selected device, the algorithm visits its immediate neighbours before exploring nodes further away.

How it works:

Select a starting node.
Mark the starting node as visited.
Add it to a queue.
Remove the next node from the queue.
Visit its unvisited neighbours.
Continue until the queue is empty.

Time Complexity: O(V + E)

Where:

V = Number of vertices (network devices).
E = Number of edges (network connections).

Application in Smart-X:

BFS provides a systematic way to explore device connectivity and understand how nodes are connected within the mesh network.

6.3 Prim's Algorithm – Minimum Spanning Tree

Algorithm: Prim's Minimum Spanning Tree (MST)

Prim's algorithm is implemented to determine a minimum spanning tree for a weighted network graph.

A minimum spanning tree connects all vertices in a connected, weighted, undirected graph while minimising the total edge weight and avoiding cycles.

How it works:

Select an initial vertex.
Identify the lowest-weight edge connecting the current tree to an unvisited vertex.
Add that edge and vertex to the spanning tree.
Repeat until all vertices are included.

Application in Smart-X:

The algorithm demonstrates how network connections can be evaluated according to their weights to identify a minimum-cost spanning structure.

In an IoT environment, this provides a useful foundation for exploring efficient network connectivity.

Important distinction: A minimum spanning tree is not automatically a complete routing protocol. Actual network deployment would also need to consider factors such as link reliability, latency, energy consumption and changing device availability.

7. Application Screenshots

The following screenshots demonstrate the implemented Smart-X application and its major functionality.

7.1 Authentication and Application Dashboard

The application provides an authentication interface before users access the main Smart-X environment.

!Smart-X Authentication and Dashboard

Figure 1: Authentication interface and main application landing page.

7.2 Network Topology and Graph Algorithms

!Smart-X Network Topology

Figure 2: Network topology visualisation demonstrating BFS traversal and Prim's Minimum Spanning Tree.

7.3 Additional Application Functionality

The following screenshots provide further evidence of the application's implemented interfaces, features and functionality.

!Smart-X Application Screenshot 3

Figure 3: Additional application interface.

!Smart-X Application Screenshot 4

Figure 4: Application functionality and system interface.

!Smart-X Application Screenshot 5

Figure 5: Additional system functionality.

!Smart-X Application Screenshot 6

Figure 6: Application interface and functionality.

!Smart-X Application Screenshot 7

Figure 7: Additional Smart-X system interface.

!Smart-X Application Screenshot 8

Figure 8: Application functionality.

!Smart-X Application Screenshot 9

Figure 9: Additional system interface.

!Smart-X Application Screenshot 10

Figure 10: Additional Smart-X functionality.

8. Installation and Execution

Follow these instructions to compile and run the Smart-X application locally.

8.1 Prerequisites

Ensure the following software is installed:

.NET 8 SDK
Visual Studio 2022
ASP.NET and web development workload in Visual Studio.
Git.
ESP32 hardware, if testing physical IoT device integration.
8.2 Clone the Repository

Open a terminal and execute:

bash
git clone https://github.com/katlegomothapo10/SmartX-IoT-PROG.git


Navigate into the repository:

bash
cd SmartX-IoT-PROG

8.3 Open the Solution

Open the solution file:

text
SmartX.sln


Alternatively, open it directly using Visual Studio 2022.

8.4 Configure Startup Projects
Open Solution Explorer.
Right-click the solution.
Select Configure Startup Projects.
Choose Multiple startup projects.
Set SmartX.API to Start.
Set SmartX.Client to Start.
Apply the configuration.
8.5 Build and Run

Press:

text
F5


Visual Studio will build and launch the configured startup projects.

Note: Ensure the API is running and accessible to the client application. If necessary, verify the API base URL and development configuration before launching.

9. Authentication

Smart-X uses JSON Web Token (JWT) authentication to demonstrate token-based authentication between the client and backend.

Authentication Workflow
The user enters their login credentials.
The application submits the authentication request.
The backend processes the request.
A JWT is issued following successful authentication.
The client uses the token when communicating with protected API endpoints.
Demo Credentials

For the current demonstration implementation:

Field	Example
Email	admin@smartx.com
Password	1234

The demonstration implementation accepts a valid email address and a password containing at least four characters.

Security note: These are demonstration authentication rules, not production-grade identity verification. A production deployment should implement proper user registration, password hashing, credential verification, token expiry and secure secret management.

10. IoT Energy Efficiency and Battery Considerations

Energy efficiency is an important consideration when designing IoT systems, particularly when using battery-powered ESP32 microcontrollers.

Smart-X explores the broader objective of reducing unnecessary energy consumption while maintaining effective device communication and network functionality.

Potential energy-efficiency considerations include:

Efficient communication: Avoiding unnecessary transmissions between devices.
Reduced processing overhead: Selecting appropriate algorithms and data structures to minimise computational work.
Optimised network connectivity: Evaluating network structures to reduce unnecessary communication links.
Power management: Using ESP32 power-management capabilities and sleep modes where supported.
Telemetry management: Avoiding unnecessary data processing and repeated requests.

The combination of network algorithms and efficient device management provides a foundation for investigating how intelligent IoT systems can balance connectivity, performance and battery consumption.

11. Learning Outcomes and Technical Objectives

The Smart-X project demonstrates the practical application of several important computer science and software engineering concepts.

Through its implementation, the project brings together:

Object-Oriented Programming: Structuring application functionality into reusable and maintainable components.
Data Structures: Applying Binary Search Trees and graph representations to organise information.
Algorithm Design: Implementing BFS and Prim's algorithm to solve network-related problems.
Software Architecture: Separating client-side functionality from backend services.
API Development: Building communication between application components using ASP.NET Core.
Authentication: Applying JWT-based authentication concepts.
IoT Integration: Exploring communication and management within an ESP32-based environment.
Performance Analysis: Evaluating algorithmic time complexity and the trade-offs associated with different approaches.

The project demonstrates that theoretical computer science concepts can be translated into practical functionality within a modern software application.

12. Future Improvements

Although Smart-X demonstrates the integration of multiple IoT and software engineering concepts, several opportunities exist for further development:

Advanced IoT Communication: Extend device-to-device communication and support additional mesh networking capabilities.
Real-Time Telemetry: Introduce continuous telemetry streaming and live sensor dashboards.
Energy-Aware Routing: Extend routing algorithms to consider battery levels and communication costs.
Improved Data Storage: Introduce more advanced indexing and persistent telemetry storage.
Enhanced Security: Implement stronger identity verification, role-based access control and secure device authentication.
Fault Tolerance: Improve network resilience by handling disconnected devices and communication failures.

These improvements would move the system closer to a more comprehensive, production-oriented IoT ecosystem.

13. Repository Information

Project: Smart-X IoT Mesh Ecosystem

Repository: SmartX-IoT-PROG

Framework: .NET 8

Primary Language: C#

Embedded Platform: ESP32

Project Scope: Parts 1, 2 and 3

14. Conclusion

Smart-X IoT Mesh Ecosystem demonstrates how embedded systems, software engineering, data structures and graph algorithms can be integrated into a unified IoT management platform.

By combining ESP32-based IoT concepts with a C# and ASP.NET Core application, the project provides a practical environment for exploring network connectivity, telemetry management, graph traversal and network optimisation.

The implementation of Binary Search Trees, Breadth-First Search and Prim's Minimum Spanning Tree highlights the importance of selecting appropriate computational techniques when developing systems that process interconnected data.

Ultimately, Smart-X is more than an IoT monitoring application. It is a practical demonstration of how theoretical computer science principles can be applied to address challenges in distributed systems, network management and intelligent device ecosystems.

Smart-X IoT Mesh Ecosystem

Connecting devices. Processing intelligence. Optimising networks.
