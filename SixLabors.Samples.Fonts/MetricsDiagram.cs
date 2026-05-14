// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace SixLabors.Samples.Fonts;

/// <summary>
/// Provides shared drawing helpers for metrics visualizer samples.
/// </summary>
internal static class MetricsDiagram
{
    private const float MeasurementLineWidth = 2;
    private const float ExtensionLineLength = 44;

    /// <summary>
    /// Draws a horizontal measurement with centered arrowheads and a label.
    /// </summary>
    public static void DrawHorizontalMeasurement(
        DrawingCanvas canvas,
        Font labelFont,
        string label,
        float start,
        float end,
        float y,
        PointF labelAnchor,
        HorizontalAlignment horizontalAlignment,
        VerticalAlignment verticalAlignment)
    {
        Pen measurementPen = Pens.Solid(Color.Black, MeasurementLineWidth);
        Pen guidePen = new PatternPen(Color.Gray, 1, [4, 4]);

        canvas.DrawLine(measurementPen, new PointF(start, y), new PointF(end, y));
        canvas.DrawLine(guidePen, new PointF(start, y - (ExtensionLineLength * .5F)), new PointF(start, y + (ExtensionLineLength * .5F)));
        canvas.DrawLine(guidePen, new PointF(end, y - (ExtensionLineLength * .5F)), new PointF(end, y + (ExtensionLineLength * .5F)));
        DrawArrowHead(canvas, new PointF(start, y), new PointF(1, 0));
        DrawArrowHead(canvas, new PointF(end, y), new PointF(-1, 0));
        DrawLabel(canvas, labelFont, label, labelAnchor, Color.Black, horizontalAlignment, verticalAlignment);
    }

    /// <summary>
    /// Draws a vertical measurement with centered arrowheads and a label.
    /// </summary>
    public static void DrawVerticalMeasurement(
        DrawingCanvas canvas,
        Font labelFont,
        string label,
        float start,
        float end,
        float x,
        PointF labelAnchor,
        HorizontalAlignment horizontalAlignment,
        VerticalAlignment verticalAlignment)
    {
        Pen measurementPen = Pens.Solid(Color.Black, MeasurementLineWidth);
        Pen guidePen = new PatternPen(Color.Gray, 1, [4, 4]);

        canvas.DrawLine(measurementPen, new PointF(x, start), new PointF(x, end));
        canvas.DrawLine(guidePen, new PointF(x - (ExtensionLineLength * .5F), start), new PointF(x + (ExtensionLineLength * .5F), start));
        canvas.DrawLine(guidePen, new PointF(x - (ExtensionLineLength * .5F), end), new PointF(x + (ExtensionLineLength * .5F), end));
        DrawArrowHead(canvas, new PointF(x, start), new PointF(0, 1));
        DrawArrowHead(canvas, new PointF(x, end), new PointF(0, -1));
        DrawLabel(canvas, labelFont, label, labelAnchor, Color.Black, horizontalAlignment, verticalAlignment);
    }

    /// <summary>
    /// Draws a text label.
    /// </summary>
    public static void DrawLabel(DrawingCanvas canvas, Font labelFont, string label, PointF origin, Color color)
        => canvas.DrawText(
            new RichTextOptions(labelFont) { Origin = origin },
            label,
            Brushes.Solid(color),
            pen: null);

    /// <summary>
    /// Draws a text label positioned relative to an anchor point.
    /// </summary>
    public static void DrawLabel(
        DrawingCanvas canvas,
        Font labelFont,
        string label,
        PointF anchor,
        Color color,
        HorizontalAlignment horizontalAlignment,
        VerticalAlignment verticalAlignment)
    {
        FontRectangle labelBounds = TextMeasurer.MeasureAdvance(label, new TextOptions(labelFont));
        PointF origin = new(
            AlignHorizontal(anchor.X, labelBounds.Width, horizontalAlignment),
            AlignVertical(anchor.Y, labelBounds.Height, verticalAlignment));

        DrawLabel(canvas, labelFont, label, origin, color);
    }

    private static void DrawArrowHead(DrawingCanvas canvas, PointF endPoint, PointF direction)
    {
        const float length = 12;
        const float halfWidth = 5;
        const float lineOffset = .5F;
        PointF tip = direction.X == 0
            ? new PointF(endPoint.X + lineOffset, endPoint.Y)
            : new PointF(endPoint.X, endPoint.Y + lineOffset);

        canvas.Fill(Brushes.Solid(Color.Black), new Polygon(
        [
            tip,
            new PointF(tip.X + (direction.X * length) - (direction.Y * halfWidth), tip.Y + (direction.Y * length) + (direction.X * halfWidth)),
            new PointF(tip.X + (direction.X * length) + (direction.Y * halfWidth), tip.Y + (direction.Y * length) - (direction.X * halfWidth)),
        ]));
    }

    private static float AlignHorizontal(float x, float width, HorizontalAlignment alignment)
        => alignment switch
        {
            HorizontalAlignment.Center => x - (width * .5F),
            HorizontalAlignment.Right => x - width,
            _ => x,
        };

    private static float AlignVertical(float y, float height, VerticalAlignment alignment)
        => alignment switch
        {
            VerticalAlignment.Center => y - (height * .5F),
            VerticalAlignment.Bottom => y - height,
            _ => y,
        };
}
