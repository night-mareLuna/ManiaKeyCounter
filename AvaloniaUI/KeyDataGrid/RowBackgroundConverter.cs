using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Styling;
using KeyCounter.ViewModels;
namespace KeyCounter;
public class RowBackgroundConverter : IValueConverter
{
	public static readonly RowBackgroundConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value != null && parameter != null)
        {
			Keys? keys = value as Keys;
			int button = int.Parse(parameter!.ToString()!);

			if(keys!.Lane==0)
				return new SolidColorBrush(Colors.Red);

			if(IsOutlier(keys!, button, 0.25))
				return new SolidColorBrush(Colors.Red);

            if(IsOutlier(keys!, button, 0.1))
                return new SolidColorBrush(Colors.Orange);

        }
        return Application.Current!.RequestedThemeVariant == ThemeVariant.Dark ? 
			new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);
    }

	private static List<Keys>? currentKeyData = null;
	private static int totalObjects = 0;
	private static bool IsOutlier(Keys keys, int button, double percentage = 0.1)
	{
		if(currentKeyData is null || !CompareKeysList(currentKeyData!, KeyDataGridViewModel.GetCurrentKeyData(button)!))
		{
			currentKeyData = KeyDataGridViewModel.GetCurrentKeyData(button);
			totalObjects = 0;

			foreach(Keys key in currentKeyData!)
			{
				totalObjects+=key.KeyCount;
			}
		}

		double consideredAveragePerKey = totalObjects / currentKeyData.Capacity;
		double percentageDifference = Math.Abs(consideredAveragePerKey - keys.KeyCount) / consideredAveragePerKey;
		
		return percentageDifference >= percentage;
	}

	private static bool CompareKeysList(List<Keys> list1, List<Keys> list2)
	{
		if(list1.Capacity!=list2.Capacity)
			return false;

		for(int i = 0; i < list1.Capacity; i++)
		{
			if(list1[i].KeyCount!=list2[i].KeyCount)
				return false;
		}
		return true;
	}

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}