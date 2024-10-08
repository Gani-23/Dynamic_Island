﻿using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Runtime.InteropServices;
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
        public ReactiveCommand<Unit, Unit> OpenDefaultMusicCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenVsCommand { get; }

        public ReactiveCommand<Unit, Unit> OpenTerminalCommand { get; }

        public MainViewModel()
        {
            // Initialize commands
            OpenVsCommand = ReactiveCommand.Create(OpenVs);
            OpenSettingsCommand = ReactiveCommand.Create(OpenSettings);
            AccountSettingsCommand = ReactiveCommand.Create(AccountSettings);
            StartStopwatchCommand = ReactiveCommand.Create(StartStopwatch);
            PauseStopwatchCommand = ReactiveCommand.Create(PauseStopwatch);
            StopStopwatchCommand = ReactiveCommand.Create(StopStopwatch);
            OpenDefaultMusicCommand = ReactiveCommand.Create(OpenDefaultMusic);
            OpenTerminalCommand = ReactiveCommand.Create(OpenTerminal);

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

        private void OpenDefaultMusic()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    // For Windows, you could use a URI scheme to open the default music player
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "mswindowsmusic:",
                        UseShellExecute = true
                    });
                }
                else if (OperatingSystem.IsMacOS())
                {
                    // For macOS, use the `open` command with a known music file or app
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "open",
                        Arguments = "-a Music",
                        UseShellExecute = true
                    });
                }
                else if (OperatingSystem.IsLinux())
                {
                    // For Linux, use `xdg-open` to open the default music application
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "xdg-open",
                        Arguments = "https://www.spotify.com", // example, usually would use a music file or app URI
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.WriteLine("Unsupported operating system.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open the default music player: {ex.Message}");
            }
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

        

        private void OpenTerminal()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    // Open Command Prompt on Windows
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        UseShellExecute = true
                    });
                }
                else if (OperatingSystem.IsMacOS())
                {
                    // Open Terminal on macOS
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "open",
                        Arguments = "-a Terminal",
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to open terminal: " + ex.Message);
            }
        }

        private void OpenVs()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    // For Windows
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "code",
                        UseShellExecute = true
                    });
                    
                }
                else if (OperatingSystem.IsMacOS())
                {
                    // For macOS
                    Process.Start("/Applications/Visual Studio Code.app/Contents/Resources/app/bin/code");
                }
                else
                {
                    Console.WriteLine("Unsupported operating system.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open VS Code: {ex.Message}");
            }
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
