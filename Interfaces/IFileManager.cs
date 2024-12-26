namespace HeicToPng.FileManager;

public interface IFileManager
{
    IEnumerable<string> GetFiles(string directory, string extension);
    void DeleteFile(string filePath);
    bool FileExists(string pngFile);
}

public interface IImageConverter
{
    void Convert(string inputFile, string outputFile);
}