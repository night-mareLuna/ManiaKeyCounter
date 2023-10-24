namespace KeyCounter
{
    class Program
    {

		static void Main()
		{
			Console.WriteLine("Relative path to osu file: ");
			var file = Console.ReadLine();
			PrintKeys.PrintKeyCount(file);
		}
    }
}