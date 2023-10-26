using System.Text.Json;
namespace KeyCounter;
public class JsonReader
{
	public static async Task<string?> GetLastSongFolderJSON()
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

	public static async void SaveToJSON(string folder)
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
}

public class OsuLastLocation
{
	public string? SongFolder { get; set; }
}
