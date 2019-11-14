
public class Configuration
{
	private MSBuildToolVersion _msBuildToolVersion;

	public string ProjectFolder {get;set;}
	public string BuildConfiguration {get;set;}
	public bool RunCleanBuilds {get;set;}
	public string SitecoreLibFolder {get;set;}

	public string BuildToolVersions
	{
		set
		{
			if(!Enum.TryParse(value, out this._msBuildToolVersion)) {
				this._msBuildToolVersion = MSBuildToolVersion.Default;
			}
		}
	}

	public string SourceFolder => $"{ProjectFolder}\\src";
	public string FoundationSrcFolder => $"{SourceFolder}\\Foundation";
	public string FeatureSrcFolder => $"{SourceFolder}\\Feature";
	public string ProjectSrcFolder => $"{SourceFolder}\\Project";
	public MSBuildToolVersion MSBuildToolVersion => this._msBuildToolVersion;
	public string BuildTargets => this.RunCleanBuilds ? "Clean;Build" : "Build";
}

public MSBuildSettings InitializeMSBuildSettings(MSBuildSettings settings)
{
	InitializeMSBuildSettingsInternal(settings)
		.WithRestore();
	return settings;
}

private MSBuildSettings InitializeMSBuildSettingsInternal(MSBuildSettings settings)
{
	settings.SetConfiguration(configuration.BuildConfiguration)
		.WithRestore()
		.SetVerbosity(Verbosity.Minimal)
		.SetMSBuildPlatform(MSBuildPlatform.Automatic)
		.SetPlatformTarget(PlatformTarget.MSIL)
		.UseToolVersion(configuration.MSBuildToolVersion)
		.SetMaxCpuCount(8);
	return settings;
}


public void PrintHeader(ConsoleColor foregroundColor) {
	cakeConsole.ForegroundColor = foregroundColor;
	cakeConsole.WriteLine("     ");
	cakeConsole.WriteLine("     ");
	cakeConsole.WriteLine(@"   ) )       /\                  ");
	cakeConsole.WriteLine(@"  =====     /  \                 ");
	cakeConsole.WriteLine(@" _|___|____/ __ \____________    ");
	cakeConsole.WriteLine(@"|:::::::::/ ==== \:::::::::::|   ");
	cakeConsole.WriteLine(@"|:::::::::/ ====  \::::::::::|   ");
	cakeConsole.WriteLine(@"|::::::::/__________\:::::::::|  ");
	cakeConsole.WriteLine(@"|_________|  ____  |_________|                                                               ");
	cakeConsole.WriteLine(@"| ______  | / || \ | _______ |            _   _       _     _ _        _     _   _");
	cakeConsole.WriteLine(@"||  |   | | ====== ||   |   ||           | | | |     | |   (_) |      | |   | | | |");
	cakeConsole.WriteLine(@"||--+---| | |    | ||---+---||           | |_| | __ _| |__  _| |_ __ _| |_  | |_| | ___  _ __ ___   ___");
	cakeConsole.WriteLine(@"||__|___| | |   o| ||___|___||           |  _  |/ _` | '_ \| | __/ _` | __| |  _  |/ _ \| '_ ` _ \ / _ \");
	cakeConsole.WriteLine(@"|======== | |____| |=========|           | | | | (_| | |_) | | || (_| | |_  | | | | (_) | | | | | |  __/");
	cakeConsole.WriteLine(@"(^^-^^^^^- |______|-^^^--^^^)            \_| |_/\__,_|_.__/|_|\__\__,_|\__| \_| |_/\___/|_| |_| |_|\___|");
	cakeConsole.WriteLine(@"(,, , ,, , |______|,,,, ,, ,)");
	cakeConsole.WriteLine(@"','',,,,'  |______|,,,',',;;");
	cakeConsole.WriteLine(@"     ");
	cakeConsole.WriteLine(@"     ");
	cakeConsole.WriteLine(@" --------------------  ------------------");
	cakeConsole.WriteLine("   " + "The Habitat Home source code, tools and processes are examples of Sitecore Features.");
	cakeConsole.WriteLine("   " + "Habitat Home is not supported by Sitecore and should be used at your own risk.");
	cakeConsole.WriteLine("     ");
	cakeConsole.WriteLine("     ");
	cakeConsole.ResetColor();
}