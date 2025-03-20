using Microsoft.VisualBasic;
using MySemanticAnalysisSample.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.FileHandling
{
    internal class CSVWriter
    {

        public static void WriteSimilarityToCSV(string outputfilePath, List<string[]> similarityDataTable)
        {

            using (var writer = new StreamWriter(outputfilePath))
            {
                foreach (var row in similarityDataTable)
                {
                    writer.WriteLine(string.Join(",", row));
                }
            }
        }

        public static void WriteEmbeddingsToCSV(string outputfilePath, Dictionary<string, float[]> embeddings)
        {
            Write(outputfilePath, embeddings);
        }

        public static void WriteEmbeddingsToCSV(string outputfilePath, Dictionary<string, List<float[]>> docEmbeddings)
        {
            var embeddings = Helper.CalculateMeanEmbedding(docEmbeddings);
            Write(outputfilePath, embeddings);

        }

        private static void Write(string outputfilePath, Dictionary<string, float[]> embeddings)
        {
            using (var writer = new StreamWriter(outputfilePath))
            {
                var firstRow = new List<string>();
                var index = embeddings.Values.First().Length;
                firstRow.Add("Categories");
                for (int i = 0; i < index; i++)
                {
                    firstRow.Add(i.ToString());
                }
                writer.WriteLine(string.Join(",", firstRow));

                foreach (var row in embeddings)
                {
                    var csvRow = new List<string>();
                    csvRow.Add(row.Key);
                    foreach (var num in row.Value)
                    {
                        csvRow.Add(num.ToString());
                    }
                    writer.WriteLine(string.Join(",", csvRow));
                }
            }
            Console.WriteLine("Embeddings successfully written to " + outputfilePath);
        }
    }
}
