using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using VkSync.Helpers;
using VkSync.Tasks;
using VkSync.ViewModels;
using VkToolkit;
using VkToolkit.Utils;

namespace VkSync.Views
{
    /// <summary>
    /// Interaction logic for Audio.xaml
    /// </summary>
    public partial class Audio : UserControl
    {
        public Audio()
        {
            InitializeComponent();

            btnGetData.Click += btnGetData_Click;
            btnSync.Click += btnSync_Click;
        }

        private ObservableCollection<AudioViewModel> AudioData
        {
            get; 
            set;
        }

        private void btnGetData_Click(object sender, RoutedEventArgs e)
        {
            var task = Task.Factory.StartNew(() => GetAudio()).
                ContinueWith(
                    t => AudioData = new ObservableCollection<AudioViewModel>(t.Result.Select(o => new AudioViewModel(o))));

            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();

            task.ContinueWith(t => { dgAudio.ItemsSource = AudioData; }, uiContext);
        }

        private IEnumerable<VkToolkit.Model.Audio> GetAudio()
        {
            var settings = VkSyncContext.Settings;

            var api = new VkApi();
            api.Authorize(settings.AppId, settings.Login, settings.Password, VkToolkit.Enums.Settings.Audio);

            var audio = api.Audio.Get(api.UserId);

            return audio;
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = sender as DataGridCell;
            var row = VisualTreeHelpers.FindAncestor<DataGridRow>(cell);

            if (!cell.IsEditing)
            {
                // enables editing on single click
                if (!cell.IsFocused)
                    cell.Focus();

                if (!cell.IsSelected)
                {
                    //cell.IsSelected = true;
                    row.IsSelected = true;
                }
            }
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            btnGetData.IsEnabled = false;

            var selected = AudioData.Where(o => o.IsSelected).ToList();

            // Create a scheduler that uses two threads. 
            var lcts = new LimitedConcurrencyLevelTaskScheduler(1);
            
            // Create a TaskFactory and pass it our custom scheduler. 
            var factory = new TaskFactory(lcts);
            var tasks = new List<Task>();

            foreach (var model in selected)
            {
                var task = factory.StartNew(Download, model);
                tasks.Add(task);
            }

            Task.WhenAll(tasks).ContinueWith((t) =>
            {
                btnGetData.Dispatcher.Invoke(() => btnGetData.IsEnabled = true);
            });

        }

        private void Download(object state)
        {
            try
            {
                Thread.Sleep(1000);
                return;

                var folder = Path.Combine(VkSyncContext.Settings.DataFolderPath, "vk_music");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var model = (AudioViewModel) state;
                var url = model.Audio.Url.ToString();
                var fileName = string.Format("{0} - {1}.mp3", model.Audio.Artist, model.Audio.Title);
                var filePath = Path.Combine(folder, fileName);

                var httpClient = new WebHttpClient(MediaType.Json);
                var length = httpClient.GetResponseLength(url);
                var bytesInPerncent = length / 100;

                using (var stream = httpClient.GetStream(url))
                using (var file = File.Create(filePath))
                {
                    StreamCopyTo(stream, file, 4096, (total) =>
                    {
                        model.PercentageDownloadComplete = (int) (total / bytesInPerncent);
                    });
                }
            }
            catch (AggregateException ex)
            {
                
            }
        }

        private void StreamCopyTo(Stream source, Stream destination, int bufferSize, Action<long> action)
        {
            var buffer = new byte[bufferSize];
            long totalReaded = 0;
            int read;

            while ((read = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                totalReaded += read;

                destination.Write(buffer, 0, read);

                if (action != null)
                    action(totalReaded);
            }
        } 
    }
}