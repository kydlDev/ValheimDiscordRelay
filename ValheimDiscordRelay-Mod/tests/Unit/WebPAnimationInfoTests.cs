using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using ValheimDiscordRelay.Client;

namespace ValheimDiscordRelay.Tests.Unit;

/// <summary>
/// Reading the frame durations back out of an animated WebP. The files here are built by hand (RIFF container,
/// ANMF frame chunks); the real encoder output is checked in game.
/// </summary>
[TestFixture]
public class WebPAnimationInfoTests
{
    private static byte[] Chunk(string id, byte[] payload)
    {
        List<byte> bytes = new List<byte>();
        bytes.AddRange(Encoding.ASCII.GetBytes(id));
        bytes.AddRange(BitConverter.GetBytes((uint)payload.Length));
        bytes.AddRange(payload);
        if (payload.Length % 2 == 1)
            bytes.Add(0);

        return bytes.ToArray();
    }

    /// <summary>An ANMF payload: X, Y, width-1, height-1, duration (3 bytes each, little endian), flags, then image bytes.</summary>
    private static byte[] FramePayload(int durationMs, int imageBytes)
    {
        List<byte> payload = new List<byte>();
        for (int i = 0; i < 4; i++)
            payload.AddRange(new byte[] { 0, 0, 0 });

        payload.AddRange(new[] { (byte)(durationMs & 0xFF), (byte)((durationMs >> 8) & 0xFF), (byte)((durationMs >> 16) & 0xFF) });
        payload.Add(0);
        payload.AddRange(new byte[imageBytes]);
        return payload.ToArray();
    }

    private static MemoryStream Animation(IEnumerable<byte[]> chunks)
    {
        List<byte> body = new List<byte>();
        body.AddRange(Encoding.ASCII.GetBytes("WEBP"));
        foreach (byte[] chunk in chunks)
            body.AddRange(chunk);

        List<byte> file = new List<byte>();
        file.AddRange(Encoding.ASCII.GetBytes("RIFF"));
        file.AddRange(BitConverter.GetBytes((uint)body.Count));
        file.AddRange(body);
        return new MemoryStream(file.ToArray());
    }

    [Test]
    public void EveryFramesDuration_IsReadInFileOrder()
    {
        using MemoryStream stream = Animation(new[]
        {
            Chunk("VP8X", new byte[10]),
            Chunk("ANIM", new byte[6]),
            Chunk("ANMF", FramePayload(33, 20)),
            Chunk("ANMF", FramePayload(66, 21)),
            Chunk("ANMF", FramePayload(500, 4))
        });

        Assert.That(WebPAnimationInfo.ReadFrameDurationsMs(stream), Is.EqualTo(new[] { 33, 66, 500 }));
    }

    [Test]
    public void ADurationAbove255_UsesAllThreeBytes()
    {
        using MemoryStream stream = Animation(new[] { Chunk("ANMF", FramePayload(0x01_02_03, 2)) });

        Assert.That(WebPAnimationInfo.ReadFrameDurationsMs(stream), Is.EqualTo(new[] { 0x01_02_03 }));
    }

    [Test]
    public void OddSizedChunks_AreSkippedWithTheirPadByte()
    {
        using MemoryStream stream = Animation(new[]
        {
            Chunk("EXIF", new byte[7]),
            Chunk("ANMF", FramePayload(40, 5)),
            Chunk("XMP ", new byte[3]),
            Chunk("ANMF", FramePayload(50, 6))
        });

        Assert.That(WebPAnimationInfo.ReadFrameDurationsMs(stream), Is.EqualTo(new[] { 40, 50 }));
    }

    [Test]
    public void AFileThatIsNotAnAnimation_HasNoFrames()
    {
        using MemoryStream stillImage = Animation(new[] { Chunk("VP8 ", new byte[30]) });
        using MemoryStream notWebP = new MemoryStream(Encoding.ASCII.GetBytes("GIF89a, definitely not a WebP file"));
        using MemoryStream empty = new MemoryStream();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(WebPAnimationInfo.ReadFrameDurationsMs(stillImage), Is.Empty);
            Assert.That(WebPAnimationInfo.ReadFrameDurationsMs(notWebP), Is.Empty);
            Assert.That(WebPAnimationInfo.ReadFrameDurationsMs(empty), Is.Empty);
        }
    }

    [Test]
    public void ATruncatedFile_ReturnsTheFramesBeforeTheCut()
    {
        byte[] whole = Animation(new[]
        {
            Chunk("ANMF", FramePayload(33, 20)),
            Chunk("ANMF", FramePayload(66, 20))
        }).ToArray();

        using MemoryStream cut = new MemoryStream(whole, 0, whole.Length - 30);

        Assert.That(WebPAnimationInfo.ReadFrameDurationsMs(cut), Is.EqualTo(new[] { 33 }));
    }

    [Test]
    public void AFrameChunkThatIsTooShortForItsHeader_StopsTheReading()
    {
        using MemoryStream stream = Animation(new[]
        {
            Chunk("ANMF", FramePayload(33, 0)),
            Chunk("ANMF", new byte[4])
        });

        Assert.That(WebPAnimationInfo.ReadFrameDurationsMs(stream), Is.EqualTo(new[] { 33 }));
    }

    [Test]
    public void ANullStream_IsRefused()
    {
        Assert.That(() => WebPAnimationInfo.ReadFrameDurationsMs(null), Throws.ArgumentNullException);
    }
}
