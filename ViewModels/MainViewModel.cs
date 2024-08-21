using ReactiveUI;
using System;
using System.Reactive.Linq;

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

        public MainViewModel()
        {
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

        ~MainViewModel()
        {
            _timerSubscription.Dispose();
        }
    }
}