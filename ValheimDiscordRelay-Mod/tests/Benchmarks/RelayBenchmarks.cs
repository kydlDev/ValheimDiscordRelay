using System;
using BenchmarkDotNet.Attributes;
using ValheimDiscordRelay.Client;
using ValheimDiscordRelay.Server;

namespace ValheimDiscordRelay.Benchmarks;

/// <summary>Relay rules that run per chat line, per death, and per captured frame of a clip.</summary>
[MemoryDiagnoser]
public class RelayBenchmarks
{
    RecentPicker _picker;
    Random _random;
    CaptureScheduler _scheduler;
    float[] _times;
    DateTime _now;
    double _t;
    static readonly Action<string> _noWarn = s => { };

    [Params(12, 200)]
    public int Quotes { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _picker = new RecentPicker();
        _random = new Random(5);
        _scheduler = new CaptureScheduler();
        _now = new DateTime(2026, 11, 16, 12, 0, 0, DateTimeKind.Utc);
        _times = new float[90];
        for (int i = 0; i < _times.Length; i++)
        {
            _times[i] = i / 30f;
        }
    }

    [Benchmark]
    public int PickQuote() => _picker.Next(Quotes, _random);

    [Benchmark]
    public bool ChatAllow()
    {
        _now = _now.AddSeconds(2);   // the sender stays within its allowance
        return ChatRateLimiter.Allow(42, "Kydl", _now, _noWarn);
    }

    [Benchmark]
    public bool CaptureBeginEnd()
    {
        _t += 0.05;
        bool ok = _scheduler.TryBegin(_t, 20);
        if (ok) _scheduler.End();
        return ok;
    }

    [Benchmark]
    public int ClipDurations() => FrameTiming.ComputeDurationsMs(_times, 30).Length;
}
