using MySemanticAnalysisSample.Preprocessing;
using System;
using System.Collections.Generic;

namespace UnitTestProject.Preprocessing
{
    [TestClass]
    public class TextProcessorTests
    {
        private readonly TextProcessor _textProcessor = new TextProcessor();

        [TestMethod]
        public void CleanDocument_RemovesUrlsEmailsAndExtraSpaces()
        {
            string input = "This is a test email test@example.com and a website http://example.com  ";
            string expected = "This is a test email and a website";

            string result = _textProcessor.CleanDocument(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ProcessWordOrPhraseList_TrimsEachElement()
        {
            var input = new List<string> { "   word1  ", " word2 ", "word3  " };
            var expected = new List<string> { "word1", "word2", "word3" };

            var result = _textProcessor.ProcessWordOrPhraseList(input);

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ChunkDocumentByWordCount_SplitsCorrectly()
        {
            string input = "one two three four five six seven eight nine ten";
            int maxWordCount = 3;
            var expected = new List<string> { "one two three", "four five six", "seven eight nine", "ten" };

            var result = _textProcessor.ChunkDocumentByWordCount(input, maxWordCount);

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ProcessDocumentList_ReturnsProcessedChunks()
        {
            var documentList = new Dictionary<string, string>
            {
                { "doc1", "one two three four five six seven eight nine ten" }
            };

            var result = _textProcessor.ProcessDocumentList(documentList, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey("doc1"));
            Assert.IsTrue(result["doc1"].Count != 0); 
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CleanDocument_ThrowsExceptionForNullInput()
        {
            _textProcessor.CleanDocument(null);
        }
    }
}
