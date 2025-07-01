namespace Nodify.Helpers;

public static class DesignatorManager
{

    private static readonly Dictionary<string, SortedSet<int>> _usedIndices = [];

    public static int Generate(string prefix)
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
        return i;

    }
    public static void Clear()
    {
        _usedIndices.Clear();
    }

    public static int Recover(string prefix, int designator)
    {

        if (!_usedIndices.TryGetValue(prefix, out var set))
        {
            set = [];
            _usedIndices[prefix] = set;
        }

        if (!set.Add(designator))
            throw new InvalidOperationException($"Designator '{designator}' уже существует.");

        return designator;

    }

    public static void Release(string prefix, int designator)
    {
        if (!_usedIndices.TryGetValue(prefix, out var set)) return;
        var item = 0;
        var find = false;
        for (var i = 0; i < set.Count; i++)
        {
            item = set.ElementAt(i);
            if (item != designator) continue;
            find = true;
            break;
        }
        if(find)
            set.Remove(item);
    }
}

