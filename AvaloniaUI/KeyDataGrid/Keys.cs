namespace KeyCounter;
public class Keys
{
	public int Lane { get; set; }
	public int KeyCount { get; set; }

	public Keys(int lane, int keyCount)
	{
		Lane = lane;
		KeyCount = keyCount;
	}
}