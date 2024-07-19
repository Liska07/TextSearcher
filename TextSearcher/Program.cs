using System.Configuration;

namespace TextSearcher
{
    public class Program
    {
        const string SourceDirectory = "SourceDirectory";
        const string DestinationDirectory = "DestinationDirectory";
        const string SearchText = "SearchText";
        const string CaseSensitive = "CaseSensitive";

        static void Main(string[] args)
        {
            string sourceDirectory = GetDirectoryPath(SourceDirectory, "source");
           
            if (string.IsNullOrEmpty(sourceDirectory))
            {
                    return;
            }

            string[] files;
            try
            {
                files = Directory.GetFiles(sourceDirectory, "*.txt");

                if (files.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No text files found in the source directory for copying.");
                    Console.ResetColor();

                    return;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred while accessing the source directory '{sourceDirectory}': {ex.Message}");
                Console.ResetColor();
            
                return;
            }

            string destinationDirectory = GetDirectoryPath(DestinationDirectory, "destination");

            if (string.IsNullOrEmpty(destinationDirectory))
            {
                return;
            }

            if (destinationDirectory == sourceDirectory)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The destination directory cannot be the same as the source directory.");
                Console.ResetColor();

                return;
            }

            string searchText = GetSearchText();

            bool caseSensitive = GetCaseSensitive();

            Results results = new();

            CopyFiles(files, destinationDirectory, results);

            SearchTextInFiles(files, searchText, caseSensitive, results);

            ShowResults(results);
        }

        static string GetDirectoryPath(string directoryKey, string directoryName)
        {
            string directoryPath = GetDirectoryPathFromConfig(directoryKey);

            if (string.IsNullOrEmpty(directoryPath))
            {
                directoryPath = AskUserForDirectoryPath(directoryName);
            }
            return directoryPath;
        }

        static string GetDirectoryPathFromConfig(string directoryKey)
        {
            string directoryPath = (ConfigurationManager.AppSettings[directoryKey] ?? "").Trim();

            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"The directory '{directoryPath}' specified for '{directoryKey}' in the 'App.config' file does not exist.");
                Console.ResetColor();

                directoryPath = "";
            }

            return directoryPath;
        }

        static string AskUserForDirectoryPath(string directoryName)
        {
            string directoryPath;

            do
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"Please enter the path to the {directoryName} folder: ");
                Console.ResetColor();
                directoryPath = (Console.ReadLine() ?? "").Trim();

                if (directoryPath?.ToUpper() != "Q" && !Directory.Exists(directoryPath))
                {
                    directoryPath = "";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The specified {directoryName} directory does not exist. Please try again (or enter 'Q' to quit).");
                    Console.ResetColor();
                }
            }
            while (directoryPath.ToUpper() != "Q" && directoryPath == "");

            if (directoryPath.ToUpper() == "Q") 
                directoryPath = "";

            return directoryPath;
        }

        static string GetSearchText()
        {
            string searchText = (ConfigurationManager.AppSettings[SearchText] ?? "").Trim();

            while (string.IsNullOrEmpty(searchText))
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("Insert text for searching: ");
                Console.ResetColor();
                searchText = (Console.ReadLine() ?? "").Trim();
            }

            return searchText;
        }

        static bool GetCaseSensitive()
        {
            string caseSensitiveString = ConfigurationManager.AppSettings[CaseSensitive] ?? "";

            bool caseSensitive;

            if (!bool.TryParse(caseSensitiveString, out caseSensitive))
            {
                caseSensitive = false;
            }

            return caseSensitive;
        }

        public static Results CopyFiles(string[] files, string destinationDirectory, Results results)
        {
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destinationDirectory, fileName);

                try
                {
                    File.Copy(file, destFile, true);
                    results.CopiedFileCount++;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"An error occurred while copying the file '{fileName}': {ex.Message}");
                    Console.ResetColor();
                    results.FailedCopyCount++;
                    continue;
                }
            }

            return results;
        }

        public static Results SearchTextInFiles(string[] files, string searchText, bool caseSensitive, Results results)
        {
            StringComparison comp = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);

                string fileContent;
                try
                {
                    fileContent = File.ReadAllText(file);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"An error occurred while reading the file '{fileName}': {ex.Message}");
                    Console.ResetColor();
                    results.FailedReadCount++;
                    continue;
                }

                if (fileContent.IndexOf(searchText, comp) >= 0)
                {
                    if (!results.TextFound)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"\nThe following files contain the text '{searchText}':");
                        Console.ResetColor();
                        results.TextFound = true;
                    }

                    Console.WriteLine($" * {fileName}");
                    results.TextFoundCount++;
                }
            }
            return results;
        }

        static void ShowResults(Results results)
        {
            if (!results.TextFound)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nNo files contain the specified text.");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nResults:");
            Console.ResetColor();

            Console.WriteLine($" - total files copied: {results.CopiedFileCount}");

            if (results.FailedCopyCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" - total files failed to copy: {results.FailedCopyCount}");
                Console.ResetColor();
            }

            Console.WriteLine($" - total files containing the specified text: {results.TextFoundCount}");

            if (results.FailedReadCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" - total files failed to read: {results.FailedReadCount}");
                Console.ResetColor();
            }
        }
    }
}
