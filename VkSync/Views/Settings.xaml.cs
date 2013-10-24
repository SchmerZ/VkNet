using System.Windows;
using System.Windows.Forms;
using VkSync.Serializers;
using VkSync.ViewModels;
using VkToolkit;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace VkSync.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();

            btnSave.Click += btnSave_Click;
            btnTestAutorization.Click += btnTestAutorization_Click;
            btnSelectDataFolder.Click += btnSelectDataFolder_Click;
        }

        private Models.Settings SettingsModel
        {
            get
            {
                return ((SettingsViewModel)DataContext).Settings;
            }
        }

        private void btnSelectDataFolder_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new FolderBrowserDialog();

            var result = openFolderDialog.ShowDialog();

            if (result == DialogResult.OK)
                txtDataFolderPath.Text = openFolderDialog.SelectedPath;
        }

        private void btnTestAutorization_Click(object sender, RoutedEventArgs e)
        {
            int appId;

            if (int.TryParse(txtAppId.Text, out appId))
            {
                var api = new VkApi();
                api.Authorize(appId, txtLogin.Text, txtPassword.Password, VkToolkit.Enums.Settings.Audio);

                MessageBox.Show(string.IsNullOrEmpty(api.AccessToken) ? "Failed" : "Ok");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SettingsSerializer.Serialize(SettingsModel);
        }
    }
}