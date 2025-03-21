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
        /// Returns the user provided input path if its valid, else returns the default suggested location to find inputs (inside app base directory)
        /// </summary>
        /// <param name="filePath">Input file path extracted from config</param>
        /// <param name="isDocFolder">Set to true if its a path points to documents folder</param>
        /// <param name="isQuery">Set to true if the path points to query text</param>
        /// <exception cref="FileNotFoundException">Thrown if the words/phrase .txt file cannot be found.</exception       
        /// <exception cref="DirectoryNotFoundException">Thrown if the document folder with .txt file documents cannot be found.</exception>
        /// <returns>Location to read the input files from</returns>
        public static string ValidateInputPath(string path, bool isDocFolder, bool isQuery)
        {
            if (isDocFolder)
            {
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path) || (Directory.GetFiles(path, "*txt")).Length < 1)
                {
                    string defaultFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InputText", isQuery ? "QueryDocuments" : "ReferenceDocuments");

                    if (!Directory.Exists(defaultFolderPath))
                    {
                        throw new DirectoryNotFoundException($"{(isQuery ? "Query" : "Reference")} Documents path not found: {defaultFolderPath}");
                    }

                    var documentFiles = Directory.GetFiles(defaultFolderPath, "*.txt");

                    if (documentFiles.Length < 1)
                    {
                        throw new FileNotFoundException($"No {(isQuery ? "query" : "reference")} .txt document files found in the directory: {defaultFolderPath}");
                    }

                    Console.WriteLine($"Reading {(isQuery ? "query" : "reference")} documents from {defaultFolderPath}\n");
                    return defaultFolderPath;
                }

                Console.WriteLine($"Reading {(isQuery ? "query" : "reference")} documents from {path}\n");
                return path; // Return the valid directory path
            }
            else
            {
                if (!string.IsNullOrEmpty(path) || !File.Exists(path) || Path.GetExtension(path).ToLower() != ".txt")
                {
                    string defaultFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InputText", isQuery ? "QueryWordsOrPhrases.txt" : "ReferenceWordsOrPhrases.txt");
                    if (!File.Exists(defaultFilePath))
                    {
                        throw new FileNotFoundException($"{(isQuery ? "Query" : "Reference")} words/phrases .txt file not found at {defaultFilePath}");
                    }
                    Console.WriteLine($"Reading {(isQuery ? "query" : "reference")} words/phrases from {defaultFilePath}\n");
                    return defaultFilePath;
                }

                Console.WriteLine($"Reading {(isQuery ? "query" : "reference")} words/phrases from {path}\n");
                return path;

            }

        }

        /// <summary>
        /// Returns the user provided output paths if it is valid. Otherwise returns a default output file path (inside app base directory).
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
                return outputFilePath;
            }
        }
    }
}
