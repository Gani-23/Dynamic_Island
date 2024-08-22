using System;
using Avalonia;
using Avalonia.Controls;
using DynamicIsland.ViewModels;

namespace DynamicIsland.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(); // Set the DataContext to the MainViewModel
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            // Retrieve the primary screen's bounds
            var screen = Screens.Primary;
            if (screen != null)
            {
                var screenBounds = screen.Bounds;

                // Retrieve the dimensions of the window
                var windowWidth = this.Bounds.Width;
                var windowHeight = this.Bounds.Height;

                // Calculate the position for the top center
                var x = (screenBounds.Width - windowWidth) / 2;
                var y = 0; // Position the window at the top

                // Set the window's position
                this.Position = new PixelPoint((int)x, (int)y);
            }
        }
    }
}