// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SixLabors.Samples.Fonts;

/// <summary>
/// Demonstrates how one laid-out glyph exposes advance, rendered bounds, and side bearings.
/// </summary>
internal static class GlyphMetricsVisualizerSample
{
    private const int ImageWidth = 780;
    private const int ImageHeight = 660;
    private const string FontFile = "roboto-regular.ttf";

    /// <summary>
    /// Creates an annotated glyph metrics diagram.
    /// </summary>
    public static void Run()
    {
        FontCollection fonts = new();
        Font font = fonts.Add(SamplePaths.Asset(FontFile)).CreateFont(260);
        Font labelFont = font.Family.CreateFont(20);
        const string text = "a";

        GlyphMetrics unpositionedGlyph = TextMeasurer.GetGlyphMetrics(text, new RichTextOptions(font)).Span[0];
        FontRectangle unpositionedAdvance = unpositionedGlyph.Advance;

        RichTextOptions textOptions = new(font)
        {
            // Position the text so the logical advance rectangle is centered in the output image.
            Origin = new PointF(
                ((ImageWidth - unpositionedAdvance.Width) * .5F) - unpositionedAdvance.X,
                ((ImageHeight - unpositionedAdvance.Height) * .5F) - unpositionedAdvance.Y),
        };

        // GetGlyphMetrics returns one entry per laid-out glyph. The rectangles are already
        // scaled to pixels and positioned with the supplied TextOptions.
        GlyphMetrics glyph = TextMeasurer.GetGlyphMetrics(text, textOptions).Span[0];
        RectangleF advance = ToRectangleF(glyph.Advance);
        RectangleF bounds = ToRectangleF(glyph.Bounds);

        using Image<Rgba32> image = new(ImageWidth, ImageHeight, Color.White.ToPixel<Rgba32>());
        image.Mutate(context => context.Paint(canvas =>
        {
            // Advance is the logical cell occupied by the glyph during layout.
            canvas.Fill(Brushes.Solid(Color.Azure), new RectanglePolygon(advance));

            // Bounds is the ink rectangle for the rendered glyph inside that logical cell.
            canvas.Fill(Brushes.Solid(Color.LightGreen.WithAlpha(.78F)), new RectanglePolygon(bounds));

            canvas.Draw(Pens.Solid(Color.DarkBlue, 3), new RectanglePolygon(advance));
            canvas.Draw(Pens.Solid(Color.DarkGreen, 2), new RectanglePolygon(bounds));
            canvas.DrawText(textOptions, text, Brushes.Solid(Color.Black), pen: null);

            // Side bearings are the gaps between the advance cell and the rendered glyph bounds.
            const float outerMeasurementGap = 64;
            const float innerMeasurementGap = 34;
            const float labelGap = 14;

            float topSideBearingY = advance.Top - outerMeasurementGap;
            float widthY = advance.Top - (outerMeasurementGap + innerMeasurementGap);
            float leftOuterMeasurementX = advance.Left - outerMeasurementGap;
            float leftInnerMeasurementX = advance.Left - innerMeasurementGap;
            float advanceHeightX = advance.Right + outerMeasurementGap;
            float advanceWidthY = advance.Bottom + outerMeasurementGap;

            MetricsDiagram.DrawHorizontalMeasurement(
                canvas,
                labelFont,
                "LeftSideBearing",
                advance.Left,
                bounds.Left,
                topSideBearingY,
                new PointF(advance.Left, topSideBearingY - labelGap),
                HorizontalAlignment.Right,
                VerticalAlignment.Bottom);

            MetricsDiagram.DrawHorizontalMeasurement(
                canvas,
                labelFont,
                "Width",
                bounds.Left,
                bounds.Right,
                widthY,
                new PointF((bounds.Left + bounds.Right) * .5F, widthY - labelGap),
                HorizontalAlignment.Center,
                VerticalAlignment.Bottom);

            MetricsDiagram.DrawHorizontalMeasurement(
                canvas,
                labelFont,
                "RightSideBearing",
                bounds.Right,
                advance.Right,
                topSideBearingY,
                new PointF(advance.Right, topSideBearingY - labelGap),
                HorizontalAlignment.Left,
                VerticalAlignment.Bottom);

            MetricsDiagram.DrawVerticalMeasurement(
                canvas,
                labelFont,
                "TopSideBearing",
                advance.Top,
                bounds.Top,
                leftOuterMeasurementX,
                new PointF(leftOuterMeasurementX - labelGap, (advance.Top + bounds.Top) * .5F),
                HorizontalAlignment.Right,
                VerticalAlignment.Center);

            MetricsDiagram.DrawVerticalMeasurement(
                canvas,
                labelFont,
                "Height",
                bounds.Top,
                bounds.Bottom,
                leftInnerMeasurementX,
                new PointF(leftInnerMeasurementX - labelGap, (bounds.Top + bounds.Bottom) * .5F),
                HorizontalAlignment.Right,
                VerticalAlignment.Center);

            MetricsDiagram.DrawVerticalMeasurement(
                canvas,
                labelFont,
                "BottomSideBearing",
                bounds.Bottom,
                advance.Bottom,
                leftOuterMeasurementX,
                new PointF(leftOuterMeasurementX - labelGap, (bounds.Bottom + advance.Bottom) * .5F),
                HorizontalAlignment.Right,
                VerticalAlignment.Center);

            MetricsDiagram.DrawHorizontalMeasurement(
                canvas,
                labelFont,
                "AdvanceWidth",
                advance.Left,
                advance.Right,
                advanceWidthY,
                new PointF((advance.Left + advance.Right) * .5F, advanceWidthY + labelGap),
                HorizontalAlignment.Center,
                VerticalAlignment.Top);

            MetricsDiagram.DrawVerticalMeasurement(
                canvas,
                labelFont,
                "AdvanceHeight",
                advance.Top,
                advance.Bottom,
                advanceHeightX,
                new PointF(advanceHeightX + labelGap, (advance.Top + advance.Bottom) * .5F),
                HorizontalAlignment.Left,
                VerticalAlignment.Center);
        }));

        image.SaveAsPng(SamplePaths.Output("glyph-metrics-visualizer.png"));
    }

    private static RectangleF ToRectangleF(FontRectangle rectangle)
        => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
}
