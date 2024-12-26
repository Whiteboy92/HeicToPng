namespace HeicToPng.FileManager;

public class FileManager : IFileManager
{
    public IEnumerable<string> GetFiles(string directory, string extension)
    {
        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException($"Directory not found: {directory}");

        return Directory.GetFiles(directory, $"*{extension}", SearchOption.AllDirectories);
    }

    public void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
    
    public bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }
}