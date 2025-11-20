namespace MGCBConvert;

public class ContentGroup
{
    public List<MGCBContentItem> Items { get; set; } = new List<MGCBContentItem>();
    public string Pattern { get; set; } = "";
    public bool UseWildcard { get; set; }
    public bool IsCopyAction { get; set; }
    public string? ImporterName { get; set; }
    public string? ProcessorName { get; set; }
    public Dictionary<string, string> ProcessorParameters { get; set; } = new Dictionary<string, string>();
    public string? ContentRoot { get; set; }
}
