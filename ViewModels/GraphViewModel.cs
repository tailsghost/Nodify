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
        var nodes = new NodeModel[n];
        Graph.Nodes.Values.CopyTo(nodes, 0);

        var adj = new List<int>[n];
        var indegree = new int[n];
        for (var i = 0; i < n; i++)
        {
            adj[i] = [];
            indegree[i] = 0;
        }

        var m = Graph.Edges.Count;
        for (var i = 0; i < m; i++)
        {
            var edge = Graph.Edges[i];
            var uNode = edge.Source.Parent as NodeModel;
            var vNode = edge.Target.Parent as NodeModel;

            var uIndex = -1;
            var vIndex = -1;
            for (var j = 0; j < n; j++)
            {
                if (nodes[j] == uNode) uIndex = j;
                if (nodes[j] == vNode) vIndex = j;
                if (uIndex != -1 && vIndex != -1) break;
            }

            if (uIndex < 0 || vIndex < 0)
                continue;

            adj[uIndex].Add(vIndex);
            indegree[vIndex]++;
        }

        var queue = new Queue<int>();
        for (var i = 0; i < n; i++)
        {
            if (indegree[i] == 0)
                queue.Enqueue(i);
        }

        var result = new List<NodeModel>(n);
        while (queue.Count > 0)
        {
            var u = queue.Dequeue();
            result.Add(nodes[u]);

            var cnt = adj[u].Count;
            for (var k = 0; k < cnt; k++)
            {
                var v = adj[u][k];
                indegree[v]--;
                if (indegree[v] == 0)
                    queue.Enqueue(v);
            }
        }

        if (result.Count != n)
            throw new InvalidOperationException("Граф содержит цикл, топологическая сортировка невозможна.");

        return result;
    }
}


