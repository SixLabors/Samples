﻿using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

const int QrCodeSize = 25;
bool[,] pattern = GetQrPattern();

// L8 is a grayscale pixel format storing a single 8-bit (1 byte) channel of luminance per pixel.
// Make sure to Dispose() the image in your app! We do it by a using statement in this example:
using Image<L8> image = RenderQrCodeToImage(pattern, 10);

string fileName = Path.Combine(Directory.GetCurrentDirectory(), "qr.png");

// Store the result as a grayscale 1 bit per pixel png for maximum compression:
image.SaveAsPng(fileName, new PngEncoder()
{
    BitDepth = PngBitDepth.Bit1,
    ColorType = PngColorType.Grayscale
});
Console.WriteLine($"Saved to: {fileName}");

static Image<L8> RenderQrCodeToImage(bool[,] pattern, int pixelSize)
{
    int imageSize = pixelSize * QrCodeSize;
    Image<L8> image = new(imageSize, imageSize);

    L8 black = new(0);
    L8 white = new(255);

    image.ProcessPixelRows(pixelAccessor =>
    {
        // Scan the QR pattern row-by-row
        for (int yQr = 0; yQr < QrCodeSize; yQr++)
        {
            // Fill 'pixelSize' number image rows that correspond to the current QR pattern row:
            for (int y = yQr * pixelSize; y < (yQr + 1) * pixelSize; y++)
            {
                // Get a Span<L8> of pixels for the current image row:
                Span<L8> pixelRow = pixelAccessor.GetRowSpan(y);

                // Loop through the values for the current QR pattern row:
                for (int xQr = 0; xQr < QrCodeSize; xQr++)
                {
                    L8 color = pattern[xQr, yQr] ? white : black;

                    // Fill 'pixelSize' number of image pixels corresponding to the current QR pattern value:
                    for (int x = xQr * pixelSize; x < (xQr + 1) * pixelSize; x++)
                    {
                        pixelRow[x] = color;
                    }
                }
            }
        }
    });

    return image;
}

static bool[,] GetQrPattern()
{
    const bool _ = true;
    const bool x = false;
    return new[,]
    {
        { x, x, x, x, x, x, x, _, x, x, _, _, _, x, _, x, x, _, x, x, x, x, x, x, x },
        { x, _, _, _, _, _, x, _, _, _, _, _, _, _, _, _, _, _, x, _, _, _, _, _, x },
        { x, _, x, x, x, _, x, _, x, _, _, x, x, x, _, x, x, _, x, _, x, x, x, _, x },
        { x, _, x, x, x, _, x, _, x, x, _, x, _, x, x, x, _, _, x, _, x, x, x, _, x },
        { x, _, x, x, x, _, x, _, x, x, x, x, _, _, x, _, x, _, x, _, x, x, x, _, x },
        { x, _, _, _, _, _, x, _, _, x, _, _, x, _, x, _, _, _, x, _, _, _, _, _, x },
        { x, x, x, x, x, x, x, _, x, _, x, _, x, _, x, _, x, _, x, x, x, x, x, x, x },
        { _, _, _, _, _, _, _, _, _, x, _, x, _, _, x, x, _, _, _, _, _, _, _, _, _ },
        { x, x, x, x, _, _, x, _, x, _, _, _, _, x, x, _, _, x, _, _, x, x, x, _, x },
        { x, x, _, x, _, _, _, x, x, x, _, _, _, _, x, _, _, _, _, x, _, _, _, x, _ },
        { _, _, _, _, x, _, x, x, x, _, _, _, _, x, x, _, x, x, x, _, _, _, _, _, _ },
        { _, x, _, _, _, x, _, _, _, _, _, x, x, x, _, x, _, x, _, x, _, x, x, _, _ },
        { _, x, _, _, _, _, x, x, _, x, _, x, _, x, x, _, _, x, x, _, x, _, x, x, x },
        { _, x, _, x, _, _, _, _, x, _, x, x, _, _, _, x, _, _, x, x, x, _, _, _, x },
        { _, x, _, _, _, x, x, x, _, x, x, _, x, _, _, _, _, x, _, _, x, _, x, x, _ },
        { x, _, x, _, x, _, _, _, x, _, _, _, x, _, _, x, _, _, x, x, x, _, _, _, x },
        { _, _, x, _, _, _, x, _, _, _, x, x, _, _, _, x, x, x, x, x, x, x, x, x, x },
        { _, _, _, _, _, _, _, _, x, _, x, _, x, _, x, x, x, _, _, _, x, _, x, _, x },
        { x, x, x, x, x, x, x, _, _, _, _, x, x, x, _, _, x, _, x, _, x, _, x, x, x },
        { x, _, _, _, _, _, x, _, _, _, x, _, x, _, x, x, x, _, _, _, x, _, _, x, x },
        { x, _, x, x, x, _, x, _, _, x, x, x, x, x, _, _, x, x, x, x, x, x, _, x, _ },
        { x, _, x, x, x, _, x, _, x, _, _, _, x, x, _, x, _, _, x, _, x, x, x, x, x },
        { x, _, x, x, x, _, x, _, x, x, x, x, x, x, _, x, x, x, x, _, x, _, x, x, _ },
        { x, _, _, _, _, _, x, _, x, _, x, _, _, _, _, _, _, x, x, _, x, _, x, _, _ },
        { x, x, x, x, x, x, x, _, x, x, _, _, x, _, x, _, _, _, _, x, x, x, x, x, x },
    };
}