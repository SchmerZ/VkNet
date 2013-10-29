using System.Windows.Input;
using VkSync.Commands;
using VkSync.Models;
using VkSync.Helpers;

namespace VkSync.ViewModels
{
    public class SettingsViewModel : BindingModel
    {
        #region Fields

        private string _confirmPassword;

		#endregion
		
		#region Constructors

        public SettingsViewModel()
        {
            Settings = VkSyncContext.Settings;
            ConfirmPassword = Settings.Password;
        }

		#endregion

		#region Properties

		public Settings Settings
		{
			get; 
			private set;
		}

        public int AppId
        {
            get
            {
                return Settings.AppId;
            }
            set
            {
                Settings.AppId = value;

                ValidationHandler.ValidateRule("AppId", "Should be positive number.", () => value > 0);

                OnPropertyChanged("AppId");
            }
        }

        public string Login
		{
			get
			{
				return Settings.Login;
			}
			set
			{
                Settings.Login = value;

				ValidationHandler.ValidateRule("Login", "Incorrect email address", value.IsValidEmail);

				OnPropertyChanged("Login");
			}
		}

		public string Password
		{
			get
			{
				return Settings.Password;
			}
			set
			{
				Settings.Password = value;

				OnPropertyChanged("Password");

                ConfirmPassword = null;
			}
		}

        public string ConfirmPassword
		{
			get
			{
                return _confirmPassword;
			}
			set
			{
                _confirmPassword = value;

                ValidationHandler.ValidateRule("ConfirmPassword", "Confirmation password is incorrect",
				                               () => string.CompareOrdinal(Password, value) == 0);

                OnPropertyChanged("ConfirmPassword");
			}
		}

        public string DataFolderPath
		{
			get
			{
				return Settings.DataFolderPath;
			}
			set
			{
                Settings.DataFolderPath = value;
				OnPropertyChanged("DataFilePath");
			}
		}

        public int ConcurrentDownloadThreadsCount
        {
            get
            {
                return Settings.ConcurrentDownloadThreadsCount;
            }
            set
            {
                Settings.ConcurrentDownloadThreadsCount = value;

                ValidationHandler.ValidateRule("ConcurrentDownloadThreadsCount", "Count should be in [1..99] range",
                                               () => value > 0 && value < 100);

                OnPropertyChanged("ConcurrentDownloadThreadsCount");
            }
        }

		#endregion

        #region Commands

        public ICommand CancelCommand
        {
            get
            {
                return new RelyCommand(OnCancelCommand);
            }
        }

        private void OnCancelCommand()
        {
            
        }

        #endregion
    }
}