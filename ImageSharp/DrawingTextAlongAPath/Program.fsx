#r "nuget: SixLabors.ImageSharp"
#r "nuget: SixLabors.ImageSharp.Drawing, 1.0.0-beta15"

open System
open SixLabors.Fonts
open SixLabors.ImageSharp
open SixLabors.ImageSharp.PixelFormats
open SixLabors.ImageSharp.Processing
open SixLabors.ImageSharp.Drawing
open SixLabors.ImageSharp.Drawing.Processing

let img = new Image<Rgba32>(1500, 500)
let pathBuilder = new PathBuilder()
pathBuilder.SetOrigin(new PointF(500f, 0f))
pathBuilder.AddCubicBezier(new PointF(50f, 450f), new PointF(200f, 50f), new PointF(300f, 50f), new PointF(450f, 450f))

// Add more complex paths and shapes here.
let path = pathBuilder.Build()

// For production application we would recomend you create a FontCollection
// singleton and manually install the ttf fonts yourself as using SystemFonts
// can be expensive and you risk font existing or not existing on a deployment
// by deployment basis.
let font = SystemFonts.CreateFont("Microsoft Sans Serif", 39f, FontStyle.Regular)
let text = "Hello World Hello World Hello World Hello World Hello World"

// Draw the text along the path wrapping at the end of the line
let textOptions = new TextOptions(font)
textOptions.WrappingLength <- path.ComputeLength()
textOptions.VerticalAlignment <- VerticalAlignment.Bottom
textOptions.HorizontalAlignment <- HorizontalAlignment.Left

// Let's generate the text as a set of vectors drawn along the path
let glyphs = TextBuilder.GenerateGlyphs(text, path, textOptions)

img.Mutate(fun ctx -> ctx.Fill(Color.White) // white background image
                         .Draw(Color.Gray, 3f, path) // draw the path so we can see what the text is supposed to be following
                         .Fill(Color.Black, glyphs)|>ignore)

img.Save("wordart.png")
