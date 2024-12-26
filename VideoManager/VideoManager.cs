using HeicToPng.Interfaces;

namespace HeicToPng.VideoManager
{
    public class VideoManager : IVideoManager
    {
        public IEnumerable<string> GetVideoFiles(string directory)
        {
            return Directory.GetFiles(directory, "*.mp4", SearchOption.AllDirectories);
        }

        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public void RenameFile(string oldPath, string newPath)
        {
            if (File.Exists(oldPath) && !File.Exists(newPath))
            {
                File.Move(oldPath, newPath);
            }
        }
    }
}