using MySemanticAnalysisSample.Utils;

namespace UnitTestProject.Utils;

[TestClass]
public class HelperTests
{
    [TestMethod]
    public void CalculateExponentiallyWeightedMean_ShouldReturnZero_ForEmptyList()
    {
        // Arrange
        var similarities = new List<float>();
        float beta = 50.0f;

        // Act
        float result = Helper.CalculateExponentiallyWeightedMean(similarities, beta);

        // Assert
        Assert.AreEqual(0, result, "Expected result to be 0 for an empty list.");
    }

    [TestMethod]
    public void CalculateExponentiallyWeightedMean_ShouldReturnSameValue_ForSingleElementList()
    {
        // Arrange
        var similarities = new List<float> { 0.8f };
        float beta = 50.0f;

        // Act
        float result = Helper.CalculateExponentiallyWeightedMean(similarities, beta);

        // Assert
        Assert.AreEqual(0.8f, result, 0.0001, "Expected the same value for a single-element list.");
    }

    [TestMethod]
    public void CalculateExponentiallyWeightedMean_ShouldGiveHigherWeight_ToLargerSimilarities()
    {
        // Arrange
        var similarities = new List<float> { 0.2f, 0.9f };
        float beta = 50.0f;

        // Act
        float result = Helper.CalculateExponentiallyWeightedMean(similarities, beta);

        // Assert
        Assert.IsTrue(result > 0.55f, "Expected higher emphasis on the larger similarity value.");
    }

    [TestMethod]
    public void CalculateExponentiallyWeightedMean_ShouldHandleNegativeValues()
    {
        // Arrange
        var similarities = new List<float> { -0.5f, 0.5f, 0.9f };
        float beta = 50.0f;

        // Act
        float result = Helper.CalculateExponentiallyWeightedMean(similarities, beta);

        // Assert
        Assert.IsTrue(result > 0, "Expected a positive weighted mean despite a negative value.");
    }

    [TestMethod]
    public void CalculateExponentiallyWeightedMean_ShouldHandleZeroBeta()
    {
        // Arrange
        var similarities = new List<float> { 0.1f, 0.5f, 0.9f };
        float beta = 0.0f; // No weighting effect

        // Act
        float result = Helper.CalculateExponentiallyWeightedMean(similarities, beta);

        // Assert
        float expectedMean = (0.1f + 0.5f + 0.9f) / 3;
        Assert.AreEqual(expectedMean, result, 0.0001, "Expected arithmetic mean when beta is zero.");
    }

    [TestMethod]
    public void CalculateExponentiallyWeightedMean_ShouldHandleLargeSimilarityScore()
    {
        // Arrange
        var similarities = new List<float> { 0.1f, 0.3f, 0.9f };
        float beta = 50.0f;//Extreme weighting

        // Act
        float result = Helper.CalculateExponentiallyWeightedMean(similarities, beta);

        // Assert
        Assert.AreEqual(0.9f, result, 0.0001, "Expected result to be dominated by the largest similarity value.");
    }
}
