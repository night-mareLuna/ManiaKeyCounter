using Avalonia.Controls;
using Avalonia.Interactivity;
using KeyCounter.ViewModels;

namespace KeyCounter
{
	public partial class MainWindow : Window
	{
	    public MainWindow()
	    {
	        InitializeComponent();
			DataContext = new KeyDataGridViewModel();

			Width = 600;
			Height = 500;

			CanResize = false;
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
	}
}