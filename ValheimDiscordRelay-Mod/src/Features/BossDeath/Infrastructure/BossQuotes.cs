using System;
using System.Text.RegularExpressions;

namespace ValheimDiscordRelay.Server;

/// <summary>
/// The messages shown in the boss death embed. One is picked at random for each kill: from the solo list when
/// one player dealt all the damage, from the party list when two or more did.
///
/// To add, remove or reword a quote, just edit the lists below. Placeholders:
///   {Boss}   the boss's name
///   {Player} the top damage dealer's name (party quotes)
///   {Damage} solo quotes: the damage the player dealt; party quotes: the top damage dealer's damage
/// Use \n\n for a blank line, and \u2019 / \u2018 / \u2014 for ' ' and the long dash (keeps the file encoding-proof).
/// </summary>
internal static class BossQuotes
{
    /// <summary>The closing paragraph shared by the solo quotes added later (the older ones carry it in their own text).</summary>
    private const string SoloThanks = "\n\nGood job, {Player}! You've somehow managed to deal {Damage} damage to it.";

    private static readonly string[] Solo =
    {
        "{Boss} has been defeated! Turns out it was no match for one moderately competent Viking.\n\nGood job, {Player}! You've somehow managed to deal {Damage} damage to it.",
        "You killed {Boss} alone? Who needs friends when you have questionable life choices?\n\nGood job, {Player}! You've somehow managed to deal {Damage} damage to it.",
        "Solo victory! {Boss} never stood a chance against that much stubbornness.\n\nGood job, {Player}! You've somehow managed to deal {Damage} damage to it.",
        "Well played. {Boss} has been slain, and your ego has gained +10.\n\nGood job, {Player}! You've somehow managed to deal {Damage} damage to it.",
        "{Boss} is dead. Please remember to thank nobody for the assistance.\n\nGood job, {Player}! You've somehow managed to deal {Damage} damage to it.",
        "A solo kill?! Incredible. We\u2019ll notify the bards immediately.\n\nGood job, {Player}! You've somehow managed to deal {Damage} damage to it.",
        "You actually killed it alone. I had money on \u2018dies horribly.\u2019\n\nGood job, {Player}! You've somehow managed to deal {Damage} damage to it.",
        "{Boss} has fallen! Unfortunately, so has your excuse for not helping the team.\n\nGood job, {Player}! You've somehow managed to deal {Damage} damage to it.",
        "Solo boss kill unlocked. Achievement: Couldn\u2019t find anyone to help.\n\nGood job, {Player}! You've somehow managed to deal {Damage} damage to it.",
        "Against all odds, and apparently against all common sense, you killed {Boss}.\n\nGood job, {Player}! You've somehow managed to deal {Damage} damage to it.",
        "{Boss}, a boss? Nah. This lone wolf made it look easy.\n\nGood job, {Player}! You've somehow managed to deal {Damage} damage to it.",
        "{Boss} is down, and nobody helped. Not even a little bit." + SoloThanks,
        "One Viking, one boss, zero witnesses worth mentioning. {Boss} has been slain." + SoloThanks,
        "{Boss} has been defeated single-handedly. The legends write themselves." + SoloThanks,
        "Lone Viking energy at its finest: {Boss} is dead and the credit is entirely yours." + SoloThanks,
        "{Boss} fell to one very determined Viking. The rest of the server is now feeling underqualified." + SoloThanks,
        "{Boss} has been slain alone. Odin raises a horn, and nobody else was invited." + SoloThanks,
        "You faced {Boss} without a party and won. Questionable planning, undeniable result." + SoloThanks,
        "{Boss} is dead. The group chat is going to hear about this for weeks." + SoloThanks,
        "A solo victory over {Boss}! The bards are tuning their instruments, reluctantly." + SoloThanks,
        "{Boss} never saw it coming, mostly because nobody else was there to warn it." + SoloThanks,
        "{Boss} has been beaten one-on-one. The word \"teamwork\" has been quietly removed from the report." + SoloThanks,
        "Single-handed, unassisted, slightly unhinged: {Boss} has fallen." + SoloThanks,
        "{Boss} did not fall to an army. It fell to one Viking with a bad attitude." + SoloThanks,
        "{Boss} is no more, and you did not need anybody's help. Show-off." + SoloThanks,
        "The trophy is yours and only yours. {Boss} has been taken down solo." + SoloThanks
    };

    private static readonly string[] Party =
    {
        "{Boss} has fallen! MVP goes to {Player} with a brutal {Damage} damage. The rest of you were there too, apparently.",
        "Victory! {Boss} is dead, thanks largely to {Player}, who dealt {Damage} damage while everyone else provided moral support.",
        "The Vikings have prevailed! Special mention to {Player} for putting {Damage} damage into {Boss}. Please try to contain your ego.",
        "{Boss} has been slain! {Player} led the damage charts with {Damage} damage. Everyone else: thanks for showing up.",
        "Another boss bites the dust! {Player} contributed a mighty {Damage} damage. The rest of the party contributed\u2026 atmosphere.",
        "Victory! {Boss} is down, and {Player} is officially the party\u2019s designated damage goblin with {Damage} damage.",
        "Well fought, Vikings! {Player} dealt {Damage} damage to {Boss}. We assume the rest of you were busy with important Viking business.",
        "{Boss} has fallen! {Player} topped the damage charts with {Damage}. Somewhere, the other Vikings are furiously checking their excuses.",
        "Boss defeated! {Player} delivered {Damage} damage and carried the party straight into Valhalla. Please collect your complimentary ego boost at the door.",
        "{Boss} is dead! Congratulations to everyone involved \u2014 especially {Player}, who apparently decided {Damage} damage was a reasonable amount of violence.",
        "You are stronger together, {Boss} had no chance against such a strong team. {Player} managed to do {Damage} to {Boss}.",
        "{Boss} has fallen! {Player} led the charge with {Damage} damage, and everyone else made it a party.",
        "Teamwork wins again! {Boss} is dead, with {Player} on top of the charts at {Damage} damage.",
        "{Boss} faced a whole group and lost. {Player} dealt {Damage} damage and gets the bragging rights.",
        "The Vikings stand victorious over {Boss}! Top damage: {Player} with {Damage}. Everyone else: you were also there.",
        "{Boss} has been taken down together! {Player} contributed {Damage} damage and will not shut up about it.",
        "A group effort, a fallen {Boss}, and {Player} sitting on {Damage} damage. Fair is fair, though.",
        "{Boss} did not stand a chance against the whole crew. {Player} hit hardest at {Damage} damage.",
        "Victory over {Boss}! {Player} topped the damage table with {Damage}. The rest handled morale.",
        "{Boss} is dead and the party is intact. {Player} deserves a toast for {Damage} damage.",
        "The group has done it: {Boss} is down! {Player} did {Damage} damage, which is the number to argue about.",
        "{Boss} has fallen to a coordinated effort. {Player} led with {Damage} damage; the others led with enthusiasm.",
        "Together you prevailed over {Boss}. Top spot goes to {Player} with {Damage} damage. Argue amongst yourselves.",
        "{Boss} lost to a crowd, and {Player}'s {Damage} damage made sure of it.",
        "Many Vikings, one dead {Boss}. {Player} put in {Damage} damage to make it official.",
        "{Boss} has been defeated! {Player} delivered {Damage} damage, and the rest of you delivered company."
    };

    // One pass over the template, so a player name that happens to contain "{Damage}" is never substituted again.
    private static readonly Regex Token = new(@"\{(Boss|Player|Damage)\}", RegexOptions.CultureInvariant);

    private static readonly Random Rng = new(Guid.NewGuid().GetHashCode());
    private static readonly object RngLock = new();
    private static readonly Client.RecentPicker RecentSolo = new();
    private static readonly Client.RecentPicker RecentParty = new();

    /// <summary>
    /// A random solo-kill message.
    /// </summary>
    /// <param name="boss">Boss name, already safe for Discord markdown.</param>
    /// <param name="player">The player's name, already safe for Discord markdown.</param>
    /// <param name="damage">The damage the player dealt, formatted.</param>
    internal static string PickSolo(string boss, string player, string damage)
    {
        string template;
        lock (RngLock)
            template = Pick(Solo, RecentSolo);

        return Fill(template, boss, player, damage);
    }

    /// <summary>
    /// A random party-kill message.
    /// </summary>
    /// <param name="boss">Boss name, already safe for Discord markdown.</param>
    /// <param name="player">Top damage dealer's name, already safe for Discord markdown.</param>
    /// <param name="damage">The top damage dealer's damage, formatted.</param>
    internal static string PickParty(string boss, string player, string damage)
    {
        string template;
        lock (RngLock)
            template = Pick(Party, RecentParty);

        return Fill(template, boss, player, damage);
    }

    /// <summary>
    /// Picks a random entry that was not one of the last seven picked from this list.
    /// </summary>
    private static string Pick(string[] quotes, Client.RecentPicker recent)
    {
        return quotes[recent.Next(quotes.Length, Rng)];
    }

    private static string Fill(string template, string boss, string player, string damage)
    {
        return Token.Replace(template, delegate (Match match)
        {
            switch (match.Groups[1].Value)
            {
                case "Boss": return boss;
                case "Player": return player;
                default: return damage;
            }
        });
    }
}
