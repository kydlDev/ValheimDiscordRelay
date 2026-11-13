using System;
using System.Collections.Generic;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// A list of quote templates that hands out a random one, never one of the last seven.
/// Pure logic, no game types: the lists themselves are in <see cref="CrushQuotes"/>, <see cref="WaterQuotes"/> and
/// <see cref="BossKillerQuotes"/>.
/// </summary>
internal sealed class QuotePool
{
    private static readonly Random Rng = new(Guid.NewGuid().GetHashCode());
    private static readonly object RngLock = new();

    private readonly string[] _quotes;
    private readonly RecentPicker _recent = new();

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
    /// A random template that was not one of the last seven handed out, so the same joke does not come back too soon.
    /// </summary>
    internal string Pick()
    {
        lock (RngLock)
            return _quotes[_recent.Next(_quotes.Length, Rng)];
    }
}
