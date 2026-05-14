// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Samples;

namespace SixLabors.Samples.ImageSharp;

/// <summary>
/// Demonstrates cropping a portrait into an avatar and clipping the result with an ImageSharp.Drawing path.
/// </summary>
internal static class AvatarAndMaskingSample
{
    /// <summary>
    /// Creates a square avatar and saves a PNG with transparent rounded corners.
    /// </summary>
    public static void Run()
    {
        using Image<Rgba32> source = Image.Load<Rgba32>(SamplePaths.Asset("portrait-04-jurica-koletic.jpg"));
        using Image<Rgba32> avatar = source.Clone(context => context
            .AutoOrient()
            .Resize(new ResizeOptions
            {
                Size = new Size(512, 512),
                Mode = ResizeMode.Crop,

                // Portrait avatars usually need a little more headroom than a centered crop provides.
                Position = AnchorPositionMode.Top,
            }));

        using Image<Rgba32> roundedAvatar = new(avatar.Width, avatar.Height);

        // The rounded rectangle is used as a clipping path. Drawing into a transparent image
        // keeps the clipped-out corners transparent in the final PNG.
        IPath clipPath = new RoundedRectanglePolygon(new Rectangle(0, 0, avatar.Width, avatar.Height), 96);

        DrawingOptions clipToAvatarShape = new()
        {
            ShapeOptions = new ShapeOptions
            {
                BooleanOperation = BooleanOperation.Intersection,
            },
        };

        roundedAvatar.Mutate(context => context.Paint(canvas =>
        {
            // Intersection keeps the rounded rectangle as the drawable area. The default Difference
            // would subtract the rounded rectangle and draw only into the corners.
            canvas.Save(clipToAvatarShape, clipPath);
            canvas.DrawImage(avatar, avatar.Bounds, avatar.Bounds, KnownResamplers.Bicubic);
            canvas.Restore();
        }));

        roundedAvatar.SaveAsPng(SamplePaths.Output("avatar-rounded.png"));
    }
}
