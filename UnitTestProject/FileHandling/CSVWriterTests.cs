using MySemanticAnalysisSample.FileHandling;

namespace UnitTestProject.FileHandling;

[TestClass]
public class CSVWriterTests
{
    private readonly string _testFolder = Directory.GetCurrentDirectory();
    private readonly string _similarityFilePath;
    private readonly string _embeddingsFilePath;

    public CSVWriterTests()
    {
        _similarityFilePath = Path.Combine(_testFolder, "test_similarity.csv");
        _embeddingsFilePath = Path.Combine(_testFolder, "test_embeddings.csv");
    }

    [TestMethod]
    public void WriteSimilarityToCSV_WritesCorrectData()
    {
        // Arrange
        var similarityData = new List<string[]>
            {
                new string[] { "Q\\R", "Ref1", "Ref2" },
                new string[] { "Query1", "0.5", "0.92" }
            };

        // Act
        CSVWriter.WriteSimilarityToCSV(_similarityFilePath, similarityData);

        // Assert
        var lines = File.ReadAllLines(_similarityFilePath);
        Assert.AreEqual(2, lines.Length);
        Assert.AreEqual("Q\\R,Ref1,Ref2", lines[0]);
        Assert.AreEqual("Query1,0.5,0.92", lines[1]);
    }

    [TestMethod]
    public void WriteEmbeddingsToCSV_WritesCorrectEmbeddings()
    {
        // Arrange
        var embeddings = new Dictionary<string, float[]>
            {
                { "Word1", new float[] { 0.1f, 0.2f, 0.3f } },
                { "Word2", new float[] { 0.4f, 0.5f, 0.6f } }
            };

        // Act
        CSVWriter.WriteEmbeddingsToCSV(_embeddingsFilePath, embeddings);

        // Assert
        var lines = File.ReadAllLines(_embeddingsFilePath);
        Assert.AreEqual(3, lines.Length);
        Assert.AreEqual("Categories,0,1,2", lines[0]);
        Assert.AreEqual("Word1,0.1,0.2,0.3", lines[1]);
        Assert.AreEqual("Word2,0.4,0.5,0.6", lines[2]);
    }
}