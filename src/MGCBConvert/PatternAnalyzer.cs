namespace MGCBConvert;

public static class PatternAnalyzer
{
    public static List<ContentGroup> AnalyzeAndGroup(List<MGCBContentItem> items)
    {
        List<ContentGroup> groups = new List<ContentGroup>();

        Logger.LogVerbose("Analyzing content patterns...");

        // Group by: CopyAction, Importer, Processor, Parameters
        Dictionary<string, List<MGCBContentItem>> itemGroups = new Dictionary<string, List<MGCBContentItem>>();

        for (int i = 0; i < items.Count; i++)
        {
            MGCBContentItem item = items[i];

            List<string> paramPairs = new List<string>();
            foreach (KeyValuePair<string, string> param in item.ProcessorParameters)
            {
                paramPairs.Add($"{param.Key}={param.Value}");
            }
            paramPairs.Sort();
            string parametersString = string.Join("|", paramPairs);

            string groupKey = $"{item.IsCopyAction}|{item.ImporterName ?? ""}|{item.ProcessorName ?? ""}|{parametersString}";

            if (!itemGroups.ContainsKey(groupKey))
            {
                itemGroups[groupKey] = new List<MGCBContentItem>();
            }
            itemGroups[groupKey].Add(item);
        }

        foreach (KeyValuePair<string, List<MGCBContentItem>> group in itemGroups)
        {
            List<MGCBContentItem> itemList = group.Value;

            string[] keyParts = group.Key.Split('|');
            string importerName = string.IsNullOrEmpty(keyParts[1]) ? "default" : keyParts[1];
            string processorName = string.IsNullOrEmpty(keyParts[2]) ? "default" : keyParts[2];

            Logger.LogVerbose($"  Processing group with {itemList.Count} items (Importer: {importerName}, Processor: {processorName})");

            if (itemList.Count == 1)
            {
                // Single item - no optimization needed
                MGCBContentItem item = itemList[0];
                groups.Add(new ContentGroup
                {
                    Items = itemList,
                    Pattern = item.SourcePath ?? "",
                    UseWildcard = false,
                    IsCopyAction = item.IsCopyAction,
                    ImporterName = item.ImporterName,
                    ProcessorName = item.ProcessorName,
                    ProcessorParameters = new Dictionary<string, string>(item.ProcessorParameters)
                });
                continue;
            }

            List<ContentGroup> patterns = FindPatterns(itemList);
            for (int i = 0; i < patterns.Count; i++)
            {
                groups.Add(patterns[i]);
            }
        }

        return groups;
    }

    private static List<ContentGroup> FindPatterns(List<MGCBContentItem> items)
    {
        List<ContentGroup> groups = new List<ContentGroup>();
        List<MGCBContentItem> remaining = new List<MGCBContentItem>(items);

        // Strategy 1: Group by directory and file extension
        Dictionary<string, List<MGCBContentItem>> byDirectory = new Dictionary<string, List<MGCBContentItem>>();

        for (int i = 0; i < items.Count; i++)
        {
            MGCBContentItem item = items[i];
            string path = item.SourcePath ?? "";
            string dir = Path.GetDirectoryName(path) ?? "";
            string ext = Path.GetExtension(path);
            string key = $"{dir}|{ext}";

            if (!byDirectory.ContainsKey(key))
            {
                byDirectory[key] = new List<MGCBContentItem>();
            }
            byDirectory[key].Add(item);
        }

        foreach (KeyValuePair<string, List<MGCBContentItem>> dirGroup in byDirectory)
        {
            if (dirGroup.Value.Count < 2)
            {
                continue;
            }

            string[] keyParts = dirGroup.Key.Split('|');
            string directory = keyParts[0];
            string extension = keyParts[1];
            List<MGCBContentItem> groupItems = dirGroup.Value;

            // Check if all files in this directory with this extension are included
            // Normalize directory path to use forward slashes for cross-platform compatibility
            string normalizedDirectory = string.IsNullOrEmpty(directory) ? "" : directory.Replace('\\', '/');
            string pattern = string.IsNullOrEmpty(normalizedDirectory)
                             ? $"*{extension}"
                             : $"{normalizedDirectory}/*{extension}";

            Logger.LogVerbose($"    Found pattern: {pattern} ({groupItems.Count} items)");

            MGCBContentItem firstItem = groupItems[0];
            groups.Add(new ContentGroup
            {
                Items = groupItems,
                Pattern = pattern,
                UseWildcard = true,
                IsCopyAction = firstItem.IsCopyAction,
                ImporterName = firstItem.ImporterName,
                ProcessorName = firstItem.ProcessorName,
                ProcessorParameters = new Dictionary<string, string>(firstItem.ProcessorParameters)
            });

            for (int j = 0; j < groupItems.Count; j++)
            {
                remaining.Remove(groupItems[j]);
            }
        }

        // Strategy 2: Group by file extension only (scattered files)
        if (remaining.Count > 0)
        {
            Dictionary<string, List<MGCBContentItem>> byExtension = new Dictionary<string, List<MGCBContentItem>>();

            for (int i = 0; i < remaining.Count; i++)
            {
                MGCBContentItem item = remaining[i];
                string extension = Path.GetExtension(item.SourcePath ?? "");

                if (!byExtension.ContainsKey(extension))
                {
                    byExtension[extension] = new List<MGCBContentItem>();
                }
                byExtension[extension].Add(item);
            }

            foreach (KeyValuePair<string, List<MGCBContentItem>> extGroup in byExtension)
            {
                if (extGroup.Value.Count < 4) continue; // Need at least 4 scattered items to make it worthwhile

                string extension = extGroup.Key;
                List<MGCBContentItem> groupItems = extGroup.Value;
                string pattern = $"**/*{extension}"; // Recursive pattern

                Logger.LogVerbose($"    Found recursive pattern: {pattern} ({groupItems.Count} items)");

                MGCBContentItem firstItem = groupItems[0];
                groups.Add(new ContentGroup
                {
                    Items = groupItems,
                    Pattern = pattern,
                    UseWildcard = true,
                    IsCopyAction = firstItem.IsCopyAction,
                    ImporterName = firstItem.ImporterName,
                    ProcessorName = firstItem.ProcessorName,
                    ProcessorParameters = new Dictionary<string, string>(firstItem.ProcessorParameters)
                });

                for (int j = 0; j < groupItems.Count; j++)
                {
                    remaining.Remove(groupItems[j]);
                }
            }
        }

        // Add remaining items individually
        for (int i = 0; i < remaining.Count; i++)
        {
            MGCBContentItem item = remaining[i];
            groups.Add(new ContentGroup
            {
                Items = new List<MGCBContentItem> { item },
                Pattern = item.SourcePath ?? "",
                UseWildcard = false,
                IsCopyAction = item.IsCopyAction,
                ImporterName = item.ImporterName,
                ProcessorName = item.ProcessorName,
                ProcessorParameters = new Dictionary<string, string>(item.ProcessorParameters)
            });
        }

        return groups;
    }
}
