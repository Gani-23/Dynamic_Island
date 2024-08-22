using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Windows.Input;

namespace DynamicIsland.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private string _time;
        private string _date;
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

        // Command to open settings
        public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }
        public ReactiveCommand<Unit, Unit> AccountSettingsCommand { get; }

        public MainViewModel()
        {
            // Initialize the command
            OpenSettingsCommand = ReactiveCommand.Create(OpenSettings);
            AccountSettingsCommand = ReactiveCommand.Create(AccountSettings);
            // Handle command errors

            AccountSettingsCommand.ThrownExceptions
                .Subscribe(ex => Console.WriteLine($"Command error: {ex.Message}"));
            
            OpenSettingsCommand.ThrownExceptions
                .Subscribe(ex => Console.WriteLine($"Command error: {ex.Message}"));

            // Set up a timer to update the time and date every second
            var timer = Observable.Interval(TimeSpan.FromSeconds(1))
                .ObserveOn(RxApp.MainThreadScheduler);

            _timerSubscription = timer.Subscribe(_ => UpdateTimeAndDate());
            UpdateTimeAndDate(); // Initialize immediately
        }

        private void UpdateTimeAndDate()
        {
            var now = DateTime.Now;
            Time = now.ToString("hh:mm:ss tt"); // 12-hour clock with AM/PM
            Date = now.ToString("MMMM d, yyyy");
        }

       
        private void OpenSettings()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    // Open Windows Settings
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "ms-settings:",
                        UseShellExecute = true
                    });
                }
                else if (OperatingSystem.IsMacOS())
                {
                    // Open macOS System Preferences
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "open",
                        Arguments = "-a 'System Preferences'",
                        UseShellExecute = true
                    });
                }
                else
                {
                    // Handle other platforms or show an error
                    Console.WriteLine("Platform not supported for opening settings.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur while trying to open settings
                Console.WriteLine($"Error opening settings: {ex.Message}");
            }
        }
        private void AccountSettings()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    // Example action for Windows
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "ms-settings:account",
                        UseShellExecute = true
                    });
                }
                else if (OperatingSystem.IsMacOS())
                {
                    // Example action for macOS
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "open",
                        Arguments = "-a 'System Preferences' --args 'Accounts'",
                        UseShellExecute = true
                    });
                }
                else
                {
                    // Handle other platforms or show an error
                    Console.WriteLine("Platform not supported for account settings.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur while trying to open account settings
                Console.WriteLine($"Error opening account settings: {ex.Message}");
            }
        }

        ~MainViewModel()
        {
            _timerSubscription.Dispose();
        }
    }
}
