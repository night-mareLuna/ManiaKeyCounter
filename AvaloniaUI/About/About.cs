using Avalonia.Controls;
using KeyCounter.ViewModels;

namespace KeyCounter;

public partial class About : Window
{
	public About()
	{
		InitializeComponent();
		DataContext = new AboutViewModel();

		Width = 300;
		Height = 300;
		CanResize = false;
	}
}