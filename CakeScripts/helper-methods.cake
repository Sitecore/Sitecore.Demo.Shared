
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
	cakeConsole.ResetColor();
}