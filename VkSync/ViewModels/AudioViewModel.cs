using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using NAudio.Wave;
using VkSync.Commands;
using VkSync.Controls;
using VkSync.Helpers;
using VkSync.Models;
using VkSync.Streams;
using VkSync.Tasks;
using VkToolkit;
using VkToolkit.Model;
using VkToolkit.Utils;
using PlaybackState = VkSync.Controls.PlaybackState;
using Timer = System.Windows.Forms.Timer;

namespace VkSync.ViewModels
{
    public class AudioViewModel : MediatorViewModel
    {
        private enum StreamingPlaybackState
        {
            Stopped,
            Playing,
            Buffering,
            Paused
        }

        #region Fields

        private BufferedWaveProvider _bufferedWaveProvider;
        private IWavePlayer _wavePlayer;
        private volatile StreamingPlaybackState _playbackState;
        private volatile bool _fullyDownloaded;
        private HttpWebRequest _webRequest;
        private VolumeWaveProvider16 _volumeProvider;

        private System.Windows.Forms.Timer _timer;

        #endregion

        #region Ctors

        public AudioViewModel()
        {
            _timer = new Timer
                {
                    Interval = 250,
                    Enabled = false,
                };

            _timer.Tick += timer_Tick;
        }

        #endregion
        
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

        public ICommand PlayPauseCommand
        {
            get
            {
                return new RelyParameterCommand(OnPlayPauseCommand);
            }
        }

        private void OnPlayPauseCommand(object parameter)
        {
            var args = (PlayPauseCommandArgs)parameter;

            if (args.Status == PlaybackState.Playing)
            {
                StartPlayback(args.Audio);
            }
            else if (args.Status == PlaybackState.Paused)
            {
                PausePlayback();
            }
        }

        private void StartPlayback(Audio audio)
        {
            if (_playbackState == StreamingPlaybackState.Stopped)
            {
                _playbackState = StreamingPlaybackState.Buffering;
                _bufferedWaveProvider = null;

                ThreadPool.QueueUserWorkItem(StreamMp3, audio.Url);
                //_timer.Change(0, 250);
                _timer.Enabled = true;
            }
            else if (_playbackState == StreamingPlaybackState.Paused)
            {
                _playbackState = StreamingPlaybackState.Buffering;
            }
        }

        private void StreamMp3(object state)
        {
            _fullyDownloaded = false;

            var url = (Uri)state;
            _webRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = null;

            try
            {
                resp = (HttpWebResponse)_webRequest.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status != WebExceptionStatus.RequestCanceled)
                {
                    //ShowError(e.Message);
                }
                return;
            }

            var buffer = new byte[16384 * 4]; // needs to be big enough to hold a decompressed frame

            IMp3FrameDecompressor decompressor = null;
            try
            {
                using (var responseStream = resp.GetResponseStream())
                {
                    var readFullyStream = new ReadFullyStream(responseStream);

                    do
                    {
                        if (_bufferedWaveProvider != null &&
                            _bufferedWaveProvider.BufferLength - _bufferedWaveProvider.BufferedBytes < _bufferedWaveProvider.WaveFormat.AverageBytesPerSecond / 4)
                        {
                            Debug.WriteLine("Buffer getting full, taking a break");
                            Thread.Sleep(500);
                        }
                        else
                        {
                            Mp3Frame frame = null;
                            
                            try
                            {
                                frame = Mp3Frame.LoadFromStream(readFullyStream);
                            }
                            catch (EndOfStreamException)
                            {
                                _fullyDownloaded = true;
                                // reached the end of the MP3 file / stream
                                break;
                            }
                            catch (WebException)
                            {
                                // probably we have aborted download from the GUI thread
                                break;
                            }

                            if (decompressor == null)
                            {
                                // don't think these details matter too much - just help ACM select the right codec
                                // however, the buffered provider doesn't know what sample rate it is working at
                                // until we have a frame
                                var waveFormat = new Mp3WaveFormat(frame.SampleRate,
                                                                   frame.ChannelMode == ChannelMode.Mono ? 1 : 2,
                                                                   frame.FrameLength, frame.BitRate);

                                decompressor = new AcmMp3FrameDecompressor(waveFormat);

                                _bufferedWaveProvider = new BufferedWaveProvider(decompressor.OutputFormat)
                                    {
                                        BufferDuration = TimeSpan.FromSeconds(20)
                                    };

                                //this.bufferedWaveProvider.BufferedDuration = 250;
                            }

                            var decompressed = decompressor.DecompressFrame(frame, buffer, 0);
                            ////Debug.WriteLine(String.Format("Decompressed a frame {0}", decompressed));
                            _bufferedWaveProvider.AddSamples(buffer, 0, decompressed);
                        }

                    } 
                    while (_playbackState != StreamingPlaybackState.Stopped);
                    
                    Debug.WriteLine("Exiting");
                    
                    // was doing this in a finally block, but for some reason
                    // we are hanging on response stream .Dispose so never get there
                    decompressor.Dispose();
                }
            }
            finally
            {
                if (decompressor != null)
                {
                    decompressor.Dispose();
                }
            }
        }

        private void PausePlayback()
        {
            if (_playbackState == StreamingPlaybackState.Playing || _playbackState == StreamingPlaybackState.Buffering)
            {
                _wavePlayer.Pause();

                Debug.WriteLine("User requested Pause, waveOut.PlaybackState={0}", _wavePlayer.PlaybackState);
                _playbackState = StreamingPlaybackState.Paused;
            }
        }

        private void StopPlayback()
        {
            if (_playbackState != StreamingPlaybackState.Stopped)
            {
                if (!_fullyDownloaded)
                {
                    _webRequest.Abort();
                }

                _playbackState = StreamingPlaybackState.Stopped;

                if (_wavePlayer != null)
                {
                    _wavePlayer.Stop();
                    _wavePlayer.Dispose();
                    _wavePlayer = null;
                }

                //_timer.Change(Timeout.Infinite, 250);
                _timer.Enabled = false;

                // n.b. streaming thread may not yet have exited
                Thread.Sleep(500);
                //ShowBufferState(0);
            }
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

        #region TimerPlayback

        private void timer_Tick(object state, EventArgs eventArgs)
        {
            if (_playbackState != StreamingPlaybackState.Stopped)
            {
                if (_wavePlayer == null && _bufferedWaveProvider != null)
                {
                    Debug.WriteLine("Creating WaveOut Device");
                    _wavePlayer = CreateWaveOut();
                   // _wavePlayer.PlaybackStopped += waveOut_PlaybackStopped;

                    _volumeProvider = new VolumeWaveProvider16(_bufferedWaveProvider);
                    _volumeProvider.Volume = 1;// this.volumeSlider1.Volume;
                    _wavePlayer.Init(_volumeProvider);
                    //progressBarBuffer.Maximum = (int)bufferedWaveProvider.BufferDuration.TotalMilliseconds;
                }
                else if (_bufferedWaveProvider != null)
                {
                    var bufferedSeconds = _bufferedWaveProvider.BufferedDuration.TotalSeconds;
                    //ShowBufferState(bufferedSeconds);
                    // make it stutter less if we buffer up a decent amount before playing
                    if (bufferedSeconds < 0.5 && _playbackState == StreamingPlaybackState.Playing && !_fullyDownloaded)
                    {
                        _playbackState = StreamingPlaybackState.Buffering;
                        _wavePlayer.Pause();

                        Debug.WriteLine(String.Format("Paused to buffer, waveOut.PlaybackState={0}", _wavePlayer.PlaybackState));
                    }
                    else if (bufferedSeconds > 4 && _playbackState == StreamingPlaybackState.Buffering)
                    {
                        _wavePlayer.Play();
                        
                        Debug.WriteLine(String.Format("Started playing, waveOut.PlaybackState={0}", _wavePlayer.PlaybackState));
                        
                        _playbackState = StreamingPlaybackState.Playing;
                    }
                    else if (_fullyDownloaded && bufferedSeconds == 0)
                    {
                        Debug.WriteLine("Reached end of stream");
                        StopPlayback();
                    }
                }

            }
        }

        private IWavePlayer CreateWaveOut()
        {
            //return new WaveOut();
            return new DirectSoundOut();
        }

        #endregion
    }
}