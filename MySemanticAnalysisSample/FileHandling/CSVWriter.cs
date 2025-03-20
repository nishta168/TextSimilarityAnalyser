using Microsoft.VisualBasic;
using MySemanticAnalysisSample.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.FileHandling
{
    /// <summary>
    /// Handles writing similarity data and embedding vectors to CSV files.
    /// </summary>
    internal class CSVWriter
    {
        /// <summary>
        /// Writes similarity results to a CSV file.
        /// </summary>
        /// <param name="outputfilePath">The path where the similarity CSV file will be written.</param>
        /// <param name="similarityDataTable">Similarity data table which is represented as a list of string arrays.</param>
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

        /// <summary>
        /// Writes corresponding embedding vectors of words, phrases or unchunked documents from input dictionary to csv 
        /// </summary>
        /// <param name="outputfilePath">The path where the CSV file will be saved.</param>
        /// <param name="embeddings">A dictionary mapping words or phrases or documents(not chunked) to their corresponding embedding vectors.</param>
        public static void WriteEmbeddingsToCSV(string outputfilePath, Dictionary<string, float[]> embeddings)
        {
            Write(outputfilePath, embeddings);
        }

        /// <summary>
        /// Writes embeddings of documents after computing the mean of chunk embeddings for each document from the dictionary to csv. 
        /// </summary>
        /// <param name="outputfilePath">The path where the CSV file will be saved.</param>
        /// <param name="docEmbeddings">A dictionary of documents where key is document name and value is a list of embedding vectors of its chunks</param>
        public static void WriteEmbeddingsToCSV(string outputfilePath, Dictionary<string, List<float[]>> docEmbeddings)
        {
            //calculating mean to represent document with one embedding and visualise it in the python visualiser
            var embeddings = Helper.CalculateMeanEmbedding(docEmbeddings);
            Write(outputfilePath, embeddings);
        }

        /// <summary>
        /// Helper method to write embeddings to a CSV file.
        /// </summary>
        /// <param name="outputfilePath">The path where the CSV file will be saved.</param>
        /// <param name="embeddings">A dictionary mapping categories (words/phrases/documents) to their embedding vectors.</param>
        private static void Write(string outputfilePath, Dictionary<string, float[]> embeddings)
        {
            using (var writer = new StreamWriter(outputfilePath))
            {
                // Write the header row
                var firstRow = new List<string>();
                var index = embeddings.Values.First().Length;
                firstRow.Add("Categories");
                for (int i = 0; i < index; i++)
                {
                    firstRow.Add(i.ToString());
                }
                writer.WriteLine(string.Join(",", firstRow));

                // Write the embedding data
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
