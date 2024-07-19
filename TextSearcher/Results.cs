namespace TextSearcher
{
    public class Results
    {
        public bool TextFound = false;
        public int CopiedFileCount = 0;
        public int FailedCopyCount = 0;
        public int FailedReadCount = 0;
        public int TextFoundCount = 0;

        public bool IsEqual(Results other)
        {
            return CopiedFileCount == other.CopiedFileCount &&
                   FailedCopyCount == other.FailedCopyCount &&
                   FailedReadCount == other.FailedReadCount &&
                   TextFoundCount == other.TextFoundCount &&
                   TextFound == other.TextFound;
        }
    }
}
