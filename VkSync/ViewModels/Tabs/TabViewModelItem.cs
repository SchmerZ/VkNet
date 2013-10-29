using VkSync.Models;

namespace VkSync.ViewModels.Tabs
{
    public class TabViewModelItem : BindingModel
    {
        private string _tabName;

        public string TabName
        {
            get { return _tabName; }
            set
            {
                _tabName = value;
                OnPropertyChanged("TabName");
            }
        }

        private BindingModel _tabContents;

        public BindingModel TabContents
        {
            get { return _tabContents; }
            set
            {
                _tabContents = value;
                OnPropertyChanged("TabContents");
            }
        }
    }
}