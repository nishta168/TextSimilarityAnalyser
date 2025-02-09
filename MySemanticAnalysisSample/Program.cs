using MySemanticAnalysisSample.Embedding;
using MySemanticAnalysisSample.FileHandling;
using MySemanticAnalysisSample.Preprocessing;
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
            //var similarityDataTable = await CompareWordsWithWordsAsync();
            var similarityDataTable = await CompareDocsWithWordsAsync();
            //var similarityDataTable = await CompareDocsWithDocsAsync();




            string outputFilePath = @"C:\Users\NISHTA\OneDrive\Univeristy\sem_1\software_eng\ML_09\Test\TextSimilarityAnalyser\MySemanticAnalysisSample\Output\similarity_result.csv";
            CSVWriter.WriteToCSV(outputFilePath, similarityDataTable);
            Console.WriteLine("Similarity data successfully written to file");
         
        }

        static async Task<List<string[]>> CompareWordsWithWordsAsync()
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

                var referenceEmbeddingDictionary = new Dictionary<string, float[]>();
                var embeddingGenerator = new ChatGPTEmbeddingGenerator();
                var similarityDataTable = new List<string[]>();
                var similarityDataTableFirstRow = new List<string>();
                similarityDataTableFirstRow.Add(" ");

                foreach (var reference in referenceTexts)
                {   
                    var processedText = processor.ProcessWordOrPhrase(reference);
                    //check for duplicate domain keys
                    var embedding = await embeddingGenerator.CreateEmbedding(processedText);
                    referenceEmbeddingDictionary.Add(processedText, embedding);
                    similarityDataTableFirstRow.Add(processedText);
                }
                similarityDataTable.Add(similarityDataTableFirstRow.ToArray());

                var similarityCalculator = new CosineSimilarityCalculator();

                foreach (var query in queryTexts)
                {
                    var processedText = processor.ProcessWordOrPhrase(query);
                    var embedding = await embeddingGenerator.CreateEmbedding(processedText);
                    var similarityDataTableRow = new List<string>();
                    similarityDataTableRow.Add(processedText);

                    foreach (var referenceEmbedding in referenceEmbeddingDictionary)
                    {
                        var similarity = similarityCalculator.CalculateSimilarity(embedding, referenceEmbedding.Value);
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

        static async Task<List<string[]>> CompareDocsWithWordsAsync()
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


                var referenceEmbeddingDictionary = new Dictionary<string, float[]>();
                var embeddingGenerator = new ChatGPTEmbeddingGenerator();
                var similarityDataTable = new List<string[]>();
                var similarityDataTableFirstRow = new List<string>();
                similarityDataTableFirstRow.Add(" ");

                foreach (var reference in referenceTexts)
                {   
                    var processedText = processor.ProcessWordOrPhrase(reference);
                    //check for duplicate domain keys
                    var embedding = await embeddingGenerator.CreateEmbedding(processedText);
                    referenceEmbeddingDictionary.Add(processedText, embedding);
                    similarityDataTableFirstRow.Add(processedText);
                }
                similarityDataTable.Add(similarityDataTableFirstRow.ToArray());

                var similarityCalculator = new CosineSimilarityCalculator();

                foreach (var query in queryDocs)
                {
                    var embedding = await embeddingGenerator.CreateEmbedding(query.Value);
                    var similarityDataTableRow = new List<string>();
                    similarityDataTableRow.Add(query.Key);

                    foreach (var referenceEmbedding in referenceEmbeddingDictionary)
                    {
                        var similarity = similarityCalculator.CalculateSimilarity(embedding, referenceEmbedding.Value);
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

        static async Task<List<string[]>> CompareDocsWithDocsAsync()
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

                var referenceEmbeddingDictionary = new Dictionary<string, float[]>();
                var embeddingGenerator = new ChatGPTEmbeddingGenerator();
                var similarityDataTable = new List<string[]>();
                var similarityDataTableFirstRow = new List<string>();
                similarityDataTableFirstRow.Add(" ");

                var processor = new TextProcessor();

                foreach (var reference in referenceDocs)
                {
                    //check for duplicate domain keys
                    var embedding = await embeddingGenerator.CreateEmbedding(reference.Value);
                    referenceEmbeddingDictionary.Add(reference.Key, embedding);
                    similarityDataTableFirstRow.Add(reference.Key);
                }
                similarityDataTable.Add(similarityDataTableFirstRow.ToArray());

                var similarityCalculator = new CosineSimilarityCalculator();

                foreach (var query in queryDocs)
                {
                    var embedding = await embeddingGenerator.CreateEmbedding(query.Value);
                    var similarityDataTableRow = new List<string>();
                    similarityDataTableRow.Add(query.Key);

                    foreach (var referenceEmbedding in referenceEmbeddingDictionary)
                    {
                        var similarity = similarityCalculator.CalculateSimilarity(embedding, referenceEmbedding.Value);
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
    }
}
