using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Common.Tools.MSBuild;
using Cake.Common.Xml;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Http;
using Cake.Npm;
using Cake.Npm.Install;
using Cake.Npm.RunScript;
using Cake.Powershell;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;


[assembly: CakeNamespaceImport("System.Text.RegularExpressions")]
namespace Cake.SitecoreDemo
{
    public static class CakeSitecoreBuildTasks
    {
        private const string nodeModulesFolder = "node_modules";

        [CakeMethodAlias]
        public static void PublishFrontEndProject(this ICakeContext context, string sourceFolder, string publishFolder)
        {
            var frontEndDestination = $"{publishFolder}\\App_Data\\FrontEnd\\";
            context.EnsureDirectoryExists(frontEndDestination);
            context.Log.Information("Source: " + sourceFolder);
            context.Log.Information("Destination: " + frontEndDestination);

            var contentFiles = context.GetFiles($"{sourceFolder}\\**\\*")
              .Where(file => !file.FullPath.ToLower().Contains(nodeModulesFolder));

            context.CopyFiles(contentFiles, frontEndDestination, true);
        }

        [CakeMethodAlias]
        public static void PublishSourceProjects(this ICakeContext context, Configuration config, string sourceFolder, string[] publishFolders)
        {
            context.PublishSourceProjects(config, sourceFolder, publishFolders, "code");
        }

        [CakeMethodAlias]
        public static void PublishSourceProjects(this ICakeContext context, Configuration config, string sourceFolder, string[] publishFolders, string projectParentFolderName)
        {
            foreach (var publishFolder in publishFolders)
            {
                context.PublishProjects(config, sourceFolder, publishFolder, new string[] {}, projectParentFolderName);
            }
        }

        [CakeMethodAlias]
        public static void PublishCoreProject(this ICakeContext context, Configuration config, string projectFile, string publishFolder)
        {
            DotNetCoreRestore(context, projectFile, config.BuildConfiguration);
            DotNetCorePublish(context, projectFile, publishFolder, config.BuildConfiguration);
        }

        private static void DotNetCorePublish(ICakeContext context, string projectFile, string publishFolder, string buildConfiguration)
        {
            var settings = new DotNetCorePublishSettings
            {
                OutputDirectory = publishFolder,
                Configuration = buildConfiguration
            };

            context.DotNetCorePublish(projectFile, settings);
        }

        private static void DotNetCoreRestore(ICakeContext context, string projectFile, string buildConfiguration)
        {
            DotNetCoreMSBuildSettings buildSettings = new DotNetCoreMSBuildSettings();
            buildSettings.SetConfiguration(buildConfiguration);

            DotNetCoreRestoreSettings restoreSettings = new DotNetCoreRestoreSettings
            {
                MSBuildSettings = buildSettings
            };

            context.DotNetCoreRestore(projectFile, restoreSettings);
        }

        private static void CopyOtherOutputFilesToDestination(ICakeContext context, string sourceFolder, string publishFolder)
        {
            var ignoredExtensions = new[] { ".dll", ".exe", ".pdb", ".xdt", ".yml" };
            var ignoredFilesPublishFolderPath = sourceFolder.ToLower().Replace("\\", "/");
            var ignoredFiles = new[] {
                                    $"{ignoredFilesPublishFolderPath}/web.config",
                                    $"{ignoredFilesPublishFolderPath}/build.website.deps.json",
                                    $"{ignoredFilesPublishFolderPath}/build.website.exe.config",
                                    $"{ignoredFilesPublishFolderPath}/build.shared.deps.json",
                                    $"{ignoredFilesPublishFolderPath}/build.shared.exe.config"
                                  };

            var contentFiles = context.GetFiles($"{sourceFolder}\\**\\*")
                                .Where(file => !ignoredExtensions.Contains(file.GetExtension().ToLower()))
                                .Where(file => !ignoredFiles.Contains(file.FullPath.ToLower()));
            DirectoryPath directoryPath1 = new DirectoryPath(publishFolder);

            context.CopyFiles(contentFiles, directoryPath1, true);
        }

        private static void CopyAssemblyFilesToDestination(ICakeContext context, string sourceFolder, string publishFolder)
        {
            var assemblyFilesFilter = $@"{sourceFolder}\*.dll";
            var pdbFilesFilter = $@"{sourceFolder}\*.pdb";
            var assemblyFiles = context.GetFiles(assemblyFilesFilter).Select(x => x.FullPath).ToList();
            var pdbFiles = context.GetFiles(pdbFilesFilter).Select(x => x.FullPath).ToList();
            context.EnsureDirectoryExists(publishFolder + "\\bin");

            DirectoryPath directoryPath = new DirectoryPath(publishFolder + "\\bin");
            context.CopyFiles(assemblyFiles, directoryPath, preserveFolderStructure: false);
            context.CopyFiles(pdbFiles, directoryPath, preserveFolderStructure: false);
        }

        [CakeMethodAlias]
        public static void CopyToDestination(this ICakeContext context, string sourceFolder, string[] publishFolders)
        {
            foreach (var publishFolder in publishFolders )
            {
                if (!string.IsNullOrEmpty(publishFolder)) {

                    context.Log.Information("Destination: " + publishFolder);

                    // Copy assembly files to publish destination
                    CopyAssemblyFilesToDestination(context, sourceFolder, publishFolder);

                    // Copy other output files to publish destination
                    CopyOtherOutputFilesToDestination(context, sourceFolder, publishFolder);
                }
            }
        }

        [CakeMethodAlias]
        public static void CopySitecoreLib(this ICakeContext context, string sourceFolder)
        {
            var files = context.GetFiles($"{sourceFolder}/Sitecore*.dll");
            var destination = "./lib";
            context.EnsureDirectoryExists(destination);
            context.CopyFiles(files, destination);
        }

        [CakeMethodAlias]
        public static void PublishYML(this ICakeContext context, string itemsRoot, string publishFolder)
        {
            var serializationFilesFilter = $@"{itemsRoot}\**\*.yml";
            var destination = $@"{publishFolder}\yml";

            context.Log.Information($"Filter: {serializationFilesFilter} - Destination: {destination}");

            if (!context.DirectoryExists(destination))
            {
                context.CreateFolder(destination);
            }
            try
            {
                var files = context.GetFiles(serializationFilesFilter).Select(x => x.FullPath).ToList();
                context.CopyFiles(files, destination, preserveFolderStructure: true);
            }
            catch (Exception ex)
            {
                context.WriteError($"ERROR: {ex.Message}");
                context.Log.Information(ex.StackTrace);
            }
        }

        [CakeMethodAlias]
        public static void CreateUpdatePackage(this ICakeContext context, string publishFolder, string packagingScript)
        {
            context.StartPowershellFile(packagingScript, new PowershellSettings()
                .SetLogOutput()
                .WithArguments(args =>
                {
                    args.Append("target", $"{publishFolder}\\yml")
                        .Append("output", $"{publishFolder}\\update\\package.update");
                }));
        }

        [CakeMethodAlias]
        public static void GenerateDacpacs(this ICakeContext context, string sitecoreAzureToolkitPath, string sourceFolder, string publishFolder, string dacpacScript)
        {
            context.StartPowershellFile(dacpacScript, new PowershellSettings()
               .SetLogOutput()
               .WithArguments(args =>
               {
                   args.Append("SitecoreAzureToolkitPath", $"{sitecoreAzureToolkitPath}")
                  .Append("updatePackagePath", $"{sourceFolder}\\update\\package.update")
                  .Append("securityPackagePath", $"{sourceFolder}\\update\\security.dacpac")
                  .Append("destinationPath", $"{publishFolder}");
               }));
        }

        [CakeMethodAlias]
        public static void TurnOnUnicorn(this ICakeContext context, string publishFolder)
        {
            var webConfigFile = context.File($"{publishFolder}/web.config");
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
        public static void ModifyContentHubVariable(this ICakeContext context, string publishFolder, bool isContentHubEnabled)
        {
            var webConfigFile = context.File($"{publishFolder}/web.config");
            var xmlSetting = new XmlPokeSettings
            {
                Namespaces = new Dictionary<string, string> {
                    {"patch", @"http://www.sitecore.net/xmlconfig/"}
                }
            };

            var appSetting = "configuration/appSettings/add[@key='contenthub:define']/@value";
            var appSettingValue = isContentHubEnabled ? "Enabled" : "Disabled";
            context.XmlPoke(webConfigFile, appSetting, appSettingValue, xmlSetting);
        }

        [CakeMethodAlias]
        public static void ModifyPublishSettings(this ICakeContext context, string projectFolder, string instanceUrl)
        {
            var publishSettingsOriginal = context.File($"{projectFolder}/publishsettings.targets");
            var destination = $"{projectFolder}/publishsettings.targets.user";

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
            context.XmlPoke(destination, publishUrlPath, $"{instanceUrl}", xmlSetting);
        }

        [CakeMethodAlias]
        public static void SyncUnicorn(this ICakeContext context, string instanceUrl, string publishFolder, string unicornSyncScript)
        {
            var authenticationFile = new FilePath($"{publishFolder}/App_config/Include/Unicorn/Unicorn.zSharedSecret.config");
            var xPath = "/configuration/sitecore/unicorn/authenticationProvider/SharedSecret";
            string sharedSecret = context.XmlPeek(authenticationFile, xPath);

            context.SyncUnicornWithSecret(instanceUrl, unicornSyncScript, sharedSecret);
        }

        [CakeMethodAlias]
        public static void SyncUnicornWithSecret(this ICakeContext context, string instanceUrl, string unicornSyncScript, string unicornSharedSecret)
        {
            var unicornUrl = instanceUrl + "/unicorn.aspx";
            context.Log.Information("Sync Unicorn items from url: " + unicornUrl);

            context.StartPowershellFile(
                unicornSyncScript,
                new PowershellSettings()
                    .SetLogOutput()
                    .WithArguments(args =>
                        {
                            args.Append("secret", unicornSharedSecret)
                                .Append("url", unicornUrl);
                        }
                    )
            );
        }

        [CakeMethodAlias]
        public static void MergeAndCopyXmlTransform(this ICakeContext context, string[] layersFolders, string sourceFolder, string publishFolder)
        {
            context.MergeAndCopyXmlTransform(layersFolders, sourceFolder, publishFolder, "code");
        }

        [CakeMethodAlias]
        public static void MergeAndCopyXmlTransform(this ICakeContext context, string[] layersFolders, string sourceFolder, string publishFolder, string projectParentFolderName)
        {
            // Method will process all transforms from the temporary locations, merge them together and copy them to the temporary Publish\Web directory
            string[] excludePattern = {"ssl", "azure"};

            context.Log.Information($"Merging {sourceFolder}\\transforms to {publishFolder}");

            // Processing dotnet core transforms from NuGet references
            context.MergeTransforms($"{sourceFolder}\\transforms", "", $"{publishFolder}", excludePattern);

            // Processing project transformations

            foreach (var layer in layersFolders)
            {
                context.Log.Information($"Merging {layer} to {publishFolder}");
                context.MergeTransforms(layer, projectParentFolderName, publishFolder, excludePattern);
            }
        }

        [CakeMethodAlias]
        public static void ApplyXmlTransform(this ICakeContext context, string[] layersFolders, string publishFolder)
        {
            context.ApplyXmlTransform(layersFolders, publishFolder, "code");
        }

        [CakeMethodAlias]
        public static void ApplyXmlTransform(this ICakeContext context, string[] layersFolders, string publishFolder, string projectParentFolderName)
        {
            // Do not apply encryption algorithm change on IIS deployments
            string[] excludePattern = { "encryption" };
            foreach (var layer in layersFolders)
            {
                context.Transform(layer, projectParentFolderName, publishFolder, excludePattern);
            }
        }

        [CakeMethodAlias]
        public static void DeployMarketingDefinitions(this ICakeContext context, string instanceUrl, string marketingDefinitionsApiKey)
        {
            var url = $"{instanceUrl}/utilities/deploymarketingdefinitions.aspx?apiKey={marketingDefinitionsApiKey}";
            var responseBody = context.HttpGet(url, settings =>
            {
                settings.AppendHeader("Connection", "keep-alive");
            });

            context.Log.Information(responseBody);
        }

        [CakeMethodAlias]
        public static void ModifyUnicornSourceFolder(this ICakeContext context, string unicornSerializationFolder, string DevSettingsFile, string sourceFolderName)
        {
            var rootXPath = "configuration/sitecore/sc.variable[@name='{0}']/@value";
            var directoryPath = FileAliases.MakeAbsolute(context, unicornSerializationFolder).FullPath;
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
        public static void ApplyDotnetCoreTransforms(this ICakeContext context, string sourceFolder, string publishFolder)
        {
            string[] excludePattern = { "ssl", "azure", "encryption" };
            context.Transform(sourceFolder, "transforms", publishFolder, excludePattern);
        }

        [CakeMethodAlias]
        public static void FrontEndNpmInstall(this ICakeContext context, string frontEndPath)
        {
            var directories = context.GetDirectories(frontEndPath);
            foreach (var directory in directories)
            {
                // TODO: Remove when we clean up old themes in Platform
                if (directory.GetDirectoryName().StartsWith("-"))
                {
                    continue;
                }

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
        public static void FrontEndNpmBuild(this ICakeContext context, string frontEndPath)
        {
            var directories = context.GetDirectories(frontEndPath);
            foreach (var directory in directories)
            {
                // TODO: Remove when we clean up old themes in Platform
                if (directory.GetDirectoryName().StartsWith("-"))
                {
                    continue;
                }

                var settings = new NpmRunScriptSettings
                {
                    ScriptName = "build",
                    LogLevel = NpmLogLevel.Info,
                    WorkingDirectory = directory
                };
                context.NpmRunScript(settings);
            }
        }
    }
}