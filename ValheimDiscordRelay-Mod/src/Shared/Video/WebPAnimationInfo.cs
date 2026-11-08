using System;
using System.Collections.Generic;
using System.IO;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// Reads the frame durations out of an animated WebP file without decoding it. Used to check that the file the encoder
/// wrote shows every frame for as long as the clip's timing says. Only the chunk headers and the first bytes of each
/// frame chunk are read, so a file of several megabytes costs a few small reads.
/// </summary>
internal static class WebPAnimationInfo
{
    private const int RiffHeaderBytes = 12;
    private const int ChunkHeaderBytes = 8;

    /// <summary>A frame chunk starts with X, Y, width, height (3 bytes each), the duration (3 bytes) and a flag byte.</summary>
    private const int FramePrefixBytes = 16;

    private const int FrameDurationOffset = 12;

    /// <summary>
    /// The duration of every frame of an animated WebP, in milliseconds, in file order.
    /// </summary>
    /// <param name="stream">A readable, seekable stream positioned anywhere; it is read from the start.</param>
    /// <returns>The durations, or an empty array when the stream is not an animated WebP.</returns>
    internal static int[] ReadFrameDurationsMs(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        stream.Seek(0, SeekOrigin.Begin);

        byte[] riffHeader = new byte[RiffHeaderBytes];
        if (!TryReadExactly(stream, riffHeader) || !IsRiffWebP(riffHeader))
            return Array.Empty<int>();

        long end = Math.Min(WebPAnimationMerger.ReadUInt32(riffHeader, 4) + 8L, stream.Length);
        long position = RiffHeaderBytes;

        List<int> durations = new List<int>();
        byte[] chunkHeader = new byte[ChunkHeaderBytes];
        byte[] framePrefix = new byte[FramePrefixBytes];

        while (position + ChunkHeaderBytes <= end)
        {
            stream.Seek(position, SeekOrigin.Begin);
            if (!TryReadExactly(stream, chunkHeader))
                break;

            long size = WebPAnimationMerger.ReadUInt32(chunkHeader, 4);

            if (IsFrameChunk(chunkHeader))
            {
                if (size < FramePrefixBytes || !TryReadExactly(stream, framePrefix))
                    break;

                durations.Add(WebPAnimationMerger.ReadUInt24(framePrefix, FrameDurationOffset));
            }

            // A chunk with an odd size is followed by one pad byte.
            position += ChunkHeaderBytes + size + (size & 1);
        }

        return durations.ToArray();
    }

    private static bool IsRiffWebP(byte[] header)
    {
        return header[0] == 'R' && header[1] == 'I' && header[2] == 'F' && header[3] == 'F' &&
               header[8] == 'W' && header[9] == 'E' && header[10] == 'B' && header[11] == 'P';
    }

    private static bool IsFrameChunk(byte[] chunkHeader)
    {
        return chunkHeader[0] == 'A' && chunkHeader[1] == 'N' && chunkHeader[2] == 'M' && chunkHeader[3] == 'F';
    }

    private static bool TryReadExactly(Stream stream, byte[] buffer)
    {
        int total = 0;
        while (total < buffer.Length)
        {
            int read = stream.Read(buffer, total, buffer.Length - total);
            if (read <= 0)
                return total == buffer.Length;

            total += read;
        }

        return total == buffer.Length;
    }
}
