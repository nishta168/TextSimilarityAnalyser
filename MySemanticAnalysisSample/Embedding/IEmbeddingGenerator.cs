
namespace MySemanticAnalysisSample.Embedding
{
    internal interface IEmbeddingGenerator
    {
        Task<float[]> EmbedTextAsync(string text);
        Task<Dictionary<string, float[]>> EmbedDocumentsListAsync(Dictionary<string, string> documents);
        Task<Dictionary<string, float[]>> EmbedWordsListAsync(List<string> words);

    }
}
