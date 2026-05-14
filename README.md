# Samples
General samples for various Six Labors projects

This repository uses [Git Large File Storage](https://docs.github.com/en/github/managing-large-files/installing-git-large-file-storage). Please follow the linked instructions to ensure you have it set up in your environment.

## ImageSharp
Runnable ImageSharp samples can be found in:

- [SixLabors.Samples.ImageSharp](./SixLabors.Samples.ImageSharp/) demonstrates common image processing workflows.
- [SixLabors.Samples.ImageSharp.SystemDrawingInterop](./SixLabors.Samples.ImageSharp.SystemDrawingInterop/) demonstrates Windows-only `System.Drawing.Bitmap` interop with `Image.WrapMemory(...)`.

## Fonts
Runnable SixLabors.Fonts samples can be found in [SixLabors.Samples.Fonts](./SixLabors.Samples.Fonts/).

## WebGPU
Runnable ImageSharp.Drawing WebGPU samples can be found in:

- [SixLabors.Samples.WebGPUWindow](./SixLabors.Samples.WebGPUWindow/) renders ImageSharp.Drawing content directly into a native WebGPU window.
- [SixLabors.Samples.WebGPUExternalSurface](./SixLabors.Samples.WebGPUExternalSurface/) renders into a WinForms-owned surface and demonstrates embedding WebGPU drawing inside an application UI.
