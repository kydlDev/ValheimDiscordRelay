using System;
using System.Collections.Generic;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// Works out how long each frame of a recorded clip is shown, from the moments the frames were really captured.
/// How many frames per second the game manages to capture depends on the player's PC and on what the game is doing, so
/// it is usually below the configured frame rate. If every frame were shown for 1/fps seconds anyway, the clip would be
/// shorter than the footage it holds and play too fast. Pure logic: no game or engine types, unit tested.
/// </summary>
internal static class FrameTiming
{
    /// <summary>Shortest time a frame is shown, in milliseconds.</summary>
    internal const int MinFrameDurationMs = 20;

    /// <summary>
    /// Longest time a frame is shown, in milliseconds. A longer gap between two captures is a hitch of the game (a
    /// loading stall around a death, for example) and is not replayed in full.
    /// </summary>
    internal const int MaxFrameDurationMs = 500;

    /// <summary>
    /// How far below the configured frame rate the real capture rate may be before the log calls it out, as a share of
    /// the configured rate.
    /// </summary>
    internal const double CaptureRateTolerance = 0.25;

    private const double MillisecondsPerSecond = 1000d;

    /// <summary>
    /// How long each frame is shown: the time until the next frame was captured. The rounding to whole milliseconds is
    /// carried from frame to frame, so the clip's length does not drift from the footage's length.
    /// </summary>
    /// <param name="captureTimes">When each frame was captured, in seconds, oldest first.</param>
    /// <param name="targetFps">The configured frame rate; used for a clip of one frame.</param>
    /// <returns>One duration in milliseconds per frame; the last frame is shown as long as the one before it.</returns>
    internal static int[] ComputeDurationsMs(IReadOnlyList<float> captureTimes, int targetFps)
    {
        if (captureTimes == null)
            throw new ArgumentNullException(nameof(captureTimes));

        int count = captureTimes.Count;
        int[] durations = new int[count];
        if (count == 0)
            return durations;

        int last = count - 1;
        double carry = 0d;

        for (int i = 0; i < last; i++)
        {
            double wanted = GapMilliseconds(captureTimes[i], captureTimes[i + 1]) + carry;

            // The part of a hitch above the maximum is dropped on purpose, so only the rounding error is carried on.
            double capped = Math.Min(wanted, MaxFrameDurationMs);
            int rounded = (int)Math.Round(capped, MidpointRounding.AwayFromZero);

            carry = capped - rounded;
            durations[i] = ClampDurationMs(rounded);
        }

        durations[last] = count > 1 ? durations[last - 1] : FallbackDurationMs(targetFps);
        return durations;
    }

    /// <summary>How long a frame is shown when nothing else is known: one frame at the configured rate.</summary>
    internal static int FallbackDurationMs(int targetFps)
    {
        int fps = Math.Max(1, targetFps);
        return ClampDurationMs((int)Math.Round(MillisecondsPerSecond / fps, MidpointRounding.AwayFromZero));
    }

    /// <summary>Keeps a frame duration inside <see cref="MinFrameDurationMs"/> and <see cref="MaxFrameDurationMs"/>.</summary>
    internal static int ClampDurationMs(int durationMs)
    {
        return Math.Min(MaxFrameDurationMs, Math.Max(MinFrameDurationMs, durationMs));
    }

    /// <summary>How fast the frames were really captured.</summary>
    /// <param name="captureTimes">When each frame was captured, in seconds, oldest first.</param>
    /// <param name="targetFps">The configured frame rate.</param>
    internal static CaptureRate MeasureRate(IReadOnlyList<float> captureTimes, int targetFps)
    {
        if (captureTimes == null)
            throw new ArgumentNullException(nameof(captureTimes));

        int count = captureTimes.Count;
        double spanSeconds = count > 1
            ? GapMilliseconds(captureTimes[0], captureTimes[count - 1]) / MillisecondsPerSecond
            : 0d;

        return new CaptureRate(count, spanSeconds, targetFps);
    }

    /// <summary>Time from one capture to the next in milliseconds; never negative, and 0 for a value that is not a number.</summary>
    private static double GapMilliseconds(float earlier, float later)
    {
        double gap = ((double)later - earlier) * MillisecondsPerSecond;
        return gap > 0d ? gap : 0d;
    }
}
