using System.Collections.Generic;
using NUnit.Framework;
using ValheimDiscordRelay.Client;

namespace ValheimDiscordRelay.Tests.Unit;

/// <summary>The planned length of a clip, which the finished animation is checked against.</summary>
[TestFixture]
public class WebPAnimationCheckTests
{
    [Test]
    public void ThePlannedLength_IsTheSumOfTheFrameDurations()
    {
        List<StagedFrame> frames = new List<StagedFrame>
        {
            new StagedFrame("a.jpg", 33),
            new StagedFrame("b.jpg", 66),
            new StagedFrame("c.jpg", 500)
        };

        Assert.That(WebPAnimationCheck.TotalDurationMs(frames), Is.EqualTo(599));
    }

    [Test]
    public void NoFrames_HaveNoLength()
    {
        Assert.That(WebPAnimationCheck.TotalDurationMs(new List<StagedFrame>()), Is.Zero);
    }

    [Test]
    public void ALongClip_DoesNotOverflow()
    {
        List<StagedFrame> frames = new List<StagedFrame>();
        for (int i = 0; i < 50000; i++)
            frames.Add(new StagedFrame("f.jpg", FrameTiming.MaxFrameDurationMs));

        Assert.That(WebPAnimationCheck.TotalDurationMs(frames), Is.EqualTo(25_000_000L));
    }
}
