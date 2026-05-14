// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace SixLabors.Samples.ImageSharp;

/// <summary>
/// Demonstrates generating responsive web image variants from one source image.
/// </summary>
internal static class ResponsiveImageSample
{
    /// <summary>
    /// Saves a cropped hero image in multiple web formats and a square thumbnail.
    /// </summary>
    public static void Run()
    {
        using Image image = Image.Load(SamplePaths.Asset("landscape-01-adam-kool.jpg"));

        using Image hero = image.Clone(context => context

            // AutoOrient honors camera EXIF orientation before resize/crop operations are applied.
            .AutoOrient()
            .Resize(new ResizeOptions
            {
                Size = new Size(1600, 900),
                Mode = ResizeMode.Crop,
            }));

        hero.Metadata.ExifProfile = null;

        // The same processed pixels can be encoded multiple ways for browsers with different format support.
        hero.SaveAsJpeg(SamplePaths.Output("responsive-hero.jpg"), new JpegEncoder { Quality = 82 });
        hero.SaveAsWebp(SamplePaths.Output("responsive-hero.webp"), new WebpEncoder { Quality = 82 });

        using Image thumbnail = image.Clone(context => context
            .AutoOrient()
            .Resize(new ResizeOptions
            {
                Size = new Size(480, 480),
                Mode = ResizeMode.Crop,
                Sampler = KnownResamplers.Lanczos3,
            }));

        thumbnail.SaveAsPng(SamplePaths.Output("responsive-thumbnail.png"), new PngEncoder());
    }
}
