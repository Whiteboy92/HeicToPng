using HeicToPng.FileManager;
using HeicToPng.Interfaces;
using HeicToPng.Services;
using HeicToPng.VideoServices;

namespace HeicToPng;

internal static class Program
{
    private static void Main()
    {
        var directoryPath1 = @"D:\Zdjęcia"; // big disc
        //var directoryPath2 = @"E:\Zdjęcia"; // small
        var ffmpegPath = @"C:\ffmpeg-2024-12-23-git-6c9218d748-full_build\bin\ffmpeg.exe";

        //HeicToPng(directoryPath1);
        ConvertVideos(directoryPath1, ffmpegPath);

        //int totalFiles1 = CountFilesInDirectory(directoryPath1);
        //int totalFiles2 = CountFilesInDirectory(directoryPath2);

        //Console.WriteLine($"\nTotal files in {directoryPath1}: {totalFiles1}");
        //Console.WriteLine($"Total files in {directoryPath2}: {totalFiles2}");

        //CompareFileNames(directoryPath1, directoryPath2);
    }

    private static void HeicToPng(string directoryPath)
    {
        IFileManager fileManager = new FileManager.FileManager();
        IImageConverter imageConverter = new HeicToPngConverter();
        var conversionService = new ImageConversionService(fileManager, imageConverter);

        conversionService.ConvertAllHeicToPng(directoryPath);

        Console.WriteLine("HEIC to PNG conversion process completed.");
    }

    private static void ConvertVideos(string directoryPath, string ffmpegPath)
    {
        IVideoManager videoManager = new VideoManager.VideoManager();
        var videoService = new VideoConversionService(videoManager, ffmpegPath);

        videoService.ConvertAllVideos(directoryPath);
    }

    private static int CountFilesInDirectory(string directoryPath)
    {
        // Get all files in the directory and subdirectories
        var allFiles = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
        return allFiles.Length;
    }

    private static void CompareFileNames(string dir1, string dir2)
    {
        // Get all files in dir1 and dir2, including subdirectories, and use the file name without extension for comparison
        var filesInDir1 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // Key: file name without extension, Value: full file path
        foreach (var file in Directory.GetFiles(dir1, "*", SearchOption.AllDirectories))
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file)!;
            filesInDir1[fileNameWithoutExtension] = file;
        }

        var filesInDir2 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // Key: file name without extension, Value: full file path
        foreach (var file in Directory.GetFiles(dir2, "*", SearchOption.AllDirectories))
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file)!;
            filesInDir2[fileNameWithoutExtension] = file;
        }

        // Find missing files from dir2 that are present in dir1 (using file names without extensions)
        var missingFiles = filesInDir1.Keys.Except(filesInDir2.Keys);

        Console.WriteLine("\nMissing files in directory 2:");
        var enumerable = missingFiles as string[] ?? missingFiles.ToArray();
        if (enumerable.Any())
        {
            foreach (var file in enumerable)
            {
                Console.WriteLine($"{file} - Missing in: {filesInDir1[file]}");
            }
        }
        else
        {
            Console.WriteLine("No missing files.");
        }
    }
}
