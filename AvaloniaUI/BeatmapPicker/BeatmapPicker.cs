using Avalonia.Platform.Storage;

namespace KeyCounter;
public class BeatmapPicker
{
	public static async Task<string[]?> GetOsuBeatmap(IStorageProvider storage)
	{
		IStorageFolder? SongFolder = 
			await storage.TryGetFolderFromPathAsync(await JsonReader.GetLastSongFolderJSON());

		var file = await storage.OpenFilePickerAsync(new FilePickerOpenOptions()
		{
			Title = "Open osu!mania File",
			AllowMultiple = false,
			FileTypeFilter = new[] { OsuFile },
			SuggestedStartLocation = SongFolder
		});

		if(file.Count > 0)
		{
			// Check if file is less than 2mb in size
			if((await file[0].GetBasicPropertiesAsync()).Size < 1024 * 1024 * 2)
			{
				string? folder = await (await file[0].GetParentAsync()).SaveBookmarkAsync();
				if(folder!=null) JsonReader.SaveToJSON(folder);
					await using var readFile = await file[0].OpenReadAsync();
				using var reader = new StreamReader(readFile);
				string[] osuFile = reader.ReadToEnd().Split('\n');
				return osuFile;
			}
			else
			{
				throw new Exception("Unexpectedly large .osu file.");
			}
		}
		return null;
	}
	
	private static FilePickerFileType OsuFile { get; } = new("osu! beatmap file")
	{
		Patterns = new[] { "*.osu" }
	};
}
