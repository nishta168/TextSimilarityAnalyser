using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tiktoken;

namespace MySemanticAnalysisSample.Preprocessing
{
    internal class TextProcessor : ITextProcessor
    {
        public Dictionary<string, List<string>> ProcessDocumentList(Dictionary<string, string> documentList, bool forWordVsDoc)
        {
            const int MaximumWordCount = 300;
            const int MaxTokenCount = 8191;  
            var processedDocumentList = new Dictionary<string, List<string>>();
            foreach (var document in documentList)
            {   
                var cleanedDocument = CleanDocument(document.Value);
                var documentChunks = new List<string>();
                if (forWordVsDoc)
                {
                    documentChunks = ChunkDocumentByWordCount(cleanedDocument, MaximumWordCount);                    
                }
                else
                {
                    documentChunks = ChunkDocumentByTokenCount(cleanedDocument, MaxTokenCount);                   
                }

                processedDocumentList.Add(document.Key, documentChunks);
                
            }
            return processedDocumentList;
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
            //text = Regex.Replace(text, @"[^a-zA-Z0-9\s.,!?]", "", RegexOptions.IgnoreCase);

            //Remove extra spaces
            text = Regex.Replace(text, @"\s+", " ").Trim();

            return text;
        }

        public List<string> ChunkDocumentByWordCount(string document, int maxWordCount)
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

        public List<string> ChunkDocumentByTokenCount(string document, int maxTokenCount)
        {
            var chunkedDocuments = new List<string>();
            var encoder = ModelToEncoder.For("text-embedding-3-large");
            var tokens = encoder.Encode(document);
            var num_tokens = tokens.Count;
            var chunk = new List<int>(); // Use a list to build each chunk

            // Iterate through the tokens and chunk them
            foreach (var token in tokens)
            {
                chunk.Add(token); // Add the current token to the chunk
                if (chunk.Count >= maxTokenCount)
                {
                    chunkedDocuments.Add(encoder.Decode(chunk)); // decode chunk and add to the chunk list
                    chunk.Clear(); // Clear the current chunk to start the next one
                }
            }

            // Add any remaining token as the last chunk
            if (chunk.Count > 0)
            {
                chunkedDocuments.Add(encoder.Decode(chunk));
            }

            return chunkedDocuments;
        }
    }
}
