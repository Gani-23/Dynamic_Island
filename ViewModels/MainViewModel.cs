using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Windows.Input;
using System.Timers;

namespace DynamicIsland.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private string _time = null!;
        private string _date = null!;
        private string _stopwatchTimeInput = "00:00:00";
        private bool _isStartButtonVisible = true;
        private bool _isPauseButtonVisible = false;
        private bool _isStopButtonVisible = false;
        private Timer _stopwatchTimer;
        private TimeSpan _stopwatchElapsed;

        private readonly IDisposable _timerSubscription;

        public string Time
        {
            get => _time;
            set => this.RaiseAndSetIfChanged(ref _time, value);
        }

        public string Date
        {
            get => _date;
            set => this.RaiseAndSetIfChanged(ref _date, value);
        }

        public string StopwatchTimeInput
        {
            get => _stopwatchTimeInput;
            set => this.RaiseAndSetIfChanged(ref _stopwatchTimeInput, value);
        }

        public bool IsStartButtonVisible
        {
            get => _isStartButtonVisible;
            set => this.RaiseAndSetIfChanged(ref _isStartButtonVisible, value);
        }

        public bool IsPauseButtonVisible
        {
            get => _isPauseButtonVisible;
            set => this.RaiseAndSetIfChanged(ref _isPauseButtonVisible, value);
        }

        public bool IsStopButtonVisible
        {
            get => _isStopButtonVisible;
            set => this.RaiseAndSetIfChanged(ref _isStopButtonVisible, value);
        }

        // Commands
        public ReactiveCommand<Unit, Unit> StartStopwatchCommand { get; }
        public ReactiveCommand<Unit, Unit> PauseStopwatchCommand { get; }
        public ReactiveCommand<Unit, Unit> StopStopwatchCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }
        public ReactiveCommand<Unit, Unit> AccountSettingsCommand { get; }

        public MainViewModel()
        {
            // Initialize commands
            OpenSettingsCommand = ReactiveCommand.Create(OpenSettings);
            AccountSettingsCommand = ReactiveCommand.Create(AccountSettings);
            StartStopwatchCommand = ReactiveCommand.Create(StartStopwatch);
            PauseStopwatchCommand = ReactiveCommand.Create(PauseStopwatch);
            StopStopwatchCommand = ReactiveCommand.Create(StopStopwatch);

            AccountSettingsCommand.ThrownExceptions
                .Subscribe(ex => Console.WriteLine($"Command error: {ex.Message}"));
            
            OpenSettingsCommand.ThrownExceptions
                .Subscribe(ex => Console.WriteLine($"Command error: {ex.Message}"));

            // Timer to update time and date
            var timer = Observable.Interval(TimeSpan.FromSeconds(1))
                .ObserveOn(RxApp.MainThreadScheduler);

            _timerSubscription = timer.Subscribe(_ => UpdateTimeAndDate());
            UpdateTimeAndDate(); // Initialize immediately

            // Initialize stopwatch
            _stopwatchElapsed = TimeSpan.Zero;
            _stopwatchTimer = new Timer(1000);
            _stopwatchTimer.Elapsed += OnStopwatchTick;
        }

        private void UpdateTimeAndDate()
        {
            var now = DateTime.Now;
            Time = now.ToString("hh:mm:ss tt"); // 12-hour clock with AM/PM
            Date = now.ToString("MMMM d, yyyy");
        }

        private void StartStopwatch()
        {
            if (TimeSpan.TryParse(StopwatchTimeInput, out var time))
            {
                _stopwatchElapsed = time;
                _stopwatchTimer.Start();
                IsStartButtonVisible = false;
                IsPauseButtonVisible = true;
                IsStopButtonVisible = true;
            }
        }

        private void PauseStopwatch()
        {
            _stopwatchTimer.Stop();
            IsStartButtonVisible = true;
            IsPauseButtonVisible = false;
        }

        private void StopStopwatch()
        {
            _stopwatchTimer.Stop();
            _stopwatchElapsed = TimeSpan.Zero;
            StopwatchTimeInput = "00:00:00";
            IsStartButtonVisible = true;
            IsPauseButtonVisible = false;
            IsStopButtonVisible = false;
        }

        private void OnStopwatchTick(object sender, ElapsedEventArgs e)
        {
            _stopwatchElapsed = _stopwatchElapsed.Add(TimeSpan.FromSeconds(1));
            StopwatchTimeInput = _stopwatchElapsed.ToString(@"hh\:mm\:ss");
        }

        private void OpenSettings()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "ms-settings:",
                        UseShellExecute = true
                    });
                }
                else if (OperatingSystem.IsMacOS())
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "open",
                        Arguments = "-a 'System Preferences'",
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.WriteLine("Platform not supported for opening settings.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening settings: {ex.Message}");
            }
        }

        private void AccountSettings()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "ms-settings:account",
                        UseShellExecute = true
                    });
                }
                else if (OperatingSystem.IsMacOS())
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "open",
                        Arguments = "-a 'System Preferences' --args 'Accounts'",
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.WriteLine("Platform not supported for account settings.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening account settings: {ex.Message}");
            }
        }

        ~MainViewModel()
        {
            _timerSubscription.Dispose();
            _stopwatchTimer.Dispose();
        }
    }
}
