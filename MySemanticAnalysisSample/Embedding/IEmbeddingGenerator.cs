
namespace MySemanticAnalysisSample.Embedding
{
    /// <summary>
    /// Describes methods provided by embedding classes to embed documents and words lists
    /// </summary>
    internal interface IEmbeddingGenerator
    {
        /// <summary>
        /// Generates embeddings for a list of documents
        /// </summary>
        /// <param name="documents">A dictionary where keys are document names and values are document content split into chunks.</param>
        /// <returns>A dictionary where keys are document names and values are list of embedding vectors corresponding to the list of chunks.</returns>
        Task<Dictionary<string, List<float[]>>> EmbedDocumentsListAsync(Dictionary<string, List<string>> documents);

        /// <summary>
        /// Generates embeddings for a list of words or phrases.
        /// </summary>
        /// <param name="words">List containing words or phrases or both to embed</param>
        /// <returns>A dictionary mapping words or phrases to their embedding vectors.</returns>
        Task<Dictionary<string, float[]>> EmbedWordsListAsync(List<string> words);

        /// <summary>
        /// Generates an embedding for a single string of text.
        /// </summary>
        /// <param name="text">The text to embed</param>
        /// <returns>Embedding vector of the string </returns>
        Task<float[]> EmbedTextAsync(string text);

    }
}
