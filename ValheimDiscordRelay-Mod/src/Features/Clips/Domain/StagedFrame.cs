using System;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// One frame of a clip after it was written to disk for the encoder: the file and how long it is shown.
/// </summary>
internal readonly struct StagedFrame
{
    internal StagedFrame(string filePath, int durationMs)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("A staged frame needs a file path.", nameof(filePath));

        FilePath = filePath;
        DurationMs = FrameTiming.ClampDurationMs(durationMs);
    }

    /// <summary>The frame's image file.</summary>
    internal string FilePath { get; }

    /// <summary>How long the frame is shown in the finished animation, in milliseconds.</summary>
    internal int DurationMs { get; }

    /// <summary>
    /// The same frame shown for longer, used when the frame after it could not be written to disk: its time is given
    /// to this frame so the clip keeps its real length.
    /// </summary>
    /// <param name="extraMs">The time to add, in milliseconds.</param>
    /// <returns>The frame with the longer duration (still within the allowed range).</returns>
    internal StagedFrame WithAddedDuration(int extraMs)
    {
        return new StagedFrame(FilePath, DurationMs + Math.Max(0, extraMs));
    }

    /// <summary>Orders frames by file name; staged files are numbered, so this is capture order.</summary>
    internal static int CompareByFilePath(StagedFrame left, StagedFrame right)
    {
        return string.Compare(left.FilePath, right.FilePath, StringComparison.OrdinalIgnoreCase);
    }
}
