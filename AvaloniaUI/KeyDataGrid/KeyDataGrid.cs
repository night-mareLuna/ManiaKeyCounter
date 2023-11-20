using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using OsuMemoryDataProvider;

namespace KeyCounter.ViewModels
{
	public partial class KeyDataGridViewModel : ObservableObject
	{
		[ObservableProperty]
		private ObservableCollection<Keys>? _KeyData1;
		[ObservableProperty]
		private ObservableCollection<Keys>? _KeyData2;
		[ObservableProperty]
		private string _DiffName1;
		[ObservableProperty]
		private string _DiffName2;
		[ObservableProperty]
		private bool _CanReadOsu = false;
		private static readonly string noResultYet = "No Result Calculated";

		private static KeyDataGridViewModel? It;
		public KeyDataGridViewModel()
		{
			It = this;

			DiffName1 = noResultYet;
			DiffName2 = noResultYet;

			CheckIsOsuOpen();
		}
		

		public static void UpdateKeyData(string[] osuFile, int button)
		{
			var keyData = new List<Keys>();
			string diffName;
			
			if(osuFile is not null)
			{
				int lanes = GetObjects.Lanes(osuFile);
				List<string> raw_objects = GetObjects.GetHitObjects(osuFile);
				diffName = GetObjects.DiffName(osuFile);

				int[] arrKeys = CountKeys.Keys(lanes, raw_objects);

				for(int i = 0; i < arrKeys.Length; i++)
				{
					keyData.Add(new Keys(i+1, arrKeys[i]));
				}
			}
			else
			{
				keyData.Add(new Keys(0,0));
				diffName = "Selected file is not an osu!mania beatmap.";
			}

			var ObsColKeyData = new ObservableCollection<Keys>(keyData);

			switch(button)
			{
				case 1:
					It!.KeyData1 = ObsColKeyData;
					It!.DiffName1 = diffName;
					break;
				case 2:
					It!.KeyData2 = ObsColKeyData;
					It!.DiffName2 = diffName;
					break;
				default:
					break;
			}
		}

		public static List<Keys>? GetCurrentKeyData(int button)
		{
			return button switch
			{
				1 => It!.KeyData1!.ToList(),
				2 => It!.KeyData2!.ToList(),
				_ => null
			};
		}

		public static void CheckIsOsuOpen()
		{
			It!.CanReadOsu = false;
			StructuredOsuMemoryReader osu = StructuredOsuMemoryReader.Instance.GetInstanceForWindowTitleHint("");
			Console.WriteLine("Trying to connect to osu!...");
			new Thread(delegate ()
			{
				while(!osu.CanRead)
				{
					Thread.Sleep(2000);
					continue;
				}

				Console.WriteLine("Connected to osu!");
				It!.CanReadOsu = true;
			}).Start();
		}
	}
}

