using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Common.Xml;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Http;
using Cake.Powershell;
using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Common.Diagnostics;
using Cake.Npm;
using Cake.Npm.Install;
using Cake.Npm.RunScript;

[assembly: CakeNamespaceImport("System.Text.RegularExpressions")]
namespace Cake.SitecoreDemo
{
    public static class CakeSitecoreBuildTasks
    {
        private const string FrontendDirectoryPath = "./FrontEnd/*";
        private const string nodeModulesFolder = "node_modules";

        [CakeMethodAlias]
        public static void PublishXConnectProjects(this ICakeContext context, bool publishLocal, Configuration config)
        {
            var xConnectProject = $"{config.ProjectSrcFolder}\\xConnect";
            var destination = config.XConnectRoot;

            if (publishLocal)
            {
                destination = config.PublishxConnectFolder;
            }

            context.PublishProjects(xConnectProject, destination, config);
        }

        [CakeMethodAlias]
        public static void PublishFrontEndProject(this ICakeContext context, bool publishLocal, Configuration config)
        {
            var source = $"{config.ProjectFolder}\\FrontEnd";
            var destination = GetDestination(publishLocal, config);

            destination = $"{destination}\\App_Data\\FrontEnd\\";
            context.EnsureDirectoryExists(destination);
            context.Log.Information("Source: " + source);
            context.Log.Information("Destination: " + destination);

            var contentFiles = context.GetFiles($"{source}\\**\\*")
                .Where(file => !file.FullPath.ToLower().Contains(nodeModulesFolder));

            context.CopyFiles(contentFiles, destination, true);
        }

        [CakeMethodAlias]
        public static void PublishSourceProjects(this ICakeContext context, bool publishLocal, string srcFolder, Configuration config)
        {
            var destination = GetDestination(publishLocal, config);
            context.PublishProjects(srcFolder, destination, config);
        }

        [CakeMethodAlias]
        public static void PublishCoreProject(this ICakeContext context, string projectFile, bool publishLocal, Configuration config)
        {
            var publishFolder = $"{config.PublishTempFolder}";
            DotNetCoreRestore(context, config, projectFile);
            DotNetCorePublish(context, config, projectFile, publishFolder);
        }

        private static void DotNetCorePublish(ICakeContext context, Configuration config, string projectFile, string publishFolder)
        {
            var settings = new DotNetCorePublishSettings
            {
                OutputDirectory = publishFolder,
                Configuration = config.BuildConfiguration
            };

            context.DotNetCorePublish(projectFile, settings);
        }

        private static void DotNetCoreRestore(ICakeContext context, Configuration config, string projectFile)
        {
            DotNetCoreMSBuildSettings buildSettings = new DotNetCoreMSBuildSettings();
            buildSettings.SetConfiguration(config.BuildConfiguration);

            DotNetCoreRestoreSettings restoreSettings = new DotNetCoreRestoreSettings
            {
                MSBuildSettings = buildSettings
            };

            context.DotNetCoreRestore(projectFile, restoreSettings);
        }

        private static void CopyOtherOutputFilesToDestination(ICakeContext context, string destination, string publishFolder)
        {
            var ignoredExtensions = new[] { ".dll", ".exe", ".pdb", ".xdt", ".yml" };
            var ignoredFilesPublishFolderPath = publishFolder.ToLower().Replace("\\", "/");
            var ignoredFiles = new[] {
                                    $"{ignoredFilesPublishFolderPath}/web.config",
                                    $"{ignoredFilesPublishFolderPath}/build.website.deps.json",
                                    $"{ignoredFilesPublishFolderPath}/build.website.exe.config",
                                    $"{ignoredFilesPublishFolderPath}/build.shared.deps.json",
                                    $"{ignoredFilesPublishFolderPath}/build.shared.exe.config"
                                  };

            var contentFiles = context.GetFiles($"{publishFolder}\\**\\*")
                                .Where(file => !ignoredExtensions.Contains(file.GetExtension().ToLower()))
                                .Where(file => !ignoredFiles.Contains(file.FullPath.ToLower()));
            DirectoryPath directoryPath1 = new DirectoryPath(destination);

            context.CopyFiles(contentFiles, directoryPath1, true);
        }

        private static void CopyAssemblyFilesToDestination(ICakeContext context, string destination, string publishFolder)
        {
            var assemblyFilesFilter = $@"{publishFolder}\*.dll";
            var assemblyFiles = context.GetFiles(assemblyFilesFilter).Select(x => x.FullPath).ToList();
            context.EnsureDirectoryExists(destination + "\\bin");

            DirectoryPath directoryPath = new DirectoryPath(destination + "\\bin");
            context.CopyFiles(assemblyFiles, directoryPath, preserveFolderStructure: false);
        }

        [CakeMethodAlias]
        public static void CopyToDestination(this ICakeContext context, bool publishLocal, Configuration config)
        {

            string[] destinations = {GetDestination(publishLocal, config), GetDestinationCD(publishLocal, config)};

            foreach (string destination in destinations ) 
            {
                if (!string.IsNullOrEmpty(destination)) {
                    
                    var publishFolder = $"{config.PublishTempFolder}";
                    context.Log.Information("Destination: " + destination);

                    // Copy assembly files to publish destination
                    CopyAssemblyFilesToDestination(context, destination, publishFolder);

                    // Copy other output files to publish destination
                    CopyOtherOutputFilesToDestination(context, destination, publishFolder);
                } 
            }
        }

        [CakeMethodAlias]
        public static void CopySitecoreLib(this ICakeContext context, Configuration config)
        {
            var files = context.GetFiles($"{config.WebsiteRoot}/bin/Sitecore*.dll");
            var destination = "./lib";
            context.EnsureDirectoryExists(destination);
            context.CopyFiles(files, destination);
        }

        [CakeMethodAlias]
        public static void PublishYML(this ICakeContext context, Configuration config)
        {
            var serializationFilesFilter = $@"{config.ProjectFolder}\items\**\*.yml";
            var destination = $@"{config.PublishTempFolder}\yml";

            context.Log.Information($"Filter: {serializationFilesFilter} - Destination: {destination}");

            Func<IFileSystemInfo, bool> exclude_build_folder = fileSystemInfo => !fileSystemInfo.Path.FullPath.Contains("Build");

            if (!context.DirectoryExists(destination))
            {
                context.CreateFolder(destination);
            }
            try
            {
                var files = context.GetFiles(serializationFilesFilter, new GlobberSettings { Predicate = exclude_build_folder })
                  .Select(x => x.FullPath).ToList();

                context.CopyFiles(files, destination, preserveFolderStructure: true);
            }
            catch (Exception ex)
            {
                context.WriteError($"ERROR: {ex.Message}");
                context.Log.Information(ex.StackTrace);
            }
        }

        [CakeMethodAlias]
        public static void CreateUpdatePackage(this ICakeContext context, Configuration config, string packagingScript)
        {
            context.StartPowershellFile(packagingScript, new PowershellSettings()
                .SetLogOutput()
                .WithArguments(args =>
                {
                    args.Append("target", $"{config.PublishTempFolder}\\yml")
                        .Append("output", $"{config.PublishTempFolder}\\update\\package.update");
                }));
        }

        [CakeMethodAlias]
        public static void GenerateDacpacs(this ICakeContext context, Configuration config, string dacpacScript)
        {
            context.StartPowershellFile(dacpacScript, new PowershellSettings()
               .SetLogOutput()
               .WithArguments(args =>
               {
                   args.Append("SitecoreAzureToolkitPath", $"{config.SitecoreAzureToolkitPath}")
                  .Append("updatePackagePath", $"{config.PublishTempFolder}\\update\\package.update")
                  .Append("securityPackagePath", $"{config.PublishTempFolder}\\update\\security.dacpac")
                  .Append("destinationPath", $"{config.PublishDataFolder}");
               }));
        }

        [CakeMethodAlias]
        public static void TurnOnUnicorn(this ICakeContext context, Configuration config)
        {
            var webConfigFile = context.File($"{config.WebsiteRoot}/web.config");
            var xmlSetting = new XmlPokeSettings
            {
                Namespaces = new Dictionary<string, string> {
                    {"patch", @"http://www.sitecore.net/xmlconfig/"}
                }
            };

            var unicornAppSettingXPath = "configuration/appSettings/add[@key='unicorn:define']/@value";
            context.XmlPoke(webConfigFile, unicornAppSettingXPath, "Enabled", xmlSetting);
        }

        [CakeMethodAlias]
        public static void ModifyPublishSettings(this ICakeContext context, Configuration config)
        {
            var publishSettingsOriginal = context.File($"{config.ProjectFolder}/publishsettings.targets");
            var destination = $"{config.ProjectFolder}/publishsettings.targets.user";

            context.CopyFile(publishSettingsOriginal, destination);

            var importXPath = "/ns:Project/ns:Import";
            var publishUrlPath = "/ns:Project/ns:PropertyGroup/ns:publishUrl";

            var xmlSetting = new XmlPokeSettings
            {
                Namespaces = new Dictionary<string, string> {
                    {"ns", @"http://schemas.microsoft.com/developer/msbuild/2003"}
                }
            };

            context.XmlPoke(destination, importXPath, null, xmlSetting);
            context.XmlPoke(destination, publishUrlPath, $"{config.InstanceUrl}", xmlSetting);
        }

        [CakeMethodAlias]
        public static void SyncUnicorn(this ICakeContext context, Configuration config, string unicornSyncScript)
        {
            var unicornUrl = config.InstanceUrl + "/unicorn.aspx";
            context.Log.Information("Sync Unicorn items from url: " + unicornUrl);

            var authenticationFile = new FilePath($"{config.WebsiteRoot}/App_config/Include/Unicorn/Unicorn.zSharedSecret.config");
            var xPath = "/configuration/sitecore/unicorn/authenticationProvider/SharedSecret";
            string sharedSecret = context.XmlPeek(authenticationFile, xPath);

            context.StartPowershellFile(unicornSyncScript, new PowershellSettings()
                      .SetLogOutput()
                      .WithArguments(args =>
                      {
                          args.Append("secret", sharedSecret)
                  .Append("url", unicornUrl);
                      }));
        }

        [CakeMethodAlias]
        public static void MergeAndCopyXmlTransform(this ICakeContext context, Configuration config)
        {
            // Method will process all transforms from the temporary locations, merge them together and copy them to the temporary Publish\Web directory
            string[] excludePattern = { "ssl", "azure" };
            var PublishTempFolder = $"{config.PublishTempFolder}";
            var publishFolder = $"{config.PublishWebFolder}";

            context.Log.Information($"Merging {PublishTempFolder}\\transforms to {publishFolder}");

            // Processing dotnet core transforms from NuGet references
            context.MergeTransforms($"{PublishTempFolder}\\transforms", $"{publishFolder}", excludePattern);

            // Processing project transformations
            var layers = new [] { config.FoundationSrcFolder, config.FeatureSrcFolder, config.ProjectSrcFolder };

            foreach (var layer in layers)
            {
                context.Log.Information($"Merging {layer} to {publishFolder}");
                context.MergeTransforms(layer, publishFolder, excludePattern);
            }
        }

        [CakeMethodAlias]
        public static void ApplyXmlTransform(this ICakeContext context, Configuration config, bool publishLocal)
        {
            var layers = new [] { config.FoundationSrcFolder, config.FeatureSrcFolder, config.ProjectSrcFolder };
            var publishDestination = config.WebsiteRoot;

            if (publishLocal)
            {
                publishDestination = config.PublishWebFolder;
            }

            foreach (var layer in layers)
            {
                context.Transform(layer, "code", publishDestination, null);
            }
        }

        [CakeMethodAlias]
        public static void DeployMarketingDefinitions(this ICakeContext context, Configuration config)
        {
            var url = $"{config.InstanceUrl}/utilities/deploymarketingdefinitions.aspx?apiKey={config.MarketingDefinitionsApiKey}";
            var responseBody = context.HttpGet(url, settings =>
            {
                settings.AppendHeader("Connection", "keep-alive");
            });

            context.Log.Information(responseBody);
        }

        [CakeMethodAlias]
        public static void ModifyUnicornSourceFolder(this ICakeContext context, Configuration config, string DevSettingsFile, string sourceFolderName)
        {
            var rootXPath = "configuration/sitecore/sc.variable[@name='{0}']/@value";
            var directoryPath = FileAliases.MakeAbsolute(context, config.UnicornSerializationFolder).FullPath;
            var sourceFolderXPath = string.Format(rootXPath, sourceFolderName);

            var xmlSetting = new XmlPokeSettings
            {
                Namespaces = new Dictionary<string, string> {
                    {"patch", @"http://www.sitecore.net/xmlconfig/"}
                }
            };

            context.XmlPoke(DevSettingsFile, sourceFolderXPath, directoryPath, xmlSetting);
        }

        [CakeMethodAlias]
        public static void ApplyDotnetCoreTransforms(this ICakeContext context, Configuration config, bool publishLocal)
        {
            var publishFolder = $"{config.PublishTempFolder}";
            var destination = config.WebsiteRoot;
            if (publishLocal)
            {
                destination = config.PublishWebFolder;
            }
            string[] excludePattern = { "ssl", "azure" };
            context.Transform(publishFolder, "transforms", destination, excludePattern);
        }

        [CakeMethodAlias]
        public static void FrontEndNpmInstall(this ICakeContext context)
        {
            var directories = context.GetDirectories(FrontendDirectoryPath);
            foreach (var directory in directories)
            {
                var settings = new NpmInstallSettings
                {
                    LogLevel = NpmLogLevel.Warn, 
                    WorkingDirectory = directory
                };
                context.NpmInstall(settings);

                var template = System.IO.Path.Combine(directory.ToString(), "npm-shrinkwrap.template.json");
                var shrinkwrap = System.IO.Path.Combine(directory.ToString(), "npm-shrinkwrap.json");
                context.DeleteFile(shrinkwrap);
                context.CopyFile(template, shrinkwrap);
            }
        }

        [CakeMethodAlias]
        public static void FrontEndNpmBuild(this ICakeContext context)
        {
            var directories = context.GetDirectories(FrontendDirectoryPath);
            foreach (var directory in directories)
            {
                var settings = new NpmRunScriptSettings
                {
                    ScriptName = "build",
                    LogLevel = NpmLogLevel.Info,
                    WorkingDirectory = directory
                };
                context.NpmRunScript(settings);
            }
        }

        private static string GetDestinationCD(bool publishLocal, Configuration config)
        {
            return GetDestination(publishLocal, true, config);
        }

        private static string GetDestination(bool publishLocal, Configuration config)
        {
            return GetDestination(publishLocal, false, config);
        }

        private static string GetDestination(bool publishLocal, bool cdTarget, Configuration config)
        {
            var destination = config.WebsiteRoot;
            if (publishLocal)
            {
                if (cdTarget) {
                    destination = config.PublishWebFolderCD;
                } else {
                    destination = config.PublishWebFolder;
                }
            }

            return destination;
        }
    }
}