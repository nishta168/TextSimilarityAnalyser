using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.FileHandling
{
    internal static class FilePathValidator
    {
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
    }
}
