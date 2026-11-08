using System;

namespace ValheimDiscordRelay.Client;

/// <summary>
/// Decides when the next video frame is captured. The game is asked once per game frame; this answers "now" or "not
/// yet" so that frames are taken at the configured rate. Pure logic: no game or engine types, unit tested.
/// </summary>
/// <remarks>
/// Two things keep the real capture rate close to the configured one. The next capture time is advanced by one interval
/// instead of being measured from "now", so a late tick does not push every later frame back and the rate does not
/// drift low. And several captures may be in flight at once: reading a frame back from the graphics card takes a few game
/// frames, and waiting for each read-back before starting the next one limits the rate to one frame per read-back.
/// </remarks>
internal sealed class CaptureScheduler
{
    /// <summary>How many captures may be in flight at the same time unless told otherwise.</summary>
    internal const int DefaultMaxInFlight = 3;

    /// <summary>
    /// A tick this close before the next slot (half a millisecond) already counts as on time, so rounding in the
    /// clock does not make a game running at exactly the capture rate miss every other slot.
    /// </summary>
    private const double SlotToleranceSeconds = 0.0005d;

    private readonly int _maxInFlight;
    private double _nextDueSeconds;
    private bool _started;
    private int _inFlight;

    /// <summary>
    /// Creates a scheduler.
    /// </summary>
    /// <param name="maxInFlight">How many captures may be in flight at the same time; at least 1.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxInFlight"/> is below 1.</exception>
    internal CaptureScheduler(int maxInFlight = DefaultMaxInFlight)
    {
        if (maxInFlight < 1)
            throw new ArgumentOutOfRangeException(nameof(maxInFlight), maxInFlight, "At least one capture must be allowed.");

        _maxInFlight = maxInFlight;
    }

    /// <summary>Number of captures started and not yet finished.</summary>
    internal int InFlight => _inFlight;

    /// <summary>
    /// Asks whether a capture should start now. When the answer is true the capture counts as in flight until
    /// <see cref="End"/> is called for it.
    /// </summary>
    /// <param name="nowSeconds">The current time in seconds (any steadily increasing clock).</param>
    /// <param name="targetFps">The wanted capture rate in frames per second; values below 1 count as 1.</param>
    /// <returns>True when a capture should be started now.</returns>
    internal bool TryBegin(double nowSeconds, int targetFps)
    {
        if (_inFlight >= _maxInFlight)
            return false;

        double interval = 1d / Math.Max(1, targetFps);

        if (_started && nowSeconds < _nextDueSeconds - SlotToleranceSeconds)
            return false;

        // First capture, or a whole slot was missed (a hitch, or capture was switched off for a while): start over from
        // now instead of firing a burst of captures to catch up.
        if (!_started || nowSeconds - _nextDueSeconds > interval)
            _nextDueSeconds = nowSeconds;

        _started = true;
        _nextDueSeconds += interval;
        _inFlight++;
        return true;
    }

    /// <summary>
    /// Marks one started capture as finished, whether it delivered a frame or failed. Calling it more often than
    /// captures were started does nothing.
    /// </summary>
    internal void End()
    {
        if (_inFlight > 0)
            _inFlight--;
    }

    /// <summary>
    /// Forgets the schedule, so the next <see cref="TryBegin"/> starts a new one. Captures still in flight stay counted.
    /// </summary>
    internal void Restart()
    {
        _started = false;
    }
}
