using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using KeyCounter.ViewModels;

namespace KeyCounter
{
	public partial class MainWindow : Window
	{
	    public MainWindow()
	    {
	        InitializeComponent();
			DataContext = new KeyDataGridViewModel();

			Width = 590;
			Height = 480;

			CanResize = false;
			SetTheme();
	    }

		private static async void SetTheme()
		{
			bool? theme = await JsonReader.GetTheme();
			if(theme is null) return;
			
			Application.Current!.RequestedThemeVariant = (bool)theme ?
				ThemeVariant.Dark : ThemeVariant.Light;
		}

		protected override void OnClosing(WindowClosingEventArgs e)
		{
			KeyDataGridViewModel.CloseThread();
			base.OnClosing(e);
		}

		private async void OpenFile(object source, RoutedEventArgs args)
		{
			string buttonClicked = (source as Control)!.Name!;
			int button = (int)char.GetNumericValue(buttonClicked[^1]);

			var storage = StorageProvider;
#if Windows
			string[]? osuFile = buttonClicked.Contains("SelectFile") ? await BeatmapPicker.GetOsuBeatmap(storage) : await BeatmapPicker.GetCurrentPlayingOsuBeatmap(storage);
#elif Linux
			string[]? osuFile = await BeatmapPicker.GetOsuBeatmap(storage);
#endif
			if(osuFile is null) return;
			if(!GetObjects.IsOsuManiaFile(osuFile))
			{
				KeyDataGridViewModel.UpdateKeyData(null!, button);
				Console.WriteLine("Selected file is not an osu!mania beatmap.");
				return;
			}

			KeyDataGridViewModel.UpdateKeyData(osuFile, button);
		}

		private void OpenAbout(object source, RoutedEventArgs args)
		{
			var AboutWindow = new About();
			AboutWindow.ShowDialog(this);
		}

		private void Exit(object source, RoutedEventArgs args) => Close();
	}
}