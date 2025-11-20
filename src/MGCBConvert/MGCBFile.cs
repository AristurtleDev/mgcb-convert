namespace MGCBConvert;

public class MGCBFile
{
    public MGCBGlobalConfig GlobalConfig { get; init; } = new MGCBGlobalConfig();
    public List<MGCBContentItem> ContentItems { get; init; } = new List<MGCBContentItem>();
}
