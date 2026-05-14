# SixLabors.Samples.ImageSharp

Runnable ImageSharp samples that load real image files from `assets/imagesharp` and write generated files to `output/imagesharp`.

Run the project from the repository root:

```powershell
dotnet run --project SixLabors.Samples.ImageSharp\SixLabors.Samples.ImageSharp.csproj -c Release
```

## Samples

### ResponsiveImageSample

Shows a typical web image pipeline:

- loads one landscape JPEG
- applies `AutoOrient()` before resize operations
- crops a 16:9 hero image with `ResizeMode.Crop`
- removes EXIF metadata from the exported hero image
- writes the same processed pixels as JPEG and WebP
- writes a square PNG thumbnail

Outputs:

- `responsive-hero.jpg`
- `responsive-hero.webp`
- `responsive-thumbnail.png`

### MetadataAndFormatConversionSample

Shows how to inspect an encoded image before decoding all pixels:

- uses `Image.Identify(...)` to read size information
- uses `Image.DetectFormat(...)` to identify the source format
- reads pixel depth, alpha representation, bounds, frame count, and resolution from `ImageInfo`
- loads, orients, resizes, annotates the image with a metadata HUD, and converts the JPEG source to PNG

Outputs:

- `metadata-converted.png`

### ThumbnailContactSheetSample

Shows image composition with a fixed layout:

- loads landscape and portrait inputs
- normalizes orientation
- crops each source to a uniform tile size
- draws every tile into one larger image

Output:

- `contact-sheet.jpg`

### PixelRowsSample

Shows direct pixel generation with `ProcessPixelRows(...)`:

- creates a blank `Image<Rgba32>`
- maps each pixel to the Mandelbrot complex plane
- writes each generated color directly into the row span

This is intentionally not a grayscale, histogram, or color-conversion sample; ImageSharp already provides higher-level processors for those workflows.

Output:

- `mandelbrot-fractal.png`

### AvatarAndMaskingSample

Shows ImageSharp.Drawing clipping with a real image:

- crops a portrait into a square avatar
- builds a rounded clip shape with `RoundedRectanglePolygon`
- uses `DrawingCanvas.Save(..., clipPath)` to clip subsequent drawing
- draws the avatar into a transparent image so the clipped corners remain transparent

Output:

- `avatar-rounded.png`

### PolygonClippingSample

Shows boolean path clipping with ImageSharp.Drawing:

- builds a subject path and a clipping path
- groups symmetric operations (`Intersection`, `Union`, and `Xor`) separately from `Difference`
- labels each operation with both its name and operator form, such as `Union: A | B`
- shows both `Difference: A - B` and `Difference: B - A` because `Difference` depends on operand order
- draws the clipped result with a photo fill
- overlays the original paths so the operation can be compared visually

Output:

- `polygon-clipping.png`

### TextMeasurementVisualizerSample

Shows how text measurement maps to drawn glyphs:

- measures the same text in separate panels
- prepares the text once with `TextBlock`
- measures and renders that prepared block instead of shaping text twice
- fills the logical advance, glyph bounds, and renderable bounds regions
- labels renderable bounds as the union of advance and bounds
- keeps each measurement separate so the regions can be compared without overlapping outlines

Output:

- `text-measurement-visualizer.png`
