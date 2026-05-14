// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Samples;

namespace SixLabors.Samples.ImageSharp;

/// <summary>
/// Demonstrates composing many source images into one contact sheet.
/// </summary>
internal static class ThumbnailContactSheetSample
{
    private static readonly string[] SourceFiles =
    [
        "landscape-01-adam-kool.jpg",
        "landscape-02-jim-cooke.jpg",
        "landscape-03-johannes-plenio.jpg",
        "landscape-04-john-fowler.jpg",
        "landscape-05-pietro-de-grandi.jpg",
        "landscape-06-tobias-keller.jpg",
        "portrait-01-filipp-romanovski.jpg",
        "portrait-02-good-faces.jpg",
        "portrait-03-jessica-radanavong.jpg",
        "portrait-04-jurica-koletic.jpg",
        "portrait-05-ludovic-migneault.jpg",
        "portrait-06-prince-akachi.jpg",
    ];

    /// <summary>
    /// Saves a contact sheet made from landscape and portrait input images.
    /// </summary>
    public static void Run()
    {
        const int columns = 4;
        const int tileWidth = 300;
        const int tileHeight = 220;
        const int gap = 18;
        const int padding = 24;
        int rows = (SourceFiles.Length + columns - 1) / columns;
        int sheetWidth = (columns * tileWidth) + ((columns - 1) * gap) + (padding * 2);
        int sheetHeight = (rows * tileHeight) + ((rows - 1) * gap) + (padding * 2);

        using Image<Rgba32> sheet = new(sheetWidth, sheetHeight, Color.White.ToPixel<Rgba32>());

        for (int i = 0; i < SourceFiles.Length; i++)
        {
            int column = i % columns;
            int row = i / columns;
            int x = padding + (column * (tileWidth + gap));
            int y = padding + (row * (tileHeight + gap));

            using Image thumbnail = Image.Load(SamplePaths.Asset(SourceFiles[i]));
            thumbnail.Mutate(context => context

                // Pad preserves the full portrait or landscape image, avoiding subject crops while
                // still producing a consistent tile size for the contact sheet grid.
                .AutoOrient()
                .Resize(new ResizeOptions
                {
                    Size = new Size(tileWidth, tileHeight),
                    Mode = ResizeMode.Pad,
                    PadColor = Color.White,
                }));

            sheet.Mutate(context => context.DrawImage(thumbnail, new Point(x, y), 1F));
        }

        sheet.SaveAsJpeg(SamplePaths.Output("contact-sheet.jpg"));
    }
}
