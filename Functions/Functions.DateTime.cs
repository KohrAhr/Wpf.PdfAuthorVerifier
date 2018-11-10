using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfAuthorVerifier.Functions
{
    public class DateTimeFunctions
    {
        /// <summary>
        ///     19/11/2004 12:36:35
        ///     D:20041119123645
        ///     D:20130404222733+04'00'
        ///     D:20160408230159Z
        ///     
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DateTime? FixDateTime(string value)
        {
            DateTime parsedDate;
            DateTime? result = null;

            if (!String.IsNullOrEmpty(value))
            {
                int l = value.IndexOf("+");
                if (l > 0)
                {
                    value = value.Remove(l, value.Length - l);
                }
                l = value.IndexOf("-");
                if (l > 0)
                {
                    value = value.Remove(l, value.Length - l);
                }

                string[] patterns =
                {
                    "D:yyyyMMddHHmmss",
                    "D:yyyyMMddHHmmsszzz",
                };

                foreach (string pattern in patterns)
                {
                    if (DateTime.TryParseExact(value, pattern, null, DateTimeStyles.None, out parsedDate))
                    {
                        result = parsedDate;
                        break;
                    }
                }

                //

                string[] patternsUTC =
                {
                    "D:yyyyMMddHHmmssZ",
                    "D:yyyyMMddHHmmssZzzz"
                };

                foreach (string pattern in patternsUTC)
                {
                    if (DateTime.TryParseExact(value, pattern, null, DateTimeStyles.AdjustToUniversal, out parsedDate))
                    {
                        result = parsedDate;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
