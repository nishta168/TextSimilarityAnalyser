using OpenAI.Embeddings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.Embedding
{
    /// <summary>
    /// Implementation of IEmbeddingGenerator using OpenAI's Embedding API.
    /// </summary>
    internal class ChatGPTEmbeddingGenerator : IEmbeddingGenerator
    {
        private readonly EmbeddingClient _client;

        /// <summary>
        /// Constructor that initializes the EmbeddingClient with the OpenAI API key.
        /// </summary>
        public ChatGPTEmbeddingGenerator()
        {
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"); //move to appsettings.json later

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("The OpenAI API key is not set in the environment variables.");
            }

            _client = new EmbeddingClient("text-embedding-ada-002", apiKey);
        }

        /// <summary>
        /// Generates an embedding for the provided text using the OpenAI API.
        /// </summary>
        /// <param name="text">The input text to generate the embedding for.</param>
        /// <returns>A float array representing the embedding of the input text.</returns>
        public async Task<float[]> CreateEmbedding(string text) //take chunking flag input also
        {
            
            // Call the API asynchronously
            OpenAIEmbedding embedding = await _client.GenerateEmbeddingAsync(text);
 
            // Convert embedding to float array
            return embedding.ToFloats().ToArray();
        }
    }
}
