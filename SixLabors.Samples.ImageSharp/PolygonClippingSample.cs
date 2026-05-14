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
/// Demonstrates boolean path clipping operations with ImageSharp.Drawing shapes.
/// </summary>
internal static class PolygonClippingSample
{
    private const string FontFile = "roboto-regular.ttf";

    /// <summary>
    /// Creates a visual comparison of the main boolean clipping operations.
    /// </summary>
    public static void Run()
    {
        FontCollection fonts = new();
        Font titleFont = fonts.Add(SamplePaths.Asset(FontFile)).CreateFont(28);
        Font labelFont = titleFont.Family.CreateFont(18);

        using Image<Rgba32> photo = Image.Load<Rgba32>(SamplePaths.Asset("landscape-05-pietro-de-grandi.jpg"));
        using Image<Rgba32> sheet = new(1180, 980, Color.White.ToPixel<Rgba32>());

        sheet.Mutate(context => context.Paint(canvas =>
        {
            canvas.DrawText(
                new RichTextOptions(titleFont) { Origin = new PointF(40, 28) },
                "Polygon clipping operations",
                Brushes.Solid(Color.Black),
                pen: null);

            DrawSectionTitle(canvas, labelFont, "Symmetric", 50, 86);
            DrawOperation(canvas, photo, CreateSymmetricTileBounds(0), "Intersection: A & B", BooleanOperation.Intersection, reverseOperands: false, labelFont);
            DrawOperation(canvas, photo, CreateSymmetricTileBounds(1), "Union: A | B", BooleanOperation.Union, reverseOperands: false, labelFont);
            DrawOperation(canvas, photo, CreateSymmetricTileBounds(2), "Xor: A ^ B", BooleanOperation.Xor, reverseOperands: false, labelFont);

            DrawSectionTitle(canvas, labelFont, "Asymmetric", 640, 86);
            DrawOperation(canvas, photo, CreateAsymmetricTileBounds(0), "Difference: A - B", BooleanOperation.Difference, reverseOperands: false, labelFont);
            DrawOperation(canvas, photo, CreateAsymmetricTileBounds(1), "Difference: B - A", BooleanOperation.Difference, reverseOperands: true, labelFont);
        }));

        sheet.SaveAsPng(SamplePaths.Output("polygon-clipping.png"));
    }

    private static RectangleF CreateSymmetricTileBounds(int index)
    {
        const float tileWidth = 490;
        const float tileHeight = 250;
        const float gap = 34;
        const float left = 50;
        const float top = 122;

        return new RectangleF(
            left,
            top + (index * (tileHeight + gap)),
            tileWidth,
            tileHeight);
    }

    private static RectangleF CreateAsymmetricTileBounds(int index)
    {
        const float tileWidth = 490;
        const float tileHeight = 250;
        const float gap = 34;
        const float left = 640;
        const float top = 122;

        return new RectangleF(
            left,
            top + (index * (tileHeight + gap)),
            tileWidth,
            tileHeight);
    }

    private static void DrawSectionTitle(DrawingCanvas canvas, Font labelFont, string text, float x, float y)
        => canvas.DrawText(
            new RichTextOptions(labelFont) { Origin = new PointF(x, y) },
            text,
            Brushes.Solid(Color.DimGray),
            pen: null);

    private static void DrawOperation(
        DrawingCanvas canvas,
        Image<Rgba32> photo,
        RectangleF tile,
        string label,
        BooleanOperation operation,
        bool reverseOperands,
        Font labelFont)
    {
        RectangleF preview = new(tile.X, tile.Y + 34, tile.Width, tile.Height - 34);
        PointF center = new(preview.X + (preview.Width * .52F), preview.Y + (preview.Height * .52F));

        // The subject and clip paths deliberately overlap without sharing the same center.
        // That makes each boolean operation visibly different in the final clipped image.
        IPath subject = new StarPolygon(center.X - 44, center.Y, prongs: 6, innerRadii: 58, outerRadii: 116, angle: -18);
        IPath clipPath = new RegularPolygon(center.X + 44, center.Y, vertices: 6, radius: 110, angle: 30);
        IPath firstPath = reverseOperands ? clipPath : subject;
        IPath secondPath = reverseOperands ? subject : clipPath;

        ShapeOptions clipOptions = new()
        {
            BooleanOperation = operation,
        };

        DrawingOptions drawInsideResult = new()
        {
            ShapeOptions = new ShapeOptions
            {
                BooleanOperation = BooleanOperation.Intersection,
            },
        };

        // Clip computes a new path. The original subject and clipping paths are left intact
        // so they can still be drawn as guides over the resulting clipped image. Difference is
        // directional, so the asymmetric column intentionally reverses the operand order.
        IPath result = firstPath.Clip(clipOptions, secondPath);

        canvas.Fill(Brushes.Solid(Color.GhostWhite), new RectanglePolygon(preview));
        canvas.Save(drawInsideResult, result);
        canvas.DrawImage(photo, photo.Bounds, preview, KnownResamplers.Bicubic);
        canvas.Restore();

        canvas.Draw(Pens.Solid(Color.DeepSkyBlue, 3), subject);
        canvas.Draw(Pens.Solid(Color.OrangeRed, 3), clipPath);
        canvas.Draw(Pens.Solid(Color.Black, 2), result);

        canvas.DrawText(
            new RichTextOptions(labelFont) { Origin = new PointF(tile.X, tile.Y) },
            label,
            Brushes.Solid(Color.Black),
            pen: null);
    }
}
