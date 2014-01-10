using System.Windows;

using System.Windows.Controls;
using System.Windows.Input;

using VkSync.Commands;
using VkToolkit.Model;

namespace VkSync.Controls
{
    public partial class VkPlayer : UserControl
    {
        #region Ctors

        public VkPlayer()
        {
            InitializeComponent();

            DownloadButton.Command = new RelyCommand(OnDownloadSelectedAudioClick, () => SelectedAudio != null);
            PlayPauseButton.Command = new RelyCommand(OnPlayPauseClick, () => SelectedAudio != null);
        }

        #endregion

        public static readonly DependencyProperty SelectedAudioProperty =
            DependencyProperty.Register("SelectedAudio", typeof (Audio), typeof (VkPlayer),
                new PropertyMetadata(OnSelectedAudioChanged));

        public static readonly DependencyProperty DownloadSelectedAudioCommandProperty =
            DependencyProperty.Register("DownloadSelectedAudioCommand", typeof (ICommand), typeof (VkPlayer));

        public static readonly DependencyProperty PlayPauseCommandProperty =
            DependencyProperty.Register("PlayPauseCommand", typeof(ICommand), typeof(VkPlayer));

        public Audio SelectedAudio
        {
            get { return (Audio)GetValue(SelectedAudioProperty); }
            set { SetValue(SelectedAudioProperty, value); }
        }

        private static void OnSelectedAudioChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var player = (VkPlayer) sender;
            player.SeekSlider.Value = 0;
        }

        public ICommand DownloadSelectedAudioCommand
        {
            get { return (ICommand)GetValue(DownloadSelectedAudioCommandProperty); }
            set { SetValue(DownloadSelectedAudioCommandProperty, value); }
        }

        private void OnDownloadSelectedAudioClick()
        {
            if (DownloadSelectedAudioCommand != null)
                DownloadSelectedAudioCommand.Execute(SelectedAudio);
        }

        public ICommand PlayPauseCommand
        {
            get { return (ICommand)GetValue(PlayPauseCommandProperty); }
            set { SetValue(PlayPauseCommandProperty, value); }
        }

        private void OnPlayPauseClick()
        {
            if (PlayPauseCommand != null)
            {
                var args = new PlayPauseCommandArgs
                    {
                        Status = PlayPauseButton.IsChecked.GetValueOrDefault()
                                     ? PlaybackState.Playing
                                     : PlaybackState.Paused,
                        Audio = SelectedAudio
                    };

                PlayPauseCommand.Execute(args);
            }
        }
    }
}