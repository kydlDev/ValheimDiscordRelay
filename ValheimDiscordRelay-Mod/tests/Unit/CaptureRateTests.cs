using System.Globalization;
using System.Threading;
using NUnit.Framework;
using ValheimDiscordRelay.Client;

namespace ValheimDiscordRelay.Tests.Unit;

/// <summary>The capture-rate numbers that go into the log line of every clip.</summary>
[TestFixture]
public class CaptureRateTests
{
    private CultureInfo _previousCulture;

    [SetUp]
    public void UseAGermanPc()
    {
        _previousCulture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
    }

    [TearDown]
    public void RestoreTheCulture()
    {
        Thread.CurrentThread.CurrentCulture = _previousCulture;
    }

    [Test]
    public void TheLogText_NamesFramesSpanAndRatesWithDecimalPoints_WhateverThePcLanguageIs()
    {
        CaptureRate rate = new CaptureRate(162, 7.98d, 30);

        Assert.That(rate.ToString(), Is.EqualTo("162 frames over 7.98 s = 20.2 fps (target 30 fps)"));
    }

    [Test]
    public void NegativeInput_IsTreatedAsZero()
    {
        CaptureRate rate = new CaptureRate(-4, -1d, 30);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(rate.FrameCount, Is.Zero);
            Assert.That(rate.SpanSeconds, Is.Zero);
            Assert.That(rate.EffectiveFps, Is.Zero);
        }
    }

    [Test]
    public void WithoutATarget_TheShareOfTargetIsZero()
    {
        Assert.That(new CaptureRate(31, 1d, 0).ShareOfTarget, Is.Zero);
    }

    [Test]
    public void TheTargetRate_IsReachedExactlyWhenEveryGapIsOneFrame()
    {
        CaptureRate rate = new CaptureRate(31, 1d, 30);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(rate.EffectiveFps, Is.EqualTo(30d).Within(0.0001));
            Assert.That(rate.ShareOfTarget, Is.EqualTo(1d).Within(0.0001));
        }
    }
}
