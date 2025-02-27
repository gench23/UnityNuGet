﻿using System;
using System.Collections.Generic;
using System.Linq;
using Scriban;

namespace UnityNuGet
{
    /// <summary>
    /// Helper methods to create Unity .meta files
    /// </summary>
    internal static class UnityMeta
    {
        public static string GetMetaForExtension(Guid guid, string extension)
        {
            switch (extension)
            {
                case ".pdb":
                    break;

                case ".json":
                case ".xml":
                case ".txt":
                case ".md":
                    return GetMetaForText(guid);
            }

            return null;
        }

        public static string GetMetaForDll(Guid guid, IEnumerable<string> defineConstraints)
        {
            const string text = @"fileFormatVersion: 2
guid: {{ guid }}
PluginImporter:
  externalObjects: {}
  serializedVersion: 2
  iconMap: {}
  executionOrder: {}
{{ constraints }}  isPreloaded: 0
  isOverridable: 0
  isExplicitlyReferenced: 0
  validateReferences: 1
  platformData:
  - first:
      Any: 
    second:
      enabled: 1
      settings: {}
  - first:
      Editor: Editor
    second:
      enabled: 0
      settings:
        DefaultValueInitialized: true
  - first:
      Windows Store Apps: WindowsStoreApps
    second:
      enabled: 0
      settings:
        CPU: AnyCPU
  userData: 
  assetBundleName: 
  assetBundleVariant: 
";

            var allConstraints = defineConstraints.ToList();

            string FormatConstraints() => string.Join(
                string.Empty,
                allConstraints.Select(d => $"  - {d}\n"));

            return Template
                .Parse(text)
                .Render(new
                {
                    guid = guid.ToString("N"),
                    constraints = allConstraints.Count == 0
                        ? string.Empty
                        : $"  defineConstraints:\n{FormatConstraints()}"
                })
                .StripWindowsNewlines();
        }

        public static string GetMetaForFolder(Guid guid)
        {
            return $@"fileFormatVersion: 2
guid: {guid:N}
folderAsset: yes
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant:
".StripWindowsNewlines();
        }

        private static string GetMetaForText(Guid guid)
        {
            return $@"fileFormatVersion: 2
guid: {guid:N}
TextScriptImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
".StripWindowsNewlines();
        }

        private static string StripWindowsNewlines(this string input) => input.Replace("\r\n", "\n");
    }
}
