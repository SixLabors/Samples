# SixLabors.Samples.ImageSharp.SystemDrawingInterop

Windows-only ImageSharp interop sample showing how to edit a `System.Drawing.Bitmap` through `Image.WrapMemory(...)`.

Run the project from the repository root:

```powershell
dotnet run --project SixLabors.Samples.ImageSharp.SystemDrawingInterop\SixLabors.Samples.ImageSharp.SystemDrawingInterop.csproj -c Release
```

## Sample

### WrapMemory System.Drawing Bitmap

Shows how to wrap externally-owned bitmap memory:

- loads a JPEG into a `System.Drawing.Bitmap`
- locks the bitmap with `LockBits(...)`
- wraps the locked `Format24bppRgb` memory as `Image<Bgr24>` with `Image.WrapMemory(...)`
- applies normal ImageSharp processors to the wrapped memory
- unlocks and saves the original `System.Drawing.Bitmap`

`Format24bppRgb` stores pixels in BGR byte order, so the sample wraps the locked bitmap as `Bgr24` and processes the bitmap in place without copying pixels into a separate ImageSharp-owned buffer.

Wrapped images should only be used with in-place operations. Do not resize a wrapped image: ImageSharp does not own the external buffer and cannot reallocate the `System.Drawing.Bitmap` storage to match a different width, height, or stride.

Output:

- `system-drawing-wrapmemory.png`
