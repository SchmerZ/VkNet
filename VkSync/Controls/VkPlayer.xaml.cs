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

            VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            SeekSlider.ValueChanged += SeekSlider_ValueChanged;
        }

        #endregion

        public static readonly DependencyProperty SelectedAudioProperty =
            DependencyProperty.Register("SelectedAudio", typeof(Audio), typeof(VkPlayer),
                new PropertyMetadata(OnSelectedAudioChanged));

        public static readonly DependencyProperty VolumeProperty =
           DependencyProperty.Register("Volume", typeof(double), typeof(VkPlayer));

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof (int), typeof (VkPlayer),
                new PropertyMetadata(OnPositionChanged));

        public static readonly DependencyProperty DownloadSelectedAudioCommandProperty =
            DependencyProperty.Register("DownloadSelectedAudioCommand", typeof(ICommand), typeof(VkPlayer));

        public static readonly DependencyProperty PlayPauseCommandProperty =
            DependencyProperty.Register("PlayPauseCommand", typeof(ICommand), typeof(VkPlayer));

        public Audio SelectedAudio
        {
            get { return (Audio)GetValue(SelectedAudioProperty); }
            set { SetValue(SelectedAudioProperty, value); }
        }

        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        public int Position
        {
            get { return (int)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        private static void OnPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var player = (VkPlayer)sender;
            var position = (int)args.NewValue;

            player.SeekSlider.Value = position;
        }

        private static void OnSelectedAudioChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var player = (VkPlayer)sender;
            var selectedAudio = (Audio)args.NewValue;

            player.SeekSlider.Maximum = selectedAudio == null ? 0 : selectedAudio.Duration;
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

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Volume = e.NewValue;
        }

        private void SeekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Position = (int) e.NewValue;
        }
    }
}