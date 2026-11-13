using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// The messages shown when the local player dies. One is picked at random for each death, never the same
/// one twice in a row within a category. Categories:
///   - killed by an animal with no stars, one star, or two-or-more stars (three separate lists)
///   - killed by any other (non-animal) enemy, including other players
///   - environmental deaths (burning, freezing, falling, poison, ...)
/// Some deaths have lists of their own, in the Domain folder: a boss killing the player (BossKillerQuotes), crushing by a
/// tree, cart, boat, structure, stone spike, drawbridge, turret, catapult or impact (CrushQuotes), and water deaths (WaterQuotes).
///
/// To add, remove or reword a quote, just edit the lists below. Placeholders:
///   {Player.name}    the player's name
///   {Creature.name}  the killer's name. Animals are written in lower case mid-sentence and capitalised at the
///                    start of a sentence; every other enemy keeps the name Valheim shows (e.g. "Greydwarf Brute")
///   {level}          the killer's stars, with a leading space: " (2 stars)" in game chat (the game font has no
///                    emoji) and a row of star emoji in Discord. Empty for a creature with no stars.
///   {star}           a single star: "star" in game chat and a star emoji in Discord
///   {Cause}          environmental deaths only: what killed the player, e.g. "drowning" (see DescribeCause in
///                    DeathMessageBuilder.cs)
/// Use \u2019 / \u2018 / \u2014 for ' ' and the long dash, and \u00e9 and \u00e5 for e-acute and a-ring (keeps the file encoding-proof).
/// </summary>
internal static class DeathQuotes
{
    /// <summary>
    /// Prefab names counted as animals (compared ignoring case, without the "(Clone)" suffix). Everything else
    /// that kills the player uses the non-animal list. Add a prefab name here to make another creature an animal.
    /// </summary>
    private static readonly HashSet<string> AnimalPrefabs = new(StringComparer.OrdinalIgnoreCase)
    {
        "Boar",
        "Neck",
        "Bjorn",       // the bear
        "Serpent",
        "Leech",
        "Wolf",
        "Bat",
        "Bat_swamp",
        "Lox",
        "Deathsquito",
        "Volture",
        "Asksvin",
        "Moose"
    };

    private static readonly string[] AnimalNoStar =
    {
        "{Player.name} was slain by a completely ordinary {Creature.name}. This is going to be difficult to explain.",
        "{Creature.name} has turned {Player.name} into forest food. Circle of life, Viking edition.",
        "{Creature.name} has claimed {Player.name}. You picked a fight with wildlife. Wildlife won.",
        "{Creature.name}, protecting its territory. {Player.name}, apparently part of the problem.",
        "{Player.name} has been eaten by a {Creature.name}. No stars. No excuses.",
        "A humble {Creature.name} has slain {Player.name}. Nature is healing.",
        "{Player.name} vs. an ordinary {Creature.name}, the {Creature.name} won. We may need to discuss your combat strategy.",
        "{Player.name} has fallen to a {Creature.name}. Not a particularly impressive {Creature.name}, either.",
        "The mighty Viking {Player.name} has been defeated by a regular {Creature.name}. Sk\u00e5l!",
        "{Creature.name}, 1. {Player.name}, 0. The creature didn't even have the decency to be special.",
        "{Player.name} has been bested by a {Creature.name}. The {Creature.name} did not even break a sweat.",
        "A perfectly average {Creature.name} just ended {Player.name}. Average is apparently enough.",
        "{Player.name} fell to a {Creature.name}. Please do not tell the other Vikings. They already know.",
        "The {Creature.name} didn't level up, and didn't need to. {Player.name} is down anyway.",
        "{Creature.name}: unarmed, unstarred, undefeated. {Player.name}: armed, confident, dead.",
        "{Player.name} was dispatched by a {Creature.name}. Even the tutorial enemies are fighting back now.",
        "Today's lesson from a {Creature.name}: gear doesn't matter if you stand still. Ask {Player.name}.",
        "{Player.name} has been defeated by a {Creature.name}. The local wildlife is holding a celebration.",
        "A {Creature.name} with zero stars and zero remorse has claimed {Player.name}.",
        "{Player.name} lost to a {Creature.name}. The bards have declined to write a song about it.",
        "{Player.name} has been outplayed by a {Creature.name}. It just walked up and did it.",
        "Somewhere a {Creature.name} is telling its friends it took down a Viking. Sadly, it's true. RIP {Player.name}.",
        "{Player.name} was slain by a {Creature.name}. Valhalla's front desk is trying not to laugh.",
        "One {Creature.name}. One Viking. One very short fight. {Player.name} did not make it.",
        "{Creature.name} has added {Player.name} to its list of achievements. It's a short list."
    };

    private static readonly string[] AnimalOneStar =
    {
        "{Player.name} was slain by a {Creature.name}{level}. Finally, an enemy with a r\u00e9sum\u00e9.",
        "A {Creature.name}{level} has defeated {Player.name}. To be fair, it had an entire extra star.",
        "{Player.name} picked a fight with a 1-star {Creature.name}. The animal had a family. What did you have?",
        "{Creature.name}{level} has claimed {Player.name}. Apparently that one extra star made all the difference.",
        "{Player.name} was killed by a {Creature.name}{level}. Congratulations to the {Creature.name}{level} on its impressive career advancement.",
        "{Creature.name}{level} has claimed another Viking. Somewhere, its proud mother is telling all the other animals.",
        "{Player.name} underestimated the {star} on that {Creature.name}{level}. The {star} did not underestimate {Player.name}.",
        "{Player.name} has been promoted to dinner by a {Creature.name}{level}.",
        "A {Creature.name}{level} just turned {Player.name} into Viking mince. Stellar performance.",
        "{Creature.name}{level} defeated {Player.name}. The star was apparently worth more than all that expensive Viking gear.",
        "{Player.name} was beaten by a {Creature.name}{level}. That single {star} was doing a lot of heavy lifting.",
        "A {Creature.name}{level} has claimed {Player.name}. One {star}, one funeral.",
        "{Player.name} ignored the {star} on a {Creature.name}{level}. The {star} remembered.",
        "{Creature.name}{level} has taken down {Player.name}. Technically a fair fight. Technically.",
        "{Player.name} has fallen to a {Creature.name}{level}. It's not the size of the star, it's how you use it.",
        "The {Creature.name}{level} read the room, saw {Player.name}, and made a career-defining move.",
        "{Player.name} lost to a {Creature.name}{level}. The {star} is now insufferable about it.",
        "A {Creature.name}{level} just got a little bit famous. Thank you for your service, {Player.name}.",
        "{Player.name} has been ended by a {Creature.name}{level}. That {star} came with a surprising amount of attitude.",
        "{Creature.name}{level} versus {Player.name}: the {star} won on points.",
        "{Player.name} was slain by a {Creature.name}{level}. Look on the bright side: it was only one {star}. It still counts.",
        "{Player.name} met a {Creature.name}{level} and learned that rank matters.",
        "A {Creature.name}{level} has eaten well tonight. {Player.name} was the main course.",
        "{Player.name} fell to a {Creature.name}{level}. This is how every great wildlife legend starts.",
        "{Creature.name}{level} has defeated {Player.name}. The animal kingdom's middle management strikes again."
    };

    private static readonly string[] AnimalMultiStar =
    {
        "{Player.name} was slain by a {Creature.name}{level}. In fairness, that thing was basically a natural disaster with legs.",
        "{Creature.name}{level} has claimed {Player.name}. You really should have read the warning label.",
        "{Player.name} has fallen to a {Creature.name}{level}. That's not a normal animal. That's an angry tax collector from Valhalla.",
        "A {Creature.name}{level} has eaten {Player.name}. At least you were defeated by something genuinely terrifying.",
        "{Player.name} challenged a {Creature.name}{level} and discovered why stars are generally considered important.",
        "{Player.name} was slain by mama {Creature.name}{level}. Her babies are safe, and you are not.",
        "{Player.name} has been slain by a very protective mother {Creature.name}{level}. She had nothing left to lose.",
        "A heavily starred {Creature.name}{level} has claimed another Viking. {Player.name} fought bravely. Briefly.",
        "{Player.name} has fallen to {Creature.name}{level}. That's not a death. That's an unfortunate wildlife encounter.",
        "{Creature.name}{level} defeated {Player.name}. The stars aligned. Unfortunately, they aligned against you.",
        "A {Creature.name}{level} has ended {Player.name}. Some animals just wake up angry and choose violence.",
        "{Player.name} was slain by a {Creature.name}{level}. That's not wildlife, that's a warning sign with teeth.",
        "{Player.name} has fallen to a {Creature.name}{level}. The stars were practically a neon sign saying \"run\".",
        "{Creature.name}{level} has claimed {Player.name}. Next time, bring friends. Or a boat. Or both.",
        "{Player.name} fought a {Creature.name}{level} and lost. The {Creature.name} was not even tired.",
        "A {Creature.name}{level} has sent {Player.name} back to the spawn screen. Brutal, efficient, a little rude.",
        "{Player.name} has been flattened by a {Creature.name}{level}. This animal did not get its rank by being polite.",
        "The {Creature.name}{level} has been feared for generations. {Player.name} has now personally confirmed it.",
        "{Player.name} was eaten by a {Creature.name}{level}. The animal has requested to be called \"Sir\".",
        "{Player.name} looked at a {Creature.name}{level} and thought \"I can take it\". Famous last words.",
        "A {Creature.name}{level} has reminded {Player.name} that the food chain has opinions.",
        "{Player.name} has fallen to a {Creature.name}{level}. We have lowered the flags and raised the difficulty.",
        "{Creature.name}{level} has claimed {Player.name}. Please enjoy a moment of silence, followed by a moment of better planning.",
        "{Player.name} did not survive a {Creature.name}{level}. Honestly, who could.",
        "A {Creature.name}{level} just ended {Player.name} without breaking stride. Terrifying. Respect."
    };

    // Written without an article ("slain by {Creature.name}") because the killer can also be another player.
    private static readonly string[] NonAnimal =
    {
        "{Player.name} has been slain by {Creature.name}{level}. Somewhere in Valheim, an enemy is feeling very proud of itself.",
        "{Creature.name}{level} has defeated {Player.name}. Another great Viking adventure ends in administrative failure.",
        "RIP {Player.name}. Killed by {Creature.name}{level}. Your gear will be missed almost as much as your competence.",
        "{Player.name} has fallen to {Creature.name}{level}. The Valheim insurance company has denied your claim.",
        "{Creature.name}{level} has claimed {Player.name}. In other news, local Viking discovers that enemies can, in fact, hurt.",
        "{Player.name} was slain by {Creature.name}{level}. It wasn't even a boss. Where's the dignity?",
        "{Creature.name}{level} defeated {Player.name}. The battle lasted longer in {Player.name}'s imagination.",
        "{Player.name} has met their untimely demise at the hands of {Creature.name}{level}. Please leave your valuables where they are.",
        "Another Viking falls! {Player.name} was defeated by {Creature.name}{level}. The enemies are becoming alarmingly competent.",
        "{Creature.name}{level} has slain {Player.name}. Congratulations, {Creature.name}. You have officially ruined someone's evening.",
        "{Player.name} got killed by a {Creature.name}{level}. The {Creature.name} is dancing on {Player.name}'s tombstone.",
        "{Player.name} was cut down by {Creature.name}{level}. The funeral will be held wherever the loot ended up.",
        "{Creature.name}{level} has ended {Player.name}. Valheim's enemies send their regards.",
        "{Player.name} has been sent to Valhalla early by {Creature.name}{level}. The reception was not ready.",
        "{Creature.name}{level} has won this round against {Player.name}. The rematch begins at the corpse run.",
        "{Player.name} fell to {Creature.name}{level}. Have you considered armour, or at least a plan?",
        "Another gravestone for {Player.name}, courtesy of {Creature.name}{level}.",
        "{Creature.name}{level} sends {Player.name} on an unscheduled trip to Valhalla. No refunds.",
        "{Player.name} was overwhelmed by {Creature.name}{level}. This is why we scout first.",
        "{Creature.name}{level} has taken {Player.name} out of the game, briefly. Respawns are free, dignity is not.",
        "{Player.name} has been defeated by {Creature.name}{level}. Odin is taking notes, and he is not impressed.",
        "{Creature.name}{level} did what it came to do. {Player.name} did what everyone expected.",
        "{Player.name} tried to fight {Creature.name}{level} and has joined the tombstone collection.",
        "The skirmish between {Player.name} and {Creature.name}{level} is over. Score it as you like. Nobody is arguing.",
        "{Player.name} has been brought down by {Creature.name}{level}. Please retrieve the gear before someone else does.",
        "{Creature.name}{level} has bested {Player.name}. A moment of silence, then a rematch."
    };

    private static readonly string[] Environmental =
    {
        "{Player.name} has been defeated by the elements. Nature didn't even need to send a creature.",
        "{Player.name} has discovered that the environment is, in fact, hostile.",
        "The world itself has claimed {Player.name}. That's one way to lose a fight.",
        "{Player.name} has discovered one of Valheim's many creative ways to die, {Cause}.",
        "{Player.name} has perished due to environmental circumstances. Skill issue.",
        "No enemy in sight. Just {Player.name} losing a fight against the environment.",
        "{Player.name} has been claimed by the elements. Odin will never know what happened here.",
        "{Player.name} has discovered one of Valheim's many creative ways to die.",
        "The environment has defeated {Player.name}. Nature is indeed very powerful.",
        "{Player.name} has died to the forces of nature. The wilderness remains undefeated.",
        "{Player.name} learned the hard way why you can't fight nature.",
        "{Player.name} has died to {Cause}. The world did not even try hard.",
        "{Player.name} has been undone by {Cause}. Nature wins again, quietly.",
        "Cause of death for {Player.name}: {Cause}. Witnesses: nobody. Excuses: many.",
        "{Player.name} has been taken by {Cause}. No monsters were needed.",
        "{Player.name} lost to {Cause}. The world is simply that hostile.",
        "{Player.name} has met {Cause}. Valheim's terrain sends its warm regards.",
        "{Player.name} was defeated by nothing in particular, namely {Cause}. Impressive.",
        "Not a single enemy involved. {Player.name} and {Cause} did this all on their own.",
        "{Player.name} has gone to Valhalla via {Cause}. Scenic route, questionable choice.",
        "{Player.name} fought the environment and the environment fought dirty, {Cause}.",
        "Another entry in the hall of shame: {Player.name}, felled by {Cause}.",
        "{Player.name} has been reminded that Valheim is not a friendly place, {Cause} being the teacher.",
        "{Player.name} has died without anyone to blame. That must sting.",
        "The wilderness has taken {Player.name}. It was nothing personal. It never is.",
        "{Player.name} has been beaten by the scenery. Odin has seen worse. Barely."
    };

    // One pass over the template, so a player name that happens to contain "{Cause}" is never substituted again.
    private static readonly Regex Token = new(
        @"\{(Player\.name|Creature\.name|level|star|Cause)\}", RegexOptions.CultureInvariant);

    private static readonly Random Rng = new(Guid.NewGuid().GetHashCode());
    private static readonly object RngLock = new();

    private static int _lastAnimalNoStar = -1;
    private static int _lastAnimalOneStar = -1;
    private static int _lastAnimalMultiStar = -1;
    private static int _lastNonAnimal = -1;
    private static int _lastEnvironmental = -1;

    /// <summary>
    /// True if the prefab name belongs to an animal. Valheim's "(Clone)" suffix is ignored.
    /// </summary>
    /// <param name="prefabName">The attacker's prefab name, or null if unknown.</param>
    internal static bool IsAnimalPrefab(string prefabName)
    {
        if (string.IsNullOrWhiteSpace(prefabName))
            return false;

        string name = BossCatalog.CleanPrefabName(prefabName);
        return AnimalPrefabs.Contains(name);
    }

    /// <summary>
    /// A random death message for a killer that is not a boss.
    /// </summary>
    /// <param name="player">The player's name, already safe for Discord markdown.</param>
    /// <param name="attacker">The creature (or player) that killed the player.</param>
    /// <returns>The message in in-game and Discord form.</returns>
    internal static DeathMessage PickCreature(string player, AttackerInfo attacker)
    {
        string template;
        lock (RngLock)
        {
            if (!attacker.IsAnimal)
                template = Pick(NonAnimal, ref _lastNonAnimal);
            else if (attacker.StarCount <= 0)
                template = Pick(AnimalNoStar, ref _lastAnimalNoStar);
            else if (attacker.StarCount == 1)
                template = Pick(AnimalOneStar, ref _lastAnimalOneStar);
            else
                template = Pick(AnimalMultiStar, ref _lastAnimalMultiStar);
        }

        return new DeathMessage(
            Fill(template, player, attacker, null, false),
            Fill(template, player, attacker, null, true));
    }

    /// <summary>
    /// A random environmental death message.
    /// </summary>
    /// <param name="player">The player's name, already safe for Discord markdown.</param>
    /// <param name="cause">What killed the player, as a short lower-case phrase (fills {Cause}).</param>
    internal static string PickEnvironmental(string player, string cause)
    {
        string template;
        lock (RngLock)
            template = Pick(Environmental, ref _lastEnvironmental);

        return Fill(template, player, null, cause, false);
    }

    /// <summary>
    /// A random death message for a boss (or a colour-tagged enemy) that killed the player.
    /// </summary>
    /// <param name="player">The player's name, already safe for Discord markdown.</param>
    /// <param name="boss">The boss that killed the player.</param>
    /// <returns>The message in in-game and Discord form; in Discord a coloured boss name keeps its ANSI colour.</returns>
    internal static DeathMessage PickBoss(string player, AttackerInfo boss)
    {
        string template = BossKillerQuotes.Pool.Pick();
        RelayDiagnostics.Debug(
            "[Death] boss quote picked for '" + boss.Name + "' with " + boss.StarCount + " star(s), one of " +
            BossKillerQuotes.Pool.Quotes.Count + ".");

        return new DeathMessage(
            Fill(template, player, boss, null, false),
            Fill(template, player, boss, null, true));
    }

    /// <summary>
    /// A random death message for a kind of death that has a list of its own (crushing, swimming, the Ashlands sea).
    /// </summary>
    /// <param name="player">The player's name, already safe for Discord markdown.</param>
    /// <param name="kind">The kind of death.</param>
    internal static string PickKind(string player, DeathQuoteKind kind)
    {
        QuotePool pool = DeathQuoteLists.For(kind);
        string template = pool.Pick();
        RelayDiagnostics.Debug("[Death] " + kind + " quote picked, one of " + pool.Quotes.Count + ".");

        return Fill(template, player, null, null, false);
    }

    /// <summary>
    /// Picks a random entry, never the same one twice in a row (when there is more than one to choose from).
    /// </summary>
    private static string Pick(string[] quotes, ref int last)
    {
        int index = Rng.Next(quotes.Length);
        if (quotes.Length > 1 && index == last)
            index = (index + 1 + Rng.Next(quotes.Length - 1)) % quotes.Length;

        last = index;
        return quotes[index];
    }

    private static string Fill(string template, string player, AttackerInfo attacker, string cause, bool discord)
    {
        return Token.Replace(template, delegate (Match match)
        {
            switch (match.Groups[1].Value)
            {
                case "Player.name":
                    return player;
                case "Creature.name":
                    string name = CreatureName(attacker, StartsSentence(template, match.Index));
                    return discord && IsColoredBoss(attacker)
                        ? DeathMessageBuilder.AnsiColorStart + name + DeathMessageBuilder.AnsiReset
                        : name;
                case "level":
                    return Level(attacker, discord);
                case "star":
                    return discord ? "\u2B50" : "star";
                default:
                    return string.IsNullOrWhiteSpace(cause) ? "the elements" : cause;
            }
        });
    }

    /// <summary>
    /// True for a boss whose game name carries a colour; Discord shows its name in that colour inside an ansi code block.
    /// </summary>
    private static bool IsColoredBoss(AttackerInfo attacker)
    {
        return attacker != null && attacker.IsBoss && attacker.HasColorTag;
    }

    private static string CreatureName(AttackerInfo attacker, bool sentenceStart)
    {
        if (attacker == null)
            return "creature";

        // Only animals are re-cased; other enemies and players keep the name as Valheim shows it.
        if (!attacker.IsAnimal)
            return attacker.Name;

        if (!sentenceStart)
            return attacker.Name.ToLowerInvariant();

        string name = attacker.Name;
        return name.Length == 0 ? name : char.ToUpperInvariant(name[0]) + name.Substring(1).ToLowerInvariant();
    }

    private static string Level(AttackerInfo attacker, bool discord)
    {
        if (attacker == null || attacker.StarCount <= 0)
            return string.Empty;

        if (discord)
            return " " + attacker.BuildStars("\u2B50");

        int stars = attacker.StarCount;
        return " (" + stars + (stars == 1 ? " star)" : " stars)");
    }

    /// <summary>
    /// True when the token at <paramref name="index"/> opens a sentence (start of the message, or after . ! ?).
    /// </summary>
    private static bool StartsSentence(string template, int index)
    {
        int i = index - 1;
        while (i >= 0 && char.IsWhiteSpace(template[i]))
            i--;

        return i < 0 || template[i] == '.' || template[i] == '!' || template[i] == '?';
    }
}
