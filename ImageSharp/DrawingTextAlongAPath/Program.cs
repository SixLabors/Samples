// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
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
            using (Image img = new Image<Rgba32>(1500, 500))
            {
                PathBuilder pathBuilder = new PathBuilder();
                pathBuilder.SetOrigin(new PointF(500, 0));
                pathBuilder.AddCubicBezier(new PointF(50, 450), new PointF(200, 50), new PointF(300, 50), new PointF(450, 450));

                // Add more complex paths and shapes here.
                IPath path = pathBuilder.Build();

                // For production application we would recomend you create a FontCollection
                // singleton and manually install the ttf fonts yourself as using SystemFonts
                // can be expensive and you risk font existing or not existing on a deployment
                // by deployment basis.
                var font = SystemFonts.CreateFont("Segoe UI", 39, FontStyle.Regular);
                const string text = "Hello World Hello World Hello World Hello World Hello World";

                // Draw the text along the path wrapping at the end of the line
                var textOptions = new TextOptions(font)
                {
                    WrappingLength = path.ComputeLength(),
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };

                // Let's generate the text as a set of vectors drawn along the path
                var glyphs = TextBuilder.GenerateGlyphs(text, path, textOptions);

                img.Mutate(ctx => ctx
                    .Fill(Color.White) // white background image
                    .Draw(Color.Gray, 3, path) // draw the path so we can see what the text is supposed to be following
                    .Fill(Color.Black, glyphs));

                img.Save("output/wordart.png");
            }
        }

        public static IImageProcessingContext ApplyScalingWaterMark(this IImageProcessingContext processingContext,
            Font font,
            string text,
            Color color,
            float padding,
            bool wordwrap)
        {
            if (wordwrap)
            {
                // return processingContext.ApplyScalingWaterMarkWordWrap(font, text, color, padding);
            }
            //else
            {
                return processingContext.ApplyScalingWaterMarkSimple(font, text, color, padding);
            }
        }

        private static IImageProcessingContext ApplyScalingWaterMarkSimple(
            this IImageProcessingContext processingContext,
            Font font,
            string text,
            Color color,
            float padding)
        {
            Size imgSize = processingContext.GetCurrentSize();

            // Measure the text size
            FontRectangle size = TextMeasurer.MeasureSize(text, new TextOptions(font));

            // Find out how much we need to scale the text to fill the space (up or down)
            float scalingFactor = Math.Min(imgSize.Width - padding / size.Width, imgSize.Height - padding / size.Height);

            // Create a new font
            Font scaledFont = new Font(font, scalingFactor * font.Size);

            var center = new PointF(imgSize.Width / 2, imgSize.Height / 2);
            var textOptions = new RichTextOptions(scaledFont)
            {
                Origin = center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            return processingContext.DrawText(textOptions, text, color);
        }

        //private static IImageProcessingContext ApplyScalingWaterMarkWordWrap(
        //    this IImageProcessingContext processingContext,
        //    Font font,
        //    string text,
        //    Color color,
        //    float padding)
        //{
        //    Size imgSize = processingContext.GetCurrentSize();
        //    float targetWidth = imgSize.Width - (padding * 2);
        //    float targetHeight = imgSize.Height - (padding * 2);

        //    float targetMinHeight = imgSize.Height - (padding * 3); // must be with in a margin width of the target height

        //    // now we are working i 2 dimensions at once and can't just scale because it will cause the text to
        //    // reflow we need to just try multiple times

        //    var scaledFont = font;
        //    FontRectangle s = new FontRectangle(0, 0, float.MaxValue, float.MaxValue);

        //    float scaleFactor = (scaledFont.Size / 2);// everytime we change direction we half this size
        //    int trapCount = (int)scaledFont.Size * 2;
        //    if (trapCount < 10)
        //    {
        //        trapCount = 10;
        //    }

        //    bool isTooSmall = false;

        //    while ((s.Height > targetHeight || s.Height < targetMinHeight) && trapCount > 0)
        //    {
        //        if (s.Height > targetHeight)
        //        {
        //            if (isTooSmall)
        //            {
        //                scaleFactor = scaleFactor / 2;
        //            }

        //            scaledFont = new Font(scaledFont, scaledFont.Size - scaleFactor);
        //            isTooSmall = false;
        //        }

        //        if (s.Height < targetMinHeight)
        //        {
        //            if (!isTooSmall)
        //            {
        //                scaleFactor = scaleFactor / 2;
        //            }
        //            scaledFont = new Font(scaledFont, scaledFont.Size + scaleFactor);
        //            isTooSmall = true;
        //        }
        //        trapCount--;

        //        s = TextMeasurer.Measure(text, new RendererOptions(scaledFont)
        //        {
        //            WrappingWidth = targetWidth
        //        });
        //    }

        //    var center = new PointF(padding, imgSize.Height / 2);
        //    var textGraphicsOptions = new TextGraphicsOptions()
        //    {
        //        TextOptions = {
        //            HorizontalAlignment = HorizontalAlignment.Left,
        //            VerticalAlignment = VerticalAlignment.Center,
        //            WrapTextWidth = targetWidth
        //        }
        //    };
        //    return processingContext.DrawText(textGraphicsOptions, text, scaledFont, color, center);
        //}
    }
}
