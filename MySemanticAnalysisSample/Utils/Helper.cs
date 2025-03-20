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

    }
}
