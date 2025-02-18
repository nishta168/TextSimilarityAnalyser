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

            _client = new EmbeddingClient("text-embedding-3-large", apiKey);
        }

        public Task<Dictionary<string, float[]>> EmbedDocumentsList(Dictionary<string, string> documents)
        {
            throw new NotImplementedException();
        }

        
        public async Task<float[]> EmbedText(string text)
        {
            //calculate the no. of tokens and if it exceeds the token limit do chunking
            OpenAIEmbedding embedding = await _client.GenerateEmbeddingAsync(text);
 
            // Convert embedding to float array
            return embedding.ToFloats().ToArray();
        }

        public Task<Dictionary<string, float[]>> EmbedWordsList(List<string> words)
        {
            throw new NotImplementedException();
        }
    }
}
