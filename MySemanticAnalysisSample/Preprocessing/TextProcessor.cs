using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tiktoken;

namespace MySemanticAnalysisSample.Preprocessing
{
    /// <summary>
    /// Provides text processing utilities, including document cleaning, chunking by word or token count, and trimming of word/phrase lists.
    /// </summary>
    internal class TextProcessor : ITextProcessor
    {
        /// <summary>
        /// Processes a list of documents by cleaning and chunking them.
        /// </summary>
        /// <param name="documentList">A dictionary where key is document name and value is its content as a single string.</param>
        /// <param name="forWordVsDoc">Boolean. Set true for words/phrases vs document comparison, if true, chunks the document at a smaller word count (300-500 words per chunk)
        /// to find better matching when comparing against specific words or phrases; Set false for document vs document comparison, if false, chunks at maximum allowed token 
        /// limit for the model as we are only comparing overall context of both documents and specific words/phrases don't need emphasis.</param>
        /// <returns>A dictionary where key is document name and value is processed and chunked contents as a list of strings.</returns>
        public Dictionary<string, List<string>> ProcessDocumentList(Dictionary<string, string> documentList, bool forWordVsDoc)
        {
            //maximum word count per chunk for documents compared against words
            const int MaximumWordCount = 300;
            //maximum token count per chunk for documnents compared against documents
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

        /// <summary>
        /// Trims a list of words or phrases.
        /// </summary>
        /// <param name="wordOrPhraseList">List of words or phrases to process.</param>
        /// <returns>A list of trimmed words or phrases.</returns>
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

        /// <summary>
        /// Cleans a document by removing URLs, email addresses, and extra whitespace.
        /// </summary>
        /// <param name="text">The text content to clean.</param>
        /// <returns>The cleaned document text.</returns>
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

        /// <summary>
        /// Splits a document into chunks based on a specified maximum word count for word vs doc comparison.
        /// </summary>
        /// <param name="document">The document content to be chunked.</param>
        /// <param name="maxWordCount">Maximum number of words per chunk. Chosen from comparison result analysis</param>
        /// <returns>A list of document chunks.</returns>
        public List<string> ChunkDocumentByWordCount(string document, int maxWordCount)
        {
            var chunkedDocuments = new List<string>();
            var wordsinDocument = document.Split(' '); // Split the document into words
            var chunk = new List<string>(); // Use a list to build each chunk

            // Iterate through the words and chunk them
            foreach (var word in wordsinDocument)
            {
                chunk.Add(word);
                if (chunk.Count >= maxWordCount)
                {
                    chunkedDocuments.Add(string.Join(" ", chunk)); // Join words and add to the chunk list
                    chunk.Clear();
                }
            }

            // Add any remaining words as the last chunk
            if (chunk.Count > 0)
            {
                chunkedDocuments.Add(string.Join(" ", chunk));
            }

            return chunkedDocuments;
        }

        /// <summary>
        /// Splits a document into chunks based on a specified maximum token count for doc vs doc comparison.
        /// </summary>
        /// <param name="document">The document content to be chunked.</param>
        /// <param name="maxTokenCount">Maximum number of tokens for the embedding model used.</param>
        /// <returns>A list of document chunks.</returns>
        public List<string> ChunkDocumentByTokenCount(string document, int maxTokenCount)
        {
            var chunkedDocuments = new List<string>();
            var encoder = ModelToEncoder.For("text-embedding-3-large");
            var tokens = encoder.Encode(document);
            var num_tokens = tokens.Count;

            if (num_tokens <= maxTokenCount)
            {
                chunkedDocuments.Add(document);
                return chunkedDocuments;
            }

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
