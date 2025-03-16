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
            _filesPath = filesPath;
        }
                
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
