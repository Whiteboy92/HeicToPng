using ImageMagick;

namespace HeicToPng.FileManager;

public class HeicToPngConverter : IImageConverter
{
    public void Convert(string inputFile, string outputFile)
    {
        using var image = new MagickImage(inputFile);
        
        image.Format = MagickFormat.Png;
        image.Write(outputFile);
    }
}