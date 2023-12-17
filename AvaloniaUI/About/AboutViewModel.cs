using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using Octokit;

namespace KeyCounter.ViewModels;

public partial class AboutViewModel : ObservableObject
{
	[ObservableProperty]
	private string _CurrentVersion;
	[ObservableProperty]
	private string _LatestVersion;

	[ObservableProperty]
	private bool _IsDarkTheme;

	//URLs
	[ObservableProperty]
	private string _URLGitHub;
	[ObservableProperty]
	private string _URLAvaloniaUI;
	[ObservableProperty]
	private string _URLOsuMemoryDataProvider;
	[ObservableProperty]
	private string _URLOctokit;
	[ObservableProperty]
	private string _URLActipro;
	[ObservableProperty]
	private string _URLAvaloniaSvg;
	[ObservableProperty]
	private string _URLOpenedFolder;
	[ObservableProperty]
	private string _URLIcons8;


	public AboutViewModel()
	{
		URLGitHub = "https://github.com/night-mareLuna/ManiaKeyCounter";
		URLAvaloniaUI = "https://github.com/AvaloniaUI/Avalonia";
		URLOsuMemoryDataProvider = "https://github.com/Piotrekol/ProcessMemoryDataFinder";
		URLOctokit = "https://github.com/octokit/octokit.net";
		URLActipro = "https://github.com/Actipro/Avalonia-Controls";
		URLAvaloniaSvg = "https://github.com/wieslawsoltes/Svg.Skia";
		URLOpenedFolder = "https://icons8.com/icon/wrVhHgaitIQx/opened-folder";
		URLIcons8 = "https://icons8.com/";

		IsDarkTheme = Avalonia.Application.Current!.ActualThemeVariant == ThemeVariant.Dark;

		CurrentVersion = "v1.4.1";
		LatestVersion = GetLatestVersion();
	}

	private static string GetLatestVersion()
	{
		var github = new GitHubClient(new ProductHeaderValue("ManiaKeyCounter"));
		var releases = github.Repository.Release.GetLatest("night-mareLuna", "ManiaKeyCounter");
		var latest = releases.Result;
		
		return latest.TagName;
	}

	public void OpenLink(string link)
	{
		var psi = new System.Diagnostics.ProcessStartInfo
		{
			UseShellExecute = true,
			FileName = link
		};
		System.Diagnostics.Process.Start(psi);
	}

	public void SetTheme(bool darkTheme)
	{
		Avalonia.Application.Current!.RequestedThemeVariant =
			darkTheme ? ThemeVariant.Dark : ThemeVariant.Light;

		JsonReader.SetTheme(darkTheme);
		KeyDataGridViewModel.ThemeChange(darkTheme);
	}
}