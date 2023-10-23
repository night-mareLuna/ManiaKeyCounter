namespace KeyCounter
{
    class Program
    {

		static void Main()
		{
			Console.WriteLine("Relative path to osu file: ");
			var file = Console.ReadLine();
			PrintKeyCount(file);
		}

		private static void PrintKeyCount(string file)
		{
			if(IsOsuManiaFile(file))
			{
				int[] lanecounter = CountLanes.Keys(GetObjects.Lanes(file), GetObjects.GetHitObjects(file));
				for(int i = 0; i < lanecounter.Length; i++)
				{
					Console.WriteLine($"Key {i+1}: {lanecounter[i]}");
				}
			}
			else
			{
				Console.WriteLine("Specified file is not an osu!mania file, could not find file or file does not exist.");
				return;
			}
		}

		private static bool IsOsuManiaFile(string file)
		{
			if(File.Exists(file))
			{
				if(file.Contains(".osu"))
				{
					string[] osuFile = File.ReadAllLines(file);
					if(!osuFile[0].Contains("osu file format")) return false;
					foreach(string line in osuFile)
					{
						if(line.Contains("Mode:"))
						{
							if(line[^1] == '3') return true;
							else return false;
						}
					}
				}
				else
				{
					return false;
				}
			}
			return false;
		}
    }
}