using Cake.Common.Tools.MSBuild;
using System;

namespace Cake.SitecoreDemo
{
    public class Configuration
    {
        private MSBuildToolVersion _msBuildToolVersion;

        private string _SourceFolder;
        private string _FoundationSrcFolder;
        private string _FeatureSrcFolder;
        private string _ProjectSrcFolder;
        private string _FrontEndFolder;

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

        public string SourceFolder {
            get {
                return _SourceFolder ?? $"{ProjectFolder}\\src";
            }
            set {
                _SourceFolder = value;
            }
        }
        public string FoundationSrcFolder {
            get {
                return _FoundationSrcFolder ?? $"{SourceFolder}\\Foundation";
            }
            set {
                _FoundationSrcFolder = value;
            }
        }
        public string FeatureSrcFolder {
            get {
                return _FeatureSrcFolder ?? $"{SourceFolder}\\Feature";
            }
            set {
                _FeatureSrcFolder = value;
            }
        }
        public string ProjectSrcFolder {
            get {
                return _ProjectSrcFolder ?? $"{SourceFolder}\\Project";
            }
            set {
                _ProjectSrcFolder = value;
            }
        }
        public string FrontEndFolder {
            get {
                return _FrontEndFolder ?? $"{ProjectFolder}\\FrontEnd";
            }
            set {
                _FrontEndFolder = value;
            }
        }

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