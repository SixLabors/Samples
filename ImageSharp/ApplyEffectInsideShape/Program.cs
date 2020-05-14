// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing.Processing.Processors.Drawing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors;

namespace CustomImageProcessor
{
    static class Program
    {
        static void Main(string[] args)
        {
            System.IO.Directory.CreateDirectory("output");

            using (Image image = Image.Load("fb.jpg"))
            {
                var outerRadii = Math.Min(image.Width, image.Height) / 2;
                var star = new Star(new PointF(image.Width / 2, image.Height / 2), 5, outerRadii / 2, outerRadii);

                // we want to clone out our source image so we can apply 
                // various effects to it without mutating the original yet.
                using (var clone = image.Clone(p =>
                {
                    p.GaussianBlur(15); // apply the effect here you and inside the shape
                }))
                {
                    // crop the cloned down to just the size of the shape (this is due to the way ImageBrush works)
                    clone.Mutate(x => x.Crop((Rectangle)star.Bounds));

                    // use an image brush to apply section of cloned image as the source for filling the shape
                    var brush = new ImageBrush(clone);

                    // now fill the shape with the image brush containing the portion of 
                    // cloned image with the effects applied
                    image.Mutate(c => c.Fill(brush, star));
                }

                image.Save("output/fb.png");
            }
        }
    }
}
