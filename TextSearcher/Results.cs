using NLog;

namespace TextSearcher
{
    public class Results
    {
        public int CopiedFileCount = 0;
        public int FailedCopyCount = 0;
        public int TextFoundCount = 0;
        public int FailedReadCount = 0;
        readonly Logger logger = LogManager.GetCurrentClassLogger();

        public bool IsEqual(Results other)
        {
            return CopiedFileCount == other.CopiedFileCount &&
                   FailedCopyCount == other.FailedCopyCount &&
                   TextFoundCount == other.TextFoundCount &&
                   FailedReadCount == other.FailedReadCount;
        }

        public void ShowResults()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nResults summary:");
            logger.Info($"Total files copied: {CopiedFileCount}");
            Console.ResetColor();

            if (FailedCopyCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                logger.Warn($"Total files failed to copy: {FailedCopyCount}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            logger.Info($"Total files containing the specified text: {TextFoundCount}");
            Console.ResetColor();

            if (FailedReadCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                logger.Warn($"Total files failed to read: {FailedReadCount}");
                Console.ResetColor();
            }
        }
    }
}
