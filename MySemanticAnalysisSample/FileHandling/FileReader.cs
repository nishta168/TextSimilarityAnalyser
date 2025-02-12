using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace MySemanticAnalysisSample.FileHandling
{
    internal class FileReader
    {
        private readonly string _filesPath;

        public FileReader(string filesPath)
        {
            if (string.IsNullOrWhiteSpace(filesPath))
            {
                throw new ArgumentException("The file path cannot be null or empty.", nameof(filesPath));
            }

            if (!Directory.Exists(filesPath) && !File.Exists(filesPath))  // Check if it's a valid directory or file
            {
                throw new DirectoryNotFoundException($"The specified path does not exist: {filesPath}");
            }

            _filesPath = filesPath;
        }

        public Dictionary<string, string> ReadDocuments()
        {
            var documents = new Dictionary<string, string>();

            try
            {
                var documentFiles = Directory.GetFiles(_filesPath, "*.txt");

                foreach (var file in documentFiles)
                {
                    try
                    {
                        string documentLabel = Path.GetFileNameWithoutExtension(file);
                        string documentContent = File.ReadAllText(file);
                        if (string.IsNullOrWhiteSpace(documentContent))
                        {
                            throw new InvalidOperationException("The file is empty or contains only whitespace.");
                        }
                        documents.Add(documentLabel, documentContent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading file {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing directory {_filesPath}: {ex.Message}");
            }

            return documents;
        }

        public List<string> ReadWordsOrPhrases()
        {
            var words = new List<string>();

            try
            {
                if (File.Exists(_filesPath))  // Ensure it's a valid file
                {
                    words = File.ReadLines(_filesPath).ToList();
                }
                else
                {
                    Console.WriteLine($"File not found: {_filesPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file {_filesPath}: {ex.Message}");
            }

            return words;
        }
    }
}
