// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace DrawingTextAlongAPath
{
    static class Program
    {
        static void Main(string[] args)
        {
            System.IO.Directory.CreateDirectory("output");

            using Image img = new Image<Rgba32>(1500, 500);
            PathBuilder pathBuilder = new();
            pathBuilder.SetOrigin(new PointF(500, 0));
            pathBuilder.AddCubicBezier(new PointF(50, 450), new PointF(200, 50), new PointF(300, 50), new PointF(450, 450));

            // Add more complex paths and shapes here.
            IPath path = pathBuilder.Build();

            // For production application we would recommend you create a FontCollection
            // singleton and manually install the ttf fonts yourself as using SystemFonts
            // can be expensive and you risk font existing or not existing on a deployment
            // by deployment basis.
            Font font = SystemFonts.CreateFont("Segoe UI", 39, FontStyle.Regular);
            const string text = "Hello World Hello World Hello World Hello World Hello World";

            // Draw the text along the path wrapping at the end of the line
            TextOptions textOptions = new(font)
            {
                WrappingLength = path.ComputeLength(),
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            // Let's generate the text as a set of vectors drawn along the path
            IPathCollection glyphs = TextBuilder.GenerateGlyphs(text, path, textOptions);

            img.Mutate(ctx => ctx
                .Fill(Color.White) // white background image
                .Draw(Color.Gray, 3, path) // draw the path so we can see what the text is supposed to be following
                .Fill(Color.Black, glyphs));

            img.Save("output/wordart.png");
        }
    }
}