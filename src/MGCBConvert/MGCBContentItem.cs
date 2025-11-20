namespace MGCBConvert;

public class MGCBContentItem
{
    public string? SourcePath { get; set; }
    public string? ImporterName { get; set; }
    public string? ProcessorName { get; set; }
    public Dictionary<string, string> ProcessorParameters { get; } = new Dictionary<string, string>();
    public bool IsCopyAction { get; set; }
}
