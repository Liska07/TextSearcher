# TextSearcher

This repository contains the source code for TextSearcher, a console application written in C# (version 8.0) that performs the following tasks:

1. Copies multiple text files from a source folder to a destination folder.
2. Searches for specified text within these copied files.
3. Displays the results in the console, indicating which files contain the specified text.

## Project Structure
- **.github/workflows:** Contains the GitHub Actions workflow file with configurations for running unit tests.
- **TextSearcher.Tests:** This related repository includes unit tests to verify the core functionality of the application.
- **TextSearcher:** 
	 - **App.config:** Contains configuration settings for the project, such as source and destination directories, search text, and case sensitivity. Written in XML format.
	 - **NLog.config:** Configures logging for the project. Written in XML format, it specifies how logs are handled, including writing logs to both files and the console.
	 - **Program.cs:** Contains the main entry point for the application and the core logic for running the console application.
	 - **Results.cs:** Tracks and displays the summary of the application’s operations, including file copying, search results, and errors.

## Running Unit Tests
Unit tests for this project are configured to run automatically using GitHub Actions. The tests are set up to run under the following conditions:
- **On Push**: Tests are triggered whenever changes are pushed to the repository.
- **On Pull Request**: Tests are executed when a pull request is created or updated.
- **Manually**: Tests can also be triggered manually through the GitHub Actions interface.

For more details on the test runs and results, you can check [GitHub Actions](https://github.com/Liska07/TextSearcher/actions).

## Releases
All versions of TextSearcher are available on [GitHub Releases](https://github.com/Liska07/TextSearcher/releases).

## Documentation
For the latest and most comprehensive instructions on installation, configuration, usage, and output, please refer to [TextSearcher Wiki](https://github.com/Liska07/TextSearcher/wiki).