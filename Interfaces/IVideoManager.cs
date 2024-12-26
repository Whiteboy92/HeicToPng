namespace HeicToPng.Interfaces
{
    public interface IVideoManager
    {
        IEnumerable<string> GetVideoFiles(string directory);
        void DeleteFile(string filePath);
        void RenameFile(string oldPath, string newPath);
    }
}