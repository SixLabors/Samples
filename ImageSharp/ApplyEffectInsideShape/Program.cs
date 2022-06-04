// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System;

namespace CustomImageProcessor
{
    static class Program
    {
        static void Main(string[] args)
        {
            System.IO.Directory.CreateDirectory("output");

            using Image image = Image.Load("fb.jpg");

            int outerRadii = Math.Min(image.Width, image.Height) / 2;
            IPath star = new Star(new PointF(image.Width / 2, image.Height / 2), 5, outerRadii / 2, outerRadii).AsClosedPath();

            // Apply the effect here inside the shape
            image.Mutate(x => x.Clip(star, y => y.GaussianBlur(15)));

            image.Save("output/fb.png");
        }
    }
}
