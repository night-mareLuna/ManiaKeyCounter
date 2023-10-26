using System.Text.Json;
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
			IStorageFolder? SongFolder = 
				await storage.TryGetFolderFromPathAsync(await GetLastSongFolderJSON());

			var file = await storage.OpenFilePickerAsync(new FilePickerOpenOptions()
			{
				Title = "Open osu!mania File",
				AllowMultiple = false,
				FileTypeFilter = new[] { OsuFile },
				SuggestedStartLocation = SongFolder
			});

			if(file.Count > 0)
			{
				// Check if file is less than 1mb in size
				if((await file[0].GetBasicPropertiesAsync()).Size < 1024 * 1024 * 2)
				{
					string? folder = await (await file[0].GetParentAsync()).SaveBookmarkAsync();
					if(folder!=null) SaveToJSON(folder);

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

		private static async void SaveToJSON(string folder)
		{
			string fileName = "config.json";

			var osuLastLocation = new OsuLastLocation
			{
				SongFolder = folder
			};

			try
			{
				using FileStream createStream = File.Create(fileName);
				await JsonSerializer.SerializeAsync(createStream, osuLastLocation);
				await createStream.DisposeAsync();
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private static async Task<string?> GetLastSongFolderJSON()
		{
			string fileName = "config.json";

			try
			{
				using FileStream openStream = File.OpenRead(fileName);
				OsuLastLocation? osuLastLocation = 
					await JsonSerializer.DeserializeAsync<OsuLastLocation>(openStream);
				return osuLastLocation?.SongFolder;
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				return null;
			}
		}

		private static FilePickerFileType OsuFile { get; } = new("osu! file format")
		{
			Patterns = new[] { "*.osu" }
		};
	}

	public class OsuLastLocation
    {
        public string? SongFolder { get; set; }
    }
}