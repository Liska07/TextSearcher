namespace TextSearcher
{
    public class Results
    {
        public int CopiedFileCount = 0;
        public int FailedCopyCount = 0;
        public int TextFoundCount = 0;
        public int FailedReadCount = 0;

        public bool IsEqual(Results other)
        {
            return CopiedFileCount == other.CopiedFileCount &&
                   FailedCopyCount == other.FailedCopyCount &&
                   TextFoundCount == other.TextFoundCount &&
                   FailedReadCount == other.FailedReadCount;
        }
    }
}
