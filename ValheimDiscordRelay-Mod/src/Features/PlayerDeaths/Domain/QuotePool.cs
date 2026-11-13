using System;
using System.Collections.Generic;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// A list of quote templates that hands out a random one, never the same one twice in a row (when there is more than one).
/// Pure logic, no game types: the lists themselves are in <see cref="CrushQuotes"/>, <see cref="WaterQuotes"/> and
/// <see cref="BossKillerQuotes"/>.
/// </summary>
internal sealed class QuotePool
{
    private static readonly Random Rng = new(Guid.NewGuid().GetHashCode());
    private static readonly object RngLock = new();

    private readonly string[] _quotes;
    private int _last = -1;

    /// <param name="quotes">The templates, with the placeholders their list documents. At least one.</param>
    internal QuotePool(params string[] quotes)
    {
        if (quotes == null || quotes.Length == 0)
            throw new ArgumentException("A quote pool needs at least one quote.", nameof(quotes));

        _quotes = quotes;
    }

    /// <summary>Every template of the pool, in the order they are written.</summary>
    internal IReadOnlyList<string> Quotes => _quotes;

    /// <summary>
    /// A random template. The one that was handed out last time is skipped, so the same joke never comes twice in a row.
    /// </summary>
    internal string Pick()
    {
        lock (RngLock)
        {
            int index = Rng.Next(_quotes.Length);
            if (_quotes.Length > 1 && index == _last)
                index = (index + 1 + Rng.Next(_quotes.Length - 1)) % _quotes.Length;

            _last = index;
            return _quotes[index];
        }
    }
}
