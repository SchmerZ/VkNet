using System.Collections.ObjectModel;

using VkSync.Models;
using VkSync.ViewModels.Tabs;

namespace VkSync.ViewModels
{
    public class MainWindowViewModel : BindingModel
    {
        private ObservableCollection<TabViewModelItem> _tabs;

        public MainWindowViewModel()
        {
            Tabs = new ObservableCollection<TabViewModelItem>
                {
                    new TabViewModelItem {TabName = "Audio", TabContents = new AudioViewModel()},
                    new TabViewModelItem {TabName = "Settings", TabContents = new SettingsViewModel()}
                };
        }

        public ObservableCollection<TabViewModelItem> Tabs
        {
            get { return _tabs; }
            set
            {
                _tabs = value;
                OnPropertyChanged("Tabs");
            }
        }

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }
    }
}