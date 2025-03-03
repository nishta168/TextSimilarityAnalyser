using OpenAI.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.Embedding
{
    /// <summary>
    /// Implementation of IEmbeddingGenerator using OpenAI's Embedding API.
    /// </summary>
    internal class ChatGPTEmbeddingGenerator: IEmbeddingGenerator
    {
        private readonly EmbeddingClient _client;

        /// <summary>
        /// Constructor that initializes the EmbeddingClient with the OpenAI API key.
        /// </summary>
        public ChatGPTEmbeddingGenerator()
        {
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"); // Move to appsettings.json later

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("The OpenAI API key is not set in the environment variables.");
            }

            _client = new EmbeddingClient("text-embedding-3-large", apiKey);
        }

        /// <summary>
        /// Generates embeddings for a batch of documents.
        /// </summary>
        /// <param name="documents">Dictionary where keys are document names and values are document content.</param>
        /// <returns>A dictionary mapping document names to their embedding vectors.</returns>
        public async Task<Dictionary<string, List<float[]>>> EmbedDocumentsListAsync(Dictionary<string, List<string>> documents)
        {
            var embeddingsDictionary = new Dictionary<string, List<float[]>>();

            try
            {
                foreach ( var document in documents)
                {
                    var chunks = document.Value.ToList();
                    var chunkEmbeddings = new List<float[]>();
                    OpenAIEmbeddingCollection embeddingResults = await _client.GenerateEmbeddingsAsync(chunks);
                    if(embeddingResults != null)
                    {
                        for( int i = 0; i < embeddingResults.Count; i++)
                        {
                            chunkEmbeddings.Add(embeddingResults[i].ToFloats().ToArray());
                        }
                        embeddingsDictionary.Add(document.Key, chunkEmbeddings);
                    }
                }                              
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating embeddings: {ex.Message}");
            }

            return embeddingsDictionary;
        }

        /// <summary>
        /// Generates an embedding for a single text input.
        /// </summary>
        /// <param name = "text" > The text to embed.</param>
        /// <returns>Embedding vector as a float array.</returns>
        public async Task<float[]> EmbedTextAsync(string text)
        {
            try
            {
                OpenAIEmbedding embedding = await _client.GenerateEmbeddingAsync(text);
                return embedding.ToFloats().ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating embedding for text: {ex.Message}");
                return Array.Empty<float>(); // Return an empty array in case of error
            }
        }

        /// <summary>
        /// Generates embeddings for a list of words.
        /// </summary>
        /// <param name="words">List of words to embed.</param>
        /// <returns>A dictionary mapping words to their embedding vectors.</returns>
        public async Task<Dictionary<string, float[]>> EmbedWordsListAsync(List<string> words)
        {
            var embeddingsDictionary = new Dictionary<string, float[]>();

            try
            {
                var wordsArray = words.ToArray();
                OpenAIEmbeddingCollection embeddingResults = await _client.GenerateEmbeddingsAsync(wordsArray);


                if (embeddingResults != null && embeddingResults.Count == words.Count)
                {
                    for (int i = 0; i < words.Count; i++)
                    {
                        embeddingsDictionary[words[i]] = embeddingResults[i].ToFloats().ToArray();
                    }
                }
                else
                {
                    Console.WriteLine("Error: Embedding response does not match input size.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating embeddings: {ex.Message}");
            }

            return embeddingsDictionary;
        }
    }
}
