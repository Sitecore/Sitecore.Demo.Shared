using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Sitecore.Demo.BuildTools.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sitecore.Demo.BuildTools
{
    public class MergeXdtTransforms : Task
    {
        [Required]
        public ITaskItem[] Transforms { get; set; }

        [Required]
        public string OutputPath { get; set; }

        [Output]
        public ITaskItem Output { get; set; }

        public override bool Execute()
        {
            if (Transforms == null || Transforms.Length == 0)
            {
                Log.LogError("No transforms were supplied");
                return false;
            }

            try
            {
                // group transforms by target config filename, so we can merge xml for each group
                var transformSets = new Dictionary<string, List<ITaskItem>>();

                foreach (var transform in Transforms)
                {
                    string transformTarget = GetTransformTarget(transform);

                    if (transformSets.ContainsKey(transformTarget))
                    {
                        if (transformSets[transformTarget] == null)
                        {
                            transformSets[transformTarget] = new List<ITaskItem>();
                        }

                        transformSets[transformTarget].Add(transform);
                    }
                    else
                    {
                        transformSets.Add(transformTarget, new List<ITaskItem> { transform });
                    }
                }

                foreach (var set in transformSets)
                {
                    var sourceTransforms = set.Value.Select(item => XmlUtility.ParseXmlDocument(item.ItemSpec));
                    var targetTransform = XmlUtility.Merge(sourceTransforms);
                    string fileOutput = string.Format("{0}{1}", OutputPath, set.Key);

                    targetTransform.Save(fileOutput);
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private string GetTransformTarget(ITaskItem transform)
        {
            string filename = transform.GetMetadata("Filename");
            string extension = transform.GetMetadata("Extension");

            //known filename formats
            // [name].config.xdt
            // [name].sc-internal.config.xdt
            // [name].azure.config.xdt

            filename = filename.Replace(".sc-internal", string.Empty).Replace(".azure", string.Empty);
            string groupKey = string.Format("{0}{1}", filename, extension);

            string relativePath = transform.GetMetadata("RelativeDir");
            Log.LogMessage(string.Format("Relative Path {0}", relativePath));

            // known paths
            // /modules/[ModuleName]/code/**/*.xdt
            // /transforms/**/*.xdt

            string[] patterns = new string[] {
                    @".*\\transforms\\(.*)",
                    @".*\\modules\\.*\\code\\(.*)"
                };

            foreach (string pattern in patterns)
            {
                var match = Regex.Match(relativePath, pattern);
                if (match.Success)
                {
                    string targetPath = match.Groups[1].Value;
                    groupKey = string.Format("{0}{1}", targetPath, groupKey);

                    // ensure directory structure is created
                    string targetDir = string.Format("{0}{1}", OutputPath, targetPath);
                    if (!Directory.Exists(targetDir))
                    {
                        Directory.CreateDirectory(targetDir);
                    }
                }
            }

            return groupKey;
        }
    }
}
