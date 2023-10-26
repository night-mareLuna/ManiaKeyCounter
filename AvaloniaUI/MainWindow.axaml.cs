using Avalonia.Controls;
using Avalonia.Interactivity;

namespace KeyCounter
{
	public partial class MainWindow : Window
	{
	    public MainWindow()
	    {
	        InitializeComponent();
	    }

		public async void OpenFile(object source, RoutedEventArgs args)
		{
			var storage = StorageProvider;
			string[]? osuFile = await BeatmapPicker.GetOsuBeatmap(storage);
			key_counter.Text = GetObjects.IsOsuManiaFile(osuFile!) ? OutputText.GetDisplayText(osuFile!) : "This is not an osu!mania file.";
		}
	}
}