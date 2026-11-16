using BenchmarkDotNet.Running;

namespace ValheimDiscordRelay.Benchmarks;

/// Entry point. Pass the arguments on (filter, job): without them BenchmarkDotNet ignores the command line.
public static class Program
{
    public static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
