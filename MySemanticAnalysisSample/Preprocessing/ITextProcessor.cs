using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.Preprocessing
{
    internal interface ITextProcessor
    {
        string ProcessWordOrPhrase(string wordOrPhrase );
        List<string> ProcessDocument(string document);
    }
}
