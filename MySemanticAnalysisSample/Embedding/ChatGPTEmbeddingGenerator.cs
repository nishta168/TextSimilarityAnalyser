using Microsoft.Extensions.Configuration;
using OpenAI.Embeddings;

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
        public ChatGPTEmbeddingGenerator(IConfiguration config)
        {
            string apiKey = config["OpenAI:apiKey"] ?? throw new ArgumentException("Missing OpenAI API Key in configuration.");
            string embeddingModel = config["OpenAI:EmbeddingModel"] ?? throw new ArgumentException("Missing OpenAI embedding model in configuration.");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("OpenAI API Key cannot be an empty string. Please provide valid API Key in configuration file");
            }
            if (string.IsNullOrEmpty(embeddingModel))
            {
                throw new ArgumentException("Please provide embedding model in configuration");
            }
            if (!ValidateOpenAIKey(apiKey, embeddingModel))
            {
                throw new ArgumentException("Invalid OpenAI API Key");
            }

            _client = new EmbeddingClient(embeddingModel, apiKey);

        }

        /// <summary>
        /// Generates embeddings for a batch of documents.
        /// </summary>
        /// <param name="documents">Dictionary where keys are document names and values are document content.</param>
        /// <returns>A dictionary mapping document names to their embedding vectors.</returns>
        public async Task<Dictionary<string, List<float[]>>> EmbedDocumentsListAsync(Dictionary<string, List<string>> documents)
        {
            var embeddingsDictionary = new Dictionary<string, List<float[]>>();

            foreach (var document in documents)
            {
                var chunks = document.Value;
                var chunkEmbeddings = new List<float[]>();

                OpenAIEmbeddingCollection embeddingResults = await _client.GenerateEmbeddingsAsync(chunks);

                if (embeddingResults == null || embeddingResults.Count != chunks.Count)
                {
                    throw new ApplicationException($"Embedding API error while embedding document '{document.Key}'.");
                }

                foreach (var embedding in embeddingResults)
                {
                    chunkEmbeddings.Add(embedding.ToFloats().ToArray());
                }

                embeddingsDictionary.Add(document.Key, chunkEmbeddings);
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
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Input text cannot be null or empty.", nameof(text));
            }

            OpenAIEmbedding embedding = await _client.GenerateEmbeddingAsync(text);

            if (embedding == null)
            {
                throw new ApplicationException($"Embedding generation failed: OpenAI API returned a null response for input '{text}'.");
            }

            return embedding.ToFloats().ToArray();

        }

        /// <summary>
        /// Generates embeddings for a list of words.
        /// </summary>
        /// <param name="words">List of words to embed.</param>
        /// <returns>A dictionary mapping words to their embedding vectors.</returns>
        public async Task<Dictionary<string, float[]>> EmbedWordsListAsync(List<string> words)
        {
            var embeddingsDictionary = new Dictionary<string, float[]>();

            OpenAIEmbeddingCollection embeddingResults = await _client.GenerateEmbeddingsAsync(words.ToArray());

            if (embeddingResults == null || embeddingResults.Count != words.Count)
            {
                throw new ApplicationException($"Embedding API error while embedding list of words.");
            }

            for (int i = 0; i < words.Count; i++)
            {
                embeddingsDictionary[words[i]] = embeddingResults[i].ToFloats().ToArray();
            }

            return embeddingsDictionary;
        }

        private static bool ValidateOpenAIKey(string apiKey, string embeddingModel)
        {
            try
            {
                var testClient = new EmbeddingClient(embeddingModel, apiKey);
                var testResult = testClient.GenerateEmbeddingAsync("test").GetAwaiter().GetResult();
                return testResult != null; // Return true if the API key is valid
            }
            catch
            {
                return false; // Return false if the API key is invalid
            }
        }
    }
}
