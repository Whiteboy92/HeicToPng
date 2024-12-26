using HeicToPng.FileManager;

namespace HeicToPng.Services;

public class ImageConversionService(IFileManager fileManager, IImageConverter imageConverter)
{
    private int processedFiles;

    public void ConvertAllHeicToPng(string directory)
    {
        var heicFiles = fileManager.GetFiles(directory, ".heic").ToList();
        int totalFiles = heicFiles.Count;

        if (totalFiles == 0)
        {
            Console.WriteLine("No .heic files found to convert.");
            return;
        }

        using var timer = new Timer(_ =>
        {
            DisplayProgress(totalFiles);
        }, null, 0, 100); // Update progress every 100ms

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount // Limit threads to CPU cores
        };

        Parallel.ForEach(heicFiles, parallelOptions, heicFile =>
        {
            try
            {
                var pngFile = Path.ChangeExtension(heicFile, ".png");

                imageConverter.Convert(heicFile, pngFile);

                if (fileManager.FileExists(pngFile))
                {
                    fileManager.DeleteFile(heicFile); // Delete only if conversion succeeded
                }
                else
                {
                    Console.WriteLine($"Conversion failed for: {heicFile}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {heicFile}: {ex.Message}");
            }
            finally
            {
                Interlocked.Increment(ref processedFiles); // Safely increment the counter
            }
        });

        timer.Change(Timeout.Infinite, Timeout.Infinite); // Stop the timer
        DisplayProgress(totalFiles); // Ensure 100% is displayed
        Console.WriteLine(); // Move to a new line after progress
    }

    private void DisplayProgress(int totalFiles)
    {
        int currentProcessed = Interlocked.CompareExchange(ref processedFiles, 0, 0);
        double percentage = (double)currentProcessed / totalFiles * 100;
        Console.Write($"\rProgress: {percentage:F2}% ({currentProcessed}/{totalFiles})");
    }
}
