Imports System.IO
Imports SixLabors.ImageSharp
Imports SixLabors.ImageSharp.Processing

Module Program
    Sub Main(args As String())
        Directory.CreateDirectory("output")

        Using image = SixLabors.ImageSharp.Image.Load("fb.jpg")

            image.Mutate(Sub(c)
                             c.Resize(image.Width / 2, image.Height / 2)
                             c.Grayscale()
                         End Sub)

            image.Save("output/fb.png") ' Automatic encoder selected based On extension.
        End Using

    End Sub
End Module
