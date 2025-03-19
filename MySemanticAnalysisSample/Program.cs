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

            try 
            { 
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

                string outputCSVPath = config["Output:outputSimilarityCSVPath"];
                outputCSVPath = FilePathValidator.ValidateOutputFilePath(outputCSVPath, "similarity_result.csv");               
                CSVWriter.WriteSimilarityToCSV(outputCSVPath, similarityDataTable);
                Console.WriteLine($"Similarity data successfully written to: {outputCSVPath}");

                }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine("Stack Trace:");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1); // Ensure the program terminates with an error code
            }

        }

        static async Task<List<string[]>> CompareWordsWithWordsAsync(IConfiguration config)
        {
            string queryWordsPath = config["queryWordsPath"];
            string referenceWordsPath = config["referenceWordsPath"];

            FilePathValidator.ValidateTxtFilePath(queryWordsPath);
            FilePathValidator.ValidateTxtFilePath(referenceWordsPath);            

            var queryWordsReader = new FileReader(queryWordsPath);
            var queryWords = queryWordsReader.ReadWordsOrPhrases();

            var referenceWordsReader = new FileReader(referenceWordsPath);
            var referenceWords = referenceWordsReader.ReadWordsOrPhrases();
                
            var processor = new TextProcessor();
            var processedQueryWords = processor.ProcessWordOrPhraseList(queryWords);
            var processedReferenceWords = processor.ProcessWordOrPhraseList(referenceWords);

            var embeddingGenerator = new ChatGPTEmbeddingGenerator(config);          
            var queryEmbeddingDictionary = await embeddingGenerator.EmbedWordsListAsync(processedQueryWords);
            var referenceEmbeddingDictionary = await embeddingGenerator.EmbedWordsListAsync(processedReferenceWords);

            var outputQueryEmbeddingCSVPath = config["Output:queryEmbeddingCSVPath"];
            outputQueryEmbeddingCSVPath = FilePathValidator.ValidateOutputFilePath(outputQueryEmbeddingCSVPath, "query_embeddings.csv");
            CSVWriter.WriteEmbeddingsToCSV(outputQueryEmbeddingCSVPath, queryEmbeddingDictionary, false);
                
            var outputReferenceEmbeddingsCSVPath = config["Output:referenceEmbeddingCSVPath"];
            outputReferenceEmbeddingsCSVPath = FilePathValidator.ValidateOutputFilePath(outputReferenceEmbeddingsCSVPath, "reference_embeddings.csv");
            CSVWriter.WriteEmbeddingsToCSV(outputReferenceEmbeddingsCSVPath, referenceEmbeddingDictionary, false);


            var similarityDataTable = new List<string[]>();
            var similarityDataTableFirstRow = new List<string>();
            similarityDataTableFirstRow.Add("   ");
                
            foreach (var reference in referenceEmbeddingDictionary)
            {                  
                similarityDataTableFirstRow.Add(reference.Key); 
            }

            similarityDataTable.Add(similarityDataTableFirstRow.ToArray());

            var similarityCalculator = new CosineSimilarityCalculator();
            //var similarityCalculator = new EuclideanDistanceCalculator();
            //var similarityCalculator = new DotProductSimilarityCalculator();
            //var similarityCalculator = new JaccardSimilarityCalculator();

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

            if (similarityDataTable.Count < 2)
            {
                throw new InvalidOperationException("Error in calculating similarities");
            }

            return similarityDataTable;

        }

        static async Task<List<string[]>> CompareDocsWithWordsAsync(IConfiguration config)
        {

            string queryDocumentsPath = config["queryDocumentsPath"];
            string referenceWordsPath = config["referenceWordsPath"];

            FilePathValidator.ValidateDocumentsFolderPath(queryDocumentsPath);
            FilePathValidator.ValidateTxtFilePath(referenceWordsPath);         
            

            var queryDocsReader = new FileReader(queryDocumentsPath);
            var queryDocs = queryDocsReader.ReadDocuments();

            var referenceWordsReader = new FileReader(referenceWordsPath);
            var referenceWords = referenceWordsReader.ReadWordsOrPhrases();            

            var processor = new TextProcessor();
            var processedQueryDocs = processor.ProcessDocumentList(queryDocs, true);
            var processedReferenceWords = processor.ProcessWordOrPhraseList(referenceWords);

            var embeddingGenerator = new ChatGPTEmbeddingGenerator(config);
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
                        var expWeights = similarities.Select(x => (float)Math.Exp(beta * x)).ToList(); //address very high values later 
                        float weightedSum = similarities.Zip(expWeights, (sim, weight) => sim * weight).Sum();
                        float weightSum = expWeights.Sum();
                        float weightedSimilarity = weightSum != 0 ? weightedSum / weightSum : 0; // Avoid division by zero


                        similarityDataTableRow.Add(weightedSimilarity.ToString());
                    }


                    similarityDataTable.Add(similarityDataTableRow.ToArray());

                }
            if (similarityDataTable.Count < 2)
            {
                throw new InvalidOperationException("Error in calculating similarities");
            }

            return similarityDataTable;


        }
            


        

        static async Task<List<string[]>> CompareDocsWithDocsAsync(IConfiguration config)
        {
            string queryDocumentsPath = config["queryDocumentsPath"];
            string referenceDocumentsPath = config["referenceDocumentsPath"];

            FilePathValidator.ValidateDocumentsFolderPath(queryDocumentsPath);
            FilePathValidator.ValidateTxtFilePath(referenceDocumentsPath);          

            var queryDocsReader = new FileReader(queryDocumentsPath);
            var queryDocs = queryDocsReader.ReadDocuments();

            var referenceDocsReader = new FileReader(referenceDocumentsPath);
            var referenceDocs = referenceDocsReader.ReadDocuments();


            
               
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
            if (similarityDataTable.Count < 2)
            {
                throw new InvalidOperationException("Error in calculating similarities");
            }

            return similarityDataTable;

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
