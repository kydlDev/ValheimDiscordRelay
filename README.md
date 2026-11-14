# Valheim Discord Relay

One BepInEx plugin for Valheim that connects your world to Discord through webhooks. It relays chat, announces deaths, boss kills, players joining and leaving and server restarts, posts a weekly statistics report, and lets players send screenshots and short video clips with one key press.

The plugin is a single DLL. It contains a server part and a client part; each side activates only where it is needed.

## Contents

- [What it can do](#what-it-can-do)
- [Requirements](#requirements)
- [Installation and updating](#installation-and-updating)
- [Quick start](#quick-start)
- [Configuration](#configuration)
- [Features in detail](#features-in-detail)
  - [Chat relay](#chat-relay)
  - [Manual screenshots](#manual-screenshots)
  - [Clips](#clips)
  - [Death messages](#death-messages)
  - [Death videos and the death camera](#death-videos-and-the-death-camera)
  - [Boss death announcements](#boss-death-announcements)
  - [Server notifications](#server-notifications)
  - [Player notifications](#player-notifications)
  - [Weekly report (Discord)](#weekly-report-discord)
  - [In-game weekly report window](#in-game-weekly-report-window)
  - [Version check](#version-check)
- [Files the mod creates](#files-the-mod-creates)
- [Security and privacy](#security-and-privacy)
- [Logs and troubleshooting](#logs-and-troubleshooting)
- [Credits and licence](#credits-and-licence)

## What it can do

| Feature | In one line |
| --- | --- |
| **Chat relay** | Normal chat and shouts go to Discord, each with its own webhook, shown under the player's name. |
| **Death messages** | A randomly chosen, cause-specific message (317 in total) is shouted in game and posted to Discord when a player dies, with or without a video. |
| **Death videos** | A short animated WebP of the player's death (seconds before and after) is posted with the message, ending on a cinematic death camera. |
| **Boss announcements** | An embed with the boss's portrait and the top damage dealer, plus an optional short video of the boss dying, and a line in the in-game chat. |
| **Clips** | Press a key and the last 4 and next 4 seconds are posted as a short video. |
| **Screenshots** | Press a key and the current game screen is posted, with automatic black-screen protection. |
| **Server notifications** | Server is up / restarted / going down. |
| **Player notifications** | "Has joined the world" / "had to leave us", with the current population. |
| **Weekly report** | Once a week, one Discord message per tab: players, bosses, mobs killed, killer mobs, death causes and highest hits, with a top 5 for the server and for every player. |
| **In-game report window** | Press a key to see the same numbers as they stand right now, in a resizable, sortable, CSS-styleable window. |
| **Version check** | The server can refuse players whose copy of the mod is missing or is not exactly the server's version. |
| **Admin controls** | Webhooks and settings are set once on the server and synced to players; settings can be locked to admins. Every feature has its own switch. |

The mod has no chat or console commands; everything is configuration, hotkeys and automatic events.

## Requirements

- Valheim 1.0.12
- BepInEx 5.4.2351 (BepInEx 5.4.23.5)
- [Jotunn](https://thunderstore.io/c/valheim/p/ValheimModding/Jotunn/) and **KydlCore** (the KydlDev shared runtime: the KY hub window, the Settings tab and the shared log). Both are installed with the mod by a mod manager, and are needed on the dedicated server as well.
- Install the mod on the dedicated server **and on every player's game, all with exactly the same version.** The server refuses players who have no mod, an older version or a newer version (see [Version check](#version-check)).
- Worlds hosted from Valheim's own menu (no dedicated server) work too: install the mod on the host and set the webhook URLs in the host's config. The same version check applies to the players who join the host.
- Death videos, boss videos and clips are encoded with a bundled **Windows program** (libwebp's `img2webp.exe`). A game that cannot run it still sends the death and boss messages, without the video; a clip then fails with a notice in game.

## Installation and updating

Install the Thunderstore/Hexium package, or place `ValheimDiscordRelay.dll` in `BepInEx/plugins/ValheimDiscordRelay/`. Do this on the server and on every client.

**Updating from 1.x:** version 2.0.0 is one plugin with a new id (`com.kydldev.ValheimDiscordRelay`) and one new config file, `BepInEx/config/com.kydldev.ValheimDiscordRelay.cfg`. The old `ValheimDiscordRelay.cfg` is no longer read: remove it, start the game once so the new file is written, then fill in your webhooks again (or paste them into the KY hub, see below). Each section's `Enable` switch is now simply `Enable`, and the `[Admin]` section is now `[General]`. Remove the old 1.x plugin file so only one copy loads.

## Quick start

1. In Discord, create a webhook for each feature you want (Channel settings → Integrations → Webhooks). You may use one channel for everything, but see [Security and privacy](#security-and-privacy) for the webhooks that players' games can read.
2. Start the dedicated server once with the mod installed so `BepInEx/config/com.kydldev.ValheimDiscordRelay.cfg` is created.
3. Stop the server, open the file and paste each webhook URL into its `Server - Webhook URL` setting (see [Configuration](#configuration)). A feature whose webhook is empty sends nothing.
4. Start the server. Players who join with the same mod version receive the shared webhooks and settings automatically; they do not enter webhooks themselves.
5. Optional: set `[General] Server - Log Successful Sends = true` for a while to see every delivery in the log.

Everything is enabled by default; with no webhook set, a feature simply logs a warning and does nothing.

## Configuration

Everything is in one file: `BepInEx/config/com.kydldev.ValheimDiscordRelay.cfg`. Every entry is also on the **Settings tab of this mod in the KY hub (F6)** and in Configuration Manager (F1). Admins can change the server's values there while connected, and the change is saved on the server. BepInEx writes a description above every setting.

Setting names tell you who they belong to:

- **`Server - ...`** — set by the server admin. Either **synced** to every player, or **server only** (used by the server and never sent anywhere).
- **`Client - ...`** — each player's own setting, never synced.

Every category has an `Enable` switch (admin, affects everyone) and most have a `Client - Enabled` switch (the player opting out). **A feature that has both runs only when both are on.** `[General] EnableAll` (on by default) is the master switch: while it is off the whole mod is inert on that machine, and the same is true of a category whose `Enable` is off.

### What is synced

The mod uses ServerSync for the shared settings and a small admin-only channel for the server-only ones. Players do not enter webhooks; the admin sets them in the **server's** config, in the file or live in the KY hub (F6).

- Webhooks the **players' games upload to** (screenshots, clips, death videos and messages, boss videos) are synced to every client. This is needed because each game posts its own file straight to Discord. It means **these URLs are not secret**; see [Security and privacy](#security-and-privacy).
- Webhooks **only the server uses** (chat, server notifications, player notifications, weekly report) stay on the server and are never sent to ordinary players. An admin's game fetches them over a separate admin-only channel (the server checks `adminlist.txt` on every request), so the admin can read and change them in the KY hub; nobody else receives them.
- Settings marked *synced* below are sent to players and can only be changed by admins while `[General] Server - Lock Configuration` is on; other players see them locked in the KY hub.

### `[General]`

| Setting | Default | |
| --- | --- | --- |
| `EnableAll` | `true` | Master switch of this mod on this machine. Off = nothing is relayed, captured or measured. |
| `Server - Lock Configuration` | `true` | Synced. Only players in the server's `adminlist.txt` can change synced settings from their game. If `false`, **any** connected player can change them, webhook URLs included. Leave it on for a public server. |
| `Server - Require Same Version` | `true` | Server only. The server refuses players who do not have exactly the server's version of this mod, including players without it. See [Version check](#version-check). |
| `Server - Log Successful Sends` | `false` | Synced. Log every successful Discord delivery (the server logs its own, each player's game logs its own uploads). |

### `[Chat]`

| Setting | Default | |
| --- | --- | --- |
| `Enable` | `true` | Synced. Enable the chat relay. |
| `Server - Webhook URL` | empty | Server only. Fallback, used when the Normal / Shout webhook is empty. |
| `Server - Normal Webhook URL`, `Server - Shout Webhook URL` | empty | Server only. Webhooks for normal chat and for shouts. |
| `Server - Max Message Length` | `1800` | Synced. Longest text sent to Discord (100–1900). |
| `Server - Name Display` | `NameOnly` | Synced. `NameOnly` (`Bjorn`), `NameWithNumber` (`Bjorn [1]`, for players with the same name) or `NameWithIdSuffix` (`Bjorn [5678]`, last 4 characters of the platform ID). |
| `Server - Shout Prefix`, `Server - Normal Prefix` | dim red / dim cyan | Synced. ANSI colour codes placed before a shout / normal chat. Leave them alone unless you want other colours. |
| `Server - Queue Limit` | `100` | Synced. Most queued messages; the oldest are dropped when full (10–1000). The weekly report's messages are not counted and are never dropped by this limit. |
| `Server - Minimum Send Interval Ms` | `250` | Synced. Pause between webhook requests (50–5000), to stay under Discord's rate limits. |
| `Client - Enabled` | `true` | Send the chat typed in this game to the server. Turn off to keep your chat out of Discord. |
| `Client - Max Message Length` | `1000` | Most characters sent from this game (1–1000). The server applies its own limit too. |

### `[Screenshots]`

| Setting | Default | |
| --- | --- | --- |
| `Enable` | `true` | Synced. Turn off to disable screenshots for every player. |
| `Server - Webhook URL` | empty | Synced. The GLOBAL screenshot webhook: normal screenshots go here, and so do the other two kinds when their own webhook is empty. |
| `Server - HUD Hidden Webhook URL` | empty | Synced. Webhook of the screenshots without HUD. Empty = the global webhook. |
| `Server - HUD And Player Hidden Webhook URL` | empty | Synced. Webhook of the screenshots without HUD and without the player. Empty = the global webhook. |
| `Client - Enabled` | `true` | Allow screenshots in this game: off = none of the three kinds works. |
| `Client - Normal Enabled` | `true` | Allow the normal screenshot. Off = its key does nothing and its F6 button is hidden. |
| `Client - HUD Hidden Enabled` | `true` | Allow the screenshot without HUD (same). |
| `Client - HUD And Player Hidden Enabled` | `true` | Allow the screenshot without HUD and player (same). |
| `Client - Key` | `PageUp` | Normal screenshot (HUD on). `None` disables it. Works while other keys (walking, Shift) are held. |
| `Client - HUD Hidden Key` | `P` | Screenshot without HUD (like Ctrl+F3). `None` disables it. Not while typing. |
| `Client - HUD And Player Hidden Key` | `Alt + P` | Screenshot without HUD and without your own character. `None` disables it. Not while typing. |
| `Client - Include Player Name` | `true` | Use your character name as the Discord sender (otherwise `Valheim Player`). |

### `[Clips]`

| Setting | Default | |
| --- | --- | --- |
| `Enable` | `true` | Synced. Turn off to disable clips for every player. |
| `Server - Webhook URL` | empty | Synced. The clips webhook. |
| `Client - Enabled` | `true` | Allow the clip key in this game. |
| `Client - Key` | `PageDown` | `None` disables it. For two keys together write both, e.g. `F9 + LeftControl`: hold both, in any order; other keys held at the same time are ignored. |
| `Client - Include Player Name` | `true` | Add the line `<name> saved a clip.` to the message that carries the clip. The Discord sender is always `ValheimRelay Clips`. |
| `Client - Pre Duration`, `Client - Post Duration` | `4`, `4` | Seconds before / after the key press (2–4 each). |

Clips use the frame rate and resolution of `[Player Deaths] Client - Video FPS` and `Client - Video Resolution`.

### `[Player Deaths]`

| Setting | Default | |
| --- | --- | --- |
| `Enable` | `true` | Synced. Turn off to disable death messages, shouts and videos for every player. |
| `Server - Webhook URL` | empty | Synced. Webhook for death messages and videos. |
| `Client - Enabled` | `true` | Announce this player's deaths (in-game shout and Discord message). |
| `Client - Video Enabled` | `true` | Record a death video. **Turn it off to send only the death message, with no video** (for players who cannot or do not want to record). It also keeps the frame buffer that boss videos and clips reuse. |
| `Client - Video FPS` | `30` | `20` or `30`. Use `20` if your game runs slower than 30 FPS. |
| `Client - Video Resolution` | `960x540` | `480x270`, `640x360` or `960x540`. |
| `Client - Video Pre Duration`, `Client - Video Post Duration` | `4`, `4` | Seconds before / after the death (2–4 each). |
| `Client - Death Camera` | `true` | After you die, raise the camera and look down at your body. It also moves your own view for those seconds. |
| `Client - Death Camera Height` | `6` | Metres the camera rises (1–15). |
| `Client - Death Camera Look At Killer` | `true` | Turn the view towards whatever killed you, without losing your body from view. |

### `[Boss Death]`

| Setting | Default | |
| --- | --- | --- |
| `Enable` | `true` | Synced. Announce boss deaths. |
| `Server - Webhook URL` | empty | Synced. Used for both the embed and the video. |
| `Server - Webhook Username` | `Boss Death Announcement!` | Synced. Sender name of the embed and the video. |
| `Server - Video Source` | `Boss Owner` | Synced. Whose game records the boss video: `Boss Owner` or `Last Attacker`. |
| `Client - Enabled` | `true` | Take part in boss announcements from this game (damage measuring and video). |
| `Client - Game Chat Message` | `true` | Say the boss death in the in-game chat, even with no webhook set (only the game that owns the boss sends it). |
| `Client - Video Enabled` | `true` | Upload the boss video from this game. Turn it off to post only the "has fallen" message, with no video (for PCs that cannot or should not record). |
| `Client - Video FPS`, `Client - Video Resolution` | `30`, `960x540` | Only used when `[Player Deaths] Client - Video Enabled` is off; otherwise the boss video reuses the death video's frames. |
| `Client - Video Pre Duration`, `Client - Video Post Duration` | `4`, `4` | Seconds before / after the boss dies (2–4 each). |

### `[Server Notifications]`

| Setting | Default | |
| --- | --- | --- |
| `Enable` | `true` | Admin only. Enable the feature. |
| `Server - Webhook URL` | empty | Server only. |
| `Server - Use Discord Webhook Name` | `false` | Synced. Off: messages are sent as **Server notifications**. On: Discord shows the name set on the webhook itself. |
| `Server - Up Notification`, `Server - Down Notification`, `Server - Restart Notification` | `true` | Server only. Turn each message on or off. |
| `Server - Restart Window Seconds` | `120` | Server only. A startup within this many seconds of a clean shutdown counts as a restart (10–3600). |

### `[Player Notifications]`

| Setting | Default | |
| --- | --- | --- |
| `Enable` | `true` | Synced. On: the mod posts its own join / leave messages and Valheim's "I have arrived!" shout is not relayed. Off: nothing is posted here, and the arrival shout is relayed as a normal shout. |
| `Server - Webhook URL` | empty | Server only. |
| `Server - Arrival Avatar URL`, `Server - Leave Avatar URL` | empty | Synced. Optional `http(s)` image URLs used as the webhook icon for join / leave messages. Anything else is ignored with one warning. |

### `[Weekly Report]`

| Setting | Default | |
| --- | --- | --- |
| `Enable` | `true` | Synced. Collect the statistics and post the report. Turning it off also stops games from collecting. |
| `Server - Webhook URL` | empty | Server only. |
| `Server - Report Day` | `Sunday` | Synced. Day the report is posted. |
| `Server - Report Time` | `20:00` | Synced. `HH:mm`, in the **server's** time zone. An invalid value falls back to `20:00` with a warning. |
| `Server - Send Test Report` | `false` | Synced. Set to `true` to post a "(test)" report right now. It resets itself to `false` and does not clear the statistics. It also fires once at server start if it was left `true`. |
| `Server - Window Enabled` | `true` | Synced. Let players open the in-game report window. Does not affect the Discord report or what is collected. |
| `Client - Enabled` | `true` | Include this player in the report (this game measures your kills and damage and reports your deaths). |
| `Client - Window Key` | `Home` | Opens and closes the window; `None` disables it. Two keys together are allowed, e.g. `F8 + LeftControl`. |
| `Client - Window Style` | `Classic` | Look of the window: `Classic` (the original flat look) or `Modern` (rounded cards, pill tabs, gold primary button, key hints, Valheim's rune font for the title). Only the look changes. Applies the next time the window opens. |
| `Client - Window Scale` | `0` | `0` follows your screen height (normal size at 1080p, larger on taller screens), or `0.7`–`2.0` to set it yourself. |
| `Client - Window Width`, `Client - Window Height` | `0`, `0` | The size you gave the window by dragging its bottom-right corner. Saved automatically, so there is normally no need to edit them. `0` = the default size. |

## Features in detail

### Chat relay

Normal chat and shouts are relayed under the player's character name; **whispers never are**. The server works out who sent a message, so no Discord account linking is needed.

- Each player can send a burst of 10 lines and then about 1 per second; extra lines are not relayed.
- Text longer than `Server - Max Message Length` is cut, and nothing the mod posts can ping anyone.
- Valheim's own "I have arrived!" shout is dropped while `[Player Notifications]` is on, and the mod's death shouts are never relayed to the chat webhook.

### Manual screenshots

- `PageUp`: a screenshot of the screen as you see it.
- `P`: the same without the HUD. `Alt` + `P`: without the HUD and without your own character.
- The **Screenshots** tab of the F6 hub has one button per kind.

Each kind can have its own webhook, otherwise the global `[Screenshots] Server - Webhook URL` is used. **A black screenshot is retried automatically** with other capture methods; the one that works on your PC is remembered.

### Clips

Press the clip key (`PageDown` by default) to save the 4 seconds before and the 4 seconds after the key press (2–4 s each). The clip is uploaded straight from your game as **ValheimRelay Clips** with the line `<your name> saved a clip.`, and a short notice at the top left says when it was sent.

- Clips are cut from the same in-memory frame buffer as death videos, so they work even with death videos off, and *Pre Duration* cannot be more than 4 seconds.
- One clip at a time; a key press while a clip is still being made is ignored. Quitting the game cancels it.

### Death messages

When the local player dies, the game works out what killed them from Valheim's hit data, picks a message for that kind of death and:

1. **shouts it in game chat** 5 seconds after the death (so it never ends up in the video), and
2. **sends it to Discord** through `[Player Deaths] Server - Webhook URL`, in a code block, together with the death video. With `Client - Video Enabled` off, only the message is sent (for players who cannot or do not want to record).

Both need `[Player Deaths] Enable` and `Client - Enabled` on. Without a webhook the shout still happens and the log says why nothing went to Discord. Logging out or quitting is never counted as a death.

**Which message.** Every kind of death has its own list, and one line is picked at random. **A line is never used again within the last 7 deaths of the same kind** (for a list shorter than 8 lines, as late as possible).

| Kind of death | Lines |
| --- | --- |
| Killed by an animal, no star | 25 |
| Killed by an animal, one star | 25 |
| Killed by an animal, two or more stars | 25 |
| Killed by another enemy | 26 |
| Killed by a boss or special enemy (names with a colour tag, such as Geirrhafa; the name is shown in red in Discord) | 16 |
| Environmental death (burning, freezing, poison, smoke, lava, falling, the edge of the world and so on; the line names the cause) | 26 |
| Crushed: falling tree, cart, boat, collapsing structure, stalagmite, drawbridge, turret, catapult, impact | 16 each (9 lists) |
| Drowned while swimming | 15 |
| Boiled in the Ashlands sea | 15 |

**Unknown causes** use Valheim's own death text, or `got killed by Unknown.`

**Good to know.**

- The killer is named correctly even if it dies before you do. Burning and poison deaths are credited to the creature that hit you shortly before, and a death caused by another player names that player.
- Creature stars are shown as emoji in Discord and as `(2 stars)` in game chat.
- Dying again within 30 seconds sends the message without a new video, and quitting while a video is still being made sends the message at once.

### Death videos and the death camera

When the local player dies, the game records the configured time before and after the death and uploads it as an animated WebP straight from the client, together with the death message.

- Frames are kept in memory (the last ~4 seconds); disk is only used briefly while a video is encoded.
- The quality is lowered automatically until the file fits Discord's size limit. If no video can be made, only the message is sent.
- Encoding runs in the background at low priority and uses at most a quarter of your CPU cores. Failed uploads are retried with the same file.
- **Death camera:** with `Client - Death Camera` on, the camera rises and looks down at your body, and with `Look At Killer` also turns towards whatever killed you. It moves your own view as well as the video.

### Boss death announcements

When a boss dies, the server posts an embed to the boss webhook, and a player's game uploads a short video of the moment as a separate message. With `[Boss Death] Client - Video Enabled` off on the recording game, the embed is posted on its own, without a video.

**The embed** has the boss's portrait, the title `{Boss} has fallen!`, and a random quote naming the top damage dealer and their damage: 26 lines for a party kill and 26 for a solo kill, never repeated within the last 7 kills. With no recorded damage it says `{Boss} has been defeated.`

**In-game chat.** With `[Boss Death] Client - Game Chat Message` on (default), the game that owns the boss also shouts a short line in the game chat (5 lines, for example `Eikthyr has fallen!`). This needs no webhook.

**The video** is recorded by one game, chosen by `Server - Video Source`: `Boss Owner` (default, the game that owns the boss) or `Last Attacker` (the player who landed the last hit, using that player's own video settings; if that game cannot record, the boss owner's video is used). When a video is expected, the embed waits for it, at most 75 seconds.

- **Damage** is measured by the game that owns the boss, so that player needs the mod.
- **Bosses recognised:** Eikthyr, The Elder, Bonemass, Moder, Yagluth, The Queen, Fader and Kall Fimbulbringer (announced once, on the final phase). Any other boss is announced with its in-game name and no images.
- **Announced once:** a boss death reported again, for example after a restart, is not announced twice.
- The portraits are hosted on third-party image sites that Discord fetches; if one is down, the embed is posted without that picture.

### Server notifications

`[Server Notifications]` posts `Server **My World** is up.`, `... restarted.` or `... is going down.` A start within `Server - Restart Window Seconds` of a clean shutdown counts as a restart.

### Player notifications

`[Player Notifications]` posts when someone joins or leaves, with the current population:

```text
{Player.name} has joined the world. Population: {players.count}. Good luck!
{Player.name} had to leave us. Hope they had fun. Population: {players.count}.
```

There is exactly one join per connection and one leave per disconnect; nothing is posted on a respawn.

### Weekly report (Discord)

`[Weekly Report]` posts the report once a week through its own webhook, **one Discord message per tab**:

| Tab | What it shows |
| --- | --- |
| **Players** | Players joined, bosses defeated, highest damage, and per player: mobs killed, bosses killed, deaths, total damage. |
| **Bosses** | One block per boss kill: killing blow, highest hit received, players it killed, and when. |
| **Mobs killed** | Top 5 mobs killed on the server, then per player. |
| **Killer mobs** | Top 5 mobs that killed players, then per player. |
| **Death causes** | The most common death causes, then per player. |
| **Highest hits** | Each player's biggest single hit: damage, weapon and enemy. |

- It is posted the first time the clock passes `Server - Report Day` / `Report Time` (server time zone), or when the server next runs if it was off.
- **The week's numbers reset only after Discord accepted the whole report**; a failed report is retried, without posting anything twice.
- Kills and damage are measured by the game that owns the creature, so that player needs the mod. Deaths are reported by the dying player's own game. All names are shown in English.
- Totals are saved in `BepInEx/cache/ValheimDiscordRelay.WeeklyReport.txt`, so a restart does not lose the week. Delete the file to start over.

### In-game weekly report window

Press `Home` (default) to open a window with the same numbers as the Discord report, as they stand right now. It is read-only.

- Summary tiles, the same tabs as the Discord report (the Players tab is sortable), and a countdown to the next report in your own time zone.
- The numbers refresh every 30 seconds while the window is open, or when you click **Refresh**.
- Drag the bottom-right corner to resize it; the size is remembered.
- It needs the mod on the server. Admins can turn it off for everyone with `[Weekly Report] Server - Window Enabled`.

#### Styling the window (CSS)

Choose the look with `[Weekly Report] Client - Window Style`: **Classic** or **Modern**. The first time the window opens, its style sheet is written to `BepInEx/config/` (`ValheimDiscordRelay.report.uss` or `ValheimDiscordRelay.report.modern.uss`). Edit it and reopen the window to see the change; delete it to get the original back.

```css
:root { --vdr-gold: #7FD1FF; }        /* change a colour everywhere it is used */
.vdr-tile-value { font-size: 26px; }  /* bigger numbers in the tiles */
```

It is Unity's USS, a subset of CSS: classes, colours, variables, fonts and sizes work; `@import`, `url()` and animations do not. Anything not understood is skipped and listed in the log with its line number. The generated file lists every class.

### Version check

The server lets a player in only when they run **exactly the same version of this mod**; other players see Valheim's "Incompatible version" message and the server log says why. Update the server and players together, or turn the check off with `[General] Server - Require Same Version = false`.

## Files the mod creates

| File | Purpose |
| --- | --- |
| `BepInEx/config/com.kydldev.ValheimDiscordRelay.cfg` | All settings. |
| `BepInEx/config/ValheimDiscordRelay.report.uss` | Style sheet of the in-game report window, Classic style (created on first use). |
| `BepInEx/config/ValheimDiscordRelay.report.modern.uss` | The same for the Modern style (created the first time that style is used). |
| `BepInEx/cache/ValheimDiscordRelay.log` | The mod's own log, kept for 24 hours. |
| `BepInEx/cache/ValheimDiscordRelay.WeeklyReport.txt` | Weekly statistics and the counted boss kills (server). |
| `BepInEx/cache/ValheimDiscordRelay.BossAnnounced.txt` | Boss deaths already announced (server). |
| `BepInEx/cache/ValheimDiscordRelay.clean-shutdown` | Marks a clean shutdown so the next start can be told apart as a restart (server). |
| `BepInEx/cache/ValheimDiscordRelay.ScreenshotStrategy.txt` | The screenshot capture strategy that works on this PC (client). |
| `BepInEx/cache/ValheimDiscordRelay/WebPTool/` | The video encoder, unpacked from the DLL (client). |
| `<system temp>/ValheimDiscordRelay/` | Frames staged for a video while it is encoded; removed afterwards. |

## Security and privacy

- **Players can read the shared webhooks** (screenshots, clips, death and boss videos), because each game uploads its own files. Use a dedicated webhook and channel for each, and delete a webhook in Discord if it leaks.
- **Webhooks only the server uses** (chat, server and player notifications, weekly report) are never sent to ordinary players.
- **Keep `[General] Server - Lock Configuration` on** (default), otherwise any player can change synced settings, webhooks included.
- **Nothing can ping anyone:** every message the mod sends has mentions disabled.
- **What leaves a player's PC:** only what the features send to your webhooks. The mod makes no other network requests.
- **Opt-out:** every `Client - Enabled` switch lets a player keep their chat, deaths, kills, screenshots and clips out of Discord and the report.

## Logs and troubleshooting

Everything is written to `BepInEx/cache/ValheimDiscordRelay.log` (kept for 24 hours), and important lines also go to the BepInEx log. If nothing arrives in Discord:

- **The webhook is empty.** Screenshots, clips and videos need their webhook set in the **server's** config.
- **A switch is off:** the category's `Enable`, or `Client - Enabled` on that player's game.
- **"Incompatible version" when joining:** the player's copy of the mod is missing or not the server's version (see [Version check](#version-check)).
- **Message but no video:** video is turned off, or the encoder could not run (it is a Windows program); the log has the reason.
- **Kills missing from the weekly report:** the player who owned the creature does not have the mod.

Set `[General] Server - Log Successful Sends = true` to log every delivery too.

## Credits and licence

I made this mod because I wanted something I could not find in the other available mods. It might not be perfect, but it has worked in my tests and I hope it works for you too. Please report any issues.

The mod bundles a copy of [ServerSync](https://github.com/blaxxun-boop/ServerSync) (Copyright 2021 Tykea; permission notice in `src/Integrations/Harmony/ServerSync/LICENSE.txt`), with local changes for the strict version check described in that file's header, and the official libwebp `img2webp` encoder from Google's WebP project.

---

![KydlDev](https://raw.githubusercontent.com/kydlDev/.github/main/branding/KydlDev-banner-960x240.png)

## Get help and stay in touch
Questions, bug reports, ideas or mod requests? Come say hi in the **KydlDev Discord**: **https://discord.gg/tX2CcTNC4W**
Everyone is welcome, and it is the quickest way to reach me. Source code and issues: https://github.com/kydlDev
