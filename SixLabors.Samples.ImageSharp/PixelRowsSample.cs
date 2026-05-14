// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Samples;

namespace SixLabors.Samples.ImageSharp;

/// <summary>
/// Demonstrates direct pixel generation by writing a Mandelbrot fractal through row spans.
/// </summary>
internal static class PixelRowsSample
{
    private static readonly Rgba32 MandelbrotSetColor = Color.MidnightBlue.ToPixel<Rgba32>();
    private static readonly Vector4 FastEscapeColor = Color.DeepSkyBlue.ToPixel<Rgba32>().ToScaledVector4();
    private static readonly Vector4 SlowEscapeColor = Color.Gold.ToPixel<Rgba32>().ToScaledVector4();

    /// <summary>
    /// Generates and saves a Mandelbrot fractal image.
    /// </summary>
    public static void Run()
    {
        const int width = 1200;
        const int height = 800;
        const int maxIterations = 512;

        // The offsets move the viewport center from the origin to the most recognizable area:
        // the main cardioid, the period-2 bulb, and the detailed boundary between them.
        const double realOffset = -0.55D;
        const double imaginaryOffset = -0.08D;

        double halfWidth = width / 2D;
        double halfHeight = height / 2D;

        // These values set the complex-plane area covered by the image: 3.2 units wide
        // and 2.2 units tall, divided evenly across the output pixels.
        double realScale = 3.2D / width;
        double imaginaryScale = 2.2D / height;

        using Image<Rgba32> image = new(width, height);
        Rgba32[] palette = new Rgba32[maxIterations + 1];

        for (int i = 0; i < maxIterations; i++)
        {
            // The palette is independent of pixel location, so build it once instead of
            // calculating the same color for every pixel with the same escape count.
            // Lower values escaped quickly; higher values stayed bounded for longer.
            float t = i / (float)maxIterations;
            palette[i] = InterpolatePaletteColor(FastEscapeColor, SlowEscapeColor, t);
        }

        palette[maxIterations] = MandelbrotSetColor;

        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<Rgba32> row = accessor.GetRowSpan(y);

                // Map the image row to the imaginary axis. The scale and offset frame the
                // most recognizable part of the Mandelbrot set rather than the full plane.
                double imaginary = ((y - halfHeight) * imaginaryScale) + imaginaryOffset;

                for (int x = 0; x < row.Length; x++)
                {
                    // Map the pixel column to the real axis, matching the row mapping above.
                    double real = ((x - halfWidth) * realScale) + realOffset;
                    int iterations = CountMandelbrotIterations(real, imaginary, maxIterations);

                    // Fractal generation is pure per-pixel work, so writing each row span directly avoids
                    // allocating an intermediate bitmap or forcing the data through a higher-level processor.
                    row[x] = palette[iterations];
                }
            }
        });

        image.SaveAsPng(SamplePaths.Output("mandelbrot-fractal.png"));
    }

    /// <summary>
    /// Counts how long one point remains bounded under the Mandelbrot recurrence.
    /// </summary>
    /// <param name="real">The real component of the point being tested.</param>
    /// <param name="imaginary">The imaginary component of the point being tested.</param>
    /// <param name="maxIterations">The maximum number of iterations to test.</param>
    /// <returns>The escape iteration count, or <paramref name="maxIterations"/> for points treated as inside the set.</returns>
    private static int CountMandelbrotIterations(double real, double imaginary, int maxIterations)
    {
        // These constants describe two simple regions that are known to be inside the set.
        const double cardioidCenterReal = 0.25D;
        const double periodTwoBulbCenterReal = -1D;
        const double periodTwoBulbRadiusSquared = 0.25D * 0.25D;

        double realFromCardioidCenter = real - cardioidCenterReal;
        double imaginarySquared = imaginary * imaginary;
        double cardioidDistanceSquared = (realFromCardioidCenter * realFromCardioidCenter) + imaginarySquared;

        // The main cardioid is the large heart-shaped body of the Mandelbrot set. Points inside
        // it never escape, so the image can mark them as "inside" without running the recurrence.
        bool isInsideMainCardioid =
            (cardioidDistanceSquared * (cardioidDistanceSquared + realFromCardioidCenter))
            <= (cardioidCenterReal * imaginarySquared);

        double realFromPeriodTwoBulbCenter = real - periodTwoBulbCenterReal;

        // The circular period-2 bulb is centered at (-1, 0) with radius 0.25. Like the main
        // cardioid, every point inside it is known to be part of the set.
        bool isInsidePeriodTwoBulb =
            ((realFromPeriodTwoBulbCenter * realFromPeriodTwoBulbCenter) + imaginarySquared)
            <= periodTwoBulbRadiusSquared;

        if (isInsideMainCardioid || isInsidePeriodTwoBulb)
        {
            return maxIterations;
        }

        double zReal = 0;
        double zImaginary = 0;
        double zRealSquared = 0;
        double zImaginarySquared = 0;
        int iterations = 0;

        // Mandelbrot iteration starts at z = 0 and repeatedly applies z = z² + c.
        // Once |z|² exceeds 4, the point has escaped and can be colored by escape speed.
        while ((zRealSquared + zImaginarySquared) <= 4D && iterations < maxIterations)
        {
            double nextReal = zRealSquared - zImaginarySquared + real;
            zImaginary = (2D * zReal * zImaginary) + imaginary;
            zReal = nextReal;
            zRealSquared = zReal * zReal;
            zImaginarySquared = zImaginary * zImaginary;
            iterations++;
        }

        return iterations;
    }

    /// <summary>
    /// Blends two palette colors by a normalized amount.
    /// </summary>
    /// <param name="start">The color used when <paramref name="amount"/> is zero.</param>
    /// <param name="end">The color used when <paramref name="amount"/> is one.</param>
    /// <param name="amount">The interpolation amount from zero to one.</param>
    /// <returns>The interpolated palette color.</returns>
    private static Rgba32 InterpolatePaletteColor(Vector4 start, Vector4 end, float amount)
    {
        Vector4 result = Vector4.Lerp(start, end, amount);

        return Rgba32.FromScaledVector4(result);
    }
}
