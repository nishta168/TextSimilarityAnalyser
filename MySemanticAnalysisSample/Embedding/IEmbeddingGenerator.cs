
namespace MySemanticAnalysisSample.Embedding
{
    internal interface IEmbeddingGenerator
    {
        Task<float[]> EmbedTextAsync(string text);
        Task<Dictionary<string, List<float[]>>> EmbedDocumentsListAsync(Dictionary<string, List<string>> documents);
        Task<Dictionary<string, float[]>> EmbedWordsListAsync(List<string> words);

    }
}
