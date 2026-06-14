// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace BasicTestApp;

public class CacheService
{
    private readonly Dictionary<int, DynamicItem> _cache = new();

    public List<DynamicItem> Get(int start, int count)
    {
        var items = new List<DynamicItem>();

        for (int i = start; i < start + count; i++)
        {
            if (!_cache.TryGetValue(i, out var item))
            {
                break;
            }

            items.Add(item);
        }

        return items;
    }

    public void Add(List<DynamicItem> items, int startIndex)
    {
        for (int i = 0; i < items.Count; i++)
        {
            int targetIndex = startIndex + i;

            if (_cache.ContainsKey(targetIndex))
            {
                continue;
            }

            _cache[targetIndex] = items[i];
        }
    }
    public void Clear() => _cache.Clear();
}
public class DynamicItem
{
    public int Index { get; set; }
    public int Height { get; set; }
}
