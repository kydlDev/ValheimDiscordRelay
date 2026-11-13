using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using ValheimDiscordRelay.Client;

namespace ValheimDiscordRelay.Tests.Unit;

/// <summary>
/// The quote pools of the deaths that have their own list (crushing, water, a boss killing the player): how many quotes each
/// has, that every placeholder is one the picker fills, and that the pool never hands out the same quote twice in a row.
/// </summary>
[TestFixture]
public class DeathQuoteListsTests
{
    private const int CrushQuoteCount = 16;   // the original message plus 15 added ones
    private const int WaterQuoteCount = 15;
    private const int BossKillerQuoteCount = 16;

    private static readonly Regex Token = new(@"\{([^{}]*)\}", RegexOptions.CultureInvariant);

    private static readonly string[] CrushAndWaterTokens = { "Player.name" };
    private static readonly string[] BossTokens = { "Player.name", "Creature.name", "level" };

    // The kinds are passed by name: the enum is internal and a public test method cannot take it as a parameter.
    private static readonly string[] CrushKinds =
    {
        "Tree", "Cart", "Boat", "Structure", "Stalagmite", "DrawBridge", "Turret", "Catapult", "Impact"
    };

    private static readonly string[] WaterKinds = { "Swimming", "AshlandsSea" };

    private static IEnumerable<string> AllKinds => Enum.GetNames(typeof(DeathQuoteKind));

    private static DeathQuoteKind Kind(string name)
    {
        return (DeathQuoteKind)Enum.Parse(typeof(DeathQuoteKind), name);
    }

    private static List<string> TokensOf(string template)
    {
        return Token.Matches(template).Cast<Match>().Select(m => m.Groups[1].Value).ToList();
    }

    [TestCaseSource(nameof(AllKinds))]
    public void EveryKind_HasAPool(string kindName)
    {
        Assert.That(DeathQuoteLists.For(Kind(kindName)), Is.Not.Null);
    }

    [Test]
    public void EveryKind_IsEitherCrushOrWater_SoNoneIsLeftOutOfTheCounts()
    {
        Assert.That(CrushKinds.Concat(WaterKinds).OrderBy(k => k), Is.EqualTo(AllKinds.OrderBy(k => k)));
    }

    [Test]
    public void UnknownKind_IsRefused()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DeathQuoteLists.For((DeathQuoteKind)999));
    }

    [Test]
    public void EachKind_HasItsOwnPool()
    {
        var pools = AllKinds.Select(name => DeathQuoteLists.For(Kind(name))).ToList();
        Assert.That(pools.Distinct().Count(), Is.EqualTo(pools.Count));
    }

    [TestCaseSource(nameof(CrushKinds))]
    public void CrushPool_HasTheOriginalAndFifteenMore(string kindName)
    {
        DeathQuoteKind kind = Kind(kindName);
        Assert.That(DeathQuoteLists.For(kind).Quotes, Has.Count.EqualTo(CrushQuoteCount));
    }

    [TestCase("Tree", "{Player.name} got crushed by a falling tree.")]
    [TestCase("Cart", "{Player.name} got crushed by a cart.")]
    [TestCase("Boat", "{Player.name} got crushed by a boat.")]
    [TestCase("Structure", "{Player.name} got crushed by a collapsing structure.")]
    [TestCase("Stalagmite", "{Player.name} got crushed by a stalagmite.")]
    [TestCase("DrawBridge", "{Player.name} got crushed by a drawbridge.")]
    [TestCase("Turret", "{Player.name} got crushed by a turret.")]
    [TestCase("Catapult", "{Player.name} got crushed by a catapult.")]
    [TestCase("Impact", "{Player.name} got crushed by an impact.")]
    public void CrushPool_KeepsTheOriginalMessageFirst(string kindName, string original)
    {
        DeathQuoteKind kind = Kind(kindName);
        Assert.That(DeathQuoteLists.For(kind).Quotes[0], Is.EqualTo(original));
    }

    [TestCaseSource(nameof(WaterKinds))]
    public void WaterPool_HasFifteenQuotes(string kindName)
    {
        DeathQuoteKind kind = Kind(kindName);
        Assert.That(DeathQuoteLists.For(kind).Quotes, Has.Count.EqualTo(WaterQuoteCount));
    }

    [Test]
    public void BossKillerPool_HasTheOriginalAndFifteenMore()
    {
        Assert.That(BossKillerQuotes.Pool.Quotes, Has.Count.EqualTo(BossKillerQuoteCount));
    }

    [Test]
    public void BossKillerPool_KeepsTheOriginalMessageFirst()
    {
        Assert.That(
            BossKillerQuotes.Pool.Quotes[0],
            Is.EqualTo("{Creature.name}{level} managed to get one over {Player.name}. {Creature.name} roams triumphant, for now...You got this!"));
    }

    [TestCaseSource(nameof(AllKinds))]
    public void CrushAndWaterQuotes_OnlyUseThePlayerName_AndNameThePlayer(string kindName)
    {
        DeathQuoteKind kind = Kind(kindName);
        foreach (string quote in DeathQuoteLists.For(kind).Quotes)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(TokensOf(quote).Distinct(), Is.SubsetOf(CrushAndWaterTokens), quote);
                Assert.That(quote, Does.Contain("{Player.name}"), quote);
            }
        }
    }

    [Test]
    public void BossKillerQuotes_OnlyUseKnownPlaceholders_AndNameBossAndPlayer()
    {
        foreach (string quote in BossKillerQuotes.Pool.Quotes)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(TokensOf(quote).Distinct(), Is.SubsetOf(BossTokens), quote);
                Assert.That(quote, Does.Contain("{Player.name}"), quote);
                Assert.That(quote, Does.Contain("{Creature.name}"), quote);
                Assert.That(quote, Does.Contain("{level}"), quote);
            }
        }
    }

    [TestCaseSource(nameof(AllKinds))]
    public void EveryQuote_IsUniqueAsciiAndEndsWithPunctuation(string kindName)
    {
        DeathQuoteKind kind = Kind(kindName);
        IReadOnlyList<string> quotes = DeathQuoteLists.For(kind).Quotes;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(quotes.Distinct().Count(), Is.EqualTo(quotes.Count), "duplicate quote");
            foreach (string quote in quotes)
            {
                Assert.That(quote.All(c => c < 128), Is.True, "non-ASCII character in: " + quote);
                Assert.That(quote.TrimEnd(), Does.Match(@"[.!?]$"), quote);
            }
        }
    }

    [Test]
    public void BossKillerPool_EveryQuoteIsUniqueAsciiAndEndsWithPunctuation()
    {
        IReadOnlyList<string> quotes = BossKillerQuotes.Pool.Quotes;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(quotes.Distinct().Count(), Is.EqualTo(quotes.Count), "duplicate quote");
            foreach (string quote in quotes)
            {
                Assert.That(quote.All(c => c < 128), Is.True, "non-ASCII character in: " + quote);
                Assert.That(quote.TrimEnd(), Does.Match(@"[.!?]$"), quote);
            }
        }
    }

    [Test]
    public void Pool_NeverHandsOutTheSameQuoteTwiceInARow()
    {
        var pool = new QuotePool("a", "b", "c");
        string previous = null;
        for (int i = 0; i < 500; i++)
        {
            string quote = pool.Pick();
            Assert.That(quote, Is.Not.EqualTo(previous), "pick " + i);
            previous = quote;
        }
    }

    [Test]
    public void Pool_EventuallyHandsOutEveryQuote()
    {
        var pool = new QuotePool("a", "b", "c", "d");
        var seen = new HashSet<string>();
        for (int i = 0; i < 500; i++)
            seen.Add(pool.Pick());

        Assert.That(seen, Is.EquivalentTo(new[] { "a", "b", "c", "d" }));
    }

    [Test]
    public void Pool_WithOneQuote_AlwaysHandsItOut()
    {
        var pool = new QuotePool("only");
        Assert.That(new[] { pool.Pick(), pool.Pick(), pool.Pick() }, Is.All.EqualTo("only"));
    }

    [Test]
    public void Pool_WithoutQuotes_IsRefused()
    {
        Assert.Throws<ArgumentException>(() => new QuotePool());
    }
}
