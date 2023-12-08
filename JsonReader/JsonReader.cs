using System.Text.Json;
namespace KeyCounter;
public class JsonReader
{
	public static async Task<string?> GetLastSongFolderJSON()
	{
		Config? json = await ReadJson();
		return json!.SongFolder;
	}

	public static async void SetLastSongFolderJSON(string folder)
	{
		Config? json = await ReadJson();
		if(json is not null)
			json.SongFolder = folder;
		else
			json = new Config
			{
				SongFolder = folder
			};

		SaveJson(json);
	}

	public static async Task<bool?> GetTheme()
	{
		Config? json = await ReadJson();
		return json?.DarkTheme;
	}

	public static async void SetTheme(bool theme)
	{
		Config? json = await ReadJson();
		if(json is not null)
			json.DarkTheme = theme;
		else
			json = new Config
			{
				DarkTheme = theme
			};

		SaveJson(json);
	}

	private static async Task<Config?> ReadJson()
	{
		string fileName = "config.json";
		try
		{
			using FileStream openStream = File.OpenRead(fileName);
			Config? json = 
				await JsonSerializer.DeserializeAsync<Config>(openStream);
			return json;
		}

		catch(Exception e)
		{
			Console.WriteLine(e.Message);
			return null;
		}
	}

	private static async void SaveJson(Config json)
	{
		string fileName = "config.json";
		try
		{
			using FileStream createStream = File.Create(fileName);
			await JsonSerializer.SerializeAsync(createStream, json);
			await createStream.DisposeAsync();
		}
		catch(Exception e)
		{
			Console.WriteLine(e);
		}
	}
}

public class Config
{
	public string? SongFolder { get; set; }
	public bool? DarkTheme { get; set; }
}
