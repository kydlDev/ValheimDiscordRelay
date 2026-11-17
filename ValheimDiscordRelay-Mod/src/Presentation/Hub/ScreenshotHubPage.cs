using System;
using BepInEx.Configuration;
using KydlCore;
using KydlCore.Features.Hub.Contracts;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// The "Take a screenshot now" panel at the TOP of the mod's "Screenshots" tab in the KY hub (F6), above the [Screenshots] config entries
/// (user, 2026-11-17: there were two Screenshots tabs, the buttons and the config category; now it is one tab). One button per kind of screenshot. A button closes the hub, waits for it
/// to disappear and then takes the screenshot exactly like its key does.
/// </summary>
internal static class ScreenshotHubPage
{
    private const string SettingsTabId = "settings";
    /// <summary>The config category whose F6 tab gets the panel (the tab is named like the [Screenshots] section).</summary>
    internal const string ScreenshotsGroup = "Screenshots";

    /// <summary>How long after closing the hub the capture starts, so the window is not in the picture.</summary>
    private const float HubCloseDelaySeconds = 0.5f;

    /// <summary>
    /// Registers the Settings tab (KydlCore splits it into one tab per config category) and the screenshot panel at the top of its
    /// Screenshots tab.
    /// </summary>
    internal static void Register(string modGuid, ConfigFile config, ClientCaptureService service)
    {
        KyHub.AddTab(modGuid, SettingsTabId, "Settings", () => KyHub.ConfigEntries(config), 0);
        bool panel = KyHub.SetGroupPage(modGuid, ScreenshotsGroup, () => Build(service), true);
        RelayDiagnostics.Debug("[Screenshot] Registered the Settings tab and the screenshot panel at the top of the Screenshots tab in the KY hub (" + (panel ? "ok" : "FAILED") + ").");
    }

    private static KyPage Build(ClientCaptureService service)
    {
        bool normal = ClientConfig.ScreenshotKindActive(ScreenshotMode.Normal);
        bool hud = ClientConfig.ScreenshotKindActive(ScreenshotMode.HudHidden);
        bool both = ClientConfig.ScreenshotKindActive(ScreenshotMode.HudAndPlayerHidden);
        string normalKey = KeyText(ClientConfig.ScreenshotKey.Value);
        string hudKey = KeyText(ClientConfig.ScreenshotHudHiddenKey.Value);
        string bothKey = KeyText(ClientConfig.ScreenshotHudPlayerHiddenKey.Value);

        KyPage page = new KyPage { State = normal + "|" + hud + "|" + both + "|" + normalKey + "|" + hudKey + "|" + bothKey };
        KySection section = page.Section("Take a screenshot now");

        if (!normal && !hud && !both)
        {
            section.Note("No screenshot kind is available: screenshots are switched off (the server's [Screenshots] Enable, or your own [Screenshots] Client - Enabled and the three Client - ... Enabled switches), or no webhook is set.");
            return page;
        }

        section.Note("Each button closes this window and sends the screenshot to Discord, the same as pressing its key. A kind you switched off, or that has no webhook, has no button.");

        if (normal)
            section.Button("Screenshot with HUD (" + normalKey + ")", () => Press(service, ScreenshotMode.Normal));

        if (hud)
            section.Button("Screenshot without HUD (" + hudKey + ")", () => Press(service, ScreenshotMode.HudHidden));

        if (both)
            section.Button("Screenshot without HUD and without your character (" + bothKey + ")", () => Press(service, ScreenshotMode.HudAndPlayerHidden));

        return page;
    }

    /// <summary>How many buttons the page shows right now (read by the TestLab hooks).</summary>
    internal static int VisibleButtonCount(ClientCaptureService service)
    {
        int count = 0;
        foreach (KySection section in Build(service).Sections)
        {
            foreach (KyRow row in section.Rows)
            {
                if (row is KyButtonRow)
                    count++;
            }
        }

        return count;
    }
    /// <summary>What a button does: closes the hub, then takes the screenshot after a short delay. Also called by the TestLab hook.</summary>
    internal static void Press(ClientCaptureService service, ScreenshotMode mode)
    {
        try
        {
            KyHub.Close();
            service.RequestScreenshot(mode, HubCloseDelaySeconds);
        }
        catch (Exception ex)
        {
            RelayDiagnostics.Error("Screenshot button (" + mode + ") failed.", ex);
        }
    }

    private static string KeyText(KeyboardShortcut shortcut)
    {
        return shortcut.MainKey == UnityEngine.KeyCode.None ? "no key" : shortcut.ToString();
    }
}
