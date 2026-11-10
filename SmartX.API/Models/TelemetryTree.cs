namespace SmartX.API.Models;

public class TelemetryTreeNode
{
    public DateTime Timestamp { get; set; }
    public TelemetryRecord Data { get; set; }
    public TelemetryTreeNode? Left { get; set; }
    public TelemetryTreeNode? Right { get; set; }

    public TelemetryTreeNode(TelemetryRecord data)
    {
        Timestamp = data.Timestamp;
        Data = data;
    }
}

public class TelemetryTree
{
    public TelemetryTreeNode? Root { get; private set; }

    public void Insert(TelemetryRecord record)
    {
        Root = InsertRec(Root, record);
    }

    private TelemetryTreeNode InsertRec(TelemetryTreeNode? node, TelemetryRecord record)
    {
        if (node == null) return new TelemetryTreeNode(record);

        if (record.Timestamp < node.Timestamp)
            node.Left = InsertRec(node.Left, record);
        else if (record.Timestamp > node.Timestamp)
            node.Right = InsertRec(node.Right, record);

        return node;
    }

    // The core requirement: Range Query
    public List<TelemetryRecord> GetRange(DateTime start, DateTime end)
    {
        var results = new List<TelemetryRecord>();
        GetRangeRec(Root, start, end, results);
        return results;
    }

    private void GetRangeRec(TelemetryTreeNode? node, DateTime start, DateTime end, List<TelemetryRecord> results)
    {
        if (node == null) return;

        // If start is less than node's time, search left subtree
        if (start < node.Timestamp)
            GetRangeRec(node.Left, start, end, results);

        // If node's time is within range, add it
        if (start <= node.Timestamp && end >= node.Timestamp)
            results.Add(node.Data);

        // If end is greater than node's time, search right subtree
        if (end > node.Timestamp)
            GetRangeRec(node.Right, start, end, results);
    }
}