namespace KeyCounter;
public class GetObjects
{
	public static List<string> GetHitObjects(string[] osuFile)
	{
		List<string> hitObjects = new();
			
		bool foundObjects = false;
		for(int i = 0; i < osuFile.Length; i++)
		{
			if(foundObjects) hitObjects.Add(osuFile[i]);
			else
			{
				foundObjects = osuFile[i].Contains("[HitObjects]");
				continue;
			}
		}
		return hitObjects;
		
	}

	public static int Lanes(string[] osuFile)
	{
		foreach(string line in osuFile)
		{
			if(line.Contains("CircleSize"))
				return int.Parse(line.Split(':')[1]);
		}
		return 0;
	}

	public static string DiffName(string[] osuFile)
	{
		foreach(string line in osuFile)
		{
			if(line.Contains("Version"))
			{
				string diffName = line.Remove(0, line.Split(':')[0].Length + 1);
				diffName = diffName.Contains('\r') ? diffName.Remove(diffName.IndexOf('\r')) : diffName;

				return diffName;

			}
		}
		return "";
	}

	public static bool IsOsuManiaFile(string[] osuFile)
	{
		if(osuFile==null) return false;
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
}