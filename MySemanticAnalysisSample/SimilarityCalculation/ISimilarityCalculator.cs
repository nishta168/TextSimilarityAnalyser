using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.SimilarityCalculation
{
    /// <summary>
    /// Defines methods to be provided by implementing classes to calculate similarity between two embedding vectors.
    /// </summary>
    internal interface ISimilarityCalculator
    {
        /// <summary>
        /// Calculates the similarity between two embedding vectors.
        /// </summary>
        /// <param name="embedding1">The first embedding vector.</param>
        /// <param name="embedding2">The second embedding vector.</param>
        /// <returns>A similarity score as a float value.</returns>
        float CalculateSimilarity(float[] embedding1, float[] embedding2);
    }
}
