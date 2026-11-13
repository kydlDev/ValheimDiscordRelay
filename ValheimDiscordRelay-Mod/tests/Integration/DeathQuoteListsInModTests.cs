using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ValheimDiscordRelay.Tests.Integration;

/// <summary>
/// The death quote lists inside the built mod DLL: the five older lists with their 15 added quotes, and the new death kinds picked
/// through the real picker. What needs the running game (the choice of list from the killing hit) is tested in game.
/// </summary>
[TestFixture]
public class DeathQuoteListsInModTests
{
    const string Quotes = "ValheimDiscordRelay.Client.DeathQuotes";
    const string KindType = "ValheimDiscordRelay.Client.DeathQuoteKind";

    static readonly Regex Token = new(@"\{([^{}]*)\}", RegexOptions.CultureInvariant);

    static readonly string[] CreatureTokens = { "Player.name", "Creature.name", "level", "star" };
    static readonly string[] EnvironmentalTokens = { "Player.name", "Cause" };

    static string[] List(string field) { return (string[])ModAssembly.GetStatic(Quotes, field); }

    static IEnumerable<string> TokensOf(string template) { return Token.Matches(template).Cast<Match>().Select(m => m.Groups[1].Value); }

    static IEnumerable<object> AllKinds()
    {
        Type kind = ModAssembly.Type(KindType);
        foreach (object value in Enum.GetValues(kind))
            yield return value;
    }

    [TestCase("AnimalNoStar", 25)]
    [TestCase("AnimalOneStar", 25)]
    [TestCase("AnimalMultiStar", 25)]
    [TestCase("NonAnimal", 26)]
    [TestCase("Environmental", 26)]
    public void OlderList_HasFifteenMoreQuotes(string field, int expected)
    {
        Assert.That(List(field), Has.Length.EqualTo(expected));
    }

    [TestCase("AnimalNoStar", 10)]
    [TestCase("AnimalOneStar", 10)]
    [TestCase("AnimalMultiStar", 10)]
    [TestCase("NonAnimal", 11)]
    public void CreatureQuotes_UseOnlyKnownPlaceholders_AndTheAddedOnesNameThePlayerAndTheCreature(string field, int olderCount)
    {
        string[] quotes = List(field);
        foreach (string quote in quotes)
            Assert.That(TokensOf(quote).Distinct(), Is.SubsetOf(CreatureTokens), quote);

        // The older quotes are as they were (one of them names neither); the added ones all name both.
        foreach (string quote in quotes.Skip(olderCount))
            Assert.That(quote, Does.Contain("{Player.name}").And.Contain("{Creature.name}"), quote);
    }

    [Test]
    public void EnvironmentalQuotes_UseOnlyKnownPlaceholders_AndNameThePlayer()
    {
        foreach (string quote in List("Environmental"))
        {
            Assert.That(TokensOf(quote).Distinct(), Is.SubsetOf(EnvironmentalTokens), quote);
            Assert.That(quote, Does.Contain("{Player.name}"), quote);
        }
    }

    [TestCase("AnimalNoStar")]
    [TestCase("AnimalOneStar")]
    [TestCase("AnimalMultiStar")]
    [TestCase("NonAnimal")]
    [TestCase("Environmental")]
    public void OlderList_HasNoDuplicates(string field)
    {
        string[] quotes = List(field);
        Assert.That(quotes.Distinct().Count(), Is.EqualTo(quotes.Length));
    }

    [Test]
    public void NonAnimalQuotes_NeverPutAnArticleInFrontOfTheCreatureName_BecauseItCanBeAnotherPlayer()
    {
        foreach (string quote in List("NonAnimal").Skip(11))
            Assert.That(quote, Does.Not.Match(@"\b[Aa]n? \{Creature\.name\}"), quote);
    }

    [Test]
    public void PickEnvironmental_FillsThePlayerAndTheCause()
    {
        for (int i = 0; i < 300; i++)
        {
            string text = (string)ModAssembly.Call(Quotes, "PickEnvironmental", "Bob", "freezing");
            Assert.That(text, Does.Not.Contain("{"), text);
            Assert.That(text, Does.Contain("Bob"), text);
        }
    }

    [Test]
    public void PickKind_FillsThePlayerName_ForEveryKindOfDeath()
    {
        foreach (object kind in AllKinds())
        {
            for (int i = 0; i < 100; i++)
            {
                string text = (string)ModAssembly.Call(Quotes, "PickKind", "Bob", kind);
                Assert.That(text, Does.Not.Contain("{").And.Not.Contain("}"), kind + ": " + text);
                Assert.That(text, Does.Contain("Bob"), kind + ": " + text);
            }
        }
    }

    [Test]
    public void PickKind_ForEveryKind_ReachesMoreThanOneQuote()
    {
        foreach (object kind in AllKinds())
        {
            var seen = new HashSet<string>();
            for (int i = 0; i < 300; i++)
                seen.Add((string)ModAssembly.Call(Quotes, "PickKind", "Bob", kind));

            Assert.That(seen.Count, Is.GreaterThan(5), kind.ToString());
        }
    }
}
