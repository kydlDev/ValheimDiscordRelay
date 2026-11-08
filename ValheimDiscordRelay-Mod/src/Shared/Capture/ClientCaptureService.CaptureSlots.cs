using System;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// Hands out the "this capture is finished" callback for the frame capture schedulers.
/// </summary>
internal sealed partial class ClientCaptureService
{
    /// <summary>
    /// Creates the callback that frees a capture slot. A capture can report back more than once (a frame that was
    /// delivered and then failed while being stored, for example); only the first call frees the slot, so one capture
    /// can never free the slot of another.
    /// </summary>
    /// <param name="scheduler">The scheduler the capture was started on.</param>
    /// <returns>A callback that frees the slot once.</returns>
    private static Action CreateSlotRelease(CaptureScheduler scheduler)
    {
        bool released = false;

        return () =>
        {
            if (released)
                return;

            released = true;
            scheduler.End();
        };
    }
}
