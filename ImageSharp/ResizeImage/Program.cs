﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ResizeImage
{
    static class Program
    {
        static void Main(string[] args)
        {
            System.IO.Directory.CreateDirectory("output");

            using Image image = Image.Load("fb.jpg");
            image.Mutate(x => x
                 .Resize(image.Width / 2, image.Height / 2)
                 .Grayscale());

            image.Save("output/fb.png"); // Automatic encoder selected based on extension.
        }
    }
}
