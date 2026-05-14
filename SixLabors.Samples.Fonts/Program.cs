// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using SixLabors.Samples;
using SixLabors.Samples.Fonts;

Directory.CreateDirectory(SamplePaths.OutputDirectory);

FontLoadingSample.Run();
TextMeasurementSample.Run();
TextRunsAndInteractionSample.Run();
GlyphMetricsVisualizerSample.Run();
LineMetricsVisualizerSample.Run();

Console.WriteLine($"Saved Fonts sample output to: {SamplePaths.OutputDirectory}");
