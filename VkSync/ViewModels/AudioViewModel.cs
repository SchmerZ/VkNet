using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using VkSync.Commands;
using VkSync.Helpers;
using VkSync.Models;
using VkSync.Tasks;
using VkToolkit;
using VkToolkit.Utils;

namespace VkSync.ViewModels
{
    public class AudioViewModel : MediatorViewModel
    {
        #region Properties

        private ObservableCollection<AudioDataItemViewModel> _audioData = null;

        public ObservableCollection<AudioDataItemViewModel> AudioData
        {
            get
            {
                return _audioData;
            }
            set
            {
                _audioData = value;
                OnPropertyChanged("AudioData");
            }
        }

        private bool GetAudioDataInProgress
        {
            get;
            set;
        }

        #endregion

        #region Commands

        public ICommand GetAudioDataCommand
        {
            get
            {
                return new RelyCommand(OnGetAudioDataCommand, () => !GetAudioDataInProgress);
            }
        }

        private void OnGetAudioDataCommand()
        {
            Mediator.Notify(ViewModelMessageType.Working, true);
            Mediator.Notify(ViewModelMessageType.Notification, new StringPair("Downloading...", "Start getting audio data..."));

            GetAudioDataInProgress = true;

            var getAudioTask = Task.Factory.StartNew(() => GetAudio());

            getAudioTask.ContinueWith((t) =>
            {
                GetAudioDataInProgress = false;
            });

            var downloadTask = getAudioTask.ContinueWith(t =>
            {
                Mediator.Notify(ViewModelMessageType.Notification, new StringPair("Downloading...", "Successed"));

                AudioData = new ObservableCollection<AudioDataItemViewModel>(t.Result.Select(o => new AudioDataItemViewModel(o)));
            }, TaskContinuationOptions.NotOnFaulted);

            Task.WhenAll(new[] { getAudioTask, downloadTask })
                .ContinueWith((t) =>
                {
                    Mediator.Notify(ViewModelMessageType.Notification, new StringPair("Error", t.Exception.InnerException.Message));
                }, TaskContinuationOptions.OnlyOnFaulted);

            Task.WhenAll(new[] { getAudioTask, downloadTask })
                .ContinueWith((t) => Mediator.Notify(ViewModelMessageType.Working, false));
        }

        private IEnumerable<VkToolkit.Model.Audio> GetAudio()
        {
            var settings = VkSyncContext.Settings;

            var api = new VkApi();

            Mediator.Notify(ViewModelMessageType.Notification, "Authorization...");

            api.Authorize(settings.AppId, settings.Login, settings.Password, VkToolkit.Enums.Settings.Audio);

            Mediator.Notify(ViewModelMessageType.Notification, "Get audio data...");

            var audio = api.Audio.Get(api.UserId);

            return audio;
        }

        public ICommand SyncAudioDataCommand
        {
            get
            {
                return new RelyCommand(OnSyncAudioDataCommand);
            }
        }

        private void OnSyncAudioDataCommand()
        {
            GetAudioDataInProgress = true;

            var selected = AudioData.Where(o => o.IsSelected).ToList();

            // Create a scheduler that uses two threads. 
            var lcts = new LimitedConcurrencyLevelTaskScheduler(VkSyncContext.Settings.ConcurrentDownloadThreadsCount);

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
                    GetAudioDataInProgress = false;
                });
        }

        private void Download(object state)
        {
            var model = (AudioDataItemViewModel)state;

            var folder = Path.Combine(VkSyncContext.Settings.DataFolderPath, "vk_music");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

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
                    var complete = (int)(total / bytesInPerncent);
                    model.PercentageDownloadComplete = complete;
                    model.ProgressTag = string.Format("{0}% ({1} / {2} KiBs)", complete, total / 1024, length / 1024);
                });
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
        #endregion
    }
}