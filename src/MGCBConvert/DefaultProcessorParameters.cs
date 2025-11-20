namespace MGCBConvert;

public static class DefaultProcessorParameters
{
    private static readonly Dictionary<string, Dictionary<string, string>> _defaults = new()
    {
        // TextureProcessor defaults
        ["TextureProcessor"] = new Dictionary<string, string>
        {
            ["ColorKeyColor"] = "255,0,255,255",
            ["ColorKeyEnabled"] = "True",
            ["GenerateMipmaps"] = "False",
            ["PremultiplyAlpha"] = "True",
            ["ResizeToPowerOfTwo"] = "False",
            ["MakeSquare"] = "False",
            ["TextureFormat"] = "Color"
        },

        // FontDescriptionProcessor defaults
        ["FontDescriptionProcessor"] = new Dictionary<string, string>
        {
            ["PremultiplyAlpha"] = "True",
            ["TextureFormat"] = "Compressed"
        },

        // SoundEffectProcessor defaults
        ["SoundEffectProcessor"] = new Dictionary<string, string>
        {
            ["Quality"] = "Best"
        },

        // SongProcessor defaults  
        ["SongProcessor"] = new Dictionary<string, string>
        {
            ["Quality"] = "Best"
        },

        // ModelProcessor defaults
        ["ModelProcessor"] = new Dictionary<string, string>
        {
            ["ColorKeyColor"] = "255,0,255,255",
            ["ColorKeyEnabled"] = "True",
            ["DefaultEffect"] = "BasicEffect",
            ["GenerateMipmaps"] = "True",
            ["GenerateTangentFrames"] = "False",
            ["PremultiplyTextureAlpha"] = "True",
            ["PremultiplyVertexColors"] = "True",
            ["ResizeTexturesToPowerOfTwo"] = "False",
            ["RotationX"] = "0",
            ["RotationY"] = "0",
            ["RotationZ"] = "0",
            ["Scale"] = "1",
            ["SwapWindingOrder"] = "False",
            ["TextureFormat"] = "Compressed"
        },

        // EffectProcessor defaults
        ["EffectProcessor"] = new Dictionary<string, string>
        {
            ["DebugMode"] = "Auto",
            ["Defines"] = ""
        },

        // SpriteFontProcessor defaults (legacy)
        ["FontTextureProcessor"] = new Dictionary<string, string>
        {
            ["PremultiplyAlpha"] = "True",
            ["TextureFormat"] = "Compressed"
        }
    };

    public static Dictionary<string, string> GetDefaults(string? processorName)
    {
        if (string.IsNullOrEmpty(processorName))
        {
            return new Dictionary<string, string>();
        }

        if (_defaults.TryGetValue(processorName, out Dictionary<string, string>? defaults))
        {
            return new Dictionary<string, string>(defaults);
        }
        else
        {
            return new Dictionary<string, string>();
        }
    }

    public static Dictionary<string, string> FilterNonDefaults(string? processorName, Dictionary<string, string> parameters)
    {
        Dictionary<string, string> defaults = GetDefaults(processorName);
        Dictionary<string, string> nonDefaults = [];

        foreach (var param in parameters)
        {
            if (!defaults.TryGetValue(param.Key, out string? defaultValue) || 
                !string.Equals(param.Value, defaultValue, StringComparison.OrdinalIgnoreCase))
            {
                nonDefaults[param.Key] = param.Value;
            }
        }

        return nonDefaults;
    }

    public static bool AreDefaultParameters(string? processorName, Dictionary<string, string> parameters)
    {
        Dictionary<string, string> nonDefaults = FilterNonDefaults(processorName, parameters);
        return nonDefaults.Count == 0;
    }
}
