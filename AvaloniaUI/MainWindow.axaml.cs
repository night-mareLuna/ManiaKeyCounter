using Avalonia.Controls;
using Avalonia.Interactivity;

namespace KeyCounter
{
	public partial class MainWindow : Window
	{
	    public MainWindow()
	    {
	        InitializeComponent();
			Width = 600;
			Height = 400;

			MinWidth = 600;
			MinHeight = 400;

			MaxWidth = 600;
			MaxHeight = 400;
	    }

		public void OpenFile1(object source, RoutedEventArgs args)
		{
			OpenFile(1);
		}

		public void OpenFile2(object source, RoutedEventArgs args)
		{
			OpenFile(2);
		}

		private async void OpenFile(int button)
		{
			var storage = StorageProvider;
			string[]? osuFile = await BeatmapPicker.GetOsuBeatmap(storage);
			if(osuFile is null) return;
			
			switch(button)
			{
				case 1:
					key_counter_1.Text = GetObjects.IsOsuManiaFile(osuFile!) ? OutputText.GetDisplayText(osuFile!, true) : "This is not an osu!mania file.";
					break;
				case 2:
					key_counter_2.Text = GetObjects.IsOsuManiaFile(osuFile!) ? OutputText.GetDisplayText(osuFile!, true) : "This is not an osu!mania file.";
					break;
				default:
					break;
			}
			
		}
	}
}