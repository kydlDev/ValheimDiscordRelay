using System;
using System.Globalization;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// How fast the footage of one clip was really captured, compared with the frame rate the player asked for.
/// </summary>
internal readonly struct CaptureRate
{
    internal CaptureRate(int frameCount, double spanSeconds, int targetFps)
    {
        FrameCount = Math.Max(0, frameCount);
        SpanSeconds = Math.Max(0d, spanSeconds);
        TargetFps = targetFps;
    }

    /// <summary>How many frames the clip holds.</summary>
    internal int FrameCount { get; }

    /// <summary>Time between the first and the last frame, in seconds (0 for fewer than two frames).</summary>
    internal double SpanSeconds { get; }

    /// <summary>The frame rate the player configured.</summary>
    internal int TargetFps { get; }

    /// <summary>Frames per second the capture really delivered (0 when it cannot be measured).</summary>
    internal double EffectiveFps => FrameCount > 1 && SpanSeconds > 0d ? (FrameCount - 1) / SpanSeconds : 0d;

    /// <summary>The effective rate as a share of the target (1 = the target was met).</summary>
    internal double ShareOfTarget => TargetFps > 0 ? EffectiveFps / TargetFps : 0d;

    public override string ToString()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0} frames over {1:0.00} s = {2:0.0} fps (target {3} fps)",
            FrameCount,
            SpanSeconds,
            EffectiveFps,
            TargetFps);
    }
}
