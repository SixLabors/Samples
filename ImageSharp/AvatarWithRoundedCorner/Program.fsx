#r "nuget: SixLabors.ImageSharp"
#r "nuget: SixLabors.ImageSharp.Drawing, 1.0.0-beta15"

open SixLabors.ImageSharp
open SixLabors.ImageSharp.PixelFormats
open SixLabors.ImageSharp.Processing
open SixLabors.ImageSharp.Drawing
open SixLabors.ImageSharp.Drawing.Processing

let BuildCorners(imageWidth:float32, imageHeight:float32, cornerRadius:float32) =
    // First create a square
    let rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius)

    // Then cut out of the square a circle so we are left with a corner
    let cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius))

    // Corner is now a corner shape positions top left
    // let's make 3 more positioned correctly, we can do that by translating the original around the center of the image.
    let rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1f
    let bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1f

    // Move it across the width of the image - the width of the shape
    let cornerTopRight = cornerTopLeft.RotateDegree(90f).Translate(rightPos, 0f)
    let cornerBottomLeft = cornerTopLeft.RotateDegree(-90f).Translate(0f, bottomPos)
    let cornerBottomRight = cornerTopLeft.RotateDegree(180f).Translate(rightPos, bottomPos)

    new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight)

// This method can be seen as an inline implementation of an `IImageProcessor`:
// (The combination of `IImageOperations.Apply()` + this could be replaced with an `IImageProcessor`)
let ApplyRoundedCorners(context:IImageProcessingContext, cornerRadius: float32):IImageProcessingContext =
    let size = context.GetCurrentSize()
    let corners = BuildCorners(float32 size.Width, float32 size.Height, cornerRadius)

    context.SetGraphicsOptions(new GraphicsOptions(
            Antialias = true,
            // Enforces that any part of this shape that has color is punched out of the background
            AlphaCompositionMode = PixelAlphaCompositionMode.DestOut
    )) |> ignore

    // Mutating in here as we already have a cloned original
    // use any color (not Transparent), so the corners will be clipped
    for path in corners do
        context.Fill(Color.Red, path)|>ignore
    context

// Implements a full image mutating pipeline operating on IImageProcessingContext
let ConvertToAvatar(context:IImageProcessingContext, size:Size, cornerRadius:float32):IImageProcessingContext = 
    ApplyRoundedCorners(context.Resize(ResizeOptions(
            Size = size,
            Mode = ResizeMode.Crop
    )),cornerRadius)

let img = Image.Load("fb.jpg")
img.Clone(fun x -> ConvertToAvatar(x,new Size(200, 200), 20f) |> ignore).Save("fb.png")
img.Clone(fun x -> ConvertToAvatar(x,new Size(200, 200), 100f) |> ignore).Save("fb-round.png")
img.Clone(fun x -> ConvertToAvatar(x,new Size(200, 200), 150f) |> ignore).Save("fb-rounder.png")
