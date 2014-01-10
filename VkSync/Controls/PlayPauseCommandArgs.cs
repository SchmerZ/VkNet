using VkToolkit.Model;

namespace VkSync.Controls
{
    public enum PlaybackState
    {
        Stopped,
        Playing,
        Buffering,
        Paused
    }

    public class PlayPauseCommandArgs
    {
        public PlaybackState Status
        {
            get; 
            set;
        }

        public Audio Audio
        {
            get; 
            set;
        }
    }
}