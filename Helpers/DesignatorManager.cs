using System.Text.RegularExpressions;

namespace Nodify.Helpers;

public static class DesignatorManager
{

    private static readonly Dictionary<string, SortedSet<int>> _usedIndices = [];

    public static string Generate(string prefix)
    {

        if (!_usedIndices.TryGetValue(prefix, out var set))
        {
            set = [];
            _usedIndices[prefix] = set;
        }

        var i = 0;
        for (var j = 0; i < set.Count; j++)
        {
            var inx = set.ElementAt(j);
            if (i == inx)
                i++;
            else
                break;
        }

        set.Add(i);
        return $"{prefix}_{i}";

    }
    public static void Clear()
    {
        _usedIndices.Clear();
    }

    public static string Recover(string designator)
    {
        var parts = designator.Split('_', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 1 || !int.TryParse(parts[^1], out var index)) return Generate(designator);

        var basePrefix = string.Join("_", parts[..^1]);

        if (!_usedIndices.TryGetValue(basePrefix, out var set))
        {
            set = [];
            _usedIndices[basePrefix] = set;
        }

        if (!set.Add(index))
            throw new InvalidOperationException($"Designator '{designator}' уже существует.");

        return designator;

    }

    public static void Release(string designator)
    {
        var pos = designator.Length - 1;
        while (pos >= 0 && char.IsDigit(designator[pos]))
            pos--;

        var prefix = designator[..(pos + 1)];
        if (!int.TryParse(designator[(pos + 1)..], out var idx))
            return;

        if (_usedIndices.TryGetValue(prefix, out var set))
            set.Remove(idx);
    }
}

