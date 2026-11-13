namespace ValheimDiscordRelay.Client;

/// <summary>
/// The messages for the death of the local player at the hands of a boss (or of an enemy whose name carries a colour tag).
/// The first quote is the original message, the others were added later. Placeholders:
///   {Player.name}    the player's name
///   {Creature.name}  the boss's name (coloured in Discord when the game colours it)
///   {level}          the boss's stars, with a leading space; empty for a boss with no stars
/// To add, remove or reword a quote, just edit the list below.
/// </summary>
internal static class BossKillerQuotes
{
    /// <summary>Every quote for a boss killing the player.</summary>
    internal static readonly QuotePool Pool = new(
        "{Creature.name}{level} managed to get one over {Player.name}. {Creature.name} roams triumphant, for now...You got this!",
        "{Creature.name}{level} has crushed {Player.name}. The boss arena remains undefeated, for now.",
        "{Player.name} challenged {Creature.name}{level} and lost. Respect for the courage, none for the preparation.",
        "{Creature.name}{level} stands victorious over {Player.name}. Regroup, rest, and come back with better food.",
        "{Player.name} has fallen to {Creature.name}{level}. That's what bosses are for. Try again.",
        "A boss is a boss for a reason. {Creature.name}{level} just proved it on {Player.name}.",
        "{Creature.name}{level} sends {Player.name} back to the altar. The gods will want a word.",
        "{Player.name} has been defeated by {Creature.name}{level}. Valhalla is full of people who tried. Join them briefly, then return.",
        "{Creature.name}{level} has taught {Player.name} a valuable lesson about boss mechanics. Tuition: one death.",
        "The fight was brave, the outcome was not. {Creature.name}{level} wins against {Player.name} this time.",
        "{Player.name} fell to {Creature.name}{level}. Dodge more. Eat better. Try again.",
        "{Creature.name}{level} did not even need a second phase. {Player.name} will be back.",
        "{Player.name} has been put down by {Creature.name}{level}. The boss is celebrating. Do not let it enjoy this for long.",
        "{Creature.name}{level} has claimed {Player.name}. Your corpse run begins now, and so does your revenge.",
        "{Player.name} took on {Creature.name}{level} and learned why this one is a boss.",
        "{Creature.name}{level} roars in triumph over {Player.name}. Heal up, Viking. It is not over.");
}
