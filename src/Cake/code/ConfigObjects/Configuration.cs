using Cake.Common.Tools.MSBuild;
using System;

namespace Cake.SitecoreDemo
{
    public class Configuration
    {
        private MSBuildToolVersion _msBuildToolVersion;
        public string InstanceUrl { get; set; }
        public string SolutionName { get; set; }
        public string ProjectFolder { get; set; }
        public string BuildConfiguration { get; set; }
        public string MessageStatisticsApiKey { get; set; }
        public string MarketingDefinitionsApiKey { get; set; }
        public bool RunCleanBuilds { get; set; }
        public int DeployExmTimeout { get; set; }
        public string DeployFolder { get; set; }
        public string Version { get; set; }
        public string SitecoreAzureToolkitPath { get; set; }
        public string PublishTempFolder { get; set; }
        public string SolutionFile { get; set; }
        public string UnicornSerializationFolder { get; set; }
        public string BuildToolVersions
        {
            set
            {
                if (!Enum.TryParse(value, out _msBuildToolVersion))
                {
                    _msBuildToolVersion = MSBuildToolVersion.Default;
                }
            }
        }

        public string SourceFolder => $"{ProjectFolder}\\src";
        public string FoundationSrcFolder => $"{SourceFolder}\\Foundation";
        public string FeatureSrcFolder => $"{SourceFolder}\\Feature";
        public string ProjectSrcFolder => $"{SourceFolder}\\Project";
        public string PublishWebFolder { get; set; }
        public string PublishWebFolderCD { get; set; }
        public string PublishxConnectFolder { get; set; }
        public string PublishxConnectIndexWorkerFolder { get; set; }
        public string PublishDataFolder { get; set; }
        public bool IsContentHubEnabled { get; set; }

        public MSBuildToolVersion MSBuildToolVersion => this._msBuildToolVersion;
        public string BuildTargets => this.RunCleanBuilds ? "Clean;Build" : "Build";
    }
}