using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.Preprocessing
{
    internal interface ITextProcessor
    {
        List<string> ProcessWordOrPhraseList(List<string> wordOrPhraseList );
        Dictionary<string, List<string>> ProcessDocumentList(Dictionary<string, string> documentList, bool forWordVsDoc);
    }
}
