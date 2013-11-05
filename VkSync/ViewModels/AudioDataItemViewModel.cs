using VkSync.Models;
using VkToolkit.Model;

namespace VkSync.ViewModels
{
    public class AudioDataItemViewModel : BindingModel
    {
        public AudioDataItemViewModel(Audio audio)
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

        private string _progressTag;

        public string ProgressTag
        {
            get { return _progressTag; }
            set
            {
                _progressTag = value;
                OnPropertyChanged("ProgressTag");
            }
        }
    }
}