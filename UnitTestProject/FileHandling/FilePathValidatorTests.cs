using MySemanticAnalysisSample.FileHandling;

namespace UnitTestProject.FileHandling;

[TestClass]
public class FilePathValidatorTests
{
    private readonly string _testBasePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles");

    private string _validDocsPath;
    private string _emptyDocsPath;
    private string _queryWordsFile;
    private string _referenceWordsFile;
    private string _outputFolder;

    [TestInitialize]
    public void Setup()
    {
        _validDocsPath = Path.Combine(_testBasePath, "ValidDocuments");
        _emptyDocsPath = Path.Combine(_testBasePath, "EmptyFolder");
        _queryWordsFile = Path.Combine(_testBasePath, "QueryWordsOrPhrases.txt");
        _referenceWordsFile = Path.Combine(_testBasePath, "ReferenceWordsOrPhrases.txt");
        _outputFolder = Path.Combine(_testBasePath, "OutputFolder");
    }

    [TestMethod]
    public void ValidateInputPath_ValidDocumentFolder_ReturnsSamePath()
    {
        string result = FilePathValidator.ValidateInputPath(_validDocsPath, isDocFolder: true, isQuery: false);
        Assert.AreEqual(_validDocsPath, result);
    }

    [TestMethod]
    [ExpectedException(typeof(DirectoryNotFoundException))]
    public void ValidateInputPath_InvalidDocumentFolder_ThrowsDirectoryNotFoundException()
    {
        string invalidPath = Path.Combine(_testBasePath, "NonExistentFolder");
        FilePathValidator.ValidateInputPath(invalidPath, isDocFolder: true, isQuery: false);
    }

    [TestMethod]
    [ExpectedException(typeof(DirectoryNotFoundException))]
    public void ValidateInputPath_EmptyDocumentFolder_ThrowsDirectoryNotFoundException()
    {
        FilePathValidator.ValidateInputPath(_emptyDocsPath, isDocFolder: true, isQuery: false);
    }

    [TestMethod]
    public void ValidateInputPath_ValidQueryWordsFile_ReturnsSamePath()
    {
        string result = FilePathValidator.ValidateInputPath(_queryWordsFile, isDocFolder: false, isQuery: true);
        Assert.AreEqual(_queryWordsFile, result);
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void ValidateInputPath_InvalidQueryWordsFile_ThrowsFileNotFoundException()
    {
        string invalidPath = Path.Combine(_testBasePath, "NonExistentQuery.txt");
        FilePathValidator.ValidateInputPath(invalidPath, isDocFolder: false, isQuery: true);
    }

    [TestMethod]
    public void ValidateOutputFilePath_ValidPath_ReturnsSamePath()
    {
        string validOutputFile = Path.Combine(_outputFolder, "output.csv");
        File.WriteAllText(validOutputFile, "Test Data"); // Ensure file exists

        string result = FilePathValidator.ValidateOutputFilePath(validOutputFile, "default.csv");
        Assert.AreEqual(validOutputFile, result);
    }

    [TestMethod]
    public void ValidateOutputFilePath_InvalidPath_ReturnsDefaultPath()
    {
        string invalidPath = Path.Combine(_outputFolder, "NonExistent.csv");
        string expectedDefaultPath = Path.Combine(AppContext.BaseDirectory, "default.csv");

        string result = FilePathValidator.ValidateOutputFilePath(invalidPath, "default.csv");
        Assert.AreEqual(expectedDefaultPath, result);
    }
}