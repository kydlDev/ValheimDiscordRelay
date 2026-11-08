using System.Collections.Generic;
using static System.FormattableString;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// Frame timing of a recorded clip: how long each frame is shown, and how fast the footage was really captured. The
/// numbers come from the pure <see cref="FrameTiming"/>; this part reads the frames' capture times and writes the log.
/// </summary>
internal sealed partial class ClientCaptureService
{
    /// <summary>Largest difference, in seconds, between the footage a clip was meant to hold and what the buffer delivered.</summary>
    private const double FootageLengthToleranceSeconds = 1.0;

    /// <summary>
    /// Works out how long each selected frame is shown and reports how fast the footage was captured. The capture rate
    /// is usually below the configured frame rate (it depends on the PC and on what the game is doing), so a clip must
    /// not assume that every frame is 1/fps apart.
    /// </summary>
    /// <param name="label">Which video this is (Death, Boss or Clip), for the log lines.</param>
    /// <param name="selected">The frames of the clip, oldest first.</param>
    /// <param name="targetFps">The configured frame rate.</param>
    /// <param name="windowSeconds">The length of footage the clip was meant to hold (pre + post duration).</param>
    /// <returns>One duration in milliseconds per frame, in the order of <paramref name="selected"/>.</returns>
    private static int[] PlanFrameDurations(string label, IReadOnlyList<FrameEntry> selected, int targetFps, float windowSeconds)
    {
        int count = selected.Count;
        RelayDiagnostics.Debug(Invariant($"[{label}] Planning frame durations: frames={count}, targetFps={targetFps} fps, requested footage={windowSeconds:0.00} s."));

        float[] captureTimes = new float[count];
        for (int i = 0; i < count; i++)
            captureTimes[i] = selected[i].Time;

        int[] durations = FrameTiming.ComputeDurationsMs(captureTimes, targetFps);
        if (count < 2)
        {
            RelayDiagnostics.Debug($"[{label}] Too few frames to measure the capture rate; each frame keeps its default duration.");
            return durations;
        }

        CaptureRate rate = FrameTiming.MeasureRate(captureTimes, targetFps);
        long playbackMs = SumMilliseconds(durations);

        RelayDiagnostics.Info(Invariant(
            $"[{label}] Captured {rate}; the clip plays {playbackMs / 1000d:0.00} s for {rate.SpanSeconds:0.00} s of footage (average frame {playbackMs / (double)count:0} ms)."));

        RelayDiagnostics.Check($"{label} capture rate", rate.EffectiveFps, targetFps, targetFps * FrameTiming.CaptureRateTolerance, "fps");
        RelayDiagnostics.Check($"{label} footage length", rate.SpanSeconds, windowSeconds, FootageLengthToleranceSeconds, "s");

        return durations;
    }

    /// <summary>
    /// A frame that could not be written to disk is skipped; the frame before it is shown for its time instead, so the
    /// clip keeps its real length.
    /// </summary>
    private static void GiveDurationToPreviousFrame(List<StagedFrame> staged, int durationMs)
    {
        int last = staged.Count - 1;
        if (last < 0)
        {
            RelayDiagnostics.Debug($"A skipped first frame ({durationMs} ms) has no frame before it; its time is lost.");
            return;
        }

        staged[last] = staged[last].WithAddedDuration(durationMs);
        RelayDiagnostics.Debug($"A frame could not be staged; the frame before it now lasts {staged[last].DurationMs} ms.");
    }

    private static long SumMilliseconds(IReadOnlyList<int> durations)
    {
        long total = 0;
        for (int i = 0; i < durations.Count; i++)
            total += durations[i];

        return total;
    }
}
