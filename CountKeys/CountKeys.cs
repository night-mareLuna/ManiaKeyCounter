﻿namespace KeyCounter;
public class CountKeys
{
	public static int[] Keys(int lanes, List<string> file)
	{
		// Return an int[] (the size of the key count) for the amount of keys in each lane.
		int[] laneCount = new int[lanes];

		foreach(string line in file)
		{
			string raw_lane_string = line.Split(',')[0];
			if (int.TryParse(raw_lane_string, out int raw_lane))
			{
				double lane = raw_lane * lanes / 512;
				lane = Math.Floor(Math.Clamp(lane, 0, lanes - 1));
				laneCount[(int)lane]++;
			}
		}
		return laneCount;
	}
}
