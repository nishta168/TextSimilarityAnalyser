using MySemanticAnalysisSample.Embedding;
using MySemanticAnalysisSample.FileHandling;
using MySemanticAnalysisSample.Preprocessing;
using MySemanticAnalysisSample.SimilarityCalculation;
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




            //var similarityDataTable = await CompareWordsWithWordsAsync(config);
            //var similarityDataTable = await CompareDocsWithWordsAsync(config);
            var similarityDataTable = await CompareDocsWithDocsAsync(config);


            string outputFolder = AppContext.BaseDirectory;  // Same folder as the app
            string outputFilePath = Path.Combine(outputFolder, "similarity_result.csv");

            //string outputFilePath = @"C:\Users\NISHTA\OneDrive\Univeristy\sem_1\software_eng\ML_09\Test\TextSimilarityAnalyser\MySemanticAnalysisSample\Output\similarity_result.csv";
            CSVWriter.WriteToCSV(outputFilePath, similarityDataTable);
            Console.WriteLine("Similarity data successfully written to file");
         
        }

        static async Task<List<string[]>> CompareWordsWithWordsAsync(IConfiguration config)
        {
            //later move path to configuration
            string queryTextPath = @"C:\Users\NISHTA\OneDrive\Univeristy\sem_1\software_eng\ML_09\Test\TextSimilarityAnalyser\MySemanticAnalysisSample\Input\QueryText\";
            string referenceTextPath = @"C:\Users\NISHTA\OneDrive\Univeristy\sem_1\software_eng\ML_09\Test\TextSimilarityAnalyser\MySemanticAnalysisSample\Input\ReferenceText\";

            queryTextPath = queryTextPath + "WordsOrPhrases.txt"; //move to config
            referenceTextPath = referenceTextPath + "WordsOrPhrases.txt"; //move to config

            
            var queryTextReader = new FileReader(queryTextPath);
            var queryTexts = queryTextReader.ReadWordsOrPhrases();

            var referenceTextReader = new FileReader(referenceTextPath);
            var referenceTexts = referenceTextReader.ReadWordsOrPhrases();



            try
            {
                if (queryTexts.Count < 1 || referenceTexts.Count < 1)
                {
                    throw new NullReferenceException("Minimum one query text and reference text required to compare");
                }

                var processor = new TextProcessor();
                var embeddingGenerator = new ChatGPTEmbeddingGenerator(config);

                var processedQueryText = processor.ProcessWordOrPhraseList(queryTexts);
                var processedReferenceText = processor.ProcessWordOrPhraseList(referenceTexts);

                var queryEmbeddingDictionary = await embeddingGenerator.EmbedWordsListAsync(processedQueryText);
                var referenceEmbeddingDictionary = await embeddingGenerator.EmbedWordsListAsync(processedReferenceText);

                
                var similarityDataTable = new List<string[]>();
                var similarityDataTableFirstRow = new List<string>();
                similarityDataTableFirstRow.Add("   ");
                
                foreach (var reference in referenceEmbeddingDictionary)
                {                  
                    similarityDataTableFirstRow.Add(reference.Key);
                }

                similarityDataTable.Add(similarityDataTableFirstRow.ToArray());

                //var similarityCalculator = new CosineSimilarityCalculator();
                var similarityCalculator = new EuclideanDistanceCalculator();
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
            //later move path to configuration
            string queryDocsPath = @"C:\Users\NISHTA\OneDrive\Univeristy\sem_1\software_eng\ML_09\Test\TextSimilarityAnalyser\MySemanticAnalysisSample\Input\QueryText\Documents";
            string referenceTextPath = @"C:\Users\NISHTA\OneDrive\Univeristy\sem_1\software_eng\ML_09\Test\TextSimilarityAnalyser\MySemanticAnalysisSample\Input\ReferenceText\WordsOrPhrases.txt";

            var queryDocsReader = new FileReader(queryDocsPath);
            var queryDocs = queryDocsReader.ReadDocuments();

            var referenceTextReader = new FileReader(referenceTextPath);
            var referenceTexts = referenceTextReader.ReadWordsOrPhrases();


            try
            {
                if (queryDocs.Count < 1 || referenceTexts.Count < 1)
                {
                    throw new NullReferenceException("Minimum one query doc and reference text is required to compare");
                }

                var processor = new TextProcessor();
                var embeddingGenerator = new ChatGPTEmbeddingGenerator(config);

                var processedQueryDocs = processor.ProcessDocumentList(queryDocs, true);
                var processedReferenceText = processor.ProcessWordOrPhraseList(referenceTexts);

                var queryEmbeddingDictionary = await embeddingGenerator.EmbedDocumentsListAsync(processedQueryDocs);
                var referenceEmbeddingDictionary = await embeddingGenerator.EmbedWordsListAsync(processedReferenceText);
                
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
            //later move path to configuration
            string queryDocsPath = @"C:\Users\NISHTA\OneDrive\Univeristy\sem_1\software_eng\ML_09\Test\TextSimilarityAnalyser\MySemanticAnalysisSample\Input\QueryText\Documents";
            string referenceDocsPath = @"C:\Users\NISHTA\OneDrive\Univeristy\sem_1\software_eng\ML_09\Test\TextSimilarityAnalyser\MySemanticAnalysisSample\Input\ReferenceText\Documents";

            var queryDocsReader = new FileReader(queryDocsPath);
            var queryDocs = queryDocsReader.ReadDocuments();

            var referenceDocsReader = new FileReader(referenceDocsPath);
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
