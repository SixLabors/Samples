// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomImageProcessor
{
    static class Program
    {
        public class BlurRegion
        {
            public RectangleF Rect { get; set; }
            public bool Blur { get; set; }
        }

        static void Main(string[] args)
        {
            System.IO.Directory.CreateDirectory("output");

            using Image image = Image.Load("fb.jpg");

            // fixup the source image to be rotated correctly for the image
            image.Mutate(img => img.Transform(new AffineTransformBuilder()
                .AppendRotationDegrees(10)
            ));

            // lets get a few overlapping shapes to blur
            var parts = new[] {
                new BlurRegion()
                {
                    Rect = new RectangleF(70, 20, 150, 150),
                        Blur = true,
                },
                new BlurRegion()
                {
                    Rect = new RectangleF(120, 120, 20, 20),
                    Blur = false,
                },
                new BlurRegion()
                {
                    Rect = new RectangleF(20, 100, 300, 50),
                    Blur = true,
                },
                new BlurRegion()
                {
                    Rect = new RectangleF(170, 115, 20, 20),
                    Blur = false,
                },
            };

            using var layederd = image.Clone(o => o.BlurRegionsLayered(parts));
            layederd.Save("output/fb-layer.png");

            using var combined = image.Clone(o => o.BlurRegionsCombined(parts));
            combined.Save("output/fb-combined.png");

            image.Save("output/fb.png");
        }

        // combine into a single blur then unblur operation, combine all the blurs together, then cut out all the unblurs afterwards
        // the order of blur/unblur makes no difference in this verion
        private static IImageProcessingContext BlurRegionsCombined(this IImageProcessingContext processingContext, IEnumerable<BlurRegion> blurRegions)
        {
            var blurPaths = blurRegions.Where(x => x.Blur == true)
                .Select(x => new RectangularPolygon(x.Rect.X, x.Rect.Y, x.Rect.Width, x.Rect.Height)) // this can be any polygon/path not just rectangles
                .ToList();

            // use the first shape as the shape to combine into
            IPath path = blurPaths[0];

            // we have more than 1 blur region, combine them all together
            if (blurPaths.Count > 1)
            {
                // skip 1 as path is already the first item in the collection
                var remainingPaths = blurPaths.Skip(1);
                path = path.Clip(new ShapeOptions() { ClippingOperation = ClippingOperation.Union }, remainingPaths); // merge all the blurs together 
            }

            // now we have some blurs lets start choping out regions to leave unblured
            var unblurPaths = blurRegions.Where(x => x.Blur == false)
                .Select(x => new RectangularPolygon(x.Rect.X, x.Rect.Y, x.Rect.Width, x.Rect.Height)) // this can be any polygon/path not just rectangles
                .ToList();

            if (unblurPaths.Count > 0)
            {
                // chop out the unblur regions fro the overall blur path
                path = path.Clip(new ShapeOptions() { ClippingOperation = ClippingOperation.Difference }, unblurPaths);
            }

            return processingContext.Clip(path, y => y.GaussianBlur(15));
        }

        // apply each blur/unblur operation in turn such that a blur ontop of an unblur will reblur a section
        // the order of blur/unblur will effect the outcome in this verion
        private static IImageProcessingContext BlurRegionsLayered(this IImageProcessingContext processingContext, IEnumerable<BlurRegion> blurRegions)
        {
            var paths = blurRegions
                .Select(x => (x.Blur, path: new RectangularPolygon(x.Rect.X, x.Rect.Y, x.Rect.Width, x.Rect.Height))) // this can be any polygon/path not just rectangles
                .ToList();

            IPath path = null;
            foreach (var p in paths)
            {
                if (p.Blur)
                {
                    if (path == null)
                    {
                        path = p.path;
                    }
                    else
                    {
                        path = path.Clip(new ShapeOptions() { ClippingOperation = ClippingOperation.Union }, p.path);
                    }
                }
                else
                {
                    if (path == null)
                    {
                        // noop can't unblur when no blue has happend yet
                    }
                    else
                    {
                        path = path.Clip(new ShapeOptions() { ClippingOperation = ClippingOperation.Difference }, p.path);
                    }
                }
            }

            return processingContext.Clip(path, y => y.GaussianBlur(15));
        }
    }
}
