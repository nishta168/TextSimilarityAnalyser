using MySemanticAnalysisSample.Embedding;
using MySemanticAnalysisSample.FileHandling;
using MySemanticAnalysisSample.SimilarityCalculation;
using System.Data;

namespace MySemanticAnalysisSample
{
    internal class Program
    {
        //--------------------Program to take 2 inputs from console and calculate similarity--------------------------------------------------
        //static async Task Main(string[] args)
        //{
        //    Console.WriteLine("Welcome to semantic analysis of text data");
        //    Console.WriteLine("Enter first text");
        //    var text1 = Console.ReadLine();
        //    Console.WriteLine("Enter second text");
        //    var text2 = Console.ReadLine();

        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(text1) || string.IsNullOrEmpty(text2))
        //        {
        //            throw new ArgumentException("Input text cannot be null or whitespace.");
        //        }

        //        var embeddingGenerator = new ChatGPTEmbeddingGenerator();

        //        // Generate the embedding
        //        var embedding1 = await embeddingGenerator.CreateEmbedding(text1);
        //        var embedding2 = await embeddingGenerator.CreateEmbedding(text2);

        //        // Display embedding details
        //        //Console.WriteLine("Embedding generated successfully!");

        //        // Display the first 10 values for clarity
        //        Console.WriteLine("First 10 values of the embedding1:");
        //        Console.WriteLine(string.Join(", ", embedding1.Take(10)));

        //        Console.WriteLine("First 10 values of the embedding2:");
        //        Console.WriteLine(string.Join(", ", embedding2.Take(10)));

        //        var similarityCalculator = new CosineSimilarityCalculator();
        //        var similarity = similarityCalculator.CalculateSimilarity(embedding1, embedding2);
        //        Console.WriteLine("Similarity score is " + similarity);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //    }
        //}

        //---------program to read data from file and calculate similarity---------------------------------------------
        static async Task Main(string[] args)
        {   
            //later move hardcoded path to settings
            string inputTextPath = @"C:\Users\NISHTA\OneDrive\Univeristy\sem_1\software_eng\ML_09\Test\TextSimilarityAnalyser\MySemanticAnalysisSample\Input\InputText\";
            string domainsPath = @"C:\Users\NISHTA\OneDrive\Univeristy\sem_1\software_eng\ML_09\Test\TextSimilarityAnalyser\MySemanticAnalysisSample\Input\Domains\";

            // Check if the directories exist
            if (!Directory.Exists(inputTextPath) || !Directory.Exists(domainsPath))
            {
                Console.WriteLine("The specified path does not exist.");
                return;
            }

            // Check if the directories are empty
            if (!Directory.GetFiles(inputTextPath).Any())
            {
                Console.WriteLine("The InputText directory is empty.");
                return;
            }

            if (!Directory.GetFiles(domainsPath).Any())
            {
                Console.WriteLine("The Domains directory is empty.");
                return;
            }

            var domainReader = new FileReader(domainsPath);
            var inputTextReader = new FileReader(inputTextPath);
            var domains = domainReader.ReadDocuments();
            var inputTexts = inputTextReader.ReadDocuments();

            try
            {
                if (domains == null || inputTexts == null)
                {
                    throw new NullReferenceException("Either the domains or input texts are null.");
                }

                var domainEmbeddingDictionary = new Dictionary<string, float[]>();
                var embeddingGenerator = new ChatGPTEmbeddingGenerator();
                var similarityDataTable = new List<string[]>();
                var similarityDataTableFirstRow = new List<string>();
                similarityDataTableFirstRow.Add("Text");

                foreach (var domain in domains)
                {
                    //preprocess
                    //check for duplicate domain keys
                    var embedding = await embeddingGenerator.CreateEmbedding(domain.Value);
                    domainEmbeddingDictionary.Add(domain.Key, embedding);
                    similarityDataTableFirstRow.Add(domain.Key);
                }
                similarityDataTable.Add(similarityDataTableFirstRow.ToArray());

                var similarityCalculator = new CosineSimilarityCalculator();

                foreach (var inputText in inputTexts)
                {
                    //preprocess
                    var embedding = await embeddingGenerator.CreateEmbedding(inputText.Value);
                    var similarityDataTableRow = new List<string>();     
                    similarityDataTableRow.Add(inputText.Key);

                    foreach (var domainEmbedding in domainEmbeddingDictionary)
                    {
                        var similarity = similarityCalculator.CalculateSimilarity(embedding, domainEmbedding.Value);
                        similarityDataTableRow.Add(similarity.ToString());
                    }
                    
                    similarityDataTable.Add(similarityDataTableRow.ToArray());
                }

                string outputFilePath = @"C:\Users\NISHTA\OneDrive\Univeristy\sem_1\software_eng\ML_09\Test\TextSimilarityAnalyser\MySemanticAnalysisSample\Output\similarity_result.csv"; ;
                CSVWriter.WriteToCSV(outputFilePath, similarityDataTable);
                Console.WriteLine("Similarity data successfully written to file");

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
