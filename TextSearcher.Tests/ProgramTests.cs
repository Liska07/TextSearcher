namespace TextSearcher.Tests
{
    public class ProgramTests
    {
        string tempPath = "";
        string sourceDirectory = "";
        string destinationDirectory = "";
        string[] files = [];
        

        [OneTimeSetUp]
        public void InitializeTestFiles()
        {
            try
            {
                tempPath = Path.GetTempPath();
                sourceDirectory = Path.Combine(tempPath, "Source");

                Directory.CreateDirectory(sourceDirectory);

                File.WriteAllText(Path.Combine(sourceDirectory, "file1.txt"), "This is the content of file1.");
                File.WriteAllText(Path.Combine(sourceDirectory, "file2.txt"), "This file contains the search text. Need to find.");
                File.WriteAllText(Path.Combine(sourceDirectory, "file3.txt"), "Another file without the search text.");
                File.WriteAllText(Path.Combine(sourceDirectory, ".txt"), "This file without name.");
                File.WriteAllText(Path.Combine(sourceDirectory, "empty.txt"), "");
                File.Create(Path.Combine(sourceDirectory, "file1.jpg")).Dispose();

                files = Directory.GetFiles(sourceDirectory, "*.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while setting up test files: {ex.Message}");
                throw;
            }
        }

        [SetUp]
        public void InitializeDestinationDirectory()
        {
            try
            {
                destinationDirectory = Path.Combine(tempPath, "Destination");
                Directory.CreateDirectory(destinationDirectory);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the destination directory: {ex.Message}");
                throw;
            }
        }

        [TearDown]
        public void CleanUpDestinationDirectory()
        {
            try
            {
                Directory.Delete(destinationDirectory, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting destination directory: {ex.Message}");
            }
        }

        [OneTimeTearDown]
        public void CleanUpSourceDirectory()
        {
            try
            {
                Directory.Delete(sourceDirectory, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting source directory: {ex.Message}");
            }
        }

        [Test]
        public void Test_CopyFiles()
        {
            Results actualResults = new();

            Program.CopyFiles(files, destinationDirectory, actualResults);

            bool areAllFilesExist = true;
            bool areAllFilesEqual = true;

            foreach (var sourceFile in files)
            {
                string fileName = Path.GetFileName(sourceFile);
                string destinationFile = Path.Combine(destinationDirectory, fileName);

                if (File.Exists(destinationFile))
                {
                    bool areFilesEqual = CompareFiles(sourceFile, destinationFile);

                    if (!areFilesEqual)
                    {
                        Console.WriteLine($"File '{destinationFile}' is not equal to '{sourceFile}.");
                        areAllFilesEqual = false;
                    }
                }
                else
                {
                    Console.WriteLine($"File '{destinationFile}' does not exist in the destination directory.");
                    areAllFilesEqual = false;
                }
            }

            Assert.Multiple(() =>
            {
                Assert.That(areAllFilesExist);
                Assert.That(areAllFilesEqual);
            });
        }

        private bool CompareFiles(string file1, string file2)
        {
            byte[] file1Bytes = File.ReadAllBytes(file1);
            byte[] file2Bytes = File.ReadAllBytes(file2);

            if (file1Bytes.Length != file2Bytes.Length)
            {
                return false;
            }

            for (int i = 0; i < file1Bytes.Length; i++)
            {
                if (file1Bytes[i] != file2Bytes[i])
                {
                    return false;
                }
            }

            return true;
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void Test_SearchTextInFiles(string searchText, bool caseSensitive, Results expectedResults)
        {
            Results actualResults = new();

            Program.CopyFiles(files, destinationDirectory, actualResults);

            Program.SearchTextInFiles(files, searchText, caseSensitive, actualResults);

            Assert.That(actualResults.IsEqual(expectedResults));
        }

        private static readonly object[] TestCases =
        {
             new object[]
             {
                 "need to find",
                 false,
                 new Results()
                    {
                        TextFound = true,
                        CopiedFileCount = 5,
                        FailedCopyCount = 0,
                        FailedReadCount = 0,
                        TextFoundCount = 1
                    }
             },
             new object[]
             {
                 "need to find",
                 true,
                 new Results()
                    {
                        TextFound = false,
                        CopiedFileCount = 5,
                        FailedCopyCount = 0,
                        FailedReadCount = 0,
                        TextFoundCount = 0
                    }
             },
             new object[]
             {
                 "This",
                 false,
                 new Results()
                    {
                        TextFound = true,
                        CopiedFileCount = 5,
                        FailedCopyCount = 0,
                        FailedReadCount = 0,
                        TextFoundCount = 3
                    }
             },
             new object[]
             {
                 ".",
                 false,
                 new Results()
                    {
                        TextFound = true,
                        CopiedFileCount = 5,
                        FailedCopyCount = 0,
                        FailedReadCount = 0,
                        TextFoundCount = 4
                    }
             },
        };
    }
}