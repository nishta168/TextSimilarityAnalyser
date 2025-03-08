using System;
using System.Collections.Generic;
using System.Linq;

namespace MySemanticAnalysisSample.SimilarityCalculation
{
    internal class JaccardSimilarityCalculator : ISimilarityCalculator
    {
        public float CalculateSimilarity(float[] embedding1, float[] embedding2)
        {
            if (embedding1 == null || embedding2 == null)
                throw new ArgumentNullException("Embeddings cannot be null.");

            if (embedding1.Length != embedding2.Length)
                throw new ArgumentException("Embeddings must have the same length.");

            // Convert embeddings to sets (by treating non-zero values as elements)
            HashSet<float> set1 = new HashSet<float>();
            HashSet<float> set2 = new HashSet<float>();

            for (int i = 0; i < embedding1.Length; i++)
            {
                if (embedding1[i] != 0) set1.Add(embedding1[i]);
                if (embedding2[i] != 0) set2.Add(embedding2[i]);
            }

            // Compute intersection and union
            int intersectionCount = set1.Intersect(set2).Count();
            int unionCount = set1.Union(set2).Count();

            // Jaccard similarity = |A ∩ B| / |A ∪ B|
            return unionCount == 0 ? 0 : (float)intersectionCount / unionCount;
        }
    }
}
