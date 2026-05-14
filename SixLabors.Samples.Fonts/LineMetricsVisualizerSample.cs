// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Samples;

namespace SixLabors.Samples.Fonts;

/// <summary>
/// Demonstrates how line metrics describe the logical box and guide lines of laid-out text.
/// </summary>
internal static class LineMetricsVisualizerSample
{
    private const int ImageWidth = 1700;
    private const int ImageHeight = 520;
    private const float LineSpacingMultiplier = 1.35F;
    private const float BaselineGuideDashLength = 4;
    private const float BaselineGuideDashGap = 4;
    private const string FontFile = "roboto-regular.ttf";

    /// <summary>
    /// Creates an annotated line metrics diagram.
    /// </summary>
    public static void Run()
    {
        FontCollection fonts = new();
        Font font = fonts.Add(SamplePaths.Asset(FontFile)).CreateFont(220);
        Font labelFont = font.Family.CreateFont(24);

        // Use glyphs with descenders so the rendered bounds visibly cross below the baseline.
        const string text = "Typography";

        RichTextOptions unpositionedTextOptions = new(font)
        {
            // LineSpacing expands the logical line box without scaling the glyph outlines.
            LineSpacing = LineSpacingMultiplier,
        };

        TextMetrics unpositionedMetrics = TextMeasurer.Measure(text, unpositionedTextOptions);
        FontRectangle unpositionedAdvance = unpositionedMetrics.Advance;

        // First measure without an origin, then offset by the measured advance origin. This keeps
        // the logical line box centered even when the measured rectangle has a non-zero X or Y.
        RichTextOptions textOptions = new(font)
        {
            LineSpacing = LineSpacingMultiplier,

            // Position the single line so its logical extent is centered in the output image.
            Origin = new PointF(
                ((ImageWidth - unpositionedAdvance.Width) * .5F) - unpositionedAdvance.X,
                ((ImageHeight - unpositionedAdvance.Height) * .5F) - unpositionedAdvance.Y),
        };

        // Measure performs shaping and layout once. LineMetrics positions are in the same
        // coordinate space used when the text is drawn.
        TextMetrics metrics = TextMeasurer.Measure(text, textOptions);
        LineMetrics line = metrics.LineMetrics[0];

        // LineMetrics.Start is the top-left of the logical line box, while Extent is its
        // width and height. This is the rectangle used by layout, not the tight ink bounds.
        RectangleF lineBox = new(line.Start.X, line.Start.Y, line.Extent.X, line.Extent.Y);

        // TextMetrics.Bounds is the tight ink rectangle for the rendered glyphs. Drawing it
        // over the line box shows the difference between layout space and actual pixels.
        RectangleF bounds = ToRectangleF(metrics.Bounds);

        // These values are absolute Y coordinates in the image. The LineMetrics properties are
        // positions within the line box, so add Start.Y before drawing them.
        float ascenderY = line.Start.Y + line.Ascender;
        float baselineY = line.Start.Y + line.Baseline;
        float descenderY = line.Start.Y + line.Descender;

        using Image<Rgba32> image = new(ImageWidth, ImageHeight, Color.White.ToPixel<Rgba32>());
        image.Mutate(context => context.Paint(canvas =>
        {
            // Use the label font's measured height as the diagram spacing unit so labels and
            // measurements scale together if the annotation size changes.
            float labelHeight = TextMeasurer.MeasureAdvance("LineHeight", new TextOptions(labelFont)).Height;
            float labelGap = labelHeight;
            float measurementColumnGap = labelHeight + labelGap;
            float descenderMeasurementX = lineBox.Left - measurementColumnGap;
            float ascenderMeasurementX = descenderMeasurementX - measurementColumnGap;
            float lineHeightMeasurementX = lineBox.Right + measurementColumnGap;
            float horizontalMeasurementY = lineBox.Bottom + measurementColumnGap;

            // The baseline is a position marker rather than a measured length. Keep its guide
            // outside the text so it points at the same Y coordinate without crossing the glyphs.
            float baselineGuideStartX = ascenderMeasurementX - labelGap;
            float baselineGuideEndX = lineBox.Left - labelGap;
            Pen baselineGuidePen = new PatternPen(Color.Gray, 1, [BaselineGuideDashLength, BaselineGuideDashGap]);

            // The extent is the logical line box used for layout.
            canvas.Fill(Brushes.Solid(Color.Azure), new RectanglePolygon(lineBox));

            // Bounds is the ink rectangle for the rendered text inside the logical line box.
            canvas.Fill(Brushes.Solid(Color.LightGreen.WithAlpha(.78F)), new RectanglePolygon(bounds));

            canvas.Draw(Pens.Solid(Color.DarkBlue, 3), new RectanglePolygon(lineBox));
            canvas.Draw(Pens.Solid(Color.DarkGreen, 2), new RectanglePolygon(bounds));

            canvas.DrawText(textOptions, text, Brushes.Solid(Color.Black), pen: null);

            // Ascender is demonstrated as the distance from the ascender line down to the baseline.
            MetricsDiagram.DrawVerticalMeasurement(
                canvas,
                labelFont,
                "Ascender",
                ascenderY,
                baselineY,
                ascenderMeasurementX,
                new PointF(ascenderMeasurementX - labelGap, (ascenderY + baselineY) * .5F),
                HorizontalAlignment.Right,
                VerticalAlignment.Center);

            // Baseline is a position inside the line box, so label the dashed guide instead of
            // drawing it as a measurement with arrowheads.
            MetricsDiagram.DrawLabel(
                canvas,
                labelFont,
                "Baseline",
                new PointF(baselineGuideStartX - labelGap, baselineY),
                Color.Black,
                HorizontalAlignment.Right,
                VerticalAlignment.Center);

            canvas.DrawLine(baselineGuidePen, new PointF(baselineGuideStartX, baselineY), new PointF(baselineGuideEndX, baselineY));

            // Descender is demonstrated as the distance from the baseline down to the descender line.
            MetricsDiagram.DrawVerticalMeasurement(
                canvas,
                labelFont,
                "Descender",
                baselineY,
                descenderY,
                descenderMeasurementX,
                new PointF(descenderMeasurementX - labelGap, (baselineY + descenderY) * .5F),
                HorizontalAlignment.Right,
                VerticalAlignment.Center);

            // Extent.X is the horizontal size of the logical line box.
            MetricsDiagram.DrawHorizontalMeasurement(
                canvas,
                labelFont,
                "Extent.X",
                lineBox.Left,
                lineBox.Right,
                horizontalMeasurementY,
                new PointF(lineBox.Left + (lineBox.Width * .5F), horizontalMeasurementY + labelGap),
                HorizontalAlignment.Center,
                VerticalAlignment.Top);

            // LineHeight is the full vertical size of the logical line box after LineSpacing is applied.
            MetricsDiagram.DrawVerticalMeasurement(
                canvas,
                labelFont,
                "LineHeight",
                lineBox.Top,
                lineBox.Top + line.LineHeight,
                lineHeightMeasurementX,
                new PointF(lineHeightMeasurementX + labelGap, lineBox.Top + (line.LineHeight * .5F)),
                HorizontalAlignment.Left,
                VerticalAlignment.Center);
        }));

        image.SaveAsPng(SamplePaths.Output("line-metrics-visualizer.png"));
    }

    private static RectangleF ToRectangleF(FontRectangle rectangle)
        => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
}
