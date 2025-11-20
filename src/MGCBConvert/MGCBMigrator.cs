namespace MGCBConvert;

public class MGCBMigrator
{
    public bool OptimizePatterns { get; set; } = true;
    public MigrationStatistics Statistics { get; } = new MigrationStatistics();

    public async Task MigrateAsync(FileInfo inputFile, FileInfo outputFile)
    {
        if (!inputFile.Exists)
        {
            throw new FileNotFoundException($"MGCB file not found: {inputFile.FullName}");
        }

        Logger.LogVerbose($"Parsing {inputFile.FullName}...");
        MGCBFile mgcbFile = MGCBParser.Parse(inputFile.FullName);
        Statistics.TotalItems = mgcbFile.ContentItems.Count;

        Logger.LogVerbose($"Found {Statistics.TotalItems} content items");

        // Analyze and group content items
        List<ContentGroup> groups = OptimizePatterns
                                    ? PatternAnalyzer.AnalyzeAndGroup(mgcbFile.ContentItems)
                                    : CreateIndividualGroups(mgcbFile.ContentItems);

        Statistics.GeneratedRules = groups.Count;

        int optimizedItems = 0;
        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i].Items.Count > 1)
            {
                optimizedItems += groups[i].Items.Count - 1;
            }
        }
        Statistics.OptimizedItems = optimizedItems;

        Logger.LogVerbose($"Generating {groups.Count} content rules...");

        BuilderCodeGenerator generator = new BuilderCodeGenerator();
        string code = generator.Generate(mgcbFile.GlobalConfig, groups);

        if (outputFile.Directory != null && !outputFile.Directory.Exists)
        {
            outputFile.Directory.Create();
        }

        await File.WriteAllTextAsync(outputFile.FullName, code);
    }

    private List<ContentGroup> CreateIndividualGroups(List<MGCBContentItem> items)
    {
        List<ContentGroup> groups = new List<ContentGroup>();

        for (int i = 0; i < items.Count; i++)
        {
            MGCBContentItem item = items[i];
            ContentGroup group = new ContentGroup
            {
                Items = new List<MGCBContentItem> { item },
                Pattern = item.SourcePath ?? "",
                UseWildcard = false,
                IsCopyAction = item.IsCopyAction,
                ImporterName = item.ImporterName,
                ProcessorName = item.ProcessorName,
                ProcessorParameters = new Dictionary<string, string>(item.ProcessorParameters)
            };
            groups.Add(group);
        }

        return groups;
    }
}
