using NUnit.Framework;
using ValheimDiscordRelay.Client;

namespace ValheimDiscordRelay.Tests.Unit;

/// <summary>How a recorded video's playing time compares with its footage.</summary>
[TestFixture]
public class PlaybackReportTests
{
    [Test]
    public void AVideoThatPlaysAsLongAsItsFootage_PlaysInRealTime()
    {
        PlaybackReport report = new PlaybackReport(new CaptureRate(161, 8d, 30), 8000);

        Assert.That(report.PlaybackToFootageRatio, Is.EqualTo(1d).Within(0.001d));
        Assert.That(report.PlaysInRealTime(0.1d), Is.True);
    }

    [Test]
    public void AVideoThatPlaysInHalfTheTime_PlaysTooFast()
    {
        PlaybackReport report = new PlaybackReport(new CaptureRate(121, 8d, 30), 4000);

        Assert.That(report.PlaybackToFootageRatio, Is.EqualTo(0.5d).Within(0.001d));
        Assert.That(report.PlaysInRealTime(0.1d), Is.False);
    }

    [Test]
    public void AVideoThatPlaysMuchLonger_IsNotInRealTimeEither()
    {
        PlaybackReport report = new PlaybackReport(new CaptureRate(121, 4d, 30), 8000);

        Assert.That(report.PlaysInRealTime(0.1d), Is.False);
    }

    [Test]
    public void FootageWithoutLength_NeverPlaysInRealTime()
    {
        PlaybackReport report = new PlaybackReport(new CaptureRate(1, 0d, 30), 33);

        Assert.That(report.PlaybackToFootageRatio, Is.Zero);
        Assert.That(report.PlaysInRealTime(10d), Is.False);
    }

    [Test]
    public void TheToleranceIsInclusive()
    {
        // 9.0 s of playback for 8 s of footage is a ratio of 1.125, exact in binary floating point.
        PlaybackReport report = new PlaybackReport(new CaptureRate(241, 8d, 30), 9000);

        Assert.That(report.PlaysInRealTime(0.125d), Is.True);
    }

    [Test]
    public void ACaptureAtThreeQuartersOfTheTarget_CountsAsEnough()
    {
        // 91 frames over 4 s = 22.5 fps = 75 % of 30.
        PlaybackReport report = new PlaybackReport(new CaptureRate(91, 4d, 30), 4000);

        Assert.That(report.CapturedAtLeast(0.75d), Is.True);
    }

    [Test]
    public void ACaptureBelowThreeQuartersOfTheTarget_DoesNotCountAsEnough()
    {
        // 61 frames over 4 s = 15 fps = 50 % of 30.
        PlaybackReport report = new PlaybackReport(new CaptureRate(61, 4d, 30), 4000);

        Assert.That(report.CapturedAtLeast(0.75d), Is.False);
    }
}
