// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Text;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Samples;
using IOPath = System.IO.Path;

namespace SixLabors.Samples.ImageSharp;

/// <summary>
/// Demonstrates reading image information without decoding pixels, then converting the image to another format.
/// </summary>
internal static class MetadataAndFormatConversionSample
{
    private const string HudFontFile = "roboto-regular.ttf";

    /// <summary>
    /// Saves a resized PNG converted from the source JPEG with a metadata HUD.
    /// </summary>
    public static void Run()
    {
        string sourcePath = SamplePaths.Asset("portrait-01-filipp-romanovski.jpg");

        // Identify and DetectFormat inspect the encoded file headers without decoding the full pixel image.
        ImageInfo imageInfo = Image.Identify(sourcePath);
        IImageFormat imageFormat = Image.DetectFormat(sourcePath);
        string metadata = CreateMetadataText(sourcePath, imageInfo, imageFormat);

        // The JPEG source identifies as RGB24, which has no alpha channel. Decode to Rgba32 so
        // the semi-transparent HUD color can be alpha blended against the image pixels instead
        // of being flattened through an RGB-only working format.
        using Image<Rgba32> image = Image.Load<Rgba32>(sourcePath);
        image.Mutate(context => context

            // Max mode preserves the full image while fitting it inside the requested bounds.
            .AutoOrient()
            .Resize(new ResizeOptions
            {
                Size = new Size(900, 900),
                Mode = ResizeMode.Max,
            }));

        image.Metadata.ExifProfile = null;
        DrawMetadataHud(image, metadata);

        // The source file is a JPEG; choosing SaveAsPng demonstrates explicit format conversion.
        image.SaveAsPng(SamplePaths.Output("metadata-converted.png"), new PngEncoder());
    }

    /// <summary>
    /// Creates display text from image header information.
    /// </summary>
    /// <param name="sourcePath">The source image path.</param>
    /// <param name="imageInfo">The information returned by <see cref="Image.Identify(string)"/>.</param>
    /// <param name="imageFormat">The encoded image format.</param>
    /// <returns>Text suitable for an image HUD.</returns>
    private static string CreateMetadataText(string sourcePath, ImageInfo imageInfo, IImageFormat imageFormat)
    {
        int bitsPerPixel = imageInfo.PixelType.BitsPerPixel;
        long pixelMemorySize = imageInfo.GetPixelMemorySize();
        StringBuilder builder = new();

        builder.AppendLine($"File: {IOPath.GetFileName(sourcePath)}");
        builder.AppendLine($"Format: {imageFormat.Name}");
        builder.AppendLine($"Size: {imageInfo.Width} x {imageInfo.Height}");
        builder.AppendLine($"Bounds: {imageInfo.Bounds}");
        builder.AppendLine($"Pixel depth: {bitsPerPixel} bpp");
        builder.AppendLine($"Alpha: {imageInfo.PixelType.AlphaRepresentation}");
        builder.AppendLine($"Resolution: {imageInfo.Metadata.HorizontalResolution:0.##} x {imageInfo.Metadata.VerticalResolution:0.##} {imageInfo.Metadata.ResolutionUnits}");
        builder.AppendLine($"Frames: {imageInfo.FrameMetadataCollection.Count}");
        builder.Append($"Pixel memory size: {pixelMemorySize:N0} bytes");

        return builder.ToString();
    }

    /// <summary>
    /// Draws a readable metadata overlay onto the converted image.
    /// </summary>
    /// <param name="image">The image to annotate.</param>
    /// <param name="metadata">The metadata text to draw.</param>
    private static void DrawMetadataHud(Image<Rgba32> image, string metadata)
    {
        const float margin = 24;
        const float padding = 18;

        FontCollection fonts = new();
        Font font = fonts.Add(SamplePaths.Asset(HudFontFile)).CreateFont(18);
        float textWidth = image.Width - (margin * 2) - (padding * 2);
        RichTextOptions textOptions = new(font)
        {
            Origin = new PointF(margin + padding, margin + padding),
            WrappingLength = textWidth
        };

        // MeasureRenderableBounds uses the same shaping and wrapping options as DrawText, so
        // the HUD is sized from the actual renderable text area rather than from a line model.
        FontRectangle textBounds = TextMeasurer.MeasureRenderableBounds(metadata, textOptions);

        RectangleF hudBounds = new(
            textBounds.X - padding,
            textBounds.Y - padding,
            textBounds.Width + (padding * 2),
            textBounds.Height + (padding * 2));

        Color hudBackground = Color.Black.WithAlpha(.58F);

        image.Mutate(context => context.Paint(canvas =>
        {
            canvas.Fill(Brushes.Solid(hudBackground), new RoundedRectanglePolygon(hudBounds, 16));
            canvas.DrawText(textOptions, metadata, Brushes.Solid(Color.White), pen: null);
        }));
    }
}
