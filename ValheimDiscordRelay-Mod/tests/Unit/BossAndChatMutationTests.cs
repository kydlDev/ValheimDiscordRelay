using System;
using NUnit.Framework;
using ValheimDiscordRelay.Server;

namespace ValheimDiscordRelay.Tests.Unit;

/// <summary>Cases found by mutation testing: the last "(Clone)" is not cut, list numbers and the dot are escaped exactly, the chat limiter forgets everybody at 128 peers.</summary>
[TestFixture]
public class BossAndChatMutationTests
{
    [Test]
    public void CleanPrefabName_CutsAtTheFirstClone()
    {
        Assert.That(BossCatalog.CleanPrefabName("Boss(Clone)Tail(Clone)"), Is.EqualTo("Boss"));
    }

    [TestCase("0. start", "0\\. start")]
    [TestCase("9. end", "9\\. end")]
    [TestCase("90. mixed", "90\\. mixed")]
    [TestCase("12 no dot", "12 no dot")]
    [TestCase(".hidden", ".hidden")]
    [TestCase("a.b", "a.b")]
    public void EscapeDiscordMarkdown_NumberedListStart_EscapesOnlyTheDotAfterDigits(string input, string expected)
    {
        Assert.That(BossCatalog.EscapeDiscordMarkdown(input), Is.EqualTo(expected));
    }

    [Test]
    public void Reset_GivesEverySenderAFullBurstAgain()
    {
        ChatRateLimiter.Reset();
        DateTime now = new DateTime(2026, 11, 16, 12, 0, 0, DateTimeKind.Utc);
        for (int i = 0; i < 10; i++) Assert.That(ChatRateLimiter.Allow(1, "a", now, s => { }), Is.True, "line " + i);
        Assert.That(ChatRateLimiter.Allow(1, "a", now, s => { }), Is.False);

        ChatRateLimiter.Reset();

        Assert.That(ChatRateLimiter.Allow(1, "a", now, s => { }), Is.True);
    }

    [Test]
    public void Allow_WhenTheTableIsFull_ForgetsEveryBucketOnTheNextNewSender()
    {
        ChatRateLimiter.Reset();
        DateTime now = new DateTime(2026, 11, 16, 12, 0, 0, DateTimeKind.Utc);
        for (int i = 0; i < 10; i++) ChatRateLimiter.Allow(1, "flooder", now, s => { });
        Assert.That(ChatRateLimiter.Allow(1, "flooder", now, s => { }), Is.False, "sender 1 is out of tokens");
        for (long peer = 2; peer <= 128; peer++) ChatRateLimiter.Allow(peer, "p" + peer, now, s => { });

        ChatRateLimiter.Allow(999, "newcomer", now, s => { });

        Assert.That(ChatRateLimiter.Allow(1, "flooder", now, s => { }), Is.True, "the full table was cleared, sender 1 starts over");
        ChatRateLimiter.Reset();
    }
}
