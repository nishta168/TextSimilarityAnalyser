using Microsoft.Extensions.Configuration;
using OpenAI.Embeddings;

namespace MySemanticAnalysisSample.Embedding
{
    /// <summary>
    /// Class for generating embeddings using OpenAI's embedding model.
    /// </summary>
    internal class ChatGPTEmbeddingGenerator : IEmbeddingGenerator
    {
        private readonly EmbeddingClient _client;

        /// <summary>
        /// Constructor that initializes the EmbeddingClient with the OpenAI API key and embedding model name.
        /// </summary>
        /// <param name="config">Configuration object containing API key and model name</param>
        /// <exception cref="ArgumentException">Throws exception if valid API key or embedding name is not provided via config</exception>
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
        /// Generates embeddings for a list of documents
        /// </summary>
        /// <param name="documents">A dictionary where keys are document names and values are document content split into chunks.</param>
        /// <returns>A dictionary where keys are document names and values are list of embedding vectors corresponding to the list of chunks.</returns>
        /// <exception cref="ApplicationException">Throws exception when expected embedding is not received from API call</exception>
        public async Task<Dictionary<string, List<float[]>>> EmbedDocumentsListAsync(Dictionary<string, List<string>> documents)
        {
            var embeddingsDictionary = new Dictionary<string, List<float[]>>();

            foreach (var document in documents)
            {
                var chunks = document.Value;
                var chunkEmbeddings = new List<float[]>();

                // Generate embeddings for all chunks of current document.
                OpenAIEmbeddingCollection embeddingResults = await _client.GenerateEmbeddingsAsync(chunks);

                if (embeddingResults == null || embeddingResults.Count != chunks.Count)
                {
                    throw new ApplicationException($"Embedding API error while embedding document '{document.Key}'.");
                }

                // Convert chunk embeddings to float arrays and store them
                foreach (var embedding in embeddingResults)
                {
                    chunkEmbeddings.Add(embedding.ToFloats().ToArray());
                }

                embeddingsDictionary.Add(document.Key, chunkEmbeddings);
            }

            return embeddingsDictionary;
        }

        /// <summary>
        /// Generates embeddings for a list of words or phrases.
        /// </summary>
        /// <param name="words">List containing words or phrases or both to embed</param>
        /// <returns>A dictionary mapping words or phrases to their embedding vectors.</returns>
        /// <exception cref="ApplicationException">Throws exception when expected embeddings are not received from API call</exception>
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

        /// <summary>
        /// Generates an embedding for a single string of text.
        /// </summary>
        /// <param name="text">The text to embed</param>
        /// <returns>Embedding vector of the string </returns>
        /// <exception cref="ArgumentException">Throws exception when input is null or empty</exception>
        /// <exception cref="ApplicationException">Throws exception when expected embedding is not received from API call</exception>
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
        /// Validates the provided OpenAI API key by attempting a test embedding request.
        /// </summary>
        /// <param name="apiKey">OpenAI api key</param>
        /// <param name="embeddingModel">model name</param>
        /// <returns></returns>
        private static bool ValidateOpenAIKey(string apiKey, string embeddingModel)
        {
            try
            {
                // Create a temporary embedding request for validation.
                var testClient = new EmbeddingClient(embeddingModel, apiKey);
                var testResult = testClient.GenerateEmbeddingAsync("test").GetAwaiter().GetResult();
                return testResult != null; // Return true if the API key is valid
            }
            catch
            {
                return false; // Return false if exception occurs - the API key is invalid
            }
        }
    }
}
