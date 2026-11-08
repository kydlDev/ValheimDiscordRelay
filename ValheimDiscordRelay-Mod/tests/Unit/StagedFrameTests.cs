using System;
using System.Linq;
using NUnit.Framework;
using ValheimDiscordRelay.Client;

namespace ValheimDiscordRelay.Tests.Unit;

/// <summary>One frame on disk with the time it is shown.</summary>
[TestFixture]
public class StagedFrameTests
{
    [Test]
    public void TheDuration_IsKeptInsideTheAllowedRange()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(new StagedFrame("a.jpg", 1).DurationMs, Is.EqualTo(FrameTiming.MinFrameDurationMs));
            Assert.That(new StagedFrame("a.jpg", 33).DurationMs, Is.EqualTo(33));
            Assert.That(new StagedFrame("a.jpg", 9000).DurationMs, Is.EqualTo(FrameTiming.MaxFrameDurationMs));
        }
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void AFrameWithoutAFile_IsRefused(string path)
    {
        Assert.That(() => new StagedFrame(path, 33), Throws.ArgumentException);
    }

    [Test]
    public void ASkippedNeighbour_LengthensTheFrameBeforeIt()
    {
        StagedFrame frame = new StagedFrame("frame-00001.jpg", 33);

        StagedFrame longer = frame.WithAddedDuration(34);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(longer.DurationMs, Is.EqualTo(67));
            Assert.That(longer.FilePath, Is.EqualTo("frame-00001.jpg"));
            Assert.That(frame.DurationMs, Is.EqualTo(33), "the original is not changed");
        }
    }

    [Test]
    public void TheLengthenedFrame_NeverExceedsTheMaximumOrShrinks()
    {
        StagedFrame frame = new StagedFrame("a.jpg", 400);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(frame.WithAddedDuration(400).DurationMs, Is.EqualTo(FrameTiming.MaxFrameDurationMs));
            Assert.That(frame.WithAddedDuration(-100).DurationMs, Is.EqualTo(400), "a negative addition is ignored");
        }
    }

    [Test]
    public void Frames_AreOrderedByFileName_IgnoringCase()
    {
        StagedFrame[] frames =
        {
            new StagedFrame("frame-00010.jpg", 33),
            new StagedFrame("FRAME-00002.jpg", 33),
            new StagedFrame("frame-00001.jpg", 33)
        };

        Array.Sort(frames, StagedFrame.CompareByFilePath);

        Assert.That(
            frames.Select(frame => frame.FilePath).ToArray(),
            Is.EqualTo(new[] { "frame-00001.jpg", "FRAME-00002.jpg", "frame-00010.jpg" }));
    }
}
