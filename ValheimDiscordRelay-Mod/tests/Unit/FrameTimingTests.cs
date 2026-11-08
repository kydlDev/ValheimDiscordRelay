using System;
using System.Linq;
using NUnit.Framework;
using ValheimDiscordRelay.Client;

namespace ValheimDiscordRelay.Tests.Unit;

/// <summary>
/// How long each frame of a clip is shown. The bug these guard: frames were captured at 15-20 fps but every frame was
/// shown for 1/30 s, so an 8 s clip played in 4-5 s.
/// </summary>
[TestFixture]
public class FrameTimingTests
{
    private static float[] EvenlySpaced(int frames, double fps)
    {
        return Enumerable.Range(0, frames).Select(i => (float)(i / fps)).ToArray();
    }

    private static long SumAllButLast(int[] durations)
    {
        return durations.Take(durations.Length - 1).Sum(d => (long)d);
    }

    [Test]
    public void FramesCapturedAtTheTargetRate_AreShownForOneFrameEach()
    {
        float[] times = EvenlySpaced(31, 30d);

        int[] durations = FrameTiming.ComputeDurationsMs(times, 30);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(durations, Has.Length.EqualTo(31));
            Assert.That(durations, Has.All.InRange(33, 34));
            Assert.That(SumAllButLast(durations), Is.EqualTo(1000).Within(1), "30 gaps of 1/30 s are one second");
        }
    }

    [Test]
    public void SlowCapture_IsStretchedToTheRealLengthOfTheFootage()
    {
        // 8 s of footage captured at 15 fps although 30 fps was asked for: the old code showed 121 frames for 33 ms (4 s).
        float[] times = EvenlySpaced(121, 15d);

        int[] durations = FrameTiming.ComputeDurationsMs(times, 30);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(durations.Take(120), Has.All.InRange(66, 67));
            Assert.That(SumAllButLast(durations), Is.EqualTo(8000).Within(1));
        }
    }

    [Test]
    public void TheRoundingErrorIsCarriedOn_SoTheClipDoesNotDrift()
    {
        // Every gap is 33.33 ms. Rounding each one to 33 ms on its own would lose 10 ms per second (100 ms in ten seconds).
        float[] times = EvenlySpaced(301, 30d);

        int[] durations = FrameTiming.ComputeDurationsMs(times, 30);

        Assert.That(SumAllButLast(durations), Is.EqualTo(10000).Within(1), "300 gaps of 1/30 s are ten seconds, not 9900 ms");
    }

    [Test]
    public void AHitch_IsNotReplayedInFull()
    {
        float[] times = { 0f, 0.033f, 2.033f, 2.066f };

        int[] durations = FrameTiming.ComputeDurationsMs(times, 30);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(durations[1], Is.EqualTo(FrameTiming.MaxFrameDurationMs), "the two second stall is capped");
            Assert.That(durations[0], Is.EqualTo(33).Within(1), "the frames around it keep their own time");
            Assert.That(durations[2], Is.EqualTo(33).Within(1));
        }
    }

    [Test]
    public void FramesCapturedTooCloseTogether_AreRaisedToTheMinimum()
    {
        float[] times = { 0f, 0.001f, 0.002f, 0.1f };

        int[] durations = FrameTiming.ComputeDurationsMs(times, 30);

        Assert.That(durations.Take(2), Has.All.EqualTo(FrameTiming.MinFrameDurationMs));
    }

    [Test]
    public void TheLastFrame_IsShownAsLongAsTheOneBeforeIt()
    {
        float[] times = { 0f, 0.05f, 0.15f };

        int[] durations = FrameTiming.ComputeDurationsMs(times, 30);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(durations[0], Is.EqualTo(50));
            Assert.That(durations[1], Is.EqualTo(100));
            Assert.That(durations[2], Is.EqualTo(100));
        }
    }

    [TestCase(30, 33)]
    [TestCase(20, 50)]
    [TestCase(25, 40)]
    public void ASingleFrame_UsesOneFrameAtTheTargetRate(int fps, int expectedMs)
    {
        int[] durations = FrameTiming.ComputeDurationsMs(new[] { 5f }, fps);

        Assert.That(durations, Is.EqualTo(new[] { expectedMs }));
    }

    [Test]
    public void NoFrames_GiveNoDurations()
    {
        Assert.That(FrameTiming.ComputeDurationsMs(Array.Empty<float>(), 30), Is.Empty);
    }

    [Test]
    public void TimesThatGoBackwards_NeverGiveANegativeDuration()
    {
        float[] times = { 1f, 0.5f, 0.6f, float.NaN, 2f };

        int[] durations = FrameTiming.ComputeDurationsMs(times, 30);

        Assert.That(durations, Has.All.InRange(FrameTiming.MinFrameDurationMs, FrameTiming.MaxFrameDurationMs));
    }

    [Test]
    public void ANullList_IsRefused()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(() => FrameTiming.ComputeDurationsMs(null, 30), Throws.ArgumentNullException);
            Assert.That(() => FrameTiming.MeasureRate(null, 30), Throws.ArgumentNullException);
        }
    }

    [TestCase(0, FrameTiming.MaxFrameDurationMs)]
    [TestCase(-5, FrameTiming.MaxFrameDurationMs)]
    [TestCase(1000, FrameTiming.MinFrameDurationMs)]
    public void TheFallbackDuration_StaysInsideTheAllowedRange(int fps, int expectedMs)
    {
        Assert.That(FrameTiming.FallbackDurationMs(fps), Is.EqualTo(expectedMs));
    }

    [TestCase(-1, FrameTiming.MinFrameDurationMs)]
    [TestCase(0, FrameTiming.MinFrameDurationMs)]
    [TestCase(33, 33)]
    [TestCase(100000, FrameTiming.MaxFrameDurationMs)]
    public void ADuration_IsKeptInsideTheAllowedRange(int input, int expected)
    {
        Assert.That(FrameTiming.ClampDurationMs(input), Is.EqualTo(expected));
    }

    [Test]
    public void TheMeasuredRate_IsFramesPerSecondOfTheFootage()
    {
        float[] times = EvenlySpaced(121, 15d);

        CaptureRate rate = FrameTiming.MeasureRate(times, 30);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(rate.FrameCount, Is.EqualTo(121));
            Assert.That(rate.SpanSeconds, Is.EqualTo(8d).Within(0.001));
            Assert.That(rate.EffectiveFps, Is.EqualTo(15d).Within(0.01));
            Assert.That(rate.ShareOfTarget, Is.EqualTo(0.5d).Within(0.001));
        }
    }

    [Test]
    public void FewerThanTwoFrames_HaveNoMeasurableRate()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(FrameTiming.MeasureRate(Array.Empty<float>(), 30).EffectiveFps, Is.Zero);
            Assert.That(FrameTiming.MeasureRate(new[] { 3f }, 30).EffectiveFps, Is.Zero);
            Assert.That(FrameTiming.MeasureRate(new[] { 3f }, 30).SpanSeconds, Is.Zero);
        }
    }
}
