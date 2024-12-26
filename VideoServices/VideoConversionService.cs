using System.Diagnostics;
using HeicToPng.Interfaces;

namespace HeicToPng.VideoServices;

public class VideoConversionService(IVideoManager videoManager, string ffmpegPath)
{
    private readonly IVideoManager videoManager = videoManager ?? throw new ArgumentNullException(nameof(videoManager));
    private readonly string ffmpegPath = ffmpegPath ?? throw new ArgumentNullException(nameof(ffmpegPath));

    public void ConvertAllVideos(string directory)
    {
        var videoFiles = videoManager.GetVideoFiles(directory)
            .Where(file => Path.GetExtension(file).Equals(".mp4", StringComparison.CurrentCultureIgnoreCase) && ContainsHevcCodec(file))
            .ToList();

        int totalFiles = videoFiles.Count;
        int processedFiles = 0;

        if (totalFiles == 0)
        {
            Console.WriteLine("No HEVC .mp4 video files found for conversion.");
            return;
        }

        using var timer = new Timer(_ =>
        {
            DisplayProgress(totalFiles, processedFiles);
        }, null, 0, 100);

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
        Parallel.ForEach(videoFiles, parallelOptions, videoFile =>
        {
            try
            {
                string outputFile = GenerateOutputFileName(videoFile);
                bool success = ConvertHevcToH264(videoFile, outputFile);

                if (success && File.Exists(outputFile))
                {
                    Interlocked.Increment(ref processedFiles);
                }
                else
                {
                    Console.WriteLine($"Conversion failed for {videoFile}, file not deleted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {videoFile}: {ex.Message}");
            }
        });

        timer.Change(Timeout.Infinite, Timeout.Infinite);
        DisplayProgress(totalFiles, processedFiles);
        Console.WriteLine("\nVideo conversion process completed.");
    }


    private string GenerateOutputFileName(string inputFile)
    {
        string directory = Path.GetDirectoryName(inputFile)!;
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFile);
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return Path.Combine(directory, $"{fileNameWithoutExtension}_{timestamp}.mp4");
    }

    private bool ContainsHevcCodec(string videoPath)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = $"-i \"{videoPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = processInfo;
        process.Start();

        string output = process.StandardError.ReadToEnd();
        process.WaitForExit();
        
        if (output.Contains("Video: hevc"))
        {
            return true;
        }

        return false;
    }

    private bool ConvertHevcToH264(string inputPath, string outputPath)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = $"-i \"{inputPath}\" -c:v libx264 -preset veryfast -crf 23 -c:a copy \"{outputPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = processInfo;

        process.Start();
        string errorOutput = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            Console.WriteLine($"FFmpeg failed with exit code {process.ExitCode}. Error: {errorOutput}");
            return false;
        }

        return true;
    }

    private static void DisplayProgress(int totalFiles, int processedFiles)
    {
        double percentage = (double)processedFiles / totalFiles * 100;
        Console.Write($"\rConverting: {processedFiles}/{totalFiles} files converted ({percentage:F2}%)");
    }
}
