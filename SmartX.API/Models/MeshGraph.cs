namespace SmartX.API.Models;

public class MeshEdge
{
    public string TargetMac { get; set; } = string.Empty;
    public int Weight { get; set; } // Lower weight = better connection
}

public class MeshNode
{
    public string MacAddress { get; set; } = string.Empty;
    public List<MeshEdge> Edges { get; set; } = new();
}

public class MeshGraph
{
    public Dictionary<string, MeshNode> Nodes { get; } = new();

    public void AddNode(string mac)
    {
        if (!Nodes.ContainsKey(mac))
            Nodes[mac] = new MeshNode { MacAddress = mac };
    }

    public void AddEdge(string sourceMac, string targetMac, int weight)
    {
        if (!Nodes.ContainsKey(sourceMac) || !Nodes.ContainsKey(targetMac)) return;

        Nodes[sourceMac].Edges.Add(new MeshEdge { TargetMac = targetMac, Weight = weight });
        Nodes[targetMac].Edges.Add(new MeshEdge { TargetMac = sourceMac, Weight = weight }); // Undirected
    }

    // BFS Traversal (Finding all connected nodes)
    public List<string> BFS(string startMac)
    {
        var visited = new List<string>();
        if (!Nodes.ContainsKey(startMac)) return visited;

        var queue = new Queue<string>();
        var seen = new HashSet<string>();

        queue.Enqueue(startMac);
        seen.Add(startMac);

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
        return visited;
    }

    // Prim's Algorithm for Minimum Spanning Tree (MST)
    public List<MeshEdge> GetMinimumSpanningTree(string startMac)
    {
        var mst = new List<MeshEdge>();
        if (!Nodes.ContainsKey(startMac)) return mst;

        var visited = new HashSet<string>();
        // Priority Queue stores (Weight, SourceMac, Edge)
        var pq = new PriorityQueue<(string Source, MeshEdge Edge), int>();

        // Start with the first node
        visited.Add(startMac);
        foreach (var edge in Nodes[startMac].Edges)
        {
            pq.Enqueue((startMac, edge), edge.Weight);
        }

        while (pq.Count > 0 && visited.Count < Nodes.Count)
        {
            var (source, edge) = pq.Dequeue();

            if (visited.Contains(edge.TargetMac)) continue;

            visited.Add(edge.TargetMac);
            mst.Add(edge); // Add this edge to the MST

            // Add new edges from the newly visited node
            foreach (var nextEdge in Nodes[edge.TargetMac].Edges)
            {
                if (!visited.Contains(nextEdge.TargetMac))
                {
                    pq.Enqueue((edge.TargetMac, nextEdge), nextEdge.Weight);
                }
            }
        }

        return mst;
    }
}