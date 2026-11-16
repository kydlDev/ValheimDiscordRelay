namespace ValheimDiscordRelay
{
    /// <summary>Stand-in for the mod's RelayDiagnostics (which forwards to KydlCore's ModLog, a netstandard 2.1 assembly the net48 benchmark exe cannot load).
    /// ChatRateLimiter only needs the symbol: the benchmarks pass their own warning sink to the overload with the clock.</summary>
    internal static class RelayDiagnostics
    {
        internal static void Warning(string message) { }
    }
}
