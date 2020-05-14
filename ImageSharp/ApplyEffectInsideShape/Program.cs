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


                // no this implementation of our sample, we want to clone out our source image so we can apply 
                // various effects to it without mutating the origional yet.
                using (var clone = image.Clone(p => {
                    p.GaussianBlur(15); // apply the effect here you and inside the shape
                }))
                {
                    // crop the cloned down to just the size of the shape (this is due to the way ImageBrush works)
                    clone.Mutate(x => x.Crop((Rectangle)star.Bounds));

                    // use an image brush to apply section of cloned image as the source for filling the shape
                    var brush = new ImageBrush(clone);

                    // now fill the shape with the image brsuh cotaining the portion of 
                    // cloned image with the effects applied
                    image.Mutate(c => c.Fill(brush, star));
                }

                image.Mutate(x => x
                     .ProcessInsideShape(star, p => p.GaussianBlur(15)));

                image.Save("output/fb.png");
            }
        }
    }

    public static class ImageProcessorExtensions
    {
        // this is just the nice friendly extension method at wraps the actual image processor
        public static IImageProcessingContext ProcessInsideShape(this IImageProcessingContext context, IPath path, Action<IImageProcessingContext> innerProcessingOperations)
        {
            return context.ApplyProcessor(new RecursiveImageProcessor(context.GetShapeGraphicsOptions(), path, innerProcessingOperations));
        }
    }

    // This is the root pixel type agnostic image processor.
    public class RecursiveImageProcessor : IImageProcessor
    {
        public RecursiveImageProcessor(ShapeGraphicsOptions options, IPath path, Action<IImageProcessingContext> innerProcessing)
        {
            this.Options = options;
            this.Path = path;
            this.InnerProcessingOperations = innerProcessing;
        }
        public ShapeGraphicsOptions Options { get; }
        public IPath Path { get; }

        public Action<IImageProcessingContext> InnerProcessingOperations { get; }

        // This is called when we want to build a pixel specific image processor, this is where you get access to the target image to the first time.
        public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return new RecursiveImageProcessorInner<TPixel>(this, source, configuration, sourceRectangle);
        }

        // the main work horse class this has access to the pixel buffer but in an abstract/generic way.
        private class RecursiveImageProcessorInner<TPixel> : IImageProcessor<TPixel>
            where TPixel : unmanaged, IPixel<TPixel>
        {
            private RecursiveImageProcessor recursiveImageProcessor;
            private Image<TPixel> source;
            private readonly Configuration configuration;
            private readonly Rectangle sourceRectangle;

            public RecursiveImageProcessorInner(RecursiveImageProcessor recursiveImageProcessor, Image<TPixel> source, Configuration configuration, Rectangle sourceRectangle)
            {
                this.recursiveImageProcessor = recursiveImageProcessor;
                this.source = source;
                this.configuration = configuration;
                this.sourceRectangle = sourceRectangle;
            }

            public void Dispose()
            {
            }

            public void Execute()
            {
                // no this implementation of our sample, we want to clone out our source image so we can apply 
                // various effects to it without mutating the origional yet.
                using (var clone = source.Clone(recursiveImageProcessor.InnerProcessingOperations))
                {
                    // crop it down to just the size of the shape
                    clone.Mutate(x => x.Crop((Rectangle)recursiveImageProcessor.Path.Bounds));

                    // use an image brush to apply cloned image as the source for filling the shape
                    var brush = new ImageBrush(clone);
                    
                    // fill the shape using the image brush
                    var processor = new FillPathProcessor(recursiveImageProcessor.Options, brush, recursiveImageProcessor.Path);
                    using (var p = processor.CreatePixelSpecificProcessor<TPixel>(configuration, source, sourceRectangle))
                    {
                        p.Execute();
                    }
                }
            }
        }
    }
}
