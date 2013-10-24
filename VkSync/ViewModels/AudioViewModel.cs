using VkSync.Models;
using VkToolkit.Model;

namespace VkSync.ViewModels
{
    public class AudioViewModel : BindingModel
    {
        public AudioViewModel(Audio audio)
        {
            Audio = audio;
        }

        public Audio Audio
        {
            get;
            set;
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;

                OnPropertyChanged("IsSelected");
            }
        }

        private int _percentageDownloadComplete;

        public int PercentageDownloadComplete
        {
            get
            {
                return _percentageDownloadComplete;
            }
            set
            {
                 _percentageDownloadComplete = value;

                OnPropertyChanged("PercentageDownloadComplete");
            }
        }
    }
}