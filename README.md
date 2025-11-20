# MonoGame Content Builder Migration Tool

**MGCBConvert** is a command-line tool that automates the migration process from the legacy MonoGame Content Builder (MGCB) format to the new Content Builder system.

[![NuGet](https://img.shields.io/nuget/v/Aristurtle.Tools.MGCBConvert?color=blue&style=flat-square)](https://www.nuget.org/packages/Aristurtle.Tools.MGCBConvert)
[![License: MIT](https://img.shields.io/badge/📃%20license-MIT-blue?style=flat)](LICENSE)

## Features

- **Multi-Format Support**: Handles all standard MonoGame content types (textures, fonts, audio, models, effects)
- **Default Parameter Detection**: Only outputs processor parameters that differ from MonoGame defaults, keeping generated code clean
- **Pattern Recognition**: Automatically groups similar content files into optimized wildcard patterns

### Installation

Install via the .NET CLI:

```bash
dotnet tool install --global Aristurtle.Tools.MGCBConvert
```

### Uninstall

To remove the tool:

```bash
dotnet tool uninstall --global Aristurtle.Tools.MGCBConvert
```

## Usage

### Basic Usage

Convert a Content.mgcb file to Builder.cs:

```bash
mgcb-convert -i Content.mgcb -o Builder.cs
```

### Command-Line Options

- `-i, --input <file>` - Path to the Content.mgcb file (required)
- `-o, --output <file>` - Output Builder.cs file path (default: Builder.cs)
- `--optimize` - Enable pattern optimization (default: true)
- `-v, --verbose` - Enable verbose logging

# How It Works

## Pattern Optimization

By default, the tool analyzes your content items and groups them into optimized patterns:

- Files in the same directory with same settings -> `Textures/*.png`
- Scattered files with same extension/settings -> `**/*.png`
- Individual files that don't fit patterns -> Specific includes

**Example Output:**

If your MGCB file contains:

```text
/build:Textures/player.png
/build:Textures/enemy.png
/build:Textures/background.png
```

All using the same TextureProcessor with default settings, it generates:

```cs
contentCollection.Include<WildcardRule>("Textures/*.png");
```

Instead of three separate includes.

## Default Parameter Handling

The tool includes built-in knowledge of default parameters for all standard MonoGame processors:

- **TextureProcessor** - Handles color key, mipmaps, and alpha premultiplication
- **FontDescriptionProcessor** - Manages font size, style, and character regions
- **SoundEffectProcessor** - Controls audio quality and compression
- **SongProcessor** - Handles music quality settings
- **ModelProcessor** - Manages 3D model processing options
- **EffectProcessor** - Controls shader compilation settings

Only parameters that differ from defaults are included in the generated code, keeping your Builder.cs clean and readable.

**Example Parameter Optimization:**

If your MGCB has:

```text
/processorParam:ColorKeyEnabled=True
/processorParam:GenerateMipmaps=False
/processorParam:PremultiplyAlpha=True
```

Since `ColorKeyEnabled=True` and `PremultiplyAlpha=True` are defaults, only this is generated:

```csharp
var processor = new TextureProcessor
{
    GenerateMipmaps = false
};
```

# Migration Guide

## After Generation

Once you've generated your Builder.cs file:

1. **Copy to Project**: Move the generated Builder.cs to your Content Builder project
2. **Review Code**: Examine the generated code for any custom importer/processor references
3. **Test Build**: Run your content build to verify everything works correctly:

   ```bash
   dotnet run --project YourContentBuilder.csproj -- build
   ```

4. **Adjust References**: Update any custom importer or processor namespaces if needed

## Troubleshooting

- **Build Errors**: Check that all referenced processors are available in your project
- **Missing Content**: Verify the input MGCB file path is correct
- **Pattern Issues**: Use `--optimize false` to generate individual includes for debugging

# What Next?

- Read about the new [MonoGame Content Builder](https://docs.monogame.net/articles/getting_started/content_pipeline/content_builder_project.html) system
- Join the [MonoGame Community Discord](https://discord.gg/monogame) for help and discussion
- Follow me for updates:
  - Bluesky: [@aristurtle.bsky.social](https://bsky.app/profile/aristurtle.bsky.social)
  - Twitter: [@aristurtledev](https://twitter.com/aristurtledev)

# License

**MGCBConvert** is licensed under the **MIT License**. Please refer to the [LICENSE](LICENSE) file for full license text.
