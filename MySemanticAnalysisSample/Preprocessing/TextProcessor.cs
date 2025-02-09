using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySemanticAnalysisSample.Preprocessing
{
    internal class TextProcessor : ITextProcessor
    {
        public List<string> ProcessDocument(string document)
        {
            throw new NotImplementedException();
        }

        public string ProcessWordOrPhrase(string wordOrPhrase)
        {
            var text = wordOrPhrase.Trim();
            return text;
        }
    }
}
