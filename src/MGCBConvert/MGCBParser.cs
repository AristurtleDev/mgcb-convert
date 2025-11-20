using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;

namespace MGCBConvert;

public static class MGCBParser
{
    public static MGCBFile Parse(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"MGCB file not found: {filePath}");
        }

        string[] lines = File.ReadAllLines(filePath);
        return ParseLines(lines);
    }

    public static MGCBFile ParseLines(string[] lines)
    {
        MGCBFile mgcbFile = new MGCBFile();
        MGCBContentItem? currentItem = new MGCBContentItem();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line) || line.Length == 0 || line[0] != '/')
            {
                continue;
            }

            int colonIndex = line.IndexOf(':');
            if (colonIndex == -1)
            {
                continue;
            }

            string command = line[1..colonIndex].ToLowerInvariant();
            string argument = line[(colonIndex + 1)..].Trim();

            switch (command)
            {
                case "outputdir":
                    mgcbFile.GlobalConfig.OutputDirectory = argument;
                    break;

                case "intermediatedir":
                    mgcbFile.GlobalConfig.IntermediateDirectory = argument;
                    break;

                case "platform":
                    if (Enum.TryParse<TargetPlatform>(argument, true, out TargetPlatform platform))
                    {
                        mgcbFile.GlobalConfig.Platform = platform;
                    }
                    break;

                case "profile":
                    if (Enum.TryParse<GraphicsProfile>(argument, true, out GraphicsProfile profile))
                    {
                        mgcbFile.GlobalConfig.Profile = profile;
                    }
                    break;

                case "compress":
                    mgcbFile.GlobalConfig.Compress = bool.TryParse(argument, out bool compress) && compress;
                    break;

                case "config":
                    mgcbFile.GlobalConfig.Config = argument;
                    break;

                case "reference":
                    mgcbFile.GlobalConfig.References.Add(argument);
                    break;

                case "build":
                    if (currentItem != null)
                    {
                        currentItem.SourcePath = argument;
                        mgcbFile.ContentItems.Add(currentItem);
                    }
                    currentItem = new MGCBContentItem();
                    break;

                case "copy":
                    if (currentItem != null)
                    {
                        currentItem.SourcePath = argument;
                        currentItem.IsCopyAction = true;
                        mgcbFile.ContentItems.Add(currentItem);
                    }
                    currentItem = new MGCBContentItem();
                    break;

                case "importer":
                    if (currentItem != null)
                    {
                        currentItem.ImporterName = argument;
                    }
                    break;

                case "processor":
                    if (currentItem != null)
                    {
                        currentItem.ProcessorName = argument;
                    }
                    break;

                case "processorparam":
                    if (currentItem != null)
                    {
                        string[] paramParts = argument.Split('=', 2);
                        if (paramParts.Length == 2)
                        {
                            string parameter = paramParts[0].Trim();
                            string parameterArgument = paramParts[1].Trim();
                            currentItem.ProcessorParameters[parameter] = parameterArgument;
                        }
                    }
                    break;
            }
        }

        if (currentItem != null && !string.IsNullOrEmpty(currentItem.SourcePath))
        {
            mgcbFile.ContentItems.Add(currentItem);
        }

        return mgcbFile;
    }
}
