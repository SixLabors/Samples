// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace DrawWaterMarkOnImage
{
    static class Program
    {
        const string LongText = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec aliquet lorem at magna mollis, non semper erat aliquet. In leo tellus, sollicitudin non eleifend et, luctus vel magna. Proin at lacinia tortor, malesuada molestie nisl. Quisque mattis dui quis eros ultricies, quis faucibus turpis dapibus. Donec urna ipsum, dignissim eget condimentum at, condimentum non magna. Donec non urna sit amet lectus tincidunt interdum vitae vitae leo. Aliquam in nisl accumsan, feugiat ipsum condimentum, scelerisque diam. Vivamus quam diam, rhoncus ut semper eget, gravida in metus.
                                Nullam quis malesuada metus. In hac habitasse platea dictumst. Aliquam faucibus eget eros nec vulputate. Quisque sed dolor lacus. Proin non dolor vitae massa rhoncus vestibulum non a arcu. Morbi mollis, arcu id pretium dictum, augue dui cursus eros, eu pharetra arcu ante non lectus. Integer quis tellus ipsum. Integer feugiat augue id tempus rutrum. Ut eget interdum leo, id fermentum lacus. Morbi euismod, mi at tempus finibus, ante risus ornare eros, eu ultrices ipsum dolor vitae risus. Mauris molestie pretium massa vitae maximus. Fusce ut egestas ex, vitae semper nulla. Proin pretium elit libero, et interdum enim molestie ac.
                                Pellentesque fermentum vitae lacus non aliquet. Sed nulla ipsum, hendrerit sit amet vulputate varius, volutpat eget est. Pellentesque eget ante erat. Vestibulum venenatis ex quis pretium sagittis. Etiam vel nibh sit amet leo gravida efficitur. In hac habitasse platea dictumst. Nullam lobortis euismod sem dapibus aliquam. Proin accumsan velit a magna gravida condimentum. Nam non massa ac nibh viverra rutrum. Phasellus elit tortor, malesuada et purus nec, placerat mattis neque. Proin auctor risus vel libero ultrices, id fringilla erat facilisis. Donec rutrum, enim sit amet faucibus viverra, velit tellus aliquam tellus, et tempus tellus diam sed dui. Integer fringilla convallis nisl venenatis elementum. Sed volutpat massa ut mauris accumsan, mollis finibus tortor pretium.";
        static void Main(string[] args)
        {
            System.IO.Directory.CreateDirectory("output");
            using (var img = Image.Load("fb.jpg"))
            {
                // For production application we would recommend you create a FontCollection
                // singleton and manually install the ttf fonts yourself as using SystemFonts
                // can be expensive and you risk font existing or not existing on a deployment
                // by deployment basis.
                Font font = SystemFonts.CreateFont("Arial", 10); // The size is merely a starting point here for scaling.

                using (var img2 = img.Clone(ctx => ctx.ApplyScalingWaterMark(font, "A short piece of text", Color.White, 5, false)))
                {
                    img2.Save("output/simple.png");
                }


                using (var img2 = img.Clone(ctx => ctx.ApplyScalingWaterMark(font, LongText, Color.White, 5, true)))
                {
                    img2.Save("output/wrapped.png");
                }

                // The original `img` object has not been altered at all.
            }
        }

        private static IImageProcessingContext ApplyScalingWaterMark(this IImageProcessingContext processingContext,
            Font font,
            string text,
            Color color,
            float padding,
            bool _)
        {
            Size imageSize = processingContext.GetCurrentSize();
            float targetWidth = imageSize.Width - (padding * 2);
            float targetHeight = imageSize.Height - (padding * 2);

            // Binary search for the optimal font size
            float minFontSize = 1;
            float maxFontSize = font.Size;

            // First check to see if the unwrapped text fits the image and scale up if not.
            // We floor the result to take into consideration accumulated rounding errors.
            FontRectangle currentBounds = TextMeasurer.MeasureAdvance(text, new TextOptions(font));
            if (currentBounds.Width < targetWidth)
            {
                maxFontSize = MathF.Floor(maxFontSize * (targetWidth / currentBounds.Width));
            }

            while (minFontSize < maxFontSize)
            {
                float midFontSize = (minFontSize + maxFontSize) / 2;
                Font midFont = new(font, midFontSize);
                currentBounds = TextMeasurer.MeasureAdvance(text, new TextOptions(midFont)
                {
                    WrappingLength = targetWidth
                });

                if (currentBounds.Height > targetHeight)
                {
                    maxFontSize = midFontSize - 0.1f; // Reduce the max font size
                }
                else
                {
                    minFontSize = midFontSize + 0.1f; // Increase the min font size
                }
            }

            // Use the optimal font size found
            Font scaledFont = new(font, minFontSize);

            // Create text options with adjusted font
            RichTextOptions textOptions = new(scaledFont)
            {
                Origin = new Vector2(padding, imageSize.Height * .5f),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                WrappingLength = targetWidth
            };

            // Draw the text on the image
            return processingContext.DrawText(textOptions, text, color);
        }
    }
}
