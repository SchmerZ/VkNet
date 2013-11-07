using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using VkSync.Commands;
using VkSync.Helpers;
using VkSync.Models;
using VkSync.ViewModels.Tabs;

namespace VkSync.ViewModels
{
    public class MainWindowViewModel : MediatorViewModel
    {
        #region Fields

        private ObservableCollection<TabViewModelItem> _tabs;
        private int _selectedIndex;

        private bool _panelLoading;
        private bool _isSpinning = true;

        private string _panelMainMessage = "Main Loading Message";
        private string _panelSubMessage = "Sub Loading Message";

        #endregion

        #region Ctors

        public MainWindowViewModel()
        {
            Mediator.Register(ViewModelMessageType.Notification, (args) =>
            {
                var pair = args as Pair<string, string>;

                if (pair != null)
                {
                    PanelLoading = true;

                    PanelMainMessage = pair.First;
                    PanelSubMessage = pair.Second;
                }
                else if (args is string)
                {
                    PanelLoading = true;

                    PanelSubMessage = PanelMainMessage;
                    PanelMainMessage = (string) args;
                }
            });

            Mediator.Register(ViewModelMessageType.Navigation, (args) =>
            {
                var navigationName = (string) args;

                var item = Tabs.SingleOrDefault((o) => o.TabName == navigationName);
                SelectedIndex = Tabs.IndexOf(item);
            });

            Mediator.Register(ViewModelMessageType.Working, (args) =>
            {
                IsSpinning = (bool) args;
            });

            Tabs = new ObservableCollection<TabViewModelItem>
                {
                    new TabViewModelItem {TabName = "Audio", TabContents = new AudioViewModel()},
                    new TabViewModelItem {TabName = "Settings", TabContents = new SettingsViewModel()}
                };
        }

        #endregion

        #region Properties

        public ObservableCollection<TabViewModelItem> Tabs
        {
            get { return _tabs; }
            set
            {
                _tabs = value;
                OnPropertyChanged("Tabs");
            }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [panel loading].
        /// </summary>
        /// <value>
        /// <c>true</c> if [panel loading]; otherwise, <c>false</c>.
        /// </value>
        public bool PanelLoading
        {
            get
            {
                return _panelLoading;
            }
            set
            {
                _panelLoading = value;
                OnPropertyChanged("PanelLoading");
            }
        }

        public bool IsSpinning
        {
            get
            {
                return _isSpinning;
            }
            set
            {
                _isSpinning = value;
                OnPropertyChanged("IsSpinning");
            }
        }

        /// <summary>
        /// Gets or sets the panel main message.
        /// </summary>
        /// <value>The panel main message.</value>
        public string PanelMainMessage
        {
            get
            {
                return _panelMainMessage;
            }
            set
            {
                _panelMainMessage = value;
                OnPropertyChanged("PanelMainMessage");
            }
        }

        /// <summary>
        /// Gets or sets the panel sub message.
        /// </summary>
        /// <value>The panel sub message.</value>
        public string PanelSubMessage
        {
            get
            {
                return _panelSubMessage;
            }
            set
            {
                _panelSubMessage = value;
                OnPropertyChanged("PanelSubMessage");
            }
        }

        /// <summary>
        /// Gets the panel close command.
        /// </summary>
        public ICommand PanelCloseCommand
        {
            get
            {
                return new RelyCommand(() =>
                {
                    PanelLoading = false;
                });
            }
        }

        /// <summary>
        /// Gets the show panel command.
        /// </summary>
        public ICommand ShowPanelCommand
        {
            get
            {
                return new RelyCommand(() =>
                {
                    PanelLoading = true;
                });
            }
        }

        /// <summary>
        /// Gets the hide panel command.
        /// </summary>
        public ICommand HidePanelCommand
        {
            get
            {
                return new RelyCommand(() =>
                {
                    PanelLoading = false;
                });
            }
        }

        public ICommand NavigateTo
        {
            get
            {
                return new RelyCommand(() =>
                {
                    
                });
            }
        }

        #endregion
    }
}