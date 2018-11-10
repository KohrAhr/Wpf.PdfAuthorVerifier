using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfAuthorVerifier.Functions
{
    public class PdfFunctions
    {
        /// <summary>
        ///     Get value by Key name
        /// </summary>
        /// <param name="pdfReader"></param>
        /// <param name="key"></param>
        /// <returns>
        ///     Return empty string if Key not found
        /// </returns>
        public string GetValue(PdfReader pdfReader, string key)
        {
            string result = "";
            KeyValuePair<String, String> value = pdfReader.Info.Where(x => x.Key == key).FirstOrDefault();
            if (!String.IsNullOrEmpty(value.Key))
            {
                result = pdfReader.Info[key];
            }

            return result;
        }

        public PdfReader GetPdf(string filename)
        {
            PdfReader result = new PdfReader(filename);

            return result;
        }
    }
}
