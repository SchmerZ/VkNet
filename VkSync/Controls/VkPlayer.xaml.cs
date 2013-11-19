using System.Windows;
using System.Windows.Controls;
using VkToolkit.Model;

namespace VkSync.Controls
{
    public partial class VkPlayer : UserControl
    {
        #region Ctors

        public VkPlayer()
        {
            InitializeComponent();
        }

        #endregion

        public static readonly DependencyProperty SelectedAudioProperty =
            DependencyProperty.Register("SelectedAudio", typeof (Audio), typeof (VkPlayer),
                new PropertyMetadata(OnSelectedAudioChanged));

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
    }
}