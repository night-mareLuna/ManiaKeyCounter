using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

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
			var file = await storage.OpenFilePickerAsync(new FilePickerOpenOptions()
			{
				Title = "Open osu!mania File",
				AllowMultiple = false,
				FileTypeFilter = new[] { OsuFile }
			});

			if(file.Count > 0)
			{
				// Check if file is less than 1mb in size
				if((await file[0].GetBasicPropertiesAsync()).Size < 1024 * 1024 * 2)
				{
					await using var readFile = await file[0].OpenReadAsync();
					using var reader = new StreamReader(readFile);
					string[] osuFile = reader.ReadToEnd().Split('\n');

					key_counter.Text = IsOsuManiaFile(osuFile) ? GetDisplayText(osuFile) : "This is not an osu!mania file.";
				}
				else
				{
					throw new Exception("Unexpectedly large .osu file.");
				}
			}
		}

		private static string GetDisplayText(string[] osuFile)
		{
			string display_text = "";
			List<string> raw_hitobjects = GetObjects.GetHitObjects(osuFile);
			int key_count = GetObjects.Lanes(osuFile);
			
			int[] lanecounter = CountKeys.Keys(key_count, raw_hitobjects);
			for(int i = 0; i < lanecounter.Length; i++)
			{
				display_text+=$"Key {i+1}: {lanecounter[i]}\r";
			}

			return display_text;
		}

		private static bool IsOsuManiaFile(string[] osuFile)
		{
			if(!osuFile[0].Contains("osu file format")) return false;
			foreach(string line in osuFile)
			{
				if(line.Contains("Mode:"))
				{
					if(line.Contains('3')) return true;
					else return false;
				}
			}
			return false;
		}

		private static FilePickerFileType OsuFile { get; } = new("osu! file format")
		{
			Patterns = new[] { "*.osu" }
		};
	}
}