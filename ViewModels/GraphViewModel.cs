using Nodify.Interfaces;
using Nodify.Models;

namespace Nodify.ViewModels;

public class GraphViewModel
{
    public GraphViewModel(GraphModel graphModel)
    {
        Graph = graphModel;
    }

    public GraphModel Graph { get; }

    private List<NodeModel> GetAllNodes()
    {
        var result = new List<NodeModel>();

        foreach (var node in Graph.Nodes.Values)
            result.Add(node);

        for (var i = 0; i < Graph.Containers.Count; i++)
        {
            var container = Graph.Containers[i];
            for (var j = 0; j < container.Nodes.Count; j++)
                result.Add(container.Nodes[j]);
        }

        return result;
    }

    public List<NodeModel> GetConnectedNodes(NodeModel startNode)
    {
        var visited = new HashSet<Guid>();
        var queue = new Queue<NodeModel>();
        var result = new List<NodeModel>();

        var hasConnections = false;
        for (var i = 0; i < Graph.Edges.Count; i++)
        {
            var edge = Graph.Edges[i];
            if (edge.Source.Parent != startNode && edge.Target.Parent != startNode) continue;
            hasConnections = true;
            break;
        }

        if (!hasConnections)
            return result;

        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (visited.Contains(current.Id))
                continue;

            visited.Add(current.Id);
            result.Add(current);

            for (var i = 0; i < Graph.Edges.Count; i++)
            {
                var edge = Graph.Edges[i];

                NodeModel? neighbor = null;

                if (edge.Source.Parent == current && edge.Target.Parent is NodeModel tgtNode)
                    neighbor = tgtNode;
                else if (edge.Target.Parent == current && edge.Source.Parent is NodeModel srcNode)
                    neighbor = srcNode;

                if (neighbor != null && !visited.Contains(neighbor.Id))
                    queue.Enqueue(neighbor);
            }
        }

        return result;
    }

    public List<List<IConnectable>> GetAllConnectedGroups()
    {
        var result = new List<List<IConnectable>>();
        var visited = new HashSet<Guid>();

        var allConnectables = new List<IConnectable>();

        foreach (var node in Graph.Nodes.Values)
            allConnectables.Add(node);

        for (var i = 0; i < Graph.Containers.Count; i++)
        {
            var container = Graph.Containers[i];
            allConnectables.Add(container);

            for (var j = 0; j < container.Nodes.Count; j++)
                allConnectables.Add(container.Nodes[j]);
        }

        for (var i = 0; i < allConnectables.Count; i++)
        {
            var connectable = allConnectables[i];
            if (visited.Contains(connectable.Id))
                continue;

            var hasConnection = false;
            for (var e = 0; e < Graph.Edges.Count; e++)
            {
                var edge = Graph.Edges[e];
                if (edge.Source.Parent == connectable || edge.Target.Parent == connectable)
                {
                    hasConnection = true;
                    break;
                }
            }

            if (!hasConnection)
                continue;

            var group = new List<IConnectable>();
            var queue = new Queue<IConnectable>();
            queue.Enqueue(connectable);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (visited.Contains(current.Id))
                    continue;

                visited.Add(current.Id);
                group.Add(current);

                for (var e = 0; e < Graph.Edges.Count; e++)
                {
                    var edge = Graph.Edges[e];

                    IConnectable? neighbor = null;

                    if (edge.Source.Parent == current && edge.Target.Parent is IConnectable target)
                        neighbor = target;
                    else if (edge.Target.Parent == current && edge.Source.Parent is IConnectable source)
                        neighbor = source;

                    if (neighbor != null && !visited.Contains(neighbor.Id))
                        queue.Enqueue(neighbor);
                }
            }

            result.Add(group);
        }

        return result;
    }
}

