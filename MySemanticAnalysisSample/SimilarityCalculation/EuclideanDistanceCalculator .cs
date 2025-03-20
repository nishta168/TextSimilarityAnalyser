using System;

namespace MySemanticAnalysisSample.SimilarityCalculation
{
    /// <summary>
    /// Computes the Euclidean distance between two embedding vectors.
    /// The Euclidean distance measures the straight-line distance between two points in a multidimensional space.
    /// Lower values indicate higher similarity.
    /// </summary>
    internal class EuclideanDistanceCalculator : ISimilarityCalculator
    {
        /// <summary>
        /// Calculates the Euclidean distance between two embedding vectors.
        /// </summary>
        /// <param name="embedding1">First embedding vector.</param>
        /// <param name="embedding2">Second embedding vector.</param>
        /// <returns>The Euclidean distance as a similarity measure.</returns>
        /// <exception cref="ArgumentNullException">Thrown if either embedding is null.</exception>
        /// <exception cref="ArgumentException">Thrown if embeddings have different lengths.</exception>
        public float CalculateSimilarity(float[] embedding1, float[] embedding2)
        {
            if (embedding1 == null || embedding2 == null)
                throw new ArgumentNullException("Embeddings cannot be null.");

            if (embedding1.Length != embedding2.Length)
                throw new ArgumentException("Embeddings must have the same length.");

            double sumOfSquares = 0;

            for (int i = 0; i < embedding1.Length; i++)
            {
                double difference = embedding1[i] - embedding2[i];
                sumOfSquares += difference * difference;
            }

            return (float)Math.Sqrt(sumOfSquares);
        }
    }
}
