using ValheimDiscordRelay.Client;

namespace ValheimDiscordRelay.Server;

/// <summary>
/// The short lines said in the in-game chat when a boss dies. Placeholder: {Boss}, the boss's name. Pure logic, no game types.
/// </summary>
internal static class BossShoutQuotes
{
    internal static readonly QuotePool Pool = new QuotePool(
        "{Boss} has fallen!",
        "{Boss} has been slain. The ground is a little quieter now.",
        "{Boss} is dead. Odin nods, slowly.",
        "{Boss} has been defeated! Time for a feast.",
        "The mighty {Boss} has met its end. Skål!");

    /// <summary>A random line for this boss.</summary>
    /// <param name="boss">The boss's name.</param>
    internal static string Pick(string boss)
    {
        return Pool.Pick().Replace("{Boss}", boss);
    }
}
