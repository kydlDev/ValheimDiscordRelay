namespace ValheimDiscordRelay.Client;

/// <summary>
/// The deaths that have a quote list of their own instead of the general environmental one (see <see cref="DeathQuoteLists"/>).
/// The game's hit type is turned into one of these in DeathMessageBuilder, so the choice of list can be tested without the game.
/// </summary>
internal enum DeathQuoteKind
{
    /// <summary>Crushed by a falling tree.</summary>
    Tree,

    /// <summary>Crushed by a cart.</summary>
    Cart,

    /// <summary>Crushed by a boat.</summary>
    Boat,

    /// <summary>Crushed by a collapsing structure.</summary>
    Structure,

    /// <summary>Crushed by a falling stone spike in a cave.</summary>
    Stalagmite,

    /// <summary>Crushed by a drawbridge.</summary>
    DrawBridge,

    /// <summary>Crushed by a turret.</summary>
    Turret,

    /// <summary>Crushed by a catapult.</summary>
    Catapult,

    /// <summary>Crushed by an impact.</summary>
    Impact,

    /// <summary>Drowned or died while swimming in ordinary water.</summary>
    Swimming,

    /// <summary>Died in the boiling Ashlands sea.</summary>
    AshlandsSea
}
