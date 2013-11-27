using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

using VkSync.Commands;
using VkSync.Models;
using VkSync.Helpers;
using VkToolkit;

namespace VkSync.ViewModels
{
    public class SettingsViewModel : MediatorViewModel
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

        private Settings Settings
        {
            get; 
            set;
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
            Mediator.Notify(ViewModelMessageType.Navigation, "Audio");
        }

        public ICommand SaveCommand
        {
            get { return new RelyCommand(OnSaveCommand, () => IsValid); }
        }

        private void OnSaveCommand()
        {
            VkSyncContext.SettingsSerializer.Serialize(Settings);
        }

        public ICommand TestAuthorizationCommand
        {
            get
            {
                return new RelyCommand(OnTestAuthorizationCommand, () => IsValid);
            }
        }

        private void OnTestAuthorizationCommand()
        {
            Mediator.Notify(ViewModelMessageType.Notification, new StringPair("Authorization test", "trying to authorize..."));

            var task = Task.Run(() =>
            {
                var api = new VkApi();
                api.Authorize(AppId, Login, Password, VkToolkit.Enums.Settings.Audio);
                var isValid = !string.IsNullOrEmpty(api.AccessToken);

                Mediator.Notify(ViewModelMessageType.Notification, new StringPair("Authorization test", isValid ? "Success" : "Failed"));

                return isValid;
            });

            task.ContinueWith((t) =>
            {
                Mediator.Notify(ViewModelMessageType.Notification, new StringPair("Authorization test", t.Result ? "Success" : "Failed"));
            }, TaskContinuationOptions.NotOnFaulted);

            task.ContinueWith((t) =>
            {
                Mediator.Notify(ViewModelMessageType.Notification, new StringPair("Authorization test", string.Format("Exception occured: {0}.", t.Exception.Message)));
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public ICommand SelectDataFolderCommand
        {
            get
            {
                return new RelyCommand(OnSelectDataFolderCommand);
            }
        }

        private void OnSelectDataFolderCommand()
        {
            var openFolderDialog = new FolderBrowserDialog();

            var result = openFolderDialog.ShowDialog();

            if (result == DialogResult.OK)
                DataFolderPath = openFolderDialog.SelectedPath;
        }

        #endregion
    }
}