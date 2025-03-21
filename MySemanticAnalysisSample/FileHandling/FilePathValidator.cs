using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.FileHandling
{
    /// <summary>
    /// Provides methods to validate user given file paths and directory paths.
    /// </summary>
    internal static class FilePathValidator
    {
        /// <summary>
        /// Validates whether a valid .txt file is given by user when comparing words/phrases
        /// </summary>
        /// <param name="filePath">Input file path extracted from config</param>
        /// <exception cref="ArgumentException">Thrown if the input file path is not provided via config or not a .txt file. </exception>
        /// <exception cref="FileNotFoundException">Thrown if the provided path doesn't exist.</exception>
        public static void ValidateTxtFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("Missing required file paths. Please provide file path to the .txt file with words via command-line arguments or appsettings.json." +
                    "Command-line arguments usage example: MySemanticAnalysisSample.exe --mode CompareWordsWithWords --queryWordsPath \\\"path/to/querywords.txt\\\" --referenceWordsPath \\\"path/to/referencewords.txt\\\"\"");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Words file not found: {filePath}");
            }

            if (Path.GetExtension(filePath).ToLower() != ".txt")
            {
                throw new ArgumentException($"Invalid words file format. Expected .txt, found: {filePath}");
            }

        }

        /// <summary>
        /// Validates that the user has input a folder containing .txt files when comparing documents.
        /// </summary>
        /// <param name="folderPath">Folder path containing documents extracted from config. </param>
        /// <exception cref="ArgumentException">Thrown if the folder path in not provided via config.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown if the path provided doesn't exist.</exception>
        /// <exception cref="FileNotFoundException">Thrown if there are no .txt documents inside the folder.</exception>
        public static void ValidateDocumentsFolderPath(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentException("Missing required file paths. Please provide file path to the folder with documents via command-line arguments or appsettings.json." +
                    "Command-line arguments usage example: MySemanticAnalysisSample.exe --mode CompareDocumentsWithWords --queryDocumentsPath \\\"path/to/documentsFolder\\\" --referenceWordsPath \\\"path/to/referencewords.txt\\\"\"");
            }

            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Error: The specified path '{folderPath}' does not exist or is not a valid directory.");
            }

            var documentFiles = Directory.GetFiles(folderPath, "*.txt");

            if (documentFiles.Length < 1)
            {
                throw new FileNotFoundException($"No .txt document files found in the directory: {folderPath}");
            }

        }

        /// <summary>
        /// Returns the user provided output paths if it is valid. Otherwise returns a default output file path.
        /// </summary>
        /// <param name="filePath">File path extracted from config.</param>
        /// <param name="fileName">The default file name if no valid path is provided.</param>
        /// <returns>A valid file path where output can be written.</returns>
        public static string ValidateOutputFilePath(string filePath, string fileName)
        {
            if (Path.Exists(filePath))
            {
                // Return the valid existing file path
                return filePath;
            }
            else
            {
                // Default: Same folder as the app
                string outputFolder = AppContext.BaseDirectory;
                string outputFilePath = Path.Combine(outputFolder, fileName);
                Console.WriteLine("Valid output file path not provided for " + fileName + ". Writing to default location.");
                return outputFilePath;
            }
        }
    }
}
