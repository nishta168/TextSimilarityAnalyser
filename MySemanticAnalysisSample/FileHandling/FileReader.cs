using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace MySemanticAnalysisSample.FileHandling
{
    /// <summary>
    /// Handles reading list of words/phrases from the input .txt file path and documents as .txt files from the input folder path.
    /// </summary>
    internal class FileReader
    {
        private readonly string _filesPath;

        /// <summary>
        /// Initializes a new instance of class with the file or folder path
        /// </summary>
        /// <param name="filesPath">Path to the directory or file to read.</param>
        public FileReader(string filesPath)
        {
            _filesPath = filesPath;
        }

        /// <summary>
        /// Reads all .txt documents in the specified directory and returns their content.
        /// </summary>
        /// <returns>A dictionary mapping document names to their content.</returns>
        /// <exception cref="InvalidOperationException">Thrown if any .txt document is empty or contains only whitespace.</exception>
        public Dictionary<string, string> ReadDocuments()
        {
            var documents = new Dictionary<string, string>();

            var documentFiles = Directory.GetFiles(_filesPath, "*.txt");

            foreach (var file in documentFiles)
            {
                string documentLabel = Path.GetFileNameWithoutExtension(file);
                string documentContent = File.ReadAllText(file);

                if (string.IsNullOrWhiteSpace(documentContent))
                    throw new InvalidOperationException($"The document '{file}' is empty or contains only whitespace.");

                documents.Add(documentLabel, documentContent);
            }

            return documents;
        }

        /// <summary>
        /// Reads all words or phrases from a .txt file
        /// <returns>A list of words or phrases read from the file.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the file contains no words or phrases.</exception>
        public List<string> ReadWordsOrPhrases()
        {
            var words = new List<string>();

            words = File.ReadLines(_filesPath)
                    .Select(line => line.Trim()) // Remove leading/trailing whitespace
                    .Where(line => !string.IsNullOrWhiteSpace(line)) // Filter out empty lines
                    .ToList();

            if (words.Count < 1)
            {
                throw new InvalidOperationException($"The input .txt file '{_filesPath}' is empty or contains only whitespace. It must contain words or phrases on separate lines.");
            }

            return words;
        }
    }
}
