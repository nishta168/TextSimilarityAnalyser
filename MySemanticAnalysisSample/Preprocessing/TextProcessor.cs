using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.Preprocessing
{
    internal class TextProcessor : ITextProcessor
    {
        public Dictionary<string, string> ProcessDocumentList(Dictionary<string, string> documentList)
        {
            //var maximumWordCount = 500;
            var ProcessedDocumetList = new Dictionary<string, string>();
            foreach (var document in documentList)
            {
                var cleanedDocument = CleanDocument(document.Value);
                //var documentChunks = ChunkDocument(document, maximumWordCount);
                ProcessedDocumetList.Add(document.Key, cleanedDocument);
            }
            return ProcessedDocumetList;
        }

        public List<string> ProcessWordOrPhraseList(List<string> wordOrPhraseList)
        {   
            var ProcessedArray = new List<string>();

            foreach (var text in wordOrPhraseList)
            {
                var processedText = text.Trim();
                ProcessedArray.Add(processedText);
            }
            return ProcessedArray;
            
        }

        public string CleanDocument(string text)
        {
            
            // Remove URLs
            text = Regex.Replace(text, @"http\S+|www\S+|https\S+", "", RegexOptions.IgnoreCase);

            // Remove email addresses
            text = Regex.Replace(text, @"\S+@\S+\.\S+", "", RegexOptions.IgnoreCase);

            // Remove special characters (keeping basic punctuation)
            text = Regex.Replace(text, @"[^a-zA-Z0-9\s.,!?]", "", RegexOptions.IgnoreCase);

            // Remove extra spaces
            text = Regex.Replace(text, @"\s+", " ").Trim();

            return text;
        }

        public List<string> ChunkDocument(string document, int maxWordCount)
        {
            var chunkedDocuments = new List<string>();
            var wordsinDocument = document.Split(' '); // Split the document into words
            var chunk = new List<string>(); // Use a list to build each chunk

            // Iterate through the words and chunk them
            foreach (var word in wordsinDocument)
            {
                chunk.Add(word); // Add the current word to the chunk
                if (chunk.Count >= maxWordCount)
                {
                    chunkedDocuments.Add(string.Join(" ", chunk)); // Join words and add to the chunk list
                    chunk.Clear(); // Clear the current chunk to start the next one
                }
            }

            // Add any remaining words as the last chunk
            if (chunk.Count > 0)
            {
                chunkedDocuments.Add(string.Join(" ", chunk));
            }

            return chunkedDocuments;
        }
    }
}
