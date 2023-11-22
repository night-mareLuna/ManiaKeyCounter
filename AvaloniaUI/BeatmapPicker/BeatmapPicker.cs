using System.Diagnostics;
using Avalonia.Platform.Storage;
#if Windows
using KeyCounter.ViewModels;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels;
#endif

namespace KeyCounter;
public class BeatmapPicker
{
	public static async Task<string[]?> GetOsuBeatmap(IStorageProvider storage)
	{
		string? StrSongFolder = await TryGetSongsFolder();
		IStorageFolder? SongFolder = StrSongFolder!= null ? await storage.TryGetFolderFromPathAsync(new Uri(StrSongFolder)) : null;

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
#if Windows
	public static async Task<string[]?> GetCurrentPlayingOsuBeatmap(IStorageProvider storage)
	{
		StructuredOsuMemoryReader osu = StructuredOsuMemoryReader.Instance.GetInstanceForWindowTitleHint("");
		IStorageFile? osuFile = null;
		if(osu.CanRead)
		{
			var baseAddresses = new OsuBaseAddresses();
			osu.TryRead(baseAddresses.Beatmap);
			string osuSongsPath = TryGetSongsFolder();
			string currentBeatmap = baseAddresses.Beatmap.FolderName + '\\' + baseAddresses.Beatmap.OsuFileName;
			string fullBeatmapPath = osuSongsPath+currentBeatmap;

			osuFile = await storage.TryGetFileFromPathAsync(fullBeatmapPath);
		}
		else
		{
			Console.WriteLine("Unable to read current osu! process. Is osu! running?");
			KeyDataGridViewModel.TryConnectToOsu();
		}

		return await ProcessFile(osuFile!);
	}
#endif

	private static async Task<string?> TryGetSongsFolder()
	{
		string? fromJson = await JsonReader.GetLastSongFolderJSON();
		if(fromJson is not null) return fromJson;
#if Linux
		try
		{
			var process = new Process()
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "/bin/bash",
					Arguments = "-c \"pgrep osu\"",
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true,
				}
			};
			process.Start();
			string result = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			if(result=="") throw new Exception("Could not find a running osu! process.");
			else
			{
				process.StartInfo.Arguments = "-c \"cat /proc/`pgrep osu`/maps | grep -e osu!.exe\"";
				process.Start();
				result = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				string osuSongsPath = result[result.IndexOf('/')..result.LastIndexOf('/')]+"/Songs/";
				return osuSongsPath;
			}

		}
		catch(Exception e)
		{
			Console.WriteLine(e.Message);
			return null;
		}
#elif Windows
		try
		{
			var process = Process.GetProcessesByName("osu!").First();
			string osuSongsPath = process.MainModule!.FileName.Remove(process.MainModule!.FileName.Length-9) + "\\Songs\\";
			return osuSongsPath;
		}
		catch(Exception e)
		{
			Console.WriteLine(e.Message);
			return null;
		}
#endif
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
