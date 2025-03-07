using System;

namespace MySemanticAnalysisSample.SimilarityCalculation
{
    internal class EuclideanDistanceCalculator : ISimilarityCalculator
    {
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
