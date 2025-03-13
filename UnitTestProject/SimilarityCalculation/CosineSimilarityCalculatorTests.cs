using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySemanticAnalysisSample.SimilarityCalculation;

namespace UnitTestProject.SimilarityCalculation
{
    [TestClass]
    public class CosineSimilarityCalculatorTests
    {
        private readonly CosineSimilarityCalculator _calculator = new CosineSimilarityCalculator();

        [TestMethod]
        public void CalculateSimilarity_ValidVectors_ReturnsCorrectSimilarity()
        {
            float[] embedding1 = { 1, 2, 3 };
            float[] embedding2 = { 1, 2, 3 };

            float result = _calculator.CalculateSimilarity(embedding1, embedding2);

            Assert.AreEqual(1.0f, result, 0.0001, "Similarity should be 1 for identical vectors.");
        }

        [TestMethod]
        public void CalculateSimilarity_OrthogonalVectors_ReturnsZero()
        {
            float[] embedding1 = { 1, 0 };
            float[] embedding2 = { 0, 1 };

            float result = _calculator.CalculateSimilarity(embedding1, embedding2);

            Assert.AreEqual(0.0f, result, 0.0001, "Similarity should be 0 for orthogonal vectors.");
        }

        [TestMethod]
        public void CalculateSimilarity_DifferentVectors_ReturnsCorrectValue()
        {
            float[] embedding1 = { 1, 2, 3 };
            float[] embedding2 = { 4, 5, 6 };

            float result = _calculator.CalculateSimilarity(embedding1, embedding2);

            Assert.IsTrue(result > 0 && result < 1, "Similarity should be between 0 and 1 for different vectors.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CalculateSimilarity_NullVectors_ThrowsArgumentNullException()
        {
            _calculator.CalculateSimilarity(null, new float[] { 1, 2, 3 });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CalculateSimilarity_DifferentLengths_ThrowsArgumentException()
        {
            float[] embedding1 = { 1, 2, 3 };
            float[] embedding2 = { 1, 2 };

            _calculator.CalculateSimilarity(embedding1, embedding2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CalculateSimilarity_ZeroMagnitudeVector_ThrowsInvalidOperationException()
        {
            float[] embedding1 = { 0, 0, 0 };
            float[] embedding2 = { 1, 2, 3 };

            _calculator.CalculateSimilarity(embedding1, embedding2);
        }
    }
}
