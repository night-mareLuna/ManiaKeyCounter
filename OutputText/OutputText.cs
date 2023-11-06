namespace KeyCounter;
public class OutputText
{
	public static string GetDisplayText(string[] osuFile, bool addDiffName = false)
	{
		string display_text = addDiffName ? $"{GetObjects.DiffName(osuFile)}\r" : "";
		List<string> raw_hitobjects = GetObjects.GetHitObjects(osuFile);
		int key_count = GetObjects.Lanes(osuFile);
		
		int[] lanecounter = CountKeys.Keys(key_count, raw_hitobjects);
		for(int i = 0; i < lanecounter.Length; i++)
		{
			display_text+=$"Key {i+1}: {lanecounter[i]}\r";
		}
		return display_text;
	}
}
