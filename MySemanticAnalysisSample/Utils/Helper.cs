using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.Utils
{
    internal class Helper
    {
        public static Dictionary<string, float[]> CalculateMeanEmbedding(Dictionary<string, List<float[]>> embDictionary)
        {
            var dictionary = new Dictionary<string, float[]>();

            foreach (var doc in embDictionary)
            {
                if (doc.Value.Count > 0)
                {
                    int embeddingSize = doc.Value[0].Length;
                    var avgEmbedding = new float[embeddingSize];

                    for (int i = 0; i < embeddingSize; i++)
                    {
                        avgEmbedding[i] = doc.Value.Average(e => e[i]);
                    }

                    dictionary.Add(doc.Key, avgEmbedding);
                }
            }
            return dictionary;
        }

    }
}
