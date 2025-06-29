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
        return $"{prefix} {i}";
    }

    public static void Release(string designator)
    {
        var pos = designator.Length - 1;
        while (pos >= 0 && char.IsDigit(designator[pos]))
            pos--;

        var prefix = designator.Substring(0, pos+1);
        if (!int.TryParse(designator[(pos + 1)..], out var idx))
            return;

        if (_usedIndices.TryGetValue(prefix.Trim(), out var set))
            set.Remove(idx);
    }
}

