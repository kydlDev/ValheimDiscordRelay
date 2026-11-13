using System;
using KydlCore.Common;
using UnityEngine;
using ValheimDiscordRelay.Client;

namespace ValheimDiscordRelay.Testing;

/// <summary>
/// Hooks of the death TestLab suites: they kill the local player (or a boss) through the game's own damage code, so the mod's real
/// patches, death detection and message builder run exactly as in a normal game.
/// </summary>
internal static class DeathTestHooks
{
    private static GameObject _spawned;

    /// <summary>Instance id of the player that was just struck: the next player counts as alive only when it is a different object (the respawn).</summary>
    private static int _struckId;

    private static float _readyAt;
    private static float _struckAt;

    /// <summary>Spawns a creature of this level next to the player and lets it deal the killing hit. Returns "PASS: ..." or "FAIL: ...".</summary>
    [TestHook("valheimdiscordrelay.killPlayerBy")]
    static string KillPlayerBy(string prefab, int level)
    {
        Player player = Player.m_localPlayer;
        if (player == null || player.IsDead()) return "FAIL: no living local player";

        Character creature = Spawn(prefab, level, player.transform.position + player.transform.forward * 3f);
        if (creature == null) return "FAIL: could not spawn " + prefab;

        HitData hit = new HitData { m_attacker = creature.GetZDOID(), m_hitType = HitData.HitType.EnemyHit };
        return Strike(player, hit, "killed by " + prefab + " level " + level);
    }

    /// <summary>The killing hit has no attacker and this hit type (Fall, Drowning, AshlandsOcean, Tree...).</summary>
    [TestHook("valheimdiscordrelay.killPlayerEnv")]
    static string KillPlayerEnv(string hitType)
    {
        Player player = Player.m_localPlayer;
        if (player == null || player.IsDead()) return "FAIL: no living local player";

        HitData.HitType type;
        try { type = (HitData.HitType)Enum.Parse(typeof(HitData.HitType), hitType); }
        catch (ArgumentException) { return "FAIL: unknown hit type " + hitType; }

        return Strike(player, new HitData { m_hitType = type }, "killed by " + hitType);
    }

    /// <summary>Spawns a boss and lets the local player kill it with one hit (the boss tracker sees the player as the attacker).</summary>
    [TestHook("valheimdiscordrelay.killBoss")]
    static string KillBoss(string prefab)
    {
        Player player = Player.m_localPlayer;
        if (player == null || player.IsDead()) return "FAIL: no living local player";

        Character boss = Spawn(prefab, 1, player.transform.position + player.transform.forward * 6f);
        if (boss == null) return "FAIL: could not spawn " + prefab;

        HitData hit = new HitData { m_attacker = player.GetZDOID(), m_hitType = HitData.HitType.PlayerHit, m_point = boss.GetCenterPoint() };
        hit.m_damage.m_slash = 1000000f;
        boss.Damage(hit);
        // The damage is applied a frame later, so the test checks the log instead.
        return "PASS: hit sent to " + prefab + " (health " + boss.GetHealth() + ")";
    }

    /// <summary>Lifts the player this many metres into the air with the god mode off, so the real fall damage can kill.</summary>
    [TestHook("valheimdiscordrelay.dropPlayer")]
    static string DropPlayer(float meters)
    {
        Player player = Player.m_localPlayer;
        if (player == null || player.IsDead()) return "FAIL: no living local player";

        player.SetGodMode(false);
        StopFlying(player);
        player.SetHealth(player.GetMaxHealth());
        MarkStruck(player);
        player.transform.position += Vector3.up * meters;
        return "PASS: lifted by " + meters + " m";
    }

    /// <summary>True when a living local player exists (after the respawn).</summary>
    [TestHook("valheimdiscordrelay.playerAlive")]
    static bool PlayerAlive()
    {
        Player player = Player.m_localPlayer;
        if (player == null) return false;

        if (_struckId != 0)
        {
            if (player.GetInstanceID() == _struckId) return false;
            _struckId = 0;
            _readyAt = 0f;
        }

        bool ready = !player.IsDead() && !player.IsTeleporting() && !player.InCutscene() && Time.unscaledTime >= _readyAt;
        if (!ready && Time.unscaledTime >= _nextStateLog)
        {
            _nextStateLog = Time.unscaledTime + 5f;
            ModLog.Info("[Test] player not ready: dead=" + player.IsDead() + " teleporting=" + player.IsTeleporting() +
                        " cutscene=" + player.InCutscene() + " waitSeconds=" + (_readyAt - Time.unscaledTime).ToString("0.0"));
        }

        return ready;
    }

    private static float _nextStateLog;

    /// <summary>Removes the creature spawned by the last kill hook and gives the player the god mode back.</summary>
    [TestHook("valheimdiscordrelay.cleanup")]
    static string Cleanup()
    {
        if (_spawned != null && ZNetScene.instance != null) ZNetScene.instance.Destroy(_spawned);
        _spawned = null;
        if (Player.m_localPlayer != null) Player.m_localPlayer.SetGodMode(true);
        return "ok";
    }

    /// <summary>How many deaths the mod has reported in this game.</summary>
    [TestHook("valheimdiscordrelay.deathCount")]
    static int DeathCount() { return ClientCaptureService.DeathCount; }

    /// <summary>The last reported death: "killer=Boar;level=2;boss=False;cause=;text=...".</summary>
    [TestHook("valheimdiscordrelay.lastDeath")]
    static string LastDeath() { return ClientCaptureService.LastDeathSummary; }

    /// <summary>The lab starts a respawned player in the debug fly mode, where the game ignores every hit: land before the strike.</summary>
    private static void StopFlying(Player player)
    {
        if (player.IsDebugFlying()) player.ToggleDebugFly();
    }

    private static void MarkStruck(Player player)
    {
        _struckId = player.GetInstanceID();
        _struckAt = Time.unscaledTime;
    }

    private static Character Spawn(string prefab, int level, Vector3 position)
    {
        GameObject prefabObject = ZNetScene.instance == null ? null : ZNetScene.instance.GetPrefab(prefab);
        if (prefabObject == null) return null;

        _spawned = UnityEngine.Object.Instantiate(prefabObject, position, Quaternion.identity);
        Character character = _spawned.GetComponent<Character>();
        if (character != null && level > 1) character.SetLevel(level);
        return character;
    }

    private static string Strike(Player player, HitData hit, string what)
    {
        hit.m_damage.m_damage = 1000000f;
        hit.m_point = player.GetCenterPoint();
        hit.m_dir = player.transform.forward;

        string before = "god=" + player.InGodMode() + " teleporting=" + player.IsTeleporting() + " cutscene=" + player.InCutscene() + " health=" + player.GetHealth();
        player.SetGodMode(false);
        StopFlying(player);
        player.Damage(hit);
        string effects = string.Join(",", player.GetSEMan().GetStatusEffects().ConvertAll(e => e.name).ToArray());
        ModLog.Info("[Test] strike (" + what + "): before " + before + ", after health=" + player.GetHealth() + " dead=" + player.IsDead() +
                    " godNow=" + player.InGodMode() + " ghost=" + player.InGhostMode() + " debugFly=" + player.IsDebugFlying() +
                    " owner=" + player.IsOwner() + " attacker=" + (hit.GetAttacker() != null ? hit.GetAttacker().name : "none") + " effects=" + effects);

        // The game applies a hit through a routed RPC, so the death may only follow a frame later: the test waits for the respawn
        // (playerAlive) and then checks that the mod reported the death.
        MarkStruck(player);
        return "PASS: " + what + " (hit sent, health " + player.GetHealth() + ")";
    }
}
