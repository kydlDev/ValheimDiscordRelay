using System;
using System.Collections.Generic;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// Chooses a random entry of a list of a fixed size and never one of the last <see cref="Window"/> entries it handed out, so the same
/// message does not come back before seven others have been used (a list with fewer entries repeats as late as it can).
/// Pure logic, no game types. Not thread safe: the caller holds its own lock.
/// </summary>
internal sealed class RecentPicker
{
    /// <summary>How many of the latest choices are kept out of the next one.</summary>
    internal const int Window = 7;

    private readonly Queue<int> _recent = new();

    /// <summary>The index of the next entry.</summary>
    /// <param name="count">The number of entries of the list (the same every time for one picker).</param>
    /// <param name="random">The random source.</param>
    /// <returns>An index from 0 to count - 1 that is not one of the last <see cref="Window"/> results; 0 for a list of one.</returns>
    internal int Next(int count, Random random)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count), "A list needs at least one entry.");

        if (count == 1)
            return 0;

        int keep = Math.Min(Window, count - 1);
        while (_recent.Count > keep)
            _recent.Dequeue();

        List<int> allowed = new(count);
        for (int i = 0; i < count; i++)
        {
            if (!_recent.Contains(i))
                allowed.Add(i);
        }

        int index = allowed[random.Next(allowed.Count)];
        _recent.Enqueue(index);
        while (_recent.Count > keep)
            _recent.Dequeue();

        return index;
    }
}
