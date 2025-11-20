using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;

namespace MGCBConvert;

public class MGCBGlobalConfig
{
    public string OutputDirectory { get; set; } = "bin/$(Platform)";
    public string IntermediateDirectory { get; set; } = "obj/$(Platform)";
    public TargetPlatform Platform { get; set; } = TargetPlatform.DesktopGL;
    public GraphicsProfile Profile { get; set; } = GraphicsProfile.Reach;
    public bool Compress { get; set; }
    public string Config { get; set; } = string.Empty;
    public List<string> References { get; set; } = new List<string>();
}
