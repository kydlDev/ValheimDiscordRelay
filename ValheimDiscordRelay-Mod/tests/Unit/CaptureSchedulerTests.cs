using System;
using NUnit.Framework;
using ValheimDiscordRelay.Client;

namespace ValheimDiscordRelay.Tests.Unit;

/// <summary>When the next video frame is captured.</summary>
[TestFixture]
public class CaptureSchedulerTests
{
    private const int Fps = 30;
    private const double Interval = 1d / Fps;

    [Test]
    public void TheFirstTick_StartsACapture()
    {
        CaptureScheduler scheduler = new CaptureScheduler();

        Assert.That(scheduler.TryBegin(100d, Fps), Is.True);
        Assert.That(scheduler.InFlight, Is.EqualTo(1));
    }

    [Test]
    public void ATickBeforeTheNextSlot_StartsNothing()
    {
        CaptureScheduler scheduler = new CaptureScheduler();
        scheduler.TryBegin(100d, Fps);
        scheduler.End();

        Assert.That(scheduler.TryBegin(100d + (Interval / 2), Fps), Is.False);
    }

    [Test]
    public void ALateTick_DoesNotPushTheLaterSlotsBack()
    {
        CaptureScheduler scheduler = new CaptureScheduler();
        scheduler.TryBegin(100d, Fps);
        scheduler.End();

        // 10 ms late for the second slot ...
        Assert.That(scheduler.TryBegin(100d + Interval + 0.010d, Fps), Is.True);
        scheduler.End();

        // ... the third slot is still due 2 intervals after the start, not 2 intervals + 10 ms.
        Assert.That(scheduler.TryBegin(100d + (2 * Interval) + 0.0001d, Fps), Is.True);
    }

    [Test]
    public void AGameRunningAtThirtyFramesPerSecond_CapturesEveryFrame()
    {
        CaptureScheduler scheduler = new CaptureScheduler();
        int captures = 0;

        for (int frame = 0; frame < 300; frame++)
        {
            if (scheduler.TryBegin(100d + (frame * Interval), Fps))
            {
                captures++;
                scheduler.End();
            }
        }

        Assert.That(captures, Is.EqualTo(300));
    }

    [Test]
    public void AGameRunningAtSixtyFramesPerSecond_CapturesHalfOfThem()
    {
        CaptureScheduler scheduler = new CaptureScheduler();
        double frameTime = 1d / 60d;
        int captures = 0;

        for (int frame = 0; frame < 600; frame++)
        {
            if (scheduler.TryBegin(100d + (frame * frameTime), Fps))
            {
                captures++;
                scheduler.End();
            }
        }

        // 10 seconds at 30 fps.
        Assert.That(captures, Is.InRange(299, 301));
    }

    [Test]
    public void AGameRunningAtFortyFramesPerSecond_StillAveragesThirtyCapturesPerSecond()
    {
        CaptureScheduler scheduler = new CaptureScheduler();
        double frameTime = 1d / 40d;
        int captures = 0;

        for (int frame = 0; frame < 400; frame++)
        {
            if (scheduler.TryBegin(100d + (frame * frameTime), Fps))
            {
                captures++;
                scheduler.End();
            }
        }

        // 10 seconds at 30 fps; measuring each slot from "now" would give about 20 per second here.
        Assert.That(captures, Is.InRange(298, 301));
    }

    [Test]
    public void AHitch_DoesNotCauseABurstOfCaptures()
    {
        CaptureScheduler scheduler = new CaptureScheduler();
        scheduler.TryBegin(100d, Fps);
        scheduler.End();

        // The game stalls for half a second, then runs normally.
        double now = 100.5d;
        Assert.That(scheduler.TryBegin(now, Fps), Is.True);
        scheduler.End();
        Assert.That(scheduler.TryBegin(now + 0.001d, Fps), Is.False);
        Assert.That(scheduler.TryBegin(now + (Interval / 2), Fps), Is.False);
    }

    [Test]
    public void NoMoreCapturesThanAllowedRunAtOnce()
    {
        CaptureScheduler scheduler = new CaptureScheduler(2);

        Assert.That(scheduler.TryBegin(100d, Fps), Is.True);
        Assert.That(scheduler.TryBegin(100d + Interval, Fps), Is.True);
        Assert.That(scheduler.TryBegin(100d + (2 * Interval), Fps), Is.False);
        Assert.That(scheduler.InFlight, Is.EqualTo(2));
    }

    [Test]
    public void AFinishedCapture_FreesASlot()
    {
        CaptureScheduler scheduler = new CaptureScheduler(1);
        scheduler.TryBegin(100d, Fps);

        Assert.That(scheduler.TryBegin(100d + Interval, Fps), Is.False);

        scheduler.End();

        Assert.That(scheduler.TryBegin(100d + Interval, Fps), Is.True);
    }

    [Test]
    public void FinishingMoreOftenThanStarted_DoesNotGoNegative()
    {
        CaptureScheduler scheduler = new CaptureScheduler(1);
        scheduler.TryBegin(100d, Fps);

        scheduler.End();
        scheduler.End();
        scheduler.End();

        Assert.That(scheduler.InFlight, Is.Zero);
        Assert.That(scheduler.TryBegin(100d + Interval, Fps), Is.True);
        Assert.That(scheduler.TryBegin(100d + (2 * Interval), Fps), Is.False);
    }

    [Test]
    public void AfterARestart_TheNextTickStartsACapture()
    {
        CaptureScheduler scheduler = new CaptureScheduler();
        scheduler.TryBegin(100d, Fps);
        scheduler.End();

        scheduler.Restart();

        Assert.That(scheduler.TryBegin(100d + 0.001d, Fps), Is.True);
    }

    [Test]
    public void ARateBelowOne_CountsAsOne()
    {
        CaptureScheduler scheduler = new CaptureScheduler();
        scheduler.TryBegin(100d, 0);
        scheduler.End();

        Assert.That(scheduler.TryBegin(100.5d, 0), Is.False);
        Assert.That(scheduler.TryBegin(101d, 0), Is.True);
    }

    [Test]
    public void ZeroInFlightCaptures_AreRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CaptureScheduler(0));
    }
}
