using MySemanticAnalysisSample.Embedding;
using MySemanticAnalysisSample.FileHandling;
using MySemanticAnalysisSample.Preprocessing;
using MySemanticAnalysisSample.SimilarityCalculation;
using System.Data;

namespace MySemanticAnalysisSample
{
    internal class Program
    {
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to Text Similarity Analyser");
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
                var embeddingGenerator = new ChatGPTEmbeddingGenerator();

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

                var similarityCalculator = new CosineSimilarityCalculator();

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
                var embeddingGenerator = new ChatGPTEmbeddingGenerator();

                var processedQueryDocs = processor.ProcessDocumentList(queryDocs);
                var processedReferenceText = processor.ProcessWordOrPhraseList(referenceTexts);

                var queryEmbeddingDictionary = await embeddingGenerator.EmbedDocumentsListAsync(queryDocs);
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

                foreach (var query in queryEmbeddingDictionary)
                {
                    var similarityDataTableRow = new List<string>();
                    similarityDataTableRow.Add(query.Key);
                   
                    foreach (var reference in referenceEmbeddingDictionary)
                    {
                        var similarity = similarityCalculator.CalculateSimilarity(query.Value, reference.Value);
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

                var processor = new TextProcessor();
                var embeddingGenerator = new ChatGPTEmbeddingGenerator();

                var processedQueryDocs = processor.ProcessDocumentList(queryDocs);
                var processedReferenceDocs = processor.ProcessDocumentList(referenceDocs);

                var queryEmbeddingDictionary = await embeddingGenerator.EmbedDocumentsListAsync(processedQueryDocs);
                var referenceEmbeddingDictionary = await embeddingGenerator.EmbedDocumentsListAsync(processedReferenceDocs);
                
                var similarityDataTable = new List<string[]>();
                var similarityDataTableFirstRow = new List<string>();
                similarityDataTableFirstRow.Add("    ");

                

                foreach (var reference in referenceEmbeddingDictionary)
                {
                   similarityDataTableFirstRow.Add(reference.Key);
                }

                similarityDataTable.Add(similarityDataTableFirstRow.ToArray());

                var similarityCalculator = new CosineSimilarityCalculator();

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
    }
}
