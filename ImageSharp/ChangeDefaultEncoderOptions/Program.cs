﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ChangeDefaultEncoderOptions
{
    class Program
    {
        static void Main(string[] args)
        {
            // Let's switch out the default encoder for jpeg to one that saves at 90 quality
            Configuration.Default.ImageFormatsManager.SetEncoder(JpegFormat.Instance, new JpegEncoder()
            {
                Quality = 90
            });
        }
    }
}
