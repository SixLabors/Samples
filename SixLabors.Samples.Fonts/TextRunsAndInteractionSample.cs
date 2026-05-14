// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Numerics;
using System.Text;
using SixLabors.Fonts;
using SixLabors.Samples;

namespace SixLabors.Samples.Fonts;

/// <summary>
/// Demonstrates text runs, glyph metrics, grapheme metrics, hit testing, and caret movement.
/// </summary>
internal static class TextRunsAndInteractionSample
{
    private const string FontFile = "roboto-regular.ttf";

    /// <summary>
    /// Writes a report showing run-based layout and text interaction metrics.
    /// </summary>
    public static void Run()
    {
        FontCollection fonts = new();
        FontFamily family = fonts.Add(SamplePaths.Asset(FontFile));
        Font bodyFont = family.CreateFont(20);
        Font leadFont = family.CreateFont(34);
        const string text = "Large labels can share a paragraph with body text and still produce one shaped layout.";

        TextOptions options = new(bodyFont)
        {
            Origin = new Vector2(12, 18),
            WrappingLength = 420,

            // TextRun indices are grapheme indices, not UTF-16 char indices. This ASCII prefix
            // keeps the sample readable while showing where a larger font enters the layout.
            TextRuns =
            [
                new TextRun
                {
                    Start = 0,
                    End = 5,
                    Font = leadFont,
                },
            ],
        };

        TextMetrics metrics = TextMeasurer.Measure(text, options);
        ReadOnlySpan<GlyphMetrics> glyphs = metrics.GetGlyphMetrics().Span;
        ReadOnlySpan<GraphemeMetrics> graphemes = metrics.GraphemeMetrics;
        StringBuilder builder = new();

        builder.AppendLine("Text runs and interaction");
        builder.AppendLine("=========================");
        builder.AppendLine($"Text: {text}");
        builder.AppendLine();
        builder.AppendLine("The first five graphemes use a larger font through TextOptions.TextRuns.");
        builder.AppendLine($"Line count: {metrics.LineCount}");
        builder.AppendLine($"Renderable bounds: {metrics.RenderableBounds}");
        builder.AppendLine();
        builder.AppendLine("First glyph metrics:");

        for (int i = 0; i < Math.Min(8, glyphs.Length); i++)
        {
            GlyphMetrics glyph = glyphs[i];
            builder.AppendLine($"- Glyph {i}: code point={glyph.CodePoint}, grapheme={glyph.GraphemeIndex}, advance={glyph.Advance}, bounds={glyph.Bounds}");
        }

        builder.AppendLine();
        builder.AppendLine("First grapheme metrics:");

        for (int i = 0; i < Math.Min(8, graphemes.Length); i++)
        {
            GraphemeMetrics grapheme = graphemes[i];
            builder.AppendLine($"- Grapheme {i}: string index={grapheme.StringIndex}, advance={grapheme.Advance}, renderable={grapheme.RenderableBounds}");
        }

        // HitTest and caret APIs operate on the measured layout, so editors can answer cursor
        // and selection questions without re-shaping the same text themselves.
        Vector2 probePoint = new(120, metrics.LineMetrics[0].Baseline);
        TextHit hit = metrics.HitTest(probePoint);
        CaretPosition caret = metrics.GetCaretPosition(hit);
        CaretPosition nextWord = metrics.MoveCaret(caret, CaretMovement.NextWord);

        builder.AppendLine();
        builder.AppendLine("Interaction metrics:");
        builder.AppendLine($"- Probe point: {probePoint}");
        builder.AppendLine($"- Hit grapheme index: {hit.GraphemeIndex}");
        builder.AppendLine($"- Hit string index: {hit.StringIndex}");
        builder.AppendLine($"- Caret start: {caret.Start}");
        builder.AppendLine($"- Next-word caret start: {nextWord.Start}");

        File.WriteAllText(SamplePaths.Output("text-runs-and-interaction.txt"), builder.ToString());
    }
}
