// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Numerics;
using System.Text;
using SixLabors.Fonts;
using SixLabors.Samples;

namespace SixLabors.Samples.Fonts;

/// <summary>
/// Demonstrates text measurement, wrapping, and the difference between advance, bounds, and renderable bounds.
/// </summary>
internal static class TextMeasurementSample
{
    private const string FontFile = "roboto-regular.ttf";

    /// <summary>
    /// Writes a measurement report for wrapped text.
    /// </summary>
    public static void Run()
    {
        FontCollection fonts = new();
        Font font = fonts.Add(SamplePaths.Asset(FontFile)).CreateFont(28);
        const string text = "Measure text before drawing it.  The logical advance, ink bounds, and renderable bounds answer different layout questions.";

        TextOptions options = new(font)
        {
            Origin = new Vector2(20, 30),
            WrappingLength = 360,
            LineSpacing = 1.18F,
        };

        // Measure performs layout once and returns the commonly needed measurements together.
        TextMetrics metrics = TextMeasurer.Measure(text, options);
        StringBuilder builder = new();

        builder.AppendLine("Text measurement");
        builder.AppendLine("================");
        builder.AppendLine($"Text: {text}");
        builder.AppendLine($"Origin: {options.Origin}");
        builder.AppendLine($"Wrapping length: {options.WrappingLength}px");
        builder.AppendLine($"Line spacing: {options.LineSpacing}");
        builder.AppendLine();
        builder.AppendLine("Measured rectangles:");
        builder.AppendLine($"- Advance: {metrics.Advance}");
        builder.AppendLine($"- Bounds: {metrics.Bounds}");
        builder.AppendLine($"- Renderable bounds: {metrics.RenderableBounds}");
        builder.AppendLine();
        builder.AppendLine("What each rectangle means:");
        builder.AppendLine("- Advance is the logical layout area used to place the next text run.");
        builder.AppendLine("- Bounds is the tight glyph ink area and can start outside the logical origin.");
        builder.AppendLine("- Renderable bounds is the union to use when a panel must contain both layout and ink.");
        builder.AppendLine();
        builder.AppendLine($"Line count: {metrics.LineCount}");
        builder.AppendLine("Line metrics:");

        int lineIndex = 0;
        foreach (LineMetrics line in metrics.LineMetrics)
        {
            builder.AppendLine($"- Line {lineIndex}: start={line.Start}, extent={line.Extent}, baseline={line.Baseline:0.##}, height={line.LineHeight:0.##}, graphemes={line.GraphemeCount}");
            lineIndex++;
        }

        File.WriteAllText(SamplePaths.Output("text-measurement.txt"), builder.ToString());
    }
}
