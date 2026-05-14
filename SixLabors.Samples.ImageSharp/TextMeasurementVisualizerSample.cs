// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SixLabors.Samples.ImageSharp;

/// <summary>
/// Demonstrates how text measurement rectangles relate to the pixels drawn for a text run.
/// </summary>
internal static class TextMeasurementVisualizerSample
{
    private const int ImageWidth = 1180;
    private const int ImageHeight = 400;
    private const int PanelCount = 3;
    private const float PageMargin = 50;
    private const float PanelGap = 30;
    private const float TextLineSpacing = 1.08F;
    private const float SwatchHeightScale = .4F;
    private const float NoWrapping = -1F;
    private const string FontFile = "roboto-regular.ttf";

    /// <summary>
    /// Draws measured text as separate panels so each rectangle can be read without overlap.
    /// </summary>
    public static void Run()
    {
        FontCollection fonts = new();
        Font font = fonts.Add(SamplePaths.Asset(FontFile)).CreateFont(52);
        Font labelFont = font.Family.CreateFont(20);
        const string text = "Metrics\nfjords 2026";
        const string title = "Text measurement visualizer";
        TextOptions textOptions = new(font)
        {
            LineSpacing = TextLineSpacing,
        };

        TextOptions labelOptions = new(labelFont);

        // TextBlock prepares the shaping data once. The same prepared block is measured and
        // rendered in every panel so the highlighted rectangles describe the glyphs being drawn.
        TextBlock textBlock = new(text, textOptions);
        TextMetrics textMetrics = textBlock.Measure(NoWrapping);
        TextBlock titleBlock = new(title, labelOptions);
        TextBlock advanceLabelBlock = new("Advance (A)", labelOptions);
        TextBlock boundsLabelBlock = new("Bounds (B)", labelOptions);
        TextBlock renderableBoundsLabelBlock = new("RenderableBounds (A | B)", labelOptions);

        using Image<Rgba32> image = new(ImageWidth, ImageHeight, Color.White.ToPixel<Rgba32>());
        image.Mutate(context => context.Paint(canvas =>
        {
            FontRectangle titleSize = titleBlock.MeasureAdvance(NoWrapping);
            FontRectangle labelSize = renderableBoundsLabelBlock.MeasureAdvance(NoWrapping);
            float panelTop = PageMargin + titleSize.Height + PageMargin;
            float panelWidth = (ImageWidth - (PageMargin * 2) - (PanelGap * (PanelCount - 1))) / PanelCount;
            float panelHeight = ImageHeight - panelTop - PageMargin;
            float panelPadding = labelSize.Height;

            DrawTitle(canvas, titleBlock);

            DrawPanel(
                canvas,
                textBlock,
                textMetrics,
                CreatePanelBounds(column: 0, panelTop, panelWidth, panelHeight),
                advanceLabelBlock,
                Color.LightBlue,
                MeasurementOverlay.Advance,
                panelPadding);

            DrawPanel(
                canvas,
                textBlock,
                textMetrics,
                CreatePanelBounds(column: 1, panelTop, panelWidth, panelHeight),
                boundsLabelBlock,
                Color.LightPink,
                MeasurementOverlay.Bounds,
                panelPadding);

            DrawPanel(
                canvas,
                textBlock,
                textMetrics,
                CreatePanelBounds(column: 2, panelTop, panelWidth, panelHeight),
                renderableBoundsLabelBlock,
                Color.LightGreen,
                MeasurementOverlay.RenderableBounds,
                panelPadding);
        }));

        image.SaveAsPng(SamplePaths.Output("text-measurement-visualizer.png"));
    }

    private static RectangleF CreatePanelBounds(int column, float top, float width, float height)
        => new(
            PageMargin + (column * (width + PanelGap)),
            top,
            width,
            height);

    private static void DrawTitle(DrawingCanvas canvas, TextBlock titleBlock) => canvas.DrawText(
            titleBlock,
            new PointF(PageMargin, PageMargin),
            NoWrapping,
            Brushes.Solid(Color.Black),
            pen: null);

    private static void DrawPanel(
        DrawingCanvas canvas,
        TextBlock textBlock,
        TextMetrics textMetrics,
        RectangleF panel,
        TextBlock titleBlock,
        Color measurementColor,
        MeasurementOverlay overlay,
        float panelPadding)
    {
        FontRectangle titleSize = titleBlock.MeasureAdvance(NoWrapping);
        float contentTop = panel.Top + panelPadding + titleSize.Height + panelPadding;
        RectangleF content = new(
            panel.Left + panelPadding,
            contentTop,
            panel.Width - (panelPadding * 2),
            panel.Bottom - contentTop - panelPadding);

        // RenderableBounds is the full drawing footprint. Use it to place the prepared block
        // inside the panel, then apply the same translation to the measured rectangles.
        PointF location = new(
            content.Left + ((content.Width - textMetrics.RenderableBounds.Width) * .5F) - textMetrics.RenderableBounds.X,
            content.Top + ((content.Height - textMetrics.RenderableBounds.Height) * .5F) - textMetrics.RenderableBounds.Y);
        RectangleF advance = Translate(textMetrics.Advance, location);
        RectangleF bounds = Translate(textMetrics.Bounds, location);
        RectangleF renderableBounds = Translate(textMetrics.RenderableBounds, location);

        canvas.Fill(Brushes.Solid(Color.GhostWhite), new RectanglePolygon(panel));

        float swatchWidth = titleSize.Height;
        float swatchHeight = titleSize.Height * SwatchHeightScale;
        PointF titleOrigin = new(
            panel.Left + panelPadding + swatchWidth + panelPadding,
            panel.Top + panelPadding);

        RectangleF swatch = new(
            panel.Left + panelPadding,
            titleOrigin.Y + ((titleSize.Height - swatchHeight) * .5F),
            swatchWidth,
            swatchHeight);

        canvas.Fill(Brushes.Solid(measurementColor), new RectanglePolygon(swatch));
        canvas.DrawText(
            titleBlock,
            titleOrigin,
            NoWrapping,
            Brushes.Solid(Color.Black),
            pen: null);

        if (overlay == MeasurementOverlay.Advance)
        {
            // Advance is the logical rectangle used to place following text.
            canvas.Fill(Brushes.Solid(measurementColor), new RectanglePolygon(advance));
        }
        else if (overlay == MeasurementOverlay.Bounds)
        {
            // Bounds is the tight rectangle around rendered glyph ink.
            canvas.Fill(Brushes.Solid(measurementColor), new RectanglePolygon(bounds));
        }
        else if (overlay == MeasurementOverlay.RenderableBounds)
        {
            // RenderableBounds encloses both the logical advance and the ink bounds, which is
            // the rectangle to preserve when avoiding clipping.
            canvas.Fill(Brushes.Solid(measurementColor), new RectanglePolygon(renderableBounds));
        }

        canvas.DrawText(textBlock, location, NoWrapping, Brushes.Solid(Color.Black), pen: null);
    }

    private static RectangleF Translate(FontRectangle rectangle, PointF offset)
        => new(rectangle.X + offset.X, rectangle.Y + offset.Y, rectangle.Width, rectangle.Height);

    private enum MeasurementOverlay
    {
        Advance,
        Bounds,
        RenderableBounds,
    }
}
