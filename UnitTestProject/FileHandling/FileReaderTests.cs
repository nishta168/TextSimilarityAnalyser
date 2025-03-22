using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySemanticAnalysisSample.FileHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnitTestProject.FileHandling
{
    [TestClass]
    public class FileReaderTests
    {
        private readonly string _testBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles");

        private string _validTxtFile;
        private string _emptyTxtFile;
        private string _validPdfFile;
        private string _scannedPdfFile;

        [TestInitialize]
        public void Setup()
        {

            _validTxtFile = Path.Combine(_testBasePath, "QueryWordsOrPhrases.txt");
            _emptyTxtFile = Path.Combine(_testBasePath, "ReferenceWordsOrPhrases.txt");
            _validPdfFile = Path.Combine(_testBasePath, "test_valid.pdf");
            _scannedPdfFile = Path.Combine(_testBasePath, "ValidDocuments", "dummy.pdf");    
            
        }

        [TestMethod]
        public void ReadDocuments_ValidFolder_ShouldReturnContent()
        {
            var folderPath = Path.Combine(_testBasePath, "ValidDocuments");
            var fileReader = new FileReader(folderPath);
            var documents = fileReader.ReadDocuments();

            Assert.IsTrue(documents.ContainsKey("dummy"));
            Assert.AreEqual("valid text file", documents["dummy"]);
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void ReadDocuments_EmptyFolder_ShouldThrowException()
        {
            var folderPath = Path.Combine(_testBasePath, "EmptyFolder");
            var fileReader = new FileReader(folderPath);
            fileReader.ReadDocuments(); 
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadDocuments_ScannedPdfFile_ShouldThrowException()
        {
            var fileReader = new FileReader(_testBasePath);
            fileReader.ReadDocuments(); 
        }

        [TestMethod]
        public void ReadWordsOrPhrases_ValidTxtFile_ShouldReturnList()
        {
            var fileReader = new FileReader(_validTxtFile);
            var words = fileReader.ReadWordsOrPhrases();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("valid text file", words[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ReadWordsOrPhrases_EmptyTxtFile_ShouldThrowException()
        {
            var fileReader = new FileReader(_emptyTxtFile);
            var words = fileReader.ReadWordsOrPhrases();
        }

    }
}
