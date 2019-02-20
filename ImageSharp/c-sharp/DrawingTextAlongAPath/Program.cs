// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;

namespace DrawingTextAlongAPath
{
    // TODO: This example does no longer work with beta5!
    static class Program
    {
        static void Main(string[] args)
        {
            System.IO.Directory.CreateDirectory("output");
            using (Image<Rgba32> img = new Image<Rgba32>(1500, 500))
            {
                PathBuilder pathBuilder = new PathBuilder();
                pathBuilder.SetOrigin(new PointF(500, 0));
                pathBuilder.AddBezier(new PointF(50, 450), new PointF(200, 50), new PointF(300, 50), new PointF(450, 450));
                // add more complex paths and shapes here.

                IPath path = pathBuilder.Build();

                // For production application we would recomend you create a FontCollection
                // singleton and manually install the ttf fonts yourself as using SystemFonts
                // can be expensive and you risk font existing or not existing on a deployment
                // by deployment basis.
                var font = SystemFonts.CreateFont("Arial", 39, FontStyle.Regular);

                string text = "Hello World Hello World Hello World Hello World Hello World";
                var textGraphicsOptions = new TextGraphicsOptions(true) // draw the text along the path wrapping at the end of the line
                {
                    WrapTextWidth = path.Length
                };

                // lets generate the text as a set of vectors drawn along the path

                var glyphs = SixLabors.Shapes.TextBuilder.GenerateGlyphs(text, path, new RendererOptions(font, textGraphicsOptions.DpiX, textGraphicsOptions.DpiY)
                {
                    HorizontalAlignment = textGraphicsOptions.HorizontalAlignment,
                    TabWidth = textGraphicsOptions.TabWidth,
                    VerticalAlignment = textGraphicsOptions.VerticalAlignment,
                    WrappingWidth = textGraphicsOptions.WrapTextWidth,
                    ApplyKerning = textGraphicsOptions.ApplyKerning
                });

                img.Mutate(ctx => ctx
                    .Fill(Rgba32.White) // white background image
                    .Draw(Rgba32.Gray, 3, path) // draw the path so we can see what the text is supposed to be following
                    .Fill((GraphicsOptions)textGraphicsOptions, Rgba32.Black, glyphs));

                img.Save("output/wordart.png");
            }
        }

        public static IImageProcessingContext<TPixel> ApplyScalingWaterMark<TPixel>(this IImageProcessingContext<TPixel> processingContext, Font font, string text, TPixel color, float padding, bool wordwrap)
           where TPixel : struct, IPixel<TPixel>
        {
            if (wordwrap)
            {
                return processingContext.ApplyScalingWaterMarkWordWrap(font, text, color, padding);
            }
            else
            {
                return processingContext.ApplyScalingWaterMarkSimple(font, text, color, padding);
            }
        }

        public static IImageProcessingContext<TPixel> ApplyScalingWaterMarkSimple<TPixel>(this IImageProcessingContext<TPixel> processingContext, Font font, string text, TPixel color, float padding)
            where TPixel : struct, IPixel<TPixel>
        {
            return processingContext.Apply(img =>
            {
                float targetWidth = img.Width - (padding * 2);
                float targetHeight = img.Height - (padding * 2);

                // measure the text size
                SizeF size = TextMeasurer.Measure(text, new RendererOptions(font));

                //find out how much we need to scale the text to fill the space (up or down)
                float scalingFactor = Math.Min(img.Width / size.Width, img.Height / size.Height);

                //create a new font
                Font scaledFont = new Font(font, scalingFactor * font.Size);

                var center = new PointF(img.Width / 2, img.Height / 2);

                var textGraphicsOptions = new TextGraphicsOptions(true)
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                img.Mutate(i => i.DrawText(textGraphicsOptions, text, scaledFont, color, center));
            });
        }

        public static IImageProcessingContext<TPixel> ApplyScalingWaterMarkWordWrap<TPixel>(this IImageProcessingContext<TPixel> processingContext, Font font, string text, TPixel color, float padding)
            where TPixel : struct, IPixel<TPixel>
        {
            return processingContext.Apply(img =>
            {
                float targetWidth = img.Width - (padding * 2);
                float targetHeight = img.Height - (padding * 2);

                float targetMinHeight = img.Height - (padding * 3); // must be with in a margin width of the target height

                // now we are working i 2 dimensions at once and can't just scale because it will cause the text to
                // reflow we need to just try multiple times

                var scaledFont = font;
                SizeF s = new SizeF(float.MaxValue, float.MaxValue);

                float scaleFactor = (scaledFont.Size / 2);// everytime we change direction we half this size
                int trapCount = (int)scaledFont.Size * 2;
                if (trapCount < 10)
                {
                    trapCount = 10;
                }

                bool isTooSmall = false;

                while ((s.Height > targetHeight || s.Height < targetMinHeight) && trapCount > 0)
                {
                    if (s.Height > targetHeight)
                    {
                        if (isTooSmall)
                        {
                            scaleFactor = scaleFactor / 2;
                        }

                        scaledFont = new Font(scaledFont, scaledFont.Size - scaleFactor);
                        isTooSmall = false;
                    }

                    if (s.Height < targetMinHeight)
                    {
                        if (!isTooSmall)
                        {
                            scaleFactor = scaleFactor / 2;
                        }
                        scaledFont = new Font(scaledFont, scaledFont.Size + scaleFactor);
                        isTooSmall = true;
                    }
                    trapCount--;

                    s = TextMeasurer.Measure(text, new RendererOptions(scaledFont)
                    {
                        WrappingWidth = targetWidth
                    });
                }

                var center = new PointF(padding, img.Height / 2);
                var textGraphicsOptions = new TextGraphicsOptions(true)
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    WrapTextWidth = targetWidth
                };
                img.Mutate(i => i.DrawText(textGraphicsOptions, text, scaledFont, color, center));
            });
        }
    }
}
