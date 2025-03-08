using System;

namespace MySemanticAnalysisSample.SimilarityCalculation
{
    internal class DotProductSimilarityCalculator : ISimilarityCalculator
    {
        public float CalculateSimilarity(float[] embedding1, float[] embedding2)
        {
            if (embedding1 == null || embedding2 == null)
                throw new ArgumentNullException("Embeddings cannot be null.");

            if (embedding1.Length != embedding2.Length)
                throw new ArgumentException("Embeddings must have the same length.");

            double dotProduct = 0;

            for (int i = 0; i < embedding1.Length; i++)
            {
                dotProduct += embedding1[i] * embedding2[i];
            }

            return (float)dotProduct;
        }
    }
}
