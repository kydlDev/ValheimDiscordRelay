using System;
using System.Collections.Generic;
using System.IO;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// Checks a finished animated WebP against the timing the clip was planned with, so a clip that plays at the wrong
/// speed shows up in the log of the player who reports it.
/// </summary>
internal static class WebPAnimationCheck
{
    /// <summary>How far, in milliseconds, the written animation may differ from the planned length before the log warns.</summary>
    private const double LengthToleranceMs = 5d;

    /// <summary>The planned length of a clip: the sum of its frames' durations, in milliseconds.</summary>
    internal static long TotalDurationMs(IReadOnlyList<StagedFrame> frames)
    {
        long total = 0;
        for (int i = 0; i < frames.Count; i++)
            total += frames[i].DurationMs;

        return total;
    }

    /// <summary>
    /// Reads the finished animation back and compares its length with the planned one. A mismatch is a warning only:
    /// the clip is still sent.
    /// </summary>
    /// <param name="output">The animated WebP file.</param>
    /// <param name="frames">The frames the animation was encoded from.</param>
    /// <returns>True when the animation is as long as planned.</returns>
    internal static bool CheckLength(string output, IReadOnlyList<StagedFrame> frames)
    {
        bool confirmed = false;
        long expectedMs = TotalDurationMs(frames);

        try
        {
            int[] written;
            using (FileStream stream = File.OpenRead(output))
                written = WebPAnimationInfo.ReadFrameDurationsMs(stream);

            long writtenMs = 0;
            foreach (int duration in written)
                writtenMs += duration;

            // The encoder may merge identical neighbouring frames into one longer frame, so the frame count is only logged.
            RelayDiagnostics.Debug($"WebP animation read back: frames={written.Length} of {frames.Count}, length={writtenMs} ms, expected {expectedMs} ms.");

            confirmed = RelayDiagnostics.Check("WebP animation length", writtenMs, expectedMs, LengthToleranceMs, "ms");
        }
        catch (IOException ex)
        {
            RelayDiagnostics.Warning($"Could not read the WebP animation back to check its length: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            RelayDiagnostics.Warning($"Could not open the WebP animation to check its length: {ex.Message}");
        }

        return confirmed;
    }
}
