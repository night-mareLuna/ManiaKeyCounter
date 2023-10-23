namespace KeyCounter;
public class GetObjects
{
	public static List<string> GetHitObjects(string file)
	{
		try
		{
			string[] osuFile = File.ReadAllLines(file);
			List<string> hitObjects = new();
			
			bool foundObjects = false;
			for(int i = 0; i < osuFile.Length; i++)
			{
				if(foundObjects) hitObjects.Add(osuFile[i]);
				else
				{
					foundObjects = osuFile[i] == "[HitObjects]";
					continue;
				}
			}
			return hitObjects;
		}
		catch (IOException e)
		{
			Console.WriteLine(e.Message);
			return new();
		}
		
	}

	public static int Lanes(string file)
	{
		try
		{
			string[] osuFile = File.ReadAllLines(file);

			foreach(string line in osuFile)
			{
				if(line.Contains("CircleSize"))
				{
					return (int)Char.GetNumericValue(line[^1]);
				}
			}
		}
		catch (IOException e)
		{
			Console.WriteLine(e.Message);
		}
		return 0;
	}
}