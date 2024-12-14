using System.Diagnostics;

namespace Otus.ParallelFileReading
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Пример для 3 файлов
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Files");
            string filePath1 = $"{folderPath}\\file1.txt";
            string filePath2 = $"{folderPath}\\file2.txt";
            string filePath3 = $"{folderPath}\\file3.txt";

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Чтение 3 файлов параллельно
            Task<int> task1 = CountSpacesInFileAsync(filePath1);
            Task<int> task2 = CountSpacesInFileAsync(filePath2);
            Task<int> task3 = CountSpacesInFileAsync(filePath3);

            // Ожидание завершения всех задач
            await Task.WhenAll(task1, task2, task3);

            int totalSpaces = task1.Result + task2.Result + task3.Result;
            stopwatch.Stop();

            Console.WriteLine($"Total spaces in 3 files: {totalSpaces}");
            Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");

            // Пример для папки
            stopwatch.Restart();
            int totalFolderSpaces = await CountSpacesInFolderAsync(folderPath);
            stopwatch.Stop();

            Console.WriteLine($"Total spaces in folder: {totalFolderSpaces}");
            Console.WriteLine($"Time taken for folder: {stopwatch.ElapsedMilliseconds} ms");
        }

        static async Task<int> CountSpacesInFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} not found.");
                return 0;
            }

            string content = await File.ReadAllTextAsync(filePath);
            return content.Count(c => c == ' ');
        }

        static async Task<int> CountSpacesInFolderAsync(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"Folder {folderPath} not found.");
                return 0;
            }

            string[] files = Directory.GetFiles(folderPath);
            Task<int>[] tasks = files.Select(file => CountSpacesInFileAsync(file)).ToArray();
            int[] results = await Task.WhenAll(tasks);
            return results.Sum();
        }
    }
}
