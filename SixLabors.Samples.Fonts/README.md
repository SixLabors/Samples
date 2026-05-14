# SixLabors.Samples.Fonts

Runnable SixLabors.Fonts samples that load a real font from `assets/fonts` and write generated reports and images to `output/fonts`.

Run the project from the repository root:

```powershell
dotnet run --project SixLabors.Samples.Fonts\SixLabors.Samples.Fonts.csproj -c Release
```

## Samples

### FontLoadingSample

Shows how to load a font file into a `FontCollection`:

- adds a `.ttf` file with `FontCollection.Add(...)`
- reads `FontDescription` metadata from the font name table
- lists the styles available for the loaded `FontFamily`
- reports the source path used by the family

Output:

- `font-loading.txt`

### TextMeasurementSample

Shows how to measure wrapped text before drawing or laying out UI:

- configures `TextOptions` with an origin, wrapping length, and line spacing
- calls `TextMeasurer.Measure(...)` once to get `TextMetrics`
- compares advance, bounds, and renderable bounds
- reports per-line metrics including baseline, extent, and grapheme count

Output:

- `text-measurement.txt`

### TextRunsAndInteractionSample

Shows how a measured layout can answer editor-style questions:

- uses `TextOptions.TextRuns` to apply a different font size to part of the text
- reads glyph and grapheme metrics from the measured layout
- uses `HitTest(...)`, `GetCaretPosition(...)`, and `MoveCaret(...)`
- demonstrates that run indices are grapheme indices

Output:

- `text-runs-and-interaction.txt`

### GlyphMetricsVisualizerSample

Shows how the measured rectangles for one glyph relate to font metrics:

- calls `TextMeasurer.GetGlyphMetrics(...)` for a single laid-out glyph
- draws the logical advance cell and rendered glyph bounds
- annotates width, height, side bearings, advance width, and advance height
- writes the diagram as a PNG for visual inspection

Output:

- `glyph-metrics-visualizer.png`

### LineMetricsVisualizerSample

Shows how measured line metrics map to one laid-out line:

- calls `TextMeasurer.Measure(...)` once to get `TextMetrics.LineMetrics`
- draws the logical line extent and rendered text bounds
- uses text with descenders so the baseline-to-descender distance is visible
- shows `LineSpacing` increasing the logical line box without scaling glyphs
- annotates ascender and descender distances relative to the baseline
- labels the baseline as a position guide rather than a measurement
- annotates `Extent.X` and `LineHeight`

Output:

- `line-metrics-visualizer.png`
