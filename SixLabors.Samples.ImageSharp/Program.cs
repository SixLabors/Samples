// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using SixLabors.Samples;
using SixLabors.Samples.ImageSharp;

Directory.CreateDirectory(SamplePaths.OutputDirectory);

ResponsiveImageSample.Run();
MetadataAndFormatConversionSample.Run();
ThumbnailContactSheetSample.Run();
PixelRowsSample.Run();
AvatarAndMaskingSample.Run();
PolygonClippingSample.Run();
TextMeasurementVisualizerSample.Run();

Console.WriteLine($"Saved ImageSharp sample output to: {SamplePaths.OutputDirectory}");
