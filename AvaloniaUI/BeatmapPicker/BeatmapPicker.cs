using System.Diagnostics;
using Avalonia.Platform.Storage;
using KeyCounter.ViewModels;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels;

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
			return await ProcessFile(file[0]);

		return null;
	}

	public static async Task<string[]?> GetCurrentPlayingOsuBeatmap(IStorageProvider storage)
	{
		StructuredOsuMemoryReader osu = StructuredOsuMemoryReader.Instance.GetInstanceForWindowTitleHint("");
		IStorageFile? osuFile = null;
		if(osu.CanRead)
		{
			var process = Process.GetProcessesByName("osu!").First();
			var baseAddresses = new OsuBaseAddresses();
			osu.TryRead(baseAddresses.Beatmap);
			string osuSongsPath = process.MainModule!.FileName.Remove(process.MainModule!.FileName.Length-9) + "\\Songs\\";
			string currentBeatmap = baseAddresses.Beatmap.FolderName + '\\' + baseAddresses.Beatmap.OsuFileName;
			string fullBeatmapPath = osuSongsPath+currentBeatmap;

			osuFile = await storage.TryGetFileFromPathAsync(fullBeatmapPath);
		}
		else
		{
			Console.WriteLine("Unable to read current osu! process. Is osu! running?");
			KeyDataGridViewModel.CheckIsOsuOpen();
		}

		return await ProcessFile(osuFile!);
	}

	private static async Task<string[]?> ProcessFile(IStorageFile file)
	{
		if(file is null)
			return null;

		// Check if file is less than 2mb in size
		if((await file.GetBasicPropertiesAsync()).Size < 1024 * 1024 * 2)
		{
			string? folder = await (await file.GetParentAsync()).SaveBookmarkAsync();
			if(folder!=null) JsonReader.SaveToJSON(folder);
				await using var readFile = await file.OpenReadAsync();
			using var reader = new StreamReader(readFile);
			string[] osuFile = reader.ReadToEnd().Split('\n');
			return osuFile;
		}
		else
		{
			Console.WriteLine("Unexpectedly large .osu file");
			return null;
		}
	}
	
	private static FilePickerFileType OsuFile { get; } = new("osu! beatmap file")
	{
		Patterns = new[] { "*.osu" }
	};
}
