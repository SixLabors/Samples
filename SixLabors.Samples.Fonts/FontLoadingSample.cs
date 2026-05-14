// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Text;
using SixLabors.Fonts;
using SixLabors.Samples;

namespace SixLabors.Samples.Fonts;

/// <summary>
/// Demonstrates loading a font file and reading its family metadata.
/// </summary>
internal static class FontLoadingSample
{
    private const string FontFile = "roboto-regular.ttf";

    /// <summary>
    /// Writes a report describing the loaded font family.
    /// </summary>
    public static void Run()
    {
        string fontPath = SamplePaths.Asset(FontFile);
        FontCollection fonts = new();

        // Add returns a FontFamily that can create Font instances; the out parameter exposes
        // descriptive name-table information read from the font file.
        FontFamily family = fonts.Add(fontPath, out FontDescription description);
        StringBuilder builder = new();

        builder.AppendLine("Font loading");
        builder.AppendLine("============");
        builder.AppendLine($"File: {Path.GetFileName(fontPath)}");
        builder.AppendLine($"Family: {family.Name}");
        builder.AppendLine($"Font name: {description.FontNameInvariantCulture}");
        builder.AppendLine($"Subfamily: {description.FontSubFamilyNameInvariantCulture}");
        builder.AppendLine($"Style: {description.Style}");
        builder.AppendLine();
        builder.AppendLine("Available styles in this collection:");

        foreach (FontStyle style in family.GetAvailableStyles().Span)
        {
            builder.AppendLine($"- {style}");
        }

        builder.AppendLine();
        builder.AppendLine("Source paths:");

        // Families created from files can report the physical font paths that contributed
        // their metrics; this is useful when diagnosing which font a collection resolved.
        family.TryGetPaths(out ReadOnlyMemory<string> paths);

        foreach (string path in paths.Span)
        {
            builder.AppendLine($"- {path}");
        }

        File.WriteAllText(SamplePaths.Output("font-loading.txt"), builder.ToString());
    }
}
