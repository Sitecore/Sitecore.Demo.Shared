#addin nuget:?package=Cake.Json&version=3.0.1
#addin nuget:?package=Newtonsoft.Json&version=11.0.1

#load "local:?path=CakeScripts/helper-methods.cake"

var target = Argument<string>("Target", "Default");
var configuration = new Configuration();
var cakeConsole = new CakeConsole();

var configJsonFile = "cake-config.json";

Setup(context =>
{
  cakeConsole.ForegroundColor = ConsoleColor.Yellow;
  PrintHeader(ConsoleColor.DarkGreen);

  var configFile = new FilePath(configJsonFile);
  configuration = DeserializeJsonFromFile<Configuration>(configFile);
});

Task("Default")
.WithCriteria(configuration != null)
.IsDependentOn("CleanBuildFolders")
.IsDependentOn("Copy-Sitecore-Lib")
.IsDependentOn("Build-Solution");

Task("CleanBuildFolders").Does(() => {
  // Clean project build folders
  CleanDirectories($"{configuration.SourceFolder}/**/obj");
  CleanDirectories($"{configuration.SourceFolder}/**/bin");
});

Task("Copy-Sitecore-Lib")
  .WithCriteria(()=>(configuration.BuildConfiguration == "Local"))
  .Does(()=> {
    var files = GetFiles($"{configuration.SitecoreLibFolder}/Sitecore*.dll");
    var destination = "./lib";
    EnsureDirectoryExists(destination);
    CopyFiles(files, destination);
});

Task("Build-Solution")
.IsDependentOn("Copy-Sitecore-Lib")
.Does(() => {
  var solutionFiles = new string[] {"Sitecore.Demo.Shared.sln", "Cake.SitecoreDemo.sln"};

  foreach (var solution in solutionFiles) {
    Information($"Building :{solution}");
    NuGetRestore(solution);
    MSBuild(solution, cfg => InitializeMSBuildSettings(cfg));
  }
});

RunTarget(target);