using System;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// What a recorded video will look like when played: how fast its footage was captured, and how long it plays compared
/// with how long the footage really lasted. Pure logic: no game or engine types, unit tested.
/// </summary>
internal readonly struct PlaybackReport
{
    /// <summary>
    /// Creates a report.
    /// </summary>
    /// <param name="rate">How fast the footage was captured.</param>
    /// <param name="playbackMs">How long the finished video plays, in milliseconds.</param>
    internal PlaybackReport(CaptureRate rate, long playbackMs)
    {
        Rate = rate;
        PlaybackMs = playbackMs;
    }

    /// <summary>How fast the footage was captured.</summary>
    internal CaptureRate Rate { get; }

    /// <summary>How long the finished video plays, in milliseconds.</summary>
    internal long PlaybackMs { get; }

    /// <summary>
    /// Playing time divided by the length of the footage. 1 means the video plays in real time, below 1 means it plays
    /// too fast. 0 when the footage has no length.
    /// </summary>
    internal double PlaybackToFootageRatio => Rate.SpanSeconds <= 0d ? 0d : (PlaybackMs / 1000d) / Rate.SpanSeconds;

    /// <summary>
    /// True when the video plays in real time, give or take a share of its length.
    /// </summary>
    /// <param name="tolerance">The allowed difference as a share of the footage length, for example 0.1 for 10 %.</param>
    /// <returns>True when the playing time is within the tolerance of the footage length.</returns>
    internal bool PlaysInRealTime(double tolerance)
    {
        double ratio = PlaybackToFootageRatio;
        return ratio > 0d && Math.Abs(ratio - 1d) <= tolerance;
    }

    /// <summary>
    /// True when at least the given share of the configured frame rate was really captured.
    /// </summary>
    /// <param name="minimumShare">The smallest accepted share of the configured rate, for example 0.75.</param>
    /// <returns>True when the captured rate is high enough.</returns>
    internal bool CapturedAtLeast(double minimumShare)
    {
        return Rate.ShareOfTarget >= minimumShare;
    }
}
