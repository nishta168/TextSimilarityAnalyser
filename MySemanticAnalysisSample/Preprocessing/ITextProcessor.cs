using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.Preprocessing
{
    /// <summary>
    /// Defines methods provided by implementing classes for processing text data, including words, phrases, and documents.
    /// </summary>
    internal interface ITextProcessor
    {
        /// <summary>
        /// Processes a list of words or phrases by applying necessary text transformations.
        /// </summary>
        /// <param name="wordOrPhraseList">List of words or phrases to process.</param>
        /// <returns>A processed list of words or phrases</returns>
        List<string> ProcessWordOrPhraseList(List<string> wordOrPhraseList);

        /// <summary>
        /// Processes a dictionary of documents, applying necessary transformations.
        /// </summary>
        /// <param name="documentList">A dictionary where key is document name and value is its content as a single string.</param>
        /// <param name="forWordVsDoc">Indicates whether processing is for word-with-document comparison or not.</param>
        /// <returns>A dictionary where key is document name and value is processed and chunked contents as a list of strings.</returns>
        Dictionary<string, List<string>> ProcessDocumentList(Dictionary<string, string> documentList, bool forWordVsDoc);
    }
}
