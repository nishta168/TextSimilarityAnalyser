using System;

namespace MySemanticAnalysisSample.SimilarityCalculation
{
    /// <summary>
    /// Computes similarity between two embedding vectors using the dot product.
    /// The dot product measures the sum of the element-wise multiplications of two vectors.
    /// Higher dot product values indicate higher similarity.
    /// </summary>
    internal class DotProductSimilarityCalculator : ISimilarityCalculator
    {
        /// <summary>
        /// Calculates the dot product similarity between two embedding vectors.
        /// </summary>
        /// <param name="embedding1">First embedding vector.</param>
        /// <param name="embedding2">Second embedding vector.</param>
        /// <returns>The dot product value as a similarity score.</returns>
        /// <exception cref="ArgumentNullException">Thrown if either embedding is null.</exception>
        /// <exception cref="ArgumentException">Thrown if embeddings have different lengths.</exception>
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
