namespace ValheimDiscordRelay.Client;

/// <summary>
/// The messages for deaths in water: swimming and drowning in ordinary water, and the boiling Ashlands sea. Placeholder:
/// {Player.name} (the player's name). One is picked at random for each death, never the same one twice in a row.
/// To add, remove or reword a quote, just edit the lists below.
/// </summary>
internal static class WaterQuotes
{
    /// <summary>Drowning, or any other death from swimming in ordinary water.</summary>
    internal static readonly QuotePool Swimming = new(
        "{Player.name} has drowned. The Viking who conquered kingdoms was beaten by a puddle with ambition.",
        "{Player.name} went for a swim and did not come back up. Reviews were mixed.",
        "{Player.name} ran out of stamina and out of ocean. Mostly out of ocean.",
        "The sea has claimed {Player.name}. Njord accepts the offering, uninvited.",
        "{Player.name} has discovered that armour and swimming are not friends.",
        "{Player.name} has been defeated by water. Plain, ordinary, wet water.",
        "{Player.name} has gone for a long, deep swim. Please send flowers to the nearest shore.",
        "{Player.name} has sunk like a stone. A very heavy, very well-geared stone.",
        "{Player.name} has learned that Vikings are excellent sailors and terrible swimmers.",
        "{Player.name} has drowned. The longship was right there. It was right there.",
        "{Player.name} tried to swim for it. The distance won.",
        "{Player.name} has gone under for the last time. The fish are not sorry.",
        "{Player.name} ran out of breath long before running out of ocean.",
        "{Player.name} has been taken by the deep. It was very polite about it, and very wet.",
        "{Player.name} has proved that water is, in fact, wet and fatal.");

    /// <summary>Death in the Ashlands ocean, which boils.</summary>
    internal static readonly QuotePool AshlandsSea = new(
        "{Player.name} has been boiled alive in the Ashlands sea. Nobody ordered soup.",
        "{Player.name} went for a swim in the Ashlands. The water was not at a comfortable temperature.",
        "{Player.name} has been cooked by the Ashlands ocean. Medium rare, then well done.",
        "The Ashlands sea has claimed {Player.name}. It was a warm welcome. Far too warm.",
        "{Player.name} has learned that a hot spring and a boiling ocean are not the same thing.",
        "{Player.name} dipped a toe in the Ashlands water. Then the rest of them.",
        "{Player.name} has been simmered to perfection by the Ashlands sea.",
        "{Player.name} has been defeated by water that should not be this hot.",
        "{Player.name} was boiled in the Ashlands. Surtr's hospitality has no cold side.",
        "{Player.name} jumped into the Ashlands water. The water jumped back, scalding.",
        "{Player.name} has gone from adventurer to stew. The Ashlands sea is very proud of the recipe.",
        "{Player.name} has been poached by the Ashlands ocean. No eggs were involved.",
        "Swimming in the Ashlands is not recommended. {Player.name} has confirmed this the hard way.",
        "{Player.name} has been undone by an ocean that was already on the boil.",
        "The Ashlands sea has taken {Player.name}. It was hot, it was fast, and it was entirely avoidable.");
}
