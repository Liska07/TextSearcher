using NLog;
using System.Configuration;

namespace TextSearcher
{
    public class Program
    {
        const string SourceDirectory = "SourceDirectory";
        const string DestinationDirectory = "DestinationDirectory";
        const string SearchText = "SearchText";
        const string CaseSensitive = "CaseSensitive";
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

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
                    logger.Warn("No text files found in the source directory for copying.");
                    Console.ResetColor();

                    Console.WriteLine("\nPress any key to exit...");
                    Console.ReadKey();

                    return;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                logger.Error($"An error occurred while accessing the source directory '{sourceDirectory}': {ex.Message}");
                Console.ResetColor();

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();

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
                logger.Error("The destination directory cannot be the same as the source directory.");
                Console.ResetColor();

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();

                return;
            }

            string searchText = GetSearchText();
            bool caseSensitive = GetCaseSensitive();

            Results results = new();
            CopyFiles(files, destinationDirectory, results);
            SearchTextInFiles(files, searchText, caseSensitive, results);
            results.ShowResults();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static string GetDirectoryPath(string directoryKey, string directoryName)
        {
            string directoryPath = GetDirectoryPathFromConfig(directoryKey);

            if (string.IsNullOrEmpty(directoryPath))
            {
                directoryPath = AskUserForDirectoryPath(directoryName);
            }

            if (!string.IsNullOrEmpty(directoryPath))
            {
                string fullPath = Path.GetFullPath(directoryPath);
                logger.Info($"Full path {directoryName} directory: {fullPath}");
            }
 
            return directoryPath;
        }

        static string GetDirectoryPathFromConfig(string directoryKey)
        {
            string directoryPath = (ConfigurationManager.AppSettings[directoryKey] ?? "").Trim();

            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                logger.Error($"The directory '{directoryPath}' specified for '{directoryKey}' in the 'App.config' file does not exist.");
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
                    logger.Error($"The specified {directoryName} directory does not exist. Please try again (or enter 'Q' to quit).");
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

            if (!string.IsNullOrEmpty(searchText))
                logger.Info($"Text for searching: {searchText}");

            while (string.IsNullOrEmpty(searchText))
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("Please enter the text for searching: ");
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

            logger.Info($"Case sensitive: {caseSensitiveString}");

            return caseSensitive;
        }

        public static Results CopyFiles(string[] files, string destinationDirectory, Results results)
        {

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nCopying process:");
            Console.ResetColor();

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destinationDirectory, fileName);

                try
                {
                    File.Copy(file, destFile, true);
                    results.CopiedFileCount++;
                    logger.Info($"'{fileName}' successfully copied");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    logger.Error($"Failed to copy '{fileName}': {ex.Message}");
                    Console.ResetColor();
                    results.FailedCopyCount++;
                }
            }

            return results;
        }

        public static Results SearchTextInFiles(string[] files, string searchText, bool caseSensitive, Results results)
        {
            StringComparison comp = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nSearching process:");
            Console.ResetColor();

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
                    logger.Error($"Failed to read '{fileName}': {ex.Message}");
                    Console.ResetColor();
                    results.FailedReadCount++;
                    continue;
                }

                if (fileContent.IndexOf(searchText, comp) >= 0)
                {
                    logger.Info($"'{fileName}' contains the text '{searchText}'");
                    results.TextFoundCount++;
                }
            }

            if (results.TextFoundCount == 0)
            {
                logger.Info($"No files contain the text '{searchText}'");
            }

            return results;
        }
    }
}
