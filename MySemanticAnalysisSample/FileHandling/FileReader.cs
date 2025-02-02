

using System.Collections.Generic;

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

            var documentFiles = Directory.EnumerateFiles(_filesPath, "*.txt");

            foreach (var file in documentFiles)
            {
                try
                {
                    string documentLabel = Path.GetFileNameWithoutExtension(file);
                    string documentContent = File.ReadAllText(file);
                    documents.Add(documentLabel, documentContent);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading file {file}: {ex.Message}");
                }
            }

            return documents;
        }
    }
}
