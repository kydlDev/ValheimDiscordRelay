namespace ValheimDiscordRelay.Client;

/// <summary>
/// The messages for deaths where something heavy crushed the player. Each kind has its own list; the first quote of every
/// list is the original message, the others were added later. One is picked at random for each death, never the same one
/// twice in a row. Placeholder: {Player.name} (the player's name). There is no killer and no cause to name.
/// To add, remove or reword a quote, just edit the lists below.
/// </summary>
internal static class CrushQuotes
{
    /// <summary>Death by a falling tree. The first quote is the original one.</summary>
    internal static readonly QuotePool Tree = new(
        "{Player.name} got crushed by a falling tree.",
        "{Player.name} chopped a tree and the tree had other plans. Timber!",
        "{Player.name} was crushed by a falling tree. The forest has filed its complaint.",
        "Lumberjack {Player.name} forgot the most important rule of lumberjacking: step aside.",
        "A tree has fallen on {Player.name}, and yes, someone did hear it.",
        "{Player.name} has lost an argument with a tree. The tree was very persuasive and very heavy.",
        "The forest took its revenge on {Player.name}. One tree, one Viking, zero survivors.",
        "{Player.name} was flattened by a falling tree. Always check which way it leans.",
        "{Player.name} has learned that trees fall in one direction, and it was this one.",
        "Nature just had a heavy word with {Player.name}. The word was \"timber\".",
        "{Player.name} cut down a tree and the tree cut back. Fair is fair.",
        "The tree didn't want to be firewood. {Player.name} did not survive the disagreement.",
        "{Player.name} has been felled by the very thing they were felling. Poetic.",
        "{Player.name} was standing exactly where the tree wanted to land. Bad luck, worse planning.",
        "A falling tree has claimed {Player.name}. Please keep your distance from the next one.",
        "{Player.name} has been taught forestry the hard way. Lesson one: don't be under it.");

    /// <summary>Death by a cart. The first quote is the original one.</summary>
    internal static readonly QuotePool Cart = new(
        "{Player.name} got crushed by a cart.",
        "{Player.name} has been run over by a cart. A cart. Let that sink in.",
        "{Player.name} was flattened by a runaway cart. Valheim's finest logistics, doing their worst.",
        "The cart was only supposed to carry the cargo. {Player.name} was apparently also cargo.",
        "{Player.name} has lost a fight with a cart. The cart won on wheels alone.",
        "{Player.name} was crushed by a cart on a slope. Physics wins again.",
        "A cart rolled over {Player.name}. Please file this under \"transport incidents\".",
        "{Player.name} has been squashed by a cart. The cargo is fine, thanks for asking.",
        "Heavy load, steep hill, {Player.name} in the way. The cart did the rest.",
        "{Player.name} has been out-driven by a cart with nobody driving it.",
        "The cart did not slow down for {Player.name}, and it never will.",
        "{Player.name} has been crushed by a cart. Next time, park it on flat ground.",
        "A cart has claimed {Player.name}. The wheel has been reinvented, and it is deadly.",
        "{Player.name} was trampled by a cart. It's not even a horse, so the excuse is worse.",
        "{Player.name} stood in front of a cart full of stuff. The stuff won.",
        "{Player.name} has been defeated by the humble cart. Odin is reading the incident report.");

    /// <summary>Death by a boat. The first quote is the original one.</summary>
    internal static readonly QuotePool Boat = new(
        "{Player.name} got crushed by a boat.",
        "{Player.name} has been crushed by a boat. On land or at sea, it's still a boat.",
        "{Player.name} was flattened by a boat. The hull remains undefeated.",
        "A boat has claimed {Player.name}. Seafaring is dangerous even when you're not at sea.",
        "{Player.name} got between a boat and the rest of the world. The boat won.",
        "{Player.name} has been squashed by a longship. A noble way to go, a silly way to die.",
        "{Player.name} was crushed by a boat. The harbour master is writing a strongly worded letter.",
        "The sea has sent a boat to collect {Player.name}. Unexpectedly literal.",
        "{Player.name} has met the hull of a boat at full speed. The hull is fine.",
        "{Player.name} did not look both ways before crossing the water. Neither did the boat.",
        "{Player.name} has been boated. That's a verb now.",
        "A passing boat has flattened {Player.name}. No survivors, no apologies.",
        "{Player.name} has been undone by a vessel. Njord would be proud, and a little amused.",
        "{Player.name} was crushed by a boat. Lesson: the dock is not a safe place.",
        "{Player.name} has learned that boats are heavy. Very, very heavy.",
        "All aboard! Except {Player.name}, who was crushed by the boat instead.");

    /// <summary>Death by a collapsing structure. The first quote is the original one.</summary>
    internal static readonly QuotePool Structure = new(
        "{Player.name} got crushed by a collapsing structure.",
        "{Player.name} has been buried by a collapsing structure. The building inspector is on his way.",
        "{Player.name}'s own architecture has turned against them. Shoddy construction claims another Viking.",
        "The roof, the beams and the walls all came down on {Player.name}. Support beams matter.",
        "{Player.name} was crushed by a collapsing structure. Next time, check the structural integrity.",
        "{Player.name} has been undone by gravity and bad building choices.",
        "A building has fallen on {Player.name}. Somebody forgot a pillar.",
        "{Player.name} built it, trusted it, and was crushed by it. Classic.",
        "The structure was held together by hope. Hope has failed, and so has {Player.name}.",
        "{Player.name} has been flattened by falling timber and regret.",
        "{Player.name} pulled the wrong beam. The whole thing came down.",
        "{Player.name} has found the weakest link in their base. It was a support beam.",
        "{Player.name} was crushed when the building decided to become a pile.",
        "Home sweet collapsed home. {Player.name} did not make it out.",
        "{Player.name} has been crushed by a collapsing structure. This is why we use more supports.",
        "The structural engineers of Valheim would like a word with {Player.name}. Too late now.");

    /// <summary>Death by a falling stone spike in a cave. The first quote is the original one.</summary>
    internal static readonly QuotePool Stalagmite = new(
        "{Player.name} got crushed by a stalagmite.",
        "{Player.name} has been impaled by the ceiling. Caves are not friendly.",
        "The cave decided {Player.name} was in the way. A stone spike agreed.",
        "{Player.name} looked at the pretty cave and forgot to look up.",
        "Rocks fall, Vikings die. {Player.name} has been claimed by a stone spike.",
        "{Player.name} has been crushed by a cave formation that took thousands of years to be there at the right time.",
        "The cave has claimed {Player.name}. It had been planning this for a long time.",
        "{Player.name} has been defeated by geology. Slow, patient, deadly.",
        "{Player.name} was crushed by a falling rock spike. Wear a helmet in caves, they said.",
        "{Player.name} has discovered that cave ceilings are not decorative.",
        "Nature's pointy surprise has landed on {Player.name}.",
        "{Player.name} was minding their own business in a cave. The cave was not minding theirs.",
        "{Player.name} has been spiked by the underground. A rough welcome.",
        "A rock formation has ended {Player.name}. It was extremely patient.",
        "{Player.name} has been dropped on by a rock. The mountain says sorry. It does not mean it.",
        "{Player.name} has lost to geology. Everyone eventually does.");

    /// <summary>Death by a drawbridge. The first quote is the original one.</summary>
    internal static readonly QuotePool DrawBridge = new(
        "{Player.name} got crushed by a drawbridge.",
        "{Player.name} has been crushed by a drawbridge. Moving parts and Vikings do not mix.",
        "The drawbridge went up. {Player.name} did not go up with it.",
        "{Player.name} was crushed by a drawbridge. The castle defences work, even on the owner.",
        "{Player.name} has been squashed by a drawbridge's very effective security system.",
        "A drawbridge has claimed {Player.name}. Someone forgot to look before pulling the lever.",
        "{Player.name} has learned that \"raising the bridge\" is not just a figure of speech.",
        "{Player.name} was caught on the wrong side of a drawbridge. It was a close call, and not close enough.",
        "The drawbridge showed no mercy to {Player.name}. Engineering at its most honest.",
        "{Player.name} tried to cross while the bridge was closing. The bridge won.",
        "{Player.name} has been flattened by a drawbridge. Please use the side entrance next time.",
        "{Player.name} has been out-bridged. The bridge is fine.",
        "A drawbridge has ended {Player.name}. The moat is awfully quiet now.",
        "{Player.name} has met the underside of a drawbridge. It did not go well.",
        "The drawbridge came down on {Player.name}, literally and figuratively.",
        "{Player.name} was crushed by a drawbridge. At least the fortress is secure.");

    /// <summary>Death by a turret. The first quote is the original one.</summary>
    internal static readonly QuotePool Turret = new(
        "{Player.name} got crushed by a turret.",
        "{Player.name} has been crushed by a turret. The defence system has found a new target.",
        "{Player.name} was killed by a turret. Friendly fire was not on the safety card.",
        "A turret has claimed {Player.name}. It can't tell friend from foe, only target from not target.",
        "{Player.name} has been defeated by a turret. The automatic defence is working perfectly.",
        "{Player.name} has been out-shot by a piece of furniture with a bad attitude.",
        "{Player.name} stood in the line of fire of a turret. A bold choice, quickly corrected.",
        "The turret did exactly what turrets do. {Player.name} was in the way.",
        "{Player.name} has been bolted down by a turret. Literally.",
        "A turret has made a precise decision about {Player.name}. It was the wrong one.",
        "{Player.name} has been neutralised by a turret. Excellent aim, terrible timing.",
        "{Player.name} was crushed by a turret. The machine is perfectly calm about it.",
        "{Player.name} has been disarmed by a turret. Permanently.",
        "{Player.name} has met a turret's idea of hospitality. Not a good host.",
        "The turret has no regrets about {Player.name}. It only has bolts.",
        "{Player.name} has been defeated by their own defences. Don't build what you can't dodge.");

    /// <summary>Death by a catapult. The first quote is the original one.</summary>
    internal static readonly QuotePool Catapult = new(
        "{Player.name} got crushed by a catapult.",
        "{Player.name} has been flung into the afterlife by a catapult. Distance: considerable.",
        "A catapult has claimed {Player.name}. The siege is going well, for the wrong side.",
        "{Player.name} was crushed by a catapult. One boulder, one Viking, zero survivors.",
        "{Player.name} stood in the wrong place during a very good shot. Catapult: 1, Viking: 0.",
        "The catapult launched, {Player.name} stood still, and gravity did the rest.",
        "{Player.name} has been defeated by a catapult. Not even a castle in sight.",
        "{Player.name} was caught in the splash zone of a catapult. There was no splash. Just rock.",
        "A catapult has delivered a very heavy package to {Player.name}. Signature not required.",
        "{Player.name} has been squashed by siege equipment. Overkill for a single Viking.",
        "{Player.name} tested the catapult from the wrong end. The test was a success.",
        "{Player.name} has been catapulted into Valhalla, in a way. Mostly the dying part.",
        "The catapult has hit its target. Unfortunately, the target was {Player.name}.",
        "{Player.name} has met a catapult at point-blank range. Aim was not the problem.",
        "{Player.name} was crushed by a catapult. Please keep your siege equipment pointed away from your friends.",
        "{Player.name} has been out-ranged by a catapult. Everyone is.");

    /// <summary>Death by an impact. The first quote is the original one.</summary>
    internal static readonly QuotePool Impact = new(
        "{Player.name} got crushed by an impact.",
        "{Player.name} has been crushed by an impact. Something very heavy met something very squishy.",
        "{Player.name} has been hit by something. It was solid. It was fast. It was enough.",
        "{Player.name} was flattened by an impact. The cause is unclear. The result is not.",
        "A great force has met {Player.name}. The force won.",
        "{Player.name} has taken a very heavy hit from the world itself. The world does not apologise.",
        "{Player.name} has been undone by a sudden impact. Even Valheim's physics has a temper.",
        "Something hit {Player.name} hard enough to end the story. Ow.",
        "{Player.name} has been struck down by blunt physics.",
        "{Player.name} has been crushed by a very solid thing. Details are unavailable.",
        "{Player.name} was in the wrong place for a collision. The collision was on time.",
        "{Player.name} has been bonked by physics. The bonk was fatal.",
        "{Player.name} has been defeated by momentum. It never loses.",
        "{Player.name} was hit by something heavy and fast. Odin has questions.",
        "Impact detected. {Player.name} not detected.",
        "{Player.name} has met an unstoppable force. {Player.name} was not an immovable object.");
}
