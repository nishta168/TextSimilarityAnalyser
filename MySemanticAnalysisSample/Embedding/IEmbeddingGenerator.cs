
namespace MySemanticAnalysisSample.Embedding
{
    internal interface IEmbeddingGenerator
    {
        Task<float[]> EmbedText(string text);
        Task<Dictionary<string, float[]>> EmbedDocumentsList(Dictionary<string, string> documents);
        Task<Dictionary<string, float[]>> EmbedWordsList(List<string> words);

    }
}
