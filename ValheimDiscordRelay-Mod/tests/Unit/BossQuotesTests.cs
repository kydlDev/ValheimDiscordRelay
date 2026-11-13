using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using ValheimDiscordRelay.Server;

namespace ValheimDiscordRelay.Tests.Unit;

/// <summary>The boss kill messages of the Discord embed: how many there are, their placeholders, and that picking fills every one.</summary>
[TestFixture]
public class BossQuotesTests
{
    private const int SoloQuoteCount = 26;    // 11 older ones plus 15 added ones
    private const int PartyQuoteCount = 26;
    private const int OlderQuoteCount = 11;

    private static readonly Regex Token = new(@"\{([^{}]*)\}", RegexOptions.CultureInvariant);

    private static string[] List(string field)
    {
        FieldInfo info = typeof(BossQuotes).GetField(field, BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(info, Is.Not.Null, field);
        return (string[])info.GetValue(null);
    }

    private static List<string> TokensOf(string template)
    {
        return Token.Matches(template).Cast<Match>().Select(m => m.Groups[1].Value).ToList();
    }

    [Test]
    public void SoloList_HasTheOldQuotesAndFifteenMore()
    {
        Assert.That(List("Solo"), Has.Length.EqualTo(SoloQuoteCount));
    }

    [Test]
    public void PartyList_HasTheOldQuotesAndFifteenMore()
    {
        Assert.That(List("Party"), Has.Length.EqualTo(PartyQuoteCount));
    }

    [TestCase("Solo")]
    [TestCase("Party")]
    public void EveryQuote_UsesOnlyKnownPlaceholders_AndNamesThePlayerAndTheDamage(string field)
    {
        foreach (string quote in List(field))
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(TokensOf(quote).Distinct(), Is.SubsetOf(new[] { "Boss", "Player", "Damage" }), quote);
                Assert.That(quote, Does.Contain("{Player}"), quote);
                Assert.That(quote, Does.Contain("{Damage}"), quote);
            }
        }
    }

    [TestCase("Solo")]
    [TestCase("Party")]
    public void TheAddedQuotes_NameTheBoss(string field)
    {
        // The first 11 of each list are the older quotes; two of them deliberately do not name the boss.
        foreach (string quote in List(field).Skip(OlderQuoteCount))
            Assert.That(quote, Does.Contain("{Boss}"), quote);
    }

    [TestCase("Solo")]
    [TestCase("Party")]
    public void EveryQuote_IsUnique(string field)
    {
        string[] quotes = List(field);
        Assert.That(quotes.Distinct().Count(), Is.EqualTo(quotes.Length));
    }

    [Test]
    public void SoloQuotes_AllEndWithTheThankYouParagraph()
    {
        foreach (string quote in List("Solo"))
            Assert.That(quote, Does.EndWith("\n\nGood job, {Player}! You've somehow managed to deal {Damage} damage to it."), quote);
    }

    [Test]
    public void PickSolo_FillsEveryPlaceholder()
    {
        for (int i = 0; i < 200; i++)
        {
            string text = BossQuotes.PickSolo("Eikthyr", "Bob", "1,234");
            Assert.That(text, Does.Not.Contain("{").And.Not.Contain("}"), text);
            Assert.That(text, Does.Contain("Bob").And.Contain("1,234"), text);
        }
    }

    [Test]
    public void PickParty_FillsEveryPlaceholder()
    {
        for (int i = 0; i < 200; i++)
        {
            string text = BossQuotes.PickParty("The Elder", "Alice", "9,999");
            Assert.That(text, Does.Not.Contain("{").And.Not.Contain("}"), text);
            Assert.That(text, Does.Contain("Alice").And.Contain("9,999"), text);
        }
    }

    [Test]
    public void PickedQuote_WithPlaceholderInThePlayerName_IsNotSubstitutedTwice()
    {
        string text = BossQuotes.PickSolo("Moder", "{Damage}", "77");
        Assert.That(text, Does.Contain("{Damage}"));
    }
}
