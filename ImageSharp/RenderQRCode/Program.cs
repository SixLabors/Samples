using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

const int QrCodeSize = 25;

bool[,] pattern = GetQrPattern();

// 'L8' is a grayscale pixel format storing a single 8-bit (1 byte) channel of luminance.
// Do not forget the using keyword so the Image is properly disposed.
using Image<L8> image = RenderQrCodeToImage(pattern, 10);

string fileName = Path.Combine(Directory.GetCurrentDirectory(), "qr.png");

// Store the result as a grayscale 1 bit per pixel png for maximum compression:
image.SaveAsPng(fileName, new PngEncoder()
{
    BitDepth = PngBitDepth.Bit1,
    ColorType = PngColorType.Grayscale
});
Console.WriteLine($"Saved to: {fileName}");

// ImageSharp 2.0 will break this method: https://github.com/SixLabors/ImageSharp/issues/1739
// We will update the example after the release. See ImageSharp 2.0 variant in comments.
static Image<L8> RenderQrCodeToImage(bool[,] pattern, int pixelSize)
{
    int imageSize = pixelSize * QrCodeSize;
    Image<L8> image = new Image<L8>(imageSize, imageSize);

    L8 black = new L8(0);
    L8 white = new L8(255);

    // Scan the QR pattern row-by-row
    for (int yQr = 0; yQr < QrCodeSize; yQr++)
    {
        // Fill 'pixelSize' number image rows that correspond to the current QR pattern row:
        for (int y = yQr * pixelSize; y < (yQr + 1) * pixelSize; y++)
        {
            // Get a Span<L8> of pixels for the current image row:
            Span<L8> pixelRow = image.GetPixelRowSpan(y);
            
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
    
    // ImageSharp 2.0 variant:
    // image.ProcessPixelRows(pixelAccessor =>
    // {
    //     // Scan the QR pattern row-by-row
    //     for (int yQr = 0; yQr < QrCodeSize; yQr++)
    //     {
    //         // Fill 'pixelSize' number image rows that correspond to the current QR pattern row:
    //         for (int y = yQr * pixelSize; y < (yQr + 1) * pixelSize; y++)
    //         {
    //             // Get a Span<L8> of pixels for the current image row:
    //             Span<L8> pixelRow = pixelAccessor.GetRowSpan(y);
    //
    //             // Loop through the values for the current QR pattern row:
    //             for (int xQr = 0; xQr < QrCodeSize; xQr++)
    //             {
    //                 L8 color = pattern[xQr, yQr] ? white : black;
    //
    //                 // Fill 'pixelSize' number of image pixels corresponding to the current QR pattern value:
    //                 for (int x = xQr * pixelSize; x < (xQr + 1) * pixelSize; x++)
    //                 {
    //                     pixelRow[x] = color;
    //                 }
    //             }
    //         }
    //     }
    // });

    return image;
}

// Made with http://asciiqr.com/
const string AsciiQr = @"
█▀▀▀▀▀█ ▀▀   ▀ ▀▀ █▀▀▀▀▀█
█ ███ █ █▄ █▀█▄█▀ █ ███ █
█ ▀▀▀ █ ▀█▀▀▄ █ ▀ █ ▀▀▀ █
▀▀▀▀▀▀▀ ▀▄▀▄▀ █▄▀ ▀▀▀▀▀▀▀
██▀█  ▀▄█▄   ▀█  ▀ ▄▀▀▀▄▀
 ▄  ▀▄▀▀▀  ▄▄█▀▄▀█▀▄ ▄▄  
 █ ▄  ▀▀▄▀▄█ ▀▀▄ ▀█▄█ ▀▀█
▄▀▄ ▄▀▀▀▄▀▀ █  ▄ ▀▄▄█ ▀▀▄
  ▀   ▀ ▄ █▀▄ ▄██▀▀▀█▀█▀█
█▀▀▀▀▀█   ▄▀█▀▄▄█ ▀ █ ▀██
█ ███ █ ▄▀▀▀██ ▄▀▀█▀██▄█▄
█ ▀▀▀ █ █▀█▀▀▀ ▀▀██ █ █▀ 
▀▀▀▀▀▀▀ ▀▀  ▀ ▀    ▀▀▀▀▀▀
";

static bool[,] GetQrPattern()
{
    bool[,] pattern = new bool[QrCodeSize, QrCodeSize];

    ReadOnlySpan<char> qrStr = AsciiQr.AsSpan(2); // slice away first \r\n
    
    for (int y = 0; y < QrCodeSize; y+=2)
    {
        for (int x = 0; x < QrCodeSize; x++)
        {
            char codeChar = qrStr[(y / 2) * (QrCodeSize + 2) + x];
            pattern[x, y] = codeChar switch { '█' => false, '▀' => false, _ => true };
            
            if (y + 1 < QrCodeSize)
            {
                pattern[x, y + 1] = codeChar switch { '█' => false, '▄' => false, _ => true };
            }
        }
    }

    return pattern;
}


