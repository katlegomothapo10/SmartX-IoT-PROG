using SmartX.API.Models;

namespace SmartX.API.Services;

public class DeploymentValidator
{
    public bool ValidateDeploymentTree(string nodeId, List<DeploymentNode> tree)
    {
        if (string.IsNullOrEmpty(nodeId)) return false;
        if (tree == null || tree.Count == 0) return false;

        foreach (var root in tree)
        {
            if (ValidateNode(root, nodeId))
                return true;
        }

        return false;
    }

    private bool ValidateNode(DeploymentNode node, string targetNodeId)
    {
        if (node == null) return false;

        if (node.NodeId == targetNodeId)
        {
            return ValidateNodeConfiguration(node);
        }

        foreach (var child in node.Children)
        {
            if (ValidateNode(child, targetNodeId))
                return true;
        }

        return false;
    }

    private bool ValidateNodeConfiguration(DeploymentNode node)
    {
        if (string.IsNullOrEmpty(node.ZoneId)) return false;
        if (string.IsNullOrEmpty(node.FacilityId)) return false;
        if (string.IsNullOrEmpty(node.Name)) return false;
        if (!node.IsConfigured) return false;

        foreach (var device in node.Devices)
        {
            if (string.IsNullOrEmpty(device)) return false;
        }

        foreach (var child in node.Children)
        {
            if (!ValidateNodeConfiguration(child)) return false;
        }

        return true;
    }

    public ValidationReport ValidateEntireTree(List<DeploymentNode> tree)
    {
        var report = new ValidationReport();

        if (tree == null || tree.Count == 0)
        {
            report.IsValid = false;
            report.Errors.Add("Deployment tree is empty.");
            return report;
        }

        foreach (var root in tree)
        {
            ValidateSubtree(root, report, 0);
        }

        report.IsValid = report.Errors.Count == 0;
        return report;
    }

    private void ValidateSubtree(DeploymentNode node, ValidationReport report, int depth)
    {
        if (node == null)
        {
            report.Errors.Add($"Null node encountered at depth {depth}.");
            return;
        }

        report.TotalNodes++;
        report.MaxDepthReached = Math.Max(report.MaxDepthReached, depth);

        if (string.IsNullOrEmpty(node.NodeId))
            report.Errors.Add($"Node at depth {depth} has no NodeId.");

        if (string.IsNullOrEmpty(node.ZoneId))
            report.Errors.Add($"Node {node.NodeId} has no ZoneId.");

        if (string.IsNullOrEmpty(node.FacilityId))
            report.Errors.Add($"Node {node.NodeId} has no FacilityId.");

        if (!node.IsConfigured)
            report.Errors.Add($"Node {node.NodeId} is marked as not configured.");

        report.TotalDevices += node.Devices.Count;

        foreach (var child in node.Children)
        {
            ValidateSubtree(child, report, depth + 1);
        }
    }
}

public class ValidationReport
{
    public bool IsValid { get; set; }
    public int TotalNodes { get; set; }
    public int TotalDevices { get; set; }
    public int MaxDepthReached { get; set; }
    public List<string> Errors { get; set; } = new();
}