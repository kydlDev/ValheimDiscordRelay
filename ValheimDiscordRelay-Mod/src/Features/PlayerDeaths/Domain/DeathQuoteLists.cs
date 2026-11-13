using System;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// Which quote list belongs to which kind of death.
/// </summary>
internal static class DeathQuoteLists
{
    /// <summary>
    /// The quote list of a kind of death.
    /// </summary>
    /// <param name="kind">The kind of death.</param>
    /// <returns>The pool to pick the message from.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The kind has no list (a kind was added without one).</exception>
    internal static QuotePool For(DeathQuoteKind kind)
    {
        switch (kind)
        {
            case DeathQuoteKind.Tree: return CrushQuotes.Tree;
            case DeathQuoteKind.Cart: return CrushQuotes.Cart;
            case DeathQuoteKind.Boat: return CrushQuotes.Boat;
            case DeathQuoteKind.Structure: return CrushQuotes.Structure;
            case DeathQuoteKind.Stalagmite: return CrushQuotes.Stalagmite;
            case DeathQuoteKind.DrawBridge: return CrushQuotes.DrawBridge;
            case DeathQuoteKind.Turret: return CrushQuotes.Turret;
            case DeathQuoteKind.Catapult: return CrushQuotes.Catapult;
            case DeathQuoteKind.Impact: return CrushQuotes.Impact;
            case DeathQuoteKind.Swimming: return WaterQuotes.Swimming;
            case DeathQuoteKind.AshlandsSea: return WaterQuotes.AshlandsSea;
            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, "No quote list for this kind of death.");
        }
    }
}
