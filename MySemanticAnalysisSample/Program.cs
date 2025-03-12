using MySemanticAnalysisSample.Embedding;
using MySemanticAnalysisSample.FileHandling;
using MySemanticAnalysisSample.Preprocessing;
using MySemanticAnalysisSample.SimilarityCalculation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Data;
using System.Reflection.Metadata;

namespace MySemanticAnalysisSample
{
    internal class Program
    {
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to Text Similarity Analyser");
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Load from appsettings.json
                .AddEnvironmentVariables() // Load from environment variables
                .AddCommandLine(args) // Load from command-line arguments
                .Build();

            var mode = config["mode"]?.ToLower(); // Convert to lowercase for case-insensitive comparison

            var validModes = new HashSet<string>
            {
                "comparewordswithwords",
                "comparedocumentswithwords",
                "comparedocumentswithdocuments"
            };

            if (string.IsNullOrEmpty(mode) || !validModes.Contains(mode))
            {
                Console.WriteLine("Invalid mode! Please enter a valid mode of comparison.");
                Console.WriteLine("Usage: MySemanticAnalysisSample.exe --mode <CompareWordsWithWords | CompareDocumentsWithWords | CompareDocumentsWithDocuments>");
                return;
            }

            Console.WriteLine($"Selected mode: {mode}");

            var similarityDataTable = new List<string[]>();


            // Call the appropriate method based on the mode
            if (mode == "comparewordswithwords")
            {
                similarityDataTable = await CompareWordsWithWordsAsync(config);
            }
            else if (mode == "comparedocumentswithwords")
            {
                similarityDataTable = await CompareDocsWithWordsAsync(config);
            }
            else if (mode == "comparedocumentswithdocuments")
            {
                similarityDataTable = await CompareDocsWithDocsAsync(config);
            }


            string outputCSVPath = config["outputCSVPath"];

            if (string.IsNullOrEmpty(outputCSVPath) || !Directory.Exists(Path.GetDirectoryName(outputCSVPath)))
            {
                string outputFolder = AppContext.BaseDirectory; // Default: Same folder as the app
                string outputFilePath = Path.Combine(outputFolder, "similarity_result.csv");
                CSVWriter.WriteToCSV(outputFilePath, similarityDataTable);
                Console.WriteLine($"Similarity data successfully written to: {outputFilePath}");
            }
            else
            {
                CSVWriter.WriteToCSV(outputCSVPath, similarityDataTable);
                Console.WriteLine($"Similarity data successfully written to: {outputCSVPath}");
            }

        }

        static async Task<List<string[]>> CompareWordsWithWordsAsync(IConfiguration config)
        {
            string queryWordsPath = config["queryWordsPath"];
            string referenceWordsPath = config["referenceWordsPath"];

            if (string.IsNullOrEmpty(queryWordsPath) || string.IsNullOrEmpty(referenceWordsPath))
            {
                Console.WriteLine("Error: Missing required file paths.");
                Console.WriteLine("Please provide 'queryWordsPath' and 'referenceWordsPath' via command-line arguments or appsettings.json.");
                Console.WriteLine("Command-line arguments usage: MySemanticAnalysisSample.exe --mode CompareWordsWithWords --queryWordsPath \"path/to/querywords.txt\" --referenceWordsPath \"path/to/referencewords.txt\"");
                return null;
            }

            if (!File.Exists(queryWordsPath))
            {
                Console.WriteLine($"Error: Query words file not found at: {queryWordsPath}");
                return null;
            }

            if (!File.Exists(referenceWordsPath))
            {
                Console.WriteLine($"Error: Reference words file not found at: {referenceWordsPath}");
                return null;
            }

            if (Path.GetExtension(queryWordsPath).ToLower() != ".txt")
            {
                Console.WriteLine($"Error: Invalid query words file. The file must be a .txt file with each query word on a separate line. Found: {queryWordsPath}");
                return null;
            }

            if (Path.GetExtension(referenceWordsPath).ToLower() != ".txt")
            {
                Console.WriteLine($"Error: Invalid reference words file. The file must be a .txt file with each reference word on a separate line. Found: {referenceWordsPath}");
                return null;
            }

            var queryWordsReader = new FileReader(queryWordsPath);
            var queryWords = queryWordsReader.ReadWordsOrPhrases();

            var referenceWordsReader = new FileReader(referenceWordsPath);
            var referenceWords = referenceWordsReader.ReadWordsOrPhrases();



            try
            {
                if (queryWords.Count < 1 || referenceWords.Count < 1)
                {
                    throw new NullReferenceException("Minimum one query text and reference text required to compare");
                }

                var processor = new TextProcessor();
                var embeddingGenerator = new ChatGPTEmbeddingGenerator(config);

                var processedQueryWords = processor.ProcessWordOrPhraseList(queryWords);
                var processedReferenceWords = processor.ProcessWordOrPhraseList(referenceWords);

                var queryEmbeddingDictionary = await embeddingGenerator.EmbedWordsListAsync(processedQueryWords);
                var referenceEmbeddingDictionary = await embeddingGenerator.EmbedWordsListAsync(processedReferenceWords);

                
                var similarityDataTable = new List<string[]>();
                var similarityDataTableFirstRow = new List<string>();
                similarityDataTableFirstRow.Add("   ");
                
                foreach (var reference in referenceEmbeddingDictionary)
                {                  
                    similarityDataTableFirstRow.Add(reference.Key);
                }

                similarityDataTable.Add(similarityDataTableFirstRow.ToArray());

                //var similarityCalculator = new CosineSimilarityCalculator();
                //var similarityCalculator = new EuclideanDistanceCalculator();
                //var similarityCalculator = new DotProductSimilarityCalculator();
                var similarityCalculator = new JaccardSimilarityCalculator();
                foreach (var query in queryEmbeddingDictionary)
                {

                    var similarityDataTableRow = new List<string>();
                    similarityDataTableRow.Add(query.Key);

                    foreach (var reference in referenceEmbeddingDictionary)
                    {
                        var similarity = similarityCalculator.CalculateSimilarity(reference.Value, query.Value);
                        similarityDataTableRow.Add(similarity.ToString());
                    }

                    similarityDataTable.Add(similarityDataTableRow.ToArray());
                }
                 
                return similarityDataTable;
                

            }
            catch (Exception)
            {

                throw;
            }


        }

        static async Task<List<string[]>> CompareDocsWithWordsAsync(IConfiguration config)
        {

            string queryDocumentsPath = config["queryDocumentsPath"];
            string referenceWordsPath = config["referenceWordsPath"];

            if (string.IsNullOrEmpty(queryDocumentsPath) || string.IsNullOrEmpty(referenceWordsPath))
            {
                Console.WriteLine("Error: Missing required file paths.");
                Console.WriteLine("Please provide 'queryDocumentPath' and 'referenceWordsPath' via command-line arguments or appsettings.json.");
                Console.WriteLine("Command-line arguments usage: MySemanticAnalysisSample.exe --mode CompareDocumentsWithWords --queryDocumentsPath \"path/to/querydocumentsfolder\" --referenceWordsPath \"path/to/referencewords.txt\"");
                return null;
            }

            if (!Directory.Exists(queryDocumentsPath))
            {
                Console.WriteLine($"Error: Query documents path must be a directory containing documents as .txt files. Found: {queryDocumentsPath}");
                return null;
            }

            if (!File.Exists(referenceWordsPath))
            {
                Console.WriteLine($"Error: Reference words file not found at: {referenceWordsPath}");
                return null;
            }

            
            if (Path.GetExtension(referenceWordsPath).ToLower() != ".txt")
            {
                Console.WriteLine($"Error: Invalid reference words file. The file must be a .txt file with each reference word on a separate line. Found: {referenceWordsPath}");
                return null;
            }

            var queryDocsReader = new FileReader(queryDocumentsPath);
            var queryDocs = queryDocsReader.ReadDocuments();

            var referenceWordsReader = new FileReader(referenceWordsPath);
            var referenceWords = referenceWordsReader.ReadWordsOrPhrases();


            try
            {
                if (queryDocs.Count < 1 || referenceWords.Count < 1)
                {
                    throw new NullReferenceException("Minimum one query doc and reference text is required to compare");
                }

                var processor = new TextProcessor();
                var embeddingGenerator = new ChatGPTEmbeddingGenerator(config);

                var processedQueryDocs = processor.ProcessDocumentList(queryDocs, true);
                var processedReferenceWords = processor.ProcessWordOrPhraseList(referenceWords);

                var queryEmbeddingDictionary = await embeddingGenerator.EmbedDocumentsListAsync(processedQueryDocs);
                var referenceEmbeddingDictionary = await embeddingGenerator.EmbedWordsListAsync(processedReferenceWords);

                var similarityDataTable = new List<string[]>();
                var similarityDataTableFirstRow = new List<string>();
                similarityDataTableFirstRow.Add("    ");

                foreach (var reference in referenceEmbeddingDictionary)
                {
                    similarityDataTableFirstRow.Add(reference.Key);
                }

                similarityDataTable.Add(similarityDataTableFirstRow.ToArray());

                var similarityCalculator = new CosineSimilarityCalculator();
                const float beta = 50.0f; // Adjust beta to control weighting (higher → stronger bias toward large values)


                foreach (var query in queryEmbeddingDictionary)
                {
                    var similarityDataTableRow = new List<string>();
                    similarityDataTableRow.Add(query.Key);

                    foreach (var reference in referenceEmbeddingDictionary)
                    {
                        var similarities = new List<float>();
                        foreach (var embedding in query.Value)
                        {
                            var sim = similarityCalculator.CalculateSimilarity(embedding, reference.Value);
                            similarities.Add(sim);
                        }

                        //Exponential Weighted Mean Calculation
                        var expWeights = similarities.Select(x => (float)Math.Exp(beta * x)).ToList();
                        float weightedSum = similarities.Zip(expWeights, (sim, weight) => sim * weight).Sum();
                        float weightSum = expWeights.Sum();
                        float weightedSimilarity = weightSum != 0 ? weightedSum / weightSum : 0; // Avoid division by zero


                        similarityDataTableRow.Add(weightedSimilarity.ToString());
                    }


                    similarityDataTable.Add(similarityDataTableRow.ToArray());

                }

                return similarityDataTable;


            }
            catch (Exception)
            {

                throw;
            }


        }

        static async Task<List<string[]>> CompareDocsWithDocsAsync(IConfiguration config)
        {
            string queryDocumentsPath = config["queryDocumentsPath"];
            string referenceDocumentsPath = config["referenceDocumentsPath"];

            if (string.IsNullOrEmpty(queryDocumentsPath) || string.IsNullOrEmpty(referenceDocumentsPath))
            {
                Console.WriteLine("Error: Missing required file paths.");
                Console.WriteLine("Please provide 'queryDocumentPath' and 'referenceDocumentsPath' via command-line arguments or appsettings.json.");
                Console.WriteLine("Command-line arguments usage: MySemanticAnalysisSample.exe --mode CompareDocumentsWithDocuments --queryDocumentsPath \"path/to/querydocumentsfolder\" --referenceDocumentsPath \"path/to/referencedocumentsfolder\"");
                return null;
            }

            if (!Directory.Exists(queryDocumentsPath))
            {
                Console.WriteLine($"Error: Query documents path must be a directory containing query documents as .txt files. Found: {queryDocumentsPath}");
                return null;
            }
            if (!Directory.Exists(referenceDocumentsPath))
            {
                Console.WriteLine($"Error: Reference documents path must be a directory containing reference documents as .txt files. Found: {referenceDocumentsPath}");
                return null;
            }

            var queryDocsReader = new FileReader(queryDocumentsPath);
            var queryDocs = queryDocsReader.ReadDocuments();

            var referenceDocsReader = new FileReader(referenceDocumentsPath);
            var referenceDocs = referenceDocsReader.ReadDocuments();


            try
            {
                if (queryDocs.Count < 1 || referenceDocs.Count < 1)
                {
                    throw new NullReferenceException("Minimum one query doc and one reference doc is required to compare");
                }

                var processor = new TextProcessor();
                var embeddingGenerator = new ChatGPTEmbeddingGenerator(config);

                var processedQueryDocs = processor.ProcessDocumentList(queryDocs, false);
                var processedReferenceDocs = processor.ProcessDocumentList(referenceDocs, false);

                var queryEmbeddingDictionary = await embeddingGenerator.EmbedDocumentsListAsync(processedQueryDocs);
                var referenceEmbeddingDictionary = await embeddingGenerator.EmbedDocumentsListAsync(processedReferenceDocs);

                var queryDictionary = CalculateMeanEmbedding(queryEmbeddingDictionary);
                var refDictionary = CalculateMeanEmbedding(referenceEmbeddingDictionary);


                var similarityDataTable = new List<string[]>();
                var similarityDataTableFirstRow = new List<string>();
                similarityDataTableFirstRow.Add("    ");



                foreach (var reference in refDictionary)
                {
                    similarityDataTableFirstRow.Add(reference.Key);
                }

                similarityDataTable.Add(similarityDataTableFirstRow.ToArray());

                var similarityCalculator = new CosineSimilarityCalculator();

                foreach (var query in queryDictionary)
                {
                    var similarityDataTableRow = new List<string>();
                    similarityDataTableRow.Add(query.Key);

                    foreach (var reference in refDictionary)
                    {
                        var similarity = similarityCalculator.CalculateSimilarity(reference.Value, query.Value);
                        similarityDataTableRow.Add(similarity.ToString());

                    }

                    similarityDataTable.Add(similarityDataTableRow.ToArray());
                }

                return similarityDataTable;


            }
            catch (Exception)
            {

                throw;
            }


        }

        static Dictionary<string, float[]> CalculateMeanEmbedding (Dictionary<string, List<float[]>> embDictionary)
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
