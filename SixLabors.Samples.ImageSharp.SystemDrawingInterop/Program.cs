// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Drawing.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Samples;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingImageFormat = System.Drawing.Imaging.ImageFormat;
using DrawingPixelFormat = System.Drawing.Imaging.PixelFormat;
using DrawingRectangle = System.Drawing.Rectangle;

Directory.CreateDirectory(SamplePaths.OutputDirectory);

WrapSystemDrawingBitmap();

Console.WriteLine($"Saved System.Drawing interop sample output to: {SamplePaths.OutputDirectory}");

/// <summary>
/// Loads a System.Drawing bitmap, wraps its locked pixel buffer with ImageSharp, and processes the same memory.
/// </summary>
static unsafe void WrapSystemDrawingBitmap()
{
    using DrawingBitmap bitmap = new(SamplePaths.Asset("landscape-03-johannes-plenio.jpg"));
    DrawingRectangle bounds = new(0, 0, bitmap.Width, bitmap.Height);
    BitmapData bitmapData = bitmap.LockBits(bounds, ImageLockMode.ReadWrite, DrawingPixelFormat.Format24bppRgb);

    try
    {
        // Format24bppRgb stores each pixel in BGR byte order, which matches ImageSharp's Bgr24
        // pixel type. The locked GDI+ buffer can therefore be processed without copying.
        using Image<Bgr24> image = Image.WrapMemory<Bgr24>(
            pointer: (void*)bitmapData.Scan0,
            bufferSizeInBytes: bitmapData.Stride * bitmapData.Height,
            width: bitmapData.Width,
            height: bitmapData.Height,
            rowStrideInBytes: bitmapData.Stride);

        // Normal in-place ImageSharp processors can operate on wrapped memory. Do not resize a
        // wrapped image: ImageSharp does not own this buffer and cannot reallocate the
        // System.Drawing bitmap's storage to match a different image size.
        image.Mutate(context => context
            .Sepia(.85F)
            .Vignette(Color.Black));
    }
    finally
    {
        bitmap.UnlockBits(bitmapData);
    }

    bitmap.Save(SamplePaths.Output("system-drawing-wrapmemory.png"), DrawingImageFormat.Png);
}
