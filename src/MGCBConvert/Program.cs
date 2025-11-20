using System.CommandLine;
using MGCBConvert;

RootCommand rootCommand = new RootCommand("MonoGame Content Builder Migration Tool - Converts .mgcb files to new Builder.cs format");

Option<FileInfo> inputOption = new Option<FileInfo>(
    name: "--input",
    description: "Path to the Content.mgcb file to convert")
{
    IsRequired = true
};
inputOption.AddAlias("-i");

Option<FileInfo> outputOption = new Option<FileInfo>(
    name: "--output",
    description: "Path for the output Builder.cs file (default: Builder.cs in current directory)",
    getDefaultValue: () => new FileInfo("Builder.cs"));
outputOption.AddAlias("-o");

Option<bool> optimizeOption = new Option<bool>(
    name: "--optimize",
    description: "Enable pattern optimization to group similar content items",
    getDefaultValue: () => true);

Option<bool> verboseOption = new Option<bool>(
    name: "--verbose",
    description: "Enable verbose logging",
    getDefaultValue: () => false);
verboseOption.AddAlias("-v");

rootCommand.AddOption(inputOption);
rootCommand.AddOption(outputOption);
rootCommand.AddOption(optimizeOption);
rootCommand.AddOption(verboseOption);

rootCommand.SetHandler(async (inputFile, outputFile, optimize, verbose) =>
{
    try
    {
        Logger.SetVerbose(verbose);
        MGCBMigrator migrator = new MGCBMigrator()
        {
            OptimizePatterns = optimize
        };

        await migrator.MigrateAsync(inputFile, outputFile);

        Console.WriteLine($"✓ Successfully generated {outputFile.FullName}");
        Console.WriteLine($"  Found {migrator.Statistics.TotalItems} content items");
        Console.WriteLine($"  Generated {migrator.Statistics.GeneratedRules} content rules");

        if (optimize && migrator.Statistics.OptimizedItems > 0)
        {
            Console.WriteLine($"  Optimized {migrator.Statistics.OptimizedItems} items into patterns");
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"✗ Error: {ex.Message}");
        if (verbose)
        {
            Console.Error.WriteLine(ex.StackTrace);
        }
        Environment.Exit(1);
    }
}, inputOption, outputOption, optimizeOption, verboseOption);

return await rootCommand.InvokeAsync(args);
