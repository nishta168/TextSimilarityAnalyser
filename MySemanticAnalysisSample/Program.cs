using MySemanticAnalysisSample.Embedding;
using MySemanticAnalysisSample.FileHandling;
using MySemanticAnalysisSample.Preprocessing;
using MySemanticAnalysisSample.SimilarityCalculation;
using Microsoft.Extensions.Configuration;
using System.Data;
using MySemanticAnalysisSample.Utils;

namespace MySemanticAnalysisSample
{
    /// <summary>
    /// Loads configuration, validates the mode chosen by user, executes the appropriate comparison method and writes output to csv files.
    /// </summary>
    internal class Program
    {
        static async Task Main(string[] args)
        {            
            Console.WriteLine("Welcome to Text Similarity Analyser\n");

            if (!File.Exists("appsettings.json"))
            {
                Console.WriteLine("Error: Configuration file 'appsettings.json' not found. Please ensure it is present in the application directory.");
                Environment.Exit(1); // Ensure the program terminates with an error code
            }

            try
            {
                var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

                //read and validate user selected mode
                var mode = config["mode"]?.ToLower();

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
                    Environment.Exit(1);
                }

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

                string outputCSVPath = config["Output:similarityCSVPath"];
                outputCSVPath = FilePathValidator.ValidateOutputFilePath(outputCSVPath, "similarity_result.csv");
                CSVWriter.WriteSimilarityToCSV(outputCSVPath, similarityDataTable);
                Console.WriteLine($"Similarity data successfully written to: {outputCSVPath}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine("Stack Trace:");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }

        }

        /// <summary>
        /// Compares a list of query words/phrases with a list of reference words/phrases and provides the embedding values and similarity results.
        /// </summary>
        /// <param name="config">Application configuration</param>
        /// <returns>A list of string arrays representing the similarity table, where the first row contains reference words/phrases, the first column contains query words/phrases, and the remaining cells contain their similarity scores.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        static async Task<List<string[]>> CompareWordsWithWordsAsync(IConfiguration config)
        {
            Console.WriteLine("Selected mode: Compare Words/Phrases with Words/Phrases\n");

            // Read and validate input file paths
            string queryWordsPath = config["Input:queryWordsPath"];
            string referenceWordsPath = config["Input:referenceWordsPath"];
            queryWordsPath = FilePathValidator.ValidateInputPath(queryWordsPath, false, true);
            referenceWordsPath = FilePathValidator.ValidateInputPath(referenceWordsPath, false, false);

            // Read words/phrases from files
            var queryWordsReader = new FileReader(queryWordsPath);
            var queryWords = queryWordsReader.ReadWordsOrPhrases();
            var referenceWordsReader = new FileReader(referenceWordsPath);
            var referenceWords = referenceWordsReader.ReadWordsOrPhrases();

            // Preprocess words/phrases
            var processor = new TextProcessor();
            var processedQueryWords = processor.ProcessWordOrPhraseList(queryWords);
            var processedReferenceWords = processor.ProcessWordOrPhraseList(referenceWords);

            // Generate embeddings
            var embeddingGenerator = new ChatGPTEmbeddingGenerator(config);
            var queryEmbeddingDictionary = await embeddingGenerator.EmbedWordsListAsync(processedQueryWords);
            var referenceEmbeddingDictionary = await embeddingGenerator.EmbedWordsListAsync(processedReferenceWords);

            //write embeddings to csv files for scalar value visualisation
            var outputQueryEmbeddingCSVPath = config["Output:queryEmbeddingCSVPath"];
            outputQueryEmbeddingCSVPath = FilePathValidator.ValidateOutputFilePath(outputQueryEmbeddingCSVPath, "query_embeddings.csv");
            CSVWriter.WriteEmbeddingsToCSV(outputQueryEmbeddingCSVPath, queryEmbeddingDictionary);
            var outputReferenceEmbeddingsCSVPath = config["Output:referenceEmbeddingCSVPath"];
            outputReferenceEmbeddingsCSVPath = FilePathValidator.ValidateOutputFilePath(outputReferenceEmbeddingsCSVPath, "reference_embeddings.csv");
            CSVWriter.WriteEmbeddingsToCSV(outputReferenceEmbeddingsCSVPath, referenceEmbeddingDictionary);

            //creating similarity table
            var similarityDataTable = new List<string[]>();
            var similarityDataTableFirstRow = new List<string> { "Q\\R" };

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
                var similarityDataTableRow = new List<string> { query.Key };

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

        /// <summary>
        /// Compares a list of query documents with a list of query words/phrases and provides the embedding values and similarity results.
        /// </summary>
        /// <param name="config">Application configuration</param>
        /// <returns>A list of string arrays representing the similarity table, where the first row contains reference words/phrases, the first column contains query documents, and the remaining cells contain their similarity scores.</returns>
        static async Task<List<string[]>> CompareDocsWithWordsAsync(IConfiguration config)
        {
            Console.WriteLine("Selected mode: Compare Documents with Words/Phrases\n");

            // Read and validate input file paths
            string queryDocumentsPath = config["Input:queryDocumentsPath"];
            string referenceWordsPath = config["Input:referenceWordsPath"];
            queryDocumentsPath = FilePathValidator.ValidateInputPath(queryDocumentsPath, true, true);
            referenceWordsPath = FilePathValidator.ValidateInputPath(referenceWordsPath, false, false);

            // Read reference words/phrases and query documents 
            var queryDocsReader = new FileReader(queryDocumentsPath);
            var queryDocs = queryDocsReader.ReadDocuments();
            var referenceWordsReader = new FileReader(referenceWordsPath);
            var referenceWords = referenceWordsReader.ReadWordsOrPhrases();

            // Preprocess documents and words
            var processor = new TextProcessor();
            var processedQueryDocs = processor.ProcessDocumentList(queryDocs, true);
            var processedReferenceWords = processor.ProcessWordOrPhraseList(referenceWords);

            // Generate embeddings
            var embeddingGenerator = new ChatGPTEmbeddingGenerator(config);
            var queryEmbeddingDictionary = await embeddingGenerator.EmbedDocumentsListAsync(processedQueryDocs);
            var referenceEmbeddingDictionary = await embeddingGenerator.EmbedWordsListAsync(processedReferenceWords);

            //write embeddings to csv files for scalar value visualisation
            var outputQueryEmbeddingCSVPath = config["Output:queryEmbeddingCSVPath"];
            outputQueryEmbeddingCSVPath = FilePathValidator.ValidateOutputFilePath(outputQueryEmbeddingCSVPath, "query_embeddings.csv");
            CSVWriter.WriteEmbeddingsToCSV(outputQueryEmbeddingCSVPath, queryEmbeddingDictionary);
            var outputReferenceEmbeddingsCSVPath = config["Output:referenceEmbeddingCSVPath"];
            outputReferenceEmbeddingsCSVPath = FilePathValidator.ValidateOutputFilePath(outputReferenceEmbeddingsCSVPath, "reference_embeddings.csv");
            CSVWriter.WriteEmbeddingsToCSV(outputReferenceEmbeddingsCSVPath, referenceEmbeddingDictionary);

            //creating similarity table
            var similarityDataTable = new List<string[]>();
            var similarityDataTableFirstRow = new List<string> { "Q\\R" };

            foreach (var reference in referenceEmbeddingDictionary)
            {
                similarityDataTableFirstRow.Add(reference.Key);
            }

            similarityDataTable.Add(similarityDataTableFirstRow.ToArray());

            var similarityCalculator = new CosineSimilarityCalculator();

            const float beta = 30.0f; // Adjusting beta controls weighting for exponential mean calculation  (higher → stronger bias toward large values)

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
                    //Exponentially Weighted Mean Calculation
                    var weightedSimilarity = Helper.CalculateExponentiallyWeightedMean(similarities, beta);

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

        /// <summary>
        /// Compares a list of query documents with a list of reference documents and provides the embedding values and similarity results.
        /// </summary>
        /// <param name="config">Application configuration</param>
        /// <returns>A list of string arrays representing the similarity table, where the first row contains reference documents, the first column contains query documents, and the remaining cells contain their similarity scores.</returns>
        static async Task<List<string[]>> CompareDocsWithDocsAsync(IConfiguration config)
        {
            Console.WriteLine("Selected mode: Compare Documents with Documents\n");

            // Read and validate input file paths
            string queryDocumentsPath = config["Input:queryDocumentsPath"];
            string referenceDocumentsPath = config["Input:referenceDocumentsPath"];
            queryDocumentsPath = FilePathValidator.ValidateInputPath(queryDocumentsPath, true, true);
            referenceDocumentsPath = FilePathValidator.ValidateInputPath(referenceDocumentsPath, true, false);

            // Read documents from folders
            var queryDocsReader = new FileReader(queryDocumentsPath);
            var queryDocs = queryDocsReader.ReadDocuments();
            var referenceDocsReader = new FileReader(referenceDocumentsPath);
            var referenceDocs = referenceDocsReader.ReadDocuments();

            // Preprocess documents   
            var processor = new TextProcessor();
            var processedQueryDocs = processor.ProcessDocumentList(queryDocs, false);
            var processedReferenceDocs = processor.ProcessDocumentList(referenceDocs, false);

            // Generate embeddings
            var embeddingGenerator = new ChatGPTEmbeddingGenerator(config);
            var queryEmbeddingDictionary = await embeddingGenerator.EmbedDocumentsListAsync(processedQueryDocs);
            var referenceEmbeddingDictionary = await embeddingGenerator.EmbedDocumentsListAsync(processedReferenceDocs);

            // Calculate mean of embeddings where document size exceeds max token limit
            var queryDictionary = Helper.CalculateMeanEmbedding(queryEmbeddingDictionary);
            var refDictionary = Helper.CalculateMeanEmbedding(referenceEmbeddingDictionary);

            // Write embeddings to csv file to visualise scalar values 
            var outputQueryEmbeddingCSVPath = config["Output:queryEmbeddingCSVPath"];
            outputQueryEmbeddingCSVPath = FilePathValidator.ValidateOutputFilePath(outputQueryEmbeddingCSVPath, "query_embeddings.csv");
            CSVWriter.WriteEmbeddingsToCSV(outputQueryEmbeddingCSVPath, queryDictionary);
            var outputReferenceEmbeddingsCSVPath = config["Output:referenceEmbeddingCSVPath"];
            outputReferenceEmbeddingsCSVPath = FilePathValidator.ValidateOutputFilePath(outputReferenceEmbeddingsCSVPath, "reference_embeddings.csv");
            CSVWriter.WriteEmbeddingsToCSV(outputReferenceEmbeddingsCSVPath, refDictionary);

            // Creating similarity table
            var similarityDataTable = new List<string[]>();
            var similarityDataTableFirstRow = new List<string> { "Q\\R" };

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
    }
}
