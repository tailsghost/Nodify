using Nodify.Models;

namespace Nodify.ViewModels;

public class GraphViewModel
{
    public GraphModel Graph { get; }
    public GraphViewModel(GraphModel graphModel)
    {
        Graph = graphModel;
    }

    public List<NodeModel> TopoSort()
    {
        var n = Graph.Nodes.Count;

        var keys = new Guid[n];

        Graph.Nodes.Keys.CopyTo(keys, 0);

        var nodeArr = new NodeModel[n];

        for (var i = 0; i < n; i++)
        {
            nodeArr[i] = Graph.Nodes[keys[i]];
        }

        var inDegree = new Dictionary<NodeModel, int>();

        for (var i = 0; i < n; i++)
        {
            inDegree[nodeArr[i]] = 0;
        }

        var adj = new Dictionary<NodeModel, List<NodeModel>>(n);

        for (var i = 0; i < n; i++)
        {
            adj[nodeArr[i]] = [];
        }

        for (var i = 0; i < Graph.Edges.Count; i++)
        {
            var edge = Graph.Edges[i];
            var src = edge.Source.Parent as NodeModel;
            var tgt = edge.Target.Parent as NodeModel;

            if (src == null || tgt == null)
                continue;

            adj[src].Add(tgt);
            inDegree[tgt] = inDegree[tgt] + 1;
        }

        var queue = new Queue<NodeModel>();
        for (var i = 0; i < n; i++)
        {
            var node = nodeArr[i];
            if (inDegree[node] == 0)
                queue.Enqueue(node);
        }

        var sorted = new List<NodeModel>(n);

        while (queue.Count > 0)
        {
            var u = queue.Dequeue();
            sorted.Add(u);

            var neighbours = adj[u];
            for (var i = 0; i < neighbours.Count; i++)
            {
                var v = neighbours[i];
                inDegree[v] = inDegree[v] - 1;
                if (inDegree[v] == 0)
                    queue.Enqueue(v);
            }
        }


        if (sorted.Count != n)
            throw new InvalidOperationException("Граф содержит цикл — топологическая сортировка невозможна.");


        return sorted;
    }
}


