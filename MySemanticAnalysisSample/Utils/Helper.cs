using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.Utils
{
    /// <summary>
    /// Helper class providing utility methods for various tasks.
    /// </summary>
    internal class Helper
    {
        /// <summary>
        /// Computes the mean embedding for each document in the given dictionary.
        /// Each document may have multiple embeddings for chunks, and this method averages them.
        /// </summary>
        /// <param name="embDictionary">A dictionary where keys are document names and values are lists of embeddings of chunks (float arrays).</param>
        /// <returns>A dictionary where each document key maps to a single averaged embedding.</returns>
        public static Dictionary<string, float[]> CalculateMeanEmbedding(Dictionary<string, List<float[]>> embDictionary)
        {
            var dictionary = new Dictionary<string, float[]>();

            foreach (var doc in embDictionary)
            {
                int embeddingSize = doc.Value[0].Length;
                var avgEmbedding = new float[embeddingSize];

                for (int i = 0; i < embeddingSize; i++)
                {
                    avgEmbedding[i] = doc.Value.Average(e => e[i]);
                }

                dictionary.Add(doc.Key, avgEmbedding);
            }
            return dictionary;
        }

        /// <summary>
        /// Computes the exponentially weighted mean of a list of similarity values.  
        /// Higher similarity values receive greater weight, emphasizing higher impact of highly matching text chunks  
        /// while minimizing the influence of non-matching chunks.
        /// </summary>
        /// <param name="similarities">A list with similarity scores of all individual chunks of one document with one reference word/phrase.</param>
        /// <param name="beta">A scaling factor that controls the weighting effect (higher values emphasize larger similarities more).</param>
        /// <returns>The exponentially weighted mean similarity score.</returns>
        public static float CalculateExponentiallyWeightedMean(List<float> similarities, float beta)
        {
            var expWeights = similarities.Select(x => (float)Math.Exp(beta * x)).ToList();
            float weightedSum = similarities.Zip(expWeights, (sim, weight) => sim * weight).Sum();
            float weightSum = expWeights.Sum();
            float weightedSimilarity = weightSum != 0 ? weightedSum / weightSum : 0;
            return weightedSimilarity;
        }
    }
}
