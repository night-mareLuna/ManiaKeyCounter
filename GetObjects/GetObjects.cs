namespace KeyCounter;
public class GetObjects
{
	public static List<string> GetHitObjects(string[] osuFile)
	{
		List<string> hitObjects = new();
			
		bool foundObjects = false;
		for(int i = 0; i < osuFile.Length; i++)
		{
			string line = osuFile[i];
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
			{
				return int.Parse(line.Split(':')[1]);
			}
		}
		return 0;
	}
}