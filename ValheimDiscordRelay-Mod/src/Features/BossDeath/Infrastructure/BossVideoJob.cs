using System.Collections.Generic;

namespace ValheimDiscordRelay.Client;

/// <summary>Everything one boss-death video needs: where it goes, what it is called and the captured frames.</summary>
internal sealed class BossVideoJob
{
    public string BossName { get; set; }
    public string WebhookUrl { get; set; }
    public string Username { get; set; }
    public string AvatarUrl { get; set; }

    /// <summary>The staged frames, each with the time it is shown.</summary>
    public List<StagedFrame> Frames { get; set; }

    public string AnnounceKey { get; set; }
    public long RemoteOwner { get; set; }
}
