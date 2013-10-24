using VkSync.Models;
using VkSync.Helpers;

namespace VkSync.ViewModels
{
    public class SettingsViewModel : BindingModel
    {
        #region Fields

		private string _newPassword;

		#endregion
		
		#region Constructors

        public SettingsViewModel(Settings settings)
		{
			Settings = settings;

			NewPassword = Settings.Password;
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

				NewPassword = null;
			}
		}

		public string NewPassword
		{
			get
			{
			    return _newPassword;
			}
			set
			{
                _newPassword = value;

				ValidationHandler.ValidateRule("NewPassword", "Confirmation password is incorrect",
				                               () => string.CompareOrdinal(Password, value) == 0);

				OnPropertyChanged("NewPassword");
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
    }
}