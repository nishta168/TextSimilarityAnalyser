using System;
using MySemanticAnalysisSample.SimilarityCalculation;

namespace MySemanticAnalysisSample.SimilarityCalculation
{
    /// <summary>
    /// Calculates similarity of embedding vectors using cosine similarity
    /// </summary>
    internal class CosineSimilarityCalculator : ISimilarityCalculator
    {
        /// <summary>
        /// Computes the cosine similarity between two embedding vectors.
        /// Cosine similarity measures the cosine of the angle between two vectors,
        /// giving a value between -1 (opposite) and 1 (identical).
        /// </summary>
        /// <param name="embedding1">First embedding vector.</param>
        /// <param name="embedding2">Second embedding vector.</param>
        /// <returns>Cosine similarity score between -1 and 1.</returns>
        /// <exception cref="ArgumentNullException">Thrown if either embedding is null.</exception>
        /// <exception cref="ArgumentException">Thrown if embeddings have different lengths.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the magnitude of any vector is zero.</exception>
        public float CalculateSimilarity(float[] embedding1, float[] embedding2)
        {
            if (embedding1 == null || embedding2 == null)
                throw new ArgumentNullException("Embeddings cannot be null.");

            if (embedding1.Length != embedding2.Length)
                throw new ArgumentException("Embeddings must have the same length.");

            double dotProduct = 0;
            double magnitude1 = 0;
            double magnitude2 = 0;

            for (int i = 0; i < embedding1.Length; i++)
            {
                dotProduct += embedding1[i] * embedding2[i];
                magnitude1 += embedding1[i] * embedding1[i];
                magnitude2 += embedding2[i] * embedding2[i];
            }

            double magnitudeProduct = Math.Sqrt(magnitude1) * Math.Sqrt(magnitude2);

            if (magnitudeProduct == 0)
                throw new InvalidOperationException("Cannot calculate similarity for zero-length vectors.");
            var cosineSimilarity = (float)(dotProduct / magnitudeProduct);

            return (float)Math.Round(cosineSimilarity, 3);
        }
    }
}
